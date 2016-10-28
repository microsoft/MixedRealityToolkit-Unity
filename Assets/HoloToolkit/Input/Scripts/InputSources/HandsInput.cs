//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Input source for raw hands information, which gives finer details about current hand state and position
    /// than the standard GestureRecognizer.
    /// </summary>
    /// <remarks>This input source only triggers SourceUp and SourceDown for the hands. Everything else is handled by GesturesInput.</remarks>
    public class HandsInput : BaseInputSource
    {
        /// <summary>
        /// Data for a hand.
        /// </summary>
        private class HandData
        {
            public HandData(uint handId)
            {
                HandId = handId;
                HandPosition = Vector3.zero;
                HandDelta = Vector3.zero;
                IsFingerDown = false;
                IsFingerDownPending = false;
                FingerStateChanged = false;
                FingerStateUpdateTimer = -1;
            }

            public readonly uint HandId;
            public Vector3 HandPosition;
            public Vector3 HandDelta;
            public bool IsFingerDown;
            public bool IsFingerDownPending;
            public bool FingerStateChanged;
            public float FingerStateUpdateTimer;
        }

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
        /// Dictionary linking each hand ID to its data.
        /// </summary>
        private readonly Dictionary<uint, HandData> handIdToData = new Dictionary<uint, HandData>(4);
        private readonly List<uint> pendingHandIdDeletes = new List<uint>();

        // HashSets used to be able to quickly update the hands data when hands become visible / not visible
        private readonly HashSet<uint> currentHands = new HashSet<uint>();
        private readonly HashSet<uint> newHands = new HashSet<uint>();

        public override SupportedInputEvents SupportedEvents
        {
            get { return SupportedInputEvents.SourceUpAndDown; }
        }

        public override SupportedInputInfo SupportedInputInfo
        {
            get { return SupportedInputInfo.Position; }
        }

        public override bool TryGetPosition(uint sourceId, out Vector3 position)
        {
            HandData handData;
            if (!handIdToData.TryGetValue(sourceId, out handData))
            {
                position = Vector3.zero;
                return false;
            }

            position = handData.HandPosition;
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
            HandData handData;
            if (!handIdToData.TryGetValue(handId, out handData))
            {
                string message = string.Format("GetHandDelta called with invalid hand ID {0}.", handId);
                throw new ArgumentException(message, "handId");
            }

            return handData.HandDelta;
        }

        /// <summary>
        /// Gets the pressed state of the specified hand
        /// </summary>
        /// <param name="handId">ID of the hand to get.</param>
        /// <returns>True if the specified hand is currently in an airtap.</returns>
        public bool GetFingerState(uint handId)
        {
            HandData handData;
            if (!handIdToData.TryGetValue(handId, out handData))
            {
                var message = string.Format("GetFingerState called with invalid hand ID {0}.", handId);
                throw new ArgumentException(message, "handId");
            }

            return handData.IsFingerDown;
        }

        /// <summary>
        /// Gets whether the specified hand just started an airtap this frame
        /// </summary>
        /// <param name="handId">ID of the hand to get.</param>
        /// <returns>True for the first frame of an airtap</returns>
        public bool GetFingerDown(uint handId)
        {
            HandData handData;
            if (!handIdToData.TryGetValue(handId, out handData))
            {
                var message = string.Format("GetFingerDown called with invalid hand ID {0}.", handId);
                throw new ArgumentException(message, "handId");
            }

            return handData.IsFingerDown && handData.FingerStateChanged;
        }

        /// <summary>
        /// Gets whether the specified hand just ended an airtap this frame
        /// </summary>
        /// <param name="handId">ID of the hand to get.</param>
        /// <returns>True for the first frame of the release of an airtap</returns>
        public bool GetFingerUp(uint handId)
        {
            HandData handData;
            if (!handIdToData.TryGetValue(handId, out handData))
            {
                var message = string.Format("GetFingerUp called with invalid hand ID {0}.", handId);
                throw new ArgumentException(message, "handId");
            }

            return !handData.IsFingerDown && handData.FingerStateChanged;
        }

        private void Update()
        {
            newHands.Clear();
            currentHands.Clear();

            UpdateHandData();
            SendHandVisibilityEvents();
        }

        /// <summary>
        /// Update the hand data for the currently detected hands.
        /// </summary>
        private void UpdateHandData()
        {
            // Poll for updated reading from hands
            InteractionSourceState[] sourceStates = InteractionManager.GetCurrentReading();
            if (sourceStates != null)
            {
                for (var i = 0; i < sourceStates.Length; ++i)
                {
                    InteractionSourceState handSource = sourceStates[i];

                    if (handSource.source.kind == InteractionSourceKind.Hand)
                    {
                        HandData handData = GetOrAddHandData(handSource.source);
                        currentHands.Add(handSource.source.id);

                        UpdateHandState(handSource, handData);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the hand data for the specified hand source if it already exists, otherwise creates it.
        /// </summary>
        /// <param name="handSource">Hand source for which hands data should be retrieved.</param>
        /// <returns>The hand data requested.</returns>
        private HandData GetOrAddHandData(InteractionSource handSource)
        {
            HandData handData;
            if (!handIdToData.TryGetValue(handSource.id, out handData))
            {
                handData = new HandData(handSource.id);
                handIdToData.Add(handData.HandId, handData);
                newHands.Add(handData.HandId);

            }

            return handData;
        }

        /// <summary>
        /// Updates the hand positional information.
        /// </summary>
        /// <param name="handSource">Hand source to use to update the position.</param>
        /// <param name="handData">HandData structure to update.</param>
        private void UpdateHandState(InteractionSourceState handSource, HandData handData)
        {
            // Update hand position
            Vector3 handPosition;
            if (handSource.properties.location.TryGetPosition(out handPosition))
            {
                handData.HandDelta = handPosition - handData.HandPosition;
                handData.HandPosition = handPosition;
            }

            // Check for finger presses
            if (handSource.pressed != handData.IsFingerDownPending)
            {
                handData.IsFingerDownPending = handSource.pressed;
                handData.FingerStateUpdateTimer = FingerPressDelay;
            }

            // Finger presses are delayed to mitigate issue with hand position shifting during air tap
            handData.FingerStateChanged = false;
            if (handData.FingerStateUpdateTimer >= 0)
            {
                handData.FingerStateUpdateTimer -= Time.deltaTime;
                if (handData.FingerStateUpdateTimer < 0)
                {
                    handData.IsFingerDown = handData.IsFingerDownPending;
                    handData.FingerStateChanged = true;
                }
            }

            SendHandStateEvents(handData);
        }

        /// <summary>
        /// Sends the events for hand state changes.
        /// </summary>
        /// <param name="handData">Hand data for which events should be sent.</param>
        private void SendHandStateEvents(HandData handData)
        {
            // Hand moved event
            if (handData.HandDelta.sqrMagnitude > 0)
            {
                HandMoved.RaiseEvent(this, handData.HandId);
            }

            // Finger pressed/released events
            if (handData.FingerStateChanged)
            {
                if (handData.IsFingerDown)
                {
                    RaiseSourceDownEvent(handData.HandId);
                }
                else
                {
                    RaiseSourceUpEvent(handData.HandId);
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
                RaiseSourceDetectedEvent(newHand);
            }

            // Send event for hands that are no longer visible and remove them from our dictionary
            foreach (uint existingHand in handIdToData.Keys)
            {
                if (!currentHands.Contains(existingHand))
                {
                    pendingHandIdDeletes.Add(existingHand);
                    RaiseSourceLostEvent(existingHand);
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
