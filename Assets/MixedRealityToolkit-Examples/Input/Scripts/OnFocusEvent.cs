// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.InputHandlers;
using UnityEngine;
using UnityEngine.Events;

namespace MixedRealityToolkit.Examples.InputModule
{
    public class OnFocusEvent : FocusTarget
    {
        public UnityEvent FocusEnterEvent;
        public UnityEvent FocusLostEvent;
        public UnityEvent<FocusEventData> FocusChangedEvent;

        public override void OnFocusEnter(FocusEventData eventData)
        {
            base.OnFocusEnter(eventData);

            if (FocusEnterEvent != null)
            {
                FocusEnterEvent.Invoke();
            }
        }

        public override void OnFocusExit(FocusEventData eventData)
        {
            base.OnFocusExit(eventData);

            if (FocusLostEvent != null)
            {
                FocusLostEvent.Invoke();
            }
        }

        public override void OnBeforeFocusChange(FocusEventData eventData)
        {
            base.OnBeforeFocusChange(eventData);

            if (FocusChangedEvent != null)
            {
                FocusChangedEvent.Invoke(eventData);
            }
        }
    }
}
