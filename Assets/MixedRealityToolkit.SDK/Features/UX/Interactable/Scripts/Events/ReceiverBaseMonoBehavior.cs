// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A base class for detecting hand handling state changes from an Interactable
    /// Extend this class to build new events or receivers from Interactables
    /// 
    /// InteractableReceiver or InteractableReceiverList can be used with ReceiverBase - built-in receivers
    /// </summary>
    public class ReceiverBaseMonoBehavior : MonoBehaviour, IInteractableHandler
    {
        /// <summary>
        /// Types of directions to search for Interactable component in transform hierarchy
        /// </summary>
        public enum SearchScopes { Self, Parent, Children };

        /// <summary>
        /// Interactable to target
        /// </summary>
        public Interactable Interactable;

        /// <summary>
        /// directions to search for Interactable component in transform hierarchy if none assigned
        /// </summary>
        public SearchScopes InteractableSearchScope;

        protected State lastState;

        /// <summary>
        /// look for an Interactable if not assigned
        /// </summary>
        protected virtual void OnEnable()
        {
            if (Interactable == null)
            {
                switch (InteractableSearchScope)
                {
                    case SearchScopes.Self:
                        Interactable = GetComponent<Interactable>();
                        break;
                    case SearchScopes.Parent:
                        Interactable = GetComponentInParent<Interactable>();
                        break;
                    case SearchScopes.Children:
                        Interactable = GetComponentInChildren<Interactable>();
                        break;
                    default:
                        break;
                }
            }

            if (Interactable != null)
            {
                Interactable.AddHandler(this);
            }
        }

        /// <summary>
        /// Add an interactable and add it as a handler
        /// </summary>
        public void AddInteractable(Interactable interactable)
        {
            if (Interactable != null)
            {
                Interactable.RemoveHandler(this);
            }

            Interactable = interactable;
            Interactable.AddHandler(this);
        }

        /// <summary>
        /// Remove itself as a handler
        /// </summary>
        protected virtual void OnDisable()
        {
            if (Interactable == null)
            {
                Interactable.RemoveHandler(this);
            }
        }

        protected virtual void Update()
        {
            if (Interactable != null && Interactable.StateManager != null)
            {
                if (Interactable.StateManager.CurrentState() != lastState)
                {
                    OnStateChange(Interactable.StateManager, Interactable);

                    lastState = Interactable.StateManager.CurrentState();
                }
            }
        }

        /// <summary>
        /// a state has changed
        /// </summary>
        public virtual void OnStateChange(InteractableStates state, Interactable source)
        {
            // the state has changed, do something new
            /*
            bool hasDown = state.GetState(InteractableStates.InteractableStateEnum.Pressed).Value > 0;

            bool focused = state.GetState(InteractableStates.InteractableStateEnum.Focus).Value > 0;

            bool isDisabled = state.GetState(InteractableStates.InteractableStateEnum.Disabled).Value > 0;

            bool hasInteractive = state.GetState(InteractableStates.InteractableStateEnum.Interactive).Value > 0;

            bool hasObservation = state.GetState(InteractableStates.InteractableStateEnum.Observation).Value > 0;

            bool hasObservationTargeted = state.GetState(InteractableStates.InteractableStateEnum.ObservationTargeted).Value > 0;

            bool isTargeted = state.GetState(InteractableStates.InteractableStateEnum.Targeted).Value > 0;

            bool isToggled = state.GetState(InteractableStates.InteractableStateEnum.Toggled).Value > 0;

            bool isVisited = state.GetState(InteractableStates.InteractableStateEnum.Visited).Value > 0;

            bool isDefault = state.GetState(InteractableStates.InteractableStateEnum.Default).Value > 0;

            bool hasGesture = state.GetState(InteractableStates.InteractableStateEnum.Gesture).Value > 0;

            bool hasGestureMax = state.GetState(InteractableStates.InteractableStateEnum.GestureMax).Value > 0;

            bool hasCollistion = state.GetState(InteractableStates.InteractableStateEnum.Collision).Value > 0;

            bool hasCollistion = state.GetState(InteractableStates.InteractableStateEnum.VoiceCommand).Value > 0;

            bool hasCustom = state.GetState(InteractableStates.InteractableStateEnum.Custom).Value > 0;
            */
        }

        /// <inheritdoc />
        public virtual void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1)
        {
            // Voice Command Happened
        }

        /// <inheritdoc />
        public virtual void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null)
        {
            // Click Happened
        }
    }
}
