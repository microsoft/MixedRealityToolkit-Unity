// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.UI.Keyboard
{
    /// <summary>
    /// Class that when placed on an input field will enable keyboard on click
    /// </summary>
    public class KeyboardInputField : UnityEngine.UI.InputField
    {
        /// <summary>
        /// Internal field for overriding keyboard spawn point
        /// </summary>
        [Header("Keyboard Settings")]
        public Transform m_KeyboardSpawnPoint = null;

        /// <summary>
        /// Internal field for overriding keyboard spawn point
        /// </summary>
        [HideInInspector]
        public Keyboard.LayoutType m_KeyboardLayout = Keyboard.LayoutType.Alpha;

        /// <summary>
        /// Override OnPointerClick to spawn keybaord
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            Keyboard.Instance.Close();
            Keyboard.Instance.PresentKeyboard(this.text, m_KeyboardLayout);

            if (m_KeyboardSpawnPoint != null)
            {
                Keyboard.Instance.RepositionKeyboard(m_KeyboardSpawnPoint, null, 0.045f);
            }
            else
            {
                Keyboard.Instance.RepositionKeyboard(this.transform, null, 0.045f);
            }

            // Subscribe to keyboard delgates
            Keyboard.Instance.onTextUpdated += this.Keyboard_onTextUpdated;
            Keyboard.Instance.onClosed += this.Keyboard_onClosed;
        }

        /// <summary>
        /// Delegate function for getting keyboard input
        /// </summary>
        /// <param name="newText"></param>
        private void Keyboard_onTextUpdated(string newText)
        {
            if (!string.IsNullOrEmpty(newText))
            {
                this.text = newText;
            }
        }
 
        /// <summary>
        /// Delegate function for getting keyboard input
        /// </summary>
        /// <param name="newText"></param>
        private void Keyboard_onClosed(object sender, EventArgs e)
        {
            // Unsubscribe from delegate functions
            Keyboard.Instance.onTextUpdated -= this.Keyboard_onTextUpdated;
            Keyboard.Instance.onClosed -= this.Keyboard_onClosed;
        }
    }
}
