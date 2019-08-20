// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Basic press event receiver
    /// </summary>
    public class InteractableOnPressReceiver : ReceiverBase
    {

        [InspectorField(Type = InspectorField.FieldTypes.Event, Label = "On Release", Tooltip = "The button is released")]
        public UnityEvent OnRelease = new UnityEvent();

        public enum InteractionType
        {
            NearAndFar = 0,
            NearOnly = 1,
            FarOnly = 2
        }

        [InspectorField(Label = "Interaction Filter", Tooltip = "Specify whether press event is for near or far interaction", Type = InspectorField.FieldTypes.DropdownInt, Options = new string[] { "Near and Far", "Near Only", "Far Only" })]
        public int InteractionFilter = (int)InteractionType.NearAndFar;

        private bool hasDown;
        private State lastState;

        private bool isNear = false;

        public InteractableOnPressReceiver(UnityEvent ev) : base(ev)
        {
            Name = "OnPress";
        }

        /// <summary>
        /// checks if the received interactable state matches the press filter
        /// </summary>
        /// <returns>true if interactable state matches filter</returns>
        private bool IsFilterValid()
        {
            if (InteractionFilter == (int)InteractionType.FarOnly && isNear
                || InteractionFilter == (int)InteractionType.NearOnly && !isNear)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            bool changed = state.CurrentState() != lastState;

            bool hadDown = hasDown;
            hasDown = state.GetState(InteractableStates.InteractableStateEnum.Pressed).Value > 0;


            if (changed && hasDown != hadDown)
            {
                if (hasDown)
                {
                    isNear = state.GetState(InteractableStates.InteractableStateEnum.PhysicalTouch).Value > 0;
                    if (IsFilterValid())
                    {
                        uEvent.Invoke();
                    }
                }
                else
                {
                    if (IsFilterValid())
                    {
                        OnRelease.Invoke();
                    }
                }
            }
            
            lastState = state.CurrentState();
        }
    }
}
