// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MixedRealityToolkit.UX.Keyboard
{
    /// <summary>
    /// Class that when placed on an input field will enable keyboard on click
    /// </summary>
    public class KeyboardInputField : InputField
    {
        /// <summary>
        /// Internal field for overriding keyboard spawn point
        /// </summary>
        [Header("Keyboard Settings")]
        public Transform KeyboardSpawnPoint;

        /// <summary>
        /// Internal field for overriding keyboard spawn point
        /// </summary>
        [HideInInspector]
        public KeyboardManager.LayoutType KeyboardLayout = KeyboardManager.LayoutType.Alpha;

        private const float KeyBoardPositionOffset = 0.045f;

        /// <summary>
        /// Override OnPointerClick to spawn keyboard
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            KeyboardManager.Instance.Close();
            KeyboardManager.Instance.PresentKeyboard(text, KeyboardLayout);

            if (KeyboardSpawnPoint != null)
            {
                KeyboardManager.Instance.RepositionKeyboard(KeyboardSpawnPoint, null, KeyBoardPositionOffset);
            }
            else
            {
                KeyboardManager.Instance.RepositionKeyboard(transform, null, KeyBoardPositionOffset);
            }

            // Subscribe to keyboard delegates
            KeyboardManager.Instance.OnTextUpdated += Keyboard_OnTextUpdated;
            KeyboardManager.Instance.OnClosed += Keyboard_OnClosed;
        }

        /// <summary>
        /// Delegate function for getting keyboard input
        /// </summary>
        /// <param name="newText"></param>
        private void Keyboard_OnTextUpdated(string newText)
        {
            if (!string.IsNullOrEmpty(newText))
            {
                text = newText;
            }
        }

        /// <summary>
        /// Delegate function for getting keyboard input
        /// </summary>
        /// <param name="sender"></param>
        private void Keyboard_OnClosed(object sender, EventArgs e)
        {
            // Unsubscribe from delegate functions
            KeyboardManager.Instance.OnTextUpdated -= Keyboard_OnTextUpdated;
            KeyboardManager.Instance.OnClosed -= Keyboard_OnClosed;
        }
    }
}
