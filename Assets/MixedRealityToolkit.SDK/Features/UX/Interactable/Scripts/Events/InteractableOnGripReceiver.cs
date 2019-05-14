// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// A basic grip event receiver
    /// </summary>
    public class InteractableOnGripReceiver : ReceiverBase
    {
        [InspectorField(Type = InspectorField.FieldTypes.Event, Label = "On Grip Off", Tooltip = "Grip has left the object")]
        public UnityEvent OnGripOff = new UnityEvent();

        private bool hadGrip;
        private State lastState;

        public InteractableOnGripReceiver(UnityEvent ev) : base(ev)
        {
            Name = "OnGrip";
        }

        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            bool changed = state.CurrentState() != lastState;

            bool hasGrip = state.GetState(InteractableStates.InteractableStateEnum.Grip).Value > 0;

            if (hadGrip != hasGrip && changed)
            {
                if (hasGrip)
                {
                    uEvent.Invoke();
                }
                else
                {
                    OnGripOff.Invoke();
                }
            }

            hadGrip = hasGrip;
            lastState = state.CurrentState();
        }
    }
}
