// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Input source for fake hands information, which gives finer details about current hand state and position
    /// than the standard GestureRecognizer.
    /// </summary>
    /// <remarks>This input source only triggers SourceUp and SourceDown for the hands. Everything else is handled by GesturesInput.</remarks>
    [RequireComponent(typeof(ManualHandControl))]
    public class EditorHandsInput : BaseInputSource
    {
        /// <summary>
        /// Data for a hand.
        /// </summary>
        private class EditorHandData
        {
            public EditorHandData(IInputSource inputSource, uint handId)
            {
                HandId = handId;
                HandPosition = Vector3.zero;
                HandDelta = Vector3.zero;
                IsFingerDown = false;
                IsFingerDownPending = false;
                FingerStateChanged = false;
                FingerStateUpdateTimer = -1;
                InputSourceArgs = new InputSourceEventArgs(inputSource, handId);
            }

            public readonly uint HandId;
            public Vector3 HandPosition;
            public Vector3 HandDelta;
            public bool IsFingerDown;
            public bool IsFingerDownPending;
            public bool FingerStateChanged;
            public float FingerStateUpdateTimer;
            public float FingerDownStartTime;
            public readonly InputSourceEventArgs InputSourceArgs;
        }

        private ManualHandControl manualHandControl;

        /// <summary>
        /// Dispatched each frame that a hand is moving
        /// </summary>
        public event Action<IInputSource, uint> HandMoved;

        /// <summary>
        /// Delay before a finger pressed is considered.
        /// This mitigates fake finger taps that can sometimes be detected while the hand is moving.
        /// </summary>
        private const float FingerPressDelay = 0.07f;

        /// <summary>
        /// The maximum interval between button down and button up that will result in a clicked event.
        /// </summary>
        private const float MaxClickDuration = 0.5f;

        /// <summary>
        /// Number of fake hands supported in the editor.
        /// </summary>
        private const int EditorHandsCount = 2;

        /// <summary>
        /// Array containing the hands data for the two fake hands
        /// </summary>
        private readonly EditorHandData[] editorHandsData = new EditorHandData[EditorHandsCount];

        /// <summary>
        /// Dictionary linking each hand ID to its data.
        /// </summary>
        private readonly Dictionary<uint, EditorHandData> handIdToData = new Dictionary<uint, EditorHandData>(4);
        private readonly List<uint> pendingHandIdDeletes = new List<uint>();

        // HashSets used to be able to quickly update the hands data when hands become visible / not visible
        private readonly HashSet<uint> currentHands = new HashSet<uint>();
        private readonly HashSet<uint> newHands = new HashSet<uint>();

        public override SupportedInputEvents SupportedEvents
        {
            get
            {
                return SupportedInputEvents.SourceUpAndDown;
            }
        }

        public override SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            return SupportedInputInfo.Position;
        }

        public override bool TryGetPosition(uint sourceId, out Vector3 position)
        {
            if (sourceId >= editorHandsData.Length)
            {
                position = Vector3.zero;
                return false;
            }

            position = editorHandsData[sourceId].HandPosition;
            return true;
        }

        public override bool TryGetOrientation(uint sourceId, out Quaternion orientation)
        {
            // Orientation is not supported by hands
            orientation = Quaternion.identity;
            return false;
        }

        /// <summary>
        /// Gets the position delta of the specified hand.
        /// </summary>
        /// <param name="handId">ID of the hand to get.</param>
        /// <returns>The current movement vector of the specified hand.</returns>
        public Vector3 GetHandDelta(uint handId)
        {
            if (handId >= editorHandsData.Length)
            {
                string message = string.Format("GetHandDelta called with invalid hand ID {0}.", handId);
                throw new ArgumentException(message, "handId");
            }

            return editorHandsData[handId].HandDelta;
        }

        /// <summary>
        /// Gets the pressed state of the specified hand
        /// </summary>
        /// <param name="handId">ID of the hand to get.</param>
        /// <returns>True if the specified hand is currently in an airtap.</returns>
        public bool GetFingerState(uint handId)
        {
            if (handId >= editorHandsData.Length)
            {
                var message = string.Format("GetFingerState called with invalid hand ID {0}.", handId);
                throw new ArgumentException(message, "handId");
            }

            return editorHandsData[handId].IsFingerDown;
        }

        /// <summary>
        /// Gets whether the specified hand just started an airtap this frame
        /// </summary>
        /// <param name="handId">ID of the hand to get.</param>
        /// <returns>True for the first frame of an airtap</returns>
        public bool GetFingerDown(uint handId)
        {
            if (handId >= editorHandsData.Length)
            {
                var message = string.Format("GetFingerDown called with invalid hand ID {0}.", handId);
                throw new ArgumentException(message, "handId");
            }

            return editorHandsData[handId].IsFingerDown && editorHandsData[handId].FingerStateChanged;
        }

        /// <summary>
        /// Gets whether the specified hand just ended an airtap this frame
        /// </summary>
        /// <param name="handId">ID of the hand to get.</param>
        /// <returns>True for the first frame of the release of an airtap</returns>
        public bool GetFingerUp(uint handId)
        {
            if (handId >= editorHandsData.Length)
            {
                var message = string.Format("GetFingerUp called with invalid hand ID {0}.", handId);
                throw new ArgumentException(message, "handId");
            }

            return !editorHandsData[handId].IsFingerDown && editorHandsData[handId].FingerStateChanged;
        }

        private void Awake()
        {
#if !UNITY_EDITOR
            Destroy(this);
#endif
            manualHandControl = GetComponent<ManualHandControl>();
            for (uint i = 0; i < editorHandsData.Length; i++)
            {
                editorHandsData[i] = new EditorHandData(this, i);
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            newHands.Clear();
            currentHands.Clear();

            UpdateHandData();
            SendHandVisibilityEvents();
        }
#endif

        /// <summary>
        /// Update the hand data for the currently detected hands.
        /// </summary>
        private void UpdateHandData()
        {
            float time;
            float deltaTime;

            if (manualHandControl.UseUnscaledTime)
            {
                time = Time.unscaledTime;
                deltaTime = Time.unscaledDeltaTime;
            }
            else
            {
                time = Time.time;
                deltaTime = Time.deltaTime;
            }

            if (manualHandControl.LeftHandInView)
            {
                GetOrAddHandData(0);
                currentHands.Add(0);

                UpdateHandState(manualHandControl.LeftHandSourceState, editorHandsData[0], deltaTime, time);
            }

            if (manualHandControl.RightHandInView)
            {
                GetOrAddHandData(1);
                currentHands.Add(1);
                UpdateHandState(manualHandControl.RightHandSourceState, editorHandsData[1], deltaTime, time);
            }
        }

        /// <summary>
        /// Gets the hand data for the specified hand source if it already exists, otherwise creates it.
        /// </summary>
        /// <param name="sourceId">Hand source for which hands data should be retrieved.</param>
        /// <returns>The hand data requested.</returns>
        private EditorHandData GetOrAddHandData(uint sourceId)
        {
            EditorHandData handData;
            if (!handIdToData.TryGetValue(sourceId, out handData))
            {
                handData = new EditorHandData(this, sourceId);
                handIdToData.Add(handData.HandId, handData);
                newHands.Add(handData.HandId);

            }

            return handData;
        }

        /// <summary>
        /// Updates the hand positional information.
        /// </summary>
        /// <param name="handSource">Hand source to use to update the position.</param>
        /// <param name="editorHandData">EditorHandData structure to update.</param>
        private void UpdateHandState(DebugInteractionSourceState handSource, EditorHandData editorHandData, float deltaTime, float time)
        {
            // Update hand position
            Vector3 handPosition;
            if (handSource.Properties.Location.TryGetPosition(out handPosition))
            {
                editorHandData.HandDelta = handPosition - editorHandData.HandPosition;
                editorHandData.HandPosition = handPosition;
            }

            // Check for finger presses
            if (handSource.Pressed != editorHandData.IsFingerDownPending)
            {
                editorHandData.IsFingerDownPending = handSource.Pressed;
                editorHandData.FingerStateUpdateTimer = FingerPressDelay;
            }

            // Finger presses are delayed to mitigate issue with hand position shifting during air tap
            editorHandData.FingerStateChanged = false;
            if (editorHandData.FingerStateUpdateTimer > 0)
            {
                editorHandData.FingerStateUpdateTimer -= deltaTime;
                if (editorHandData.FingerStateUpdateTimer <= 0)
                {
                    editorHandData.IsFingerDown = editorHandData.IsFingerDownPending;
                    editorHandData.FingerStateChanged = true;
                    if (editorHandData.IsFingerDown)
                    {
                        editorHandData.FingerDownStartTime = time;
                    }
                }
            }

            SendHandStateEvents(editorHandData, time);
        }

        /// <summary>
        /// Sends the events for hand state changes.
        /// </summary>
        /// <param name="editorHandData">Hand data for which events should be sent.</param>
        private void SendHandStateEvents(EditorHandData editorHandData, float time)
        {
            // Hand moved event
            if (editorHandData.HandDelta.sqrMagnitude > 0)
            {
                HandMoved.RaiseEvent(this, editorHandData.HandId);
            }

            // Finger pressed/released events
            if (editorHandData.FingerStateChanged)
            {
                if (editorHandData.IsFingerDown)
                {
                    RaiseSourceDownEvent(editorHandData.InputSourceArgs);
                }
                else
                {
                    RaiseSourceUpEvent(editorHandData.InputSourceArgs);

                    // Also send click event when using this hands replacement input
                    if (time - editorHandData.FingerDownStartTime < MaxClickDuration)
                    {
                        // We currently only support single taps in editor
                        SourceClickEventArgs args = new SourceClickEventArgs(this, editorHandData.HandId, 1);
                        RaiseSourceClickedEvent(args);
                    }
                }
            }
        }

        /// <summary>
        /// Sends the events for hand visibility changes.
        /// </summary>
        private void SendHandVisibilityEvents()
        {
            // Send event for new hands that were added
            foreach (uint newHand in newHands)
            {
                InputSourceEventArgs args = new InputSourceEventArgs(this, newHand);
                RaiseSourceDetectedEvent(args);
            }

            // Send event for hands that are no longer visible and remove them from our dictionary
            foreach (uint existingHand in handIdToData.Keys)
            {
                if (!currentHands.Contains(existingHand))
                {
                    pendingHandIdDeletes.Add(existingHand);
                    InputSourceEventArgs args = new InputSourceEventArgs(this, existingHand);
                    RaiseSourceLostEvent(args);
                }
            }

            // Remove pending hand IDs
            for (int i = 0; i < pendingHandIdDeletes.Count; ++i)
            {
                handIdToData.Remove(pendingHandIdDeletes[i]);
            }
            pendingHandIdDeletes.Clear();
        }
    }
}