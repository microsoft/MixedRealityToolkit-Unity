// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.InspectorFields;
using Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Interactable.Events
{
    /// <summary>
    /// A basic focus event receiver
    /// </summary>
    public class InteractableOnFocusReceiver : ReceiverBase
    {
        [InspectorField(Type = InspectorField.FieldTypes.Event, Label = "On Focus Off", Tooltip = "Focus has left the object")]
        public UnityEvent OnFocusOff = new UnityEvent();

        private bool hadFocus;
        private State lastState;

        public InteractableOnFocusReceiver(UnityEvent ev) : base(ev)
        {
            Name = "OnFocus";
        }

        public override void OnUpdate(InteractableStates state, Interactable source)
        {
            bool changed = state.CurrentState() != lastState;

            bool hasFocus = state.GetState(InteractableStates.InteractableStateEnum.Focus).Value > 0;

            if (hadFocus != hasFocus && changed)
            {
                if (hasFocus)
                {
                    uEvent.Invoke();
                }
                else
                {
                    OnFocusOff.Invoke();
                }
            }

            hadFocus = hasFocus;
            lastState = state.CurrentState();
        }
    }
}
