// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class OnFocusEvent : MonoBehaviour, IFocusHandler
    {
        public UnityEvent FocusEnterEvent;
        public UnityEvent FocusLostEvent;
        public UnityEvent<FocusEventData> FocusChangedEvent;

        void IFocusHandler.OnFocusEnter(FocusEventData eventData)
        {
            if (FocusEnterEvent != null)
            {
                FocusEnterEvent.Invoke();
            }
        }

        void IFocusHandler.OnFocusExit(FocusEventData eventData)
        {
            if (FocusLostEvent != null)
            {
                FocusLostEvent.Invoke();
            }
        }

        void IFocusHandler.OnFocusChanged(FocusEventData eventData)
        {
            if (FocusChangedEvent != null)
            {
                FocusChangedEvent.Invoke(eventData);
            }
        }
    }
}
