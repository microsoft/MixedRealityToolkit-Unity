// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// HandsManager determines if the hand is currently detected or not.
    /// </summary>
    public partial class HandsManager : Singleton<HandsManager>
    {
        /// <summary>
        /// Occurs when users hand is detected or lost.
        /// </summary>
        /// <param name="handDetected">True if a hand is Detected, else false.</param>
        public delegate void HandInViewDelegate(bool handDetected);
        public event HandInViewDelegate HandInView;

        /// <summary>
        /// HandDetected tracks the hand detected state.
        /// Returns true if the list of tracked hands is not empty.
        /// </summary>
        public bool HandDetected
        {
            get { return trackedHands.Count > 0; }
        }

        /// <summary>
        /// HandPressed tracks the pressed state.
        /// Returns true if the list of pressed hands is not empty.
        /// </summary>
        public bool HandsPressed
        {
            get { return pressedHands.Count > 0; }
        }

        /// <summary>
        /// The world space position of the hand being used for the current manipulation gesture.
        /// Valid only if a manipulation gesture is in progress.
        /// </summary>
        public Vector3 ManipulationHandPosition
        {
            get
            {
                Vector3 handPosition;
                if (!currentHandState.properties.location.TryGetPosition(out handPosition))
                {
                    handPosition = Vector3.zero;
                }
                return handPosition;
            }
        }

        private InteractionSourceState currentHandState;

        private HashSet<uint> trackedHands = new HashSet<uint>();

        private HashSet<uint> pressedHands = new HashSet<uint>();

        private void OnEnable()
        {
            InteractionManager.SourceDetected += InteractionManager_SourceDetected;
            InteractionManager.SourcePressed += InteractionManager_SourcePressed;
            InteractionManager.SourceReleased += InteractionManager_SourceReleased;
            InteractionManager.SourceUpdated += InteractionManager_SourceUpdated;
            InteractionManager.SourceLost += InteractionManager_SourceLost;
        }

        private void OnDisable()
        {
            InteractionManager.SourcePressed -= InteractionManager_SourcePressed;
            InteractionManager.SourceReleased -= InteractionManager_SourceReleased;
            InteractionManager.SourceUpdated -= InteractionManager_SourceUpdated;
            InteractionManager.SourceLost -= InteractionManager_SourceLost;
        }

        /// <summary>
        /// Thrown when we detect a hand.
        /// </summary>
        /// <param name="state"></param>
        private void InteractionManager_SourceDetected(InteractionSourceState state)
        {
            // Check to see that the source is a hand.
            if (state.source.kind == InteractionSourceKind.Hand)
            {
                trackedHands.Add(state.source.id);

                if (HandInView != null)
                {
                    HandInView(HandDetected);
                }
            }
        }

        /// <summary>
        /// Thrown when the Interaction Source is pressed.
        /// </summary>
        /// <param name="state">The current state of the Interaction source.</param>
        private void InteractionManager_SourcePressed(InteractionSourceState state)
        {
            if (state.source.kind == InteractionSourceKind.Hand)
            {
                if (trackedHands.Contains(state.source.id))
                {
                    currentHandState = state;

                    if(!pressedHands.Contains(state.source.id))
                    {
                        pressedHands.Add(state.source.id);
                    }
                }
            }
        }

        /// <summary>
        /// Thrown when the Interaction Source is updated.
        /// </summary>
        /// <param name="state">The current state of the Interaction source.</param>
        private void InteractionManager_SourceUpdated(InteractionSourceState state)
        {
            if (state.source.kind == InteractionSourceKind.Hand)
            {
                if (state.source.id == currentHandState.source.id)
                {
                    currentHandState = state;
                }
            }
        }

        /// <summary>
        /// Thrown when the Interaction Source is released.
        /// </summary>
        /// <param name="state">The current state of the Interaction source.</param>
        private void InteractionManager_SourceReleased(InteractionSourceState state)
        {
            if (state.source.kind == InteractionSourceKind.Hand)
            {
                if (pressedHands.Contains(state.source.id))
                {
                    pressedHands.Remove(state.source.id);
                }
            }
        }

        /// <summary>
        /// Thrown when the Interaction Source is no longer availible.
        /// </summary>
        /// <param name="state">The current state of the Interaction source.</param>
        private void InteractionManager_SourceLost(InteractionSourceState state)
        {
            InteractionManager_SourceReleased(state);

            if (state.source.kind == InteractionSourceKind.Hand)
            {
                if (trackedHands.Contains(state.source.id))
                {
                    trackedHands.Remove(state.source.id);

                    if (HandInView != null)
                    {
                        HandInView(HandDetected);
                    }
                }
            }
        }
    }
}