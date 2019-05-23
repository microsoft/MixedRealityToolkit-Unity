// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A basic touch event receiver for detecting Physical Touch state changes in the Interactable
    /// When the physical touch states change, these events are triggered.
    /// </summary>
    public class InteractableOnTouchReceiver : ReceiverBase
    {
        [InspectorField(Type = InspectorField.FieldTypes.Event, Label = "On Touch End", Tooltip = "Touch has left the object")]
        public UnityEvent OnTouchEnd = new UnityEvent();

        private bool hadTouch;
        private State lastState;

        public InteractableOnTouchReceiver(UnityEvent ev) : base(ev)
        {
            Name = "OnTouch";
        }

        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            bool hadTouch = state.GetState(InteractableStates.InteractableStateEnum.PhysicalTouch).Value > 0;

            if (this.hadTouch != hadTouch)
            {
                if (hadTouch)
                {
                    uEvent.Invoke();
                }
                else
                {
                    OnTouchEnd.Invoke();
                }
            }

            this.hadTouch = hadTouch;
            lastState = state.CurrentState();
        }
    }
}
