// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
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

        private bool hasDown;
        private State lastState;

        public InteractableOnPressReceiver(UnityEvent ev) : base(ev)
        {
            Name = "OnPress";
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
                    uEvent.Invoke();
                }
                else
                {
                    OnRelease.Invoke();
                }
            }
            
            lastState = state.CurrentState();
        }
    }
}
