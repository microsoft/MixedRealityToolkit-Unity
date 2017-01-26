// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Very simple class that implements basic logic for a trigger button.
    /// </summary>
    public class TriggerButton : MonoBehaviour, IInputClickHandler
    {
        /// <summary>
        /// Indicates whether the button is clickable or not.
        /// </summary>
        [Tooltip("Indicates whether the button is clickable or not.")]
        public bool IsEnabled = true;

        public event Action ButtonPressed;

        /// <summary>
        /// Press the button programatically.
        /// </summary>
        public void Press()
        {
            if (IsEnabled)
            {
                ButtonPressed.RaiseEvent();
            }
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (IsEnabled)
            {
                ButtonPressed.RaiseEvent();
            }
        }
    }
}
