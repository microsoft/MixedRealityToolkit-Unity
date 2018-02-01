// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Very simple class that implements basic logic for a trigger button.
    /// </summary>
    public class TriggerButton : MonoBehaviour, IInputHandler
    {
        /// <summary>
        /// Indicates whether the button is clickable or not.
        /// </summary>
        [Tooltip("Indicates whether the button is clickable or not.")]
        public bool IsEnabled = true;

        public event Action ButtonPressed;

        /// <summary>
        /// Press the button programmatically.
        /// </summary>
        public void Press()
        {
            if (IsEnabled)
            {
                ButtonPressed.RaiseEvent();
            }
        }

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            // Nothing.
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            if (IsEnabled && eventData.PressType == InteractionSourcePressInfo.Select)
            {
                ButtonPressed.RaiseEvent();
                eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
            }
        }
    }
}
