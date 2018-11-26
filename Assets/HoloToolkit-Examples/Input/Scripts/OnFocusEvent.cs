// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class OnFocusEvent : MonoBehaviour, IFocusable
    {
        public UnityEvent FocusEnterEvent;
        public UnityEvent FocusLostEvent;

        public void OnFocusEnter()
        {
            if (FocusEnterEvent != null)
            {
                FocusEnterEvent.Invoke();
            }
        }

        public void OnFocusExit()
        {
            if (FocusLostEvent != null)
            {
                FocusLostEvent.Invoke();
            }
        }
    }
}
