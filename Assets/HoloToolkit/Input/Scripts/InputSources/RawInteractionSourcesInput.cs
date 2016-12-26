// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Input source for raw interactions sources information, which gives finer details about current source state and position
    /// than the standard GestureRecognizer.
    /// This mostly allows users to access the source up/down and detected/lost events, 
    /// which are not communicated as part of standard Windows gestures.
    /// </summary>
    /// <remarks>
    /// This input source only triggers SourceUp/SourceDown and SourceDetected/SourceLost.
    /// Everything else is handled by GesturesInput.
    /// </remarks>
    public class RawInteractionSourcesInput : BaseInputSource
    {
        /// <summary>
        /// Data for an interaction source.
        /// </summary>
        private class SourceData
        {
            public SourceData(IInputSource inputSource, uint sourceId)
            {
                SourceId = sourceId;
                HasPosition = false;
                SourcePosition = Vector3.zero;
                IsSourceDown = false;
                IsSourceDownPending = false;
                SourceStateChanged = false;
                SourceStateUpdateTimer = -1;
                InputSourceArgs = new InputSourceEventArgs(inputSource, sourceId);
            }

            public readonly uint SourceId;
            public bool HasPosition;
            public Vector3 SourcePosition;
            public bool IsSourceDown;
            public bool IsSourceDownPending;
            public bool SourceStateChanged;
            public float SourceStateUpdateTimer;
            public readonly InputSourceEventArgs InputSourceArgs;
        }

        /// <summary>
        /// Delay before a source press is considered.
        /// This mitigates fake source taps that can sometimes be detected while the input source is moving.
        /// </summary>
        private const float SourcePressDelay = 0.07f;

        [Tooltip("Use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.")]
        public bool UseUnscaledTime = true;

        /// <summary>
        /// Dictionary linking each source ID to its data.
        /// </summary>
        private readonly Dictionary<uint, SourceData> sourceIdToData = new Dictionary<uint, SourceData>(4);
        private readonly List<uint> pendingSourceIdDeletes = new List<uint>();

        // HashSets used to be able to quickly update the sources data when they become visible / not visible
        private readonly HashSet<uint> currentSources = new HashSet<uint>();
        private readonly HashSet<uint> newSources = new HashSet<uint>();

        public override SupportedInputEvents SupportedEvents
        {
            get { return SupportedInputEvents.SourceUpAndDown; }
        }

        public override SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            SupportedInputInfo retVal = SupportedInputInfo.None;

            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                if (sourceData.HasPosition)
                {
                    retVal |= SupportedInputInfo.Position;
                }
            }

            return retVal;
        }

        public override bool TryGetPosition(uint sourceId, out Vector3 position)
        {
            SourceData sourceData;
            if (sourceIdToData.TryGetValue(sourceId, out sourceData))
            {
                if (sourceData.HasPosition)
                {
                    position = sourceData.SourcePosition;
                    return true;
                }
            }

            // Else, the source doesn't have positional information
            position = Vector3.zero;
            return false;
        }

        public override bool TryGetOrientation(uint sourceId, out Quaternion orientation)
        {
            // Orientation is not supported by any Windows interaction sources
            orientation = Quaternion.identity;
            return false;
        }

        private void Update()
        {
            newSources.Clear();
            currentSources.Clear();

            UpdateSourceData();
            SendSourceVisibilityEvents();
        }

        /// <summary>
        /// Update the source data for the currently detected sources.
        /// </summary>
        private void UpdateSourceData()
        {
            // Poll for updated reading from hands
            InteractionSourceState[] sourceStates = InteractionManager.GetCurrentReading();
            if (sourceStates != null)
            {
                for (var i = 0; i < sourceStates.Length; ++i)
                {
                    InteractionSourceState handSource = sourceStates[i];
                    SourceData sourceData = GetOrAddSourceData(handSource.source);
                    currentSources.Add(handSource.source.id);

                    UpdateSourceState(handSource, sourceData);
                }
            }
        }

        /// <summary>
        /// Gets the source data for the specified interaction source if it already exists, otherwise creates it.
        /// </summary>
        /// <param name="interactionSource">Interaction source for which data should be retrieved.</param>
        /// <returns>The source data requested.</returns>
        private SourceData GetOrAddSourceData(InteractionSource interactionSource)
        {
            SourceData sourceData;
            if (!sourceIdToData.TryGetValue(interactionSource.id, out sourceData))
            {
                sourceData = new SourceData(this, interactionSource.id);
                sourceIdToData.Add(sourceData.SourceId, sourceData);
                newSources.Add(sourceData.SourceId);
            }

            return sourceData;
        }

        /// <summary>
        /// Updates the source positional information.
        /// </summary>
        /// <param name="interactionSource">Interaction source to use to update the position.</param>
        /// <param name="sourceData">SourceData structure to update.</param>
        private void UpdateSourceState(InteractionSourceState interactionSource, SourceData sourceData)
        {
            // Update source position
            Vector3 sourcePosition;
            if (interactionSource.properties.location.TryGetPosition(out sourcePosition))
            {
                sourceData.HasPosition = true;
                sourceData.SourcePosition = sourcePosition;
            }

            // Check for source presses
            if (interactionSource.pressed != sourceData.IsSourceDownPending)
            {
                sourceData.IsSourceDownPending = interactionSource.pressed;
                sourceData.SourceStateUpdateTimer = SourcePressDelay;
            }

            // Source presses are delayed to mitigate issue with hand position shifting during air tap
            sourceData.SourceStateChanged = false;
            if (sourceData.SourceStateUpdateTimer >= 0)
            {
                float deltaTime = UseUnscaledTime
                    ? Time.unscaledDeltaTime
                    : Time.deltaTime;

                sourceData.SourceStateUpdateTimer -= deltaTime;
                if (sourceData.SourceStateUpdateTimer < 0)
                {
                    sourceData.IsSourceDown = sourceData.IsSourceDownPending;
                    sourceData.SourceStateChanged = true;
                }
            }

            SendSourceStateEvents(sourceData);
        }

        /// <summary>
        /// Sends the events for source state changes.
        /// </summary>
        /// <param name="sourceData">Source data for which events should be sent.</param>
        private void SendSourceStateEvents(SourceData sourceData)
        {
            // Source pressed/released events
            if (sourceData.SourceStateChanged)
            {
                if (sourceData.IsSourceDown)
                {
                    RaiseSourceDownEvent(sourceData.InputSourceArgs);
                }
                else
                {
                    RaiseSourceUpEvent(sourceData.InputSourceArgs);
                }
            }
        }

        /// <summary>
        /// Sends the events for source visibility changes.
        /// </summary>
        private void SendSourceVisibilityEvents()
        {
            // Send event for new sources that were added
            foreach (uint newSource in newSources)
            {
                InputSourceEventArgs args = new InputSourceEventArgs(this, newSource);
                RaiseSourceDetectedEvent(args);
            }

            // Send event for sources that are no longer visible and remove them from our dictionary
            foreach (uint existingSource in sourceIdToData.Keys)
            {
                if (!currentSources.Contains(existingSource))
                {
                    pendingSourceIdDeletes.Add(existingSource);
                    InputSourceEventArgs args = new InputSourceEventArgs(this, existingSource);
                    RaiseSourceLostEvent(args);
                }
            }

            // Remove pending source IDs
            for (int i = 0; i < pendingSourceIdDeletes.Count; ++i)
            {
                sourceIdToData.Remove(pendingSourceIdDeletes[i]);
            }
            pendingSourceIdDeletes.Clear();
        }
    }
}
