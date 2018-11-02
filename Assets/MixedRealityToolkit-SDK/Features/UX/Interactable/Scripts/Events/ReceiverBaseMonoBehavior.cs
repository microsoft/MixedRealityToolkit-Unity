// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// A base class for detecting hand handling state changes from an Interactable
    /// </summary>
    public class ReceiverBaseMonoBehavior : MonoBehaviour
    {
        public enum SearchScopes { Self, Parent, Children};
        public Interactable Interactable;
        public SearchScopes InteractableSearchScope;
        protected State lastState;

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
        }

        protected virtual void Update()
        {
            if (Interactable != null && Interactable.StateManager != null)
            {
                if(Interactable.StateManager.CurrentState()!= lastState)
                {
                    OnStateChange(Interactable.StateManager, Interactable);

                    lastState = Interactable.StateManager.CurrentState();
                }
            }
        }

        /// <summary>
        /// a state has changed
        /// </summary>
        /// <param name="state"></param>
        /// <param name="source"></param>
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

            bool hasCustom = state.GetState(InteractableStates.InteractableStateEnum.Custom).Value > 0;
            */
        }
    }
}
