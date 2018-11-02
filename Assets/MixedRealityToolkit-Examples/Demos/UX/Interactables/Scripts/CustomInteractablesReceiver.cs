// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    public class CustomInteractablesReceiver : ReceiverBase
    {
        private State lastState;

        public CustomInteractablesReceiver(UnityEvent ev) : base(ev)
        {
            Name = "CustomEvent";
            HideUnityEvents = true;
        }

        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            if (state.CurrentState() != lastState)
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

            lastState = state.CurrentState();
        }
    }
}
