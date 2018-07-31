// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HoloToolkit.UI.Keyboard
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
        public Keyboard.LayoutType KeyboardLayout = Keyboard.LayoutType.Alpha;

        private const float KeyBoardPositionOffset = 0.045f;

        /// <summary>
        /// Override OnPointerClick to spawn keyboard
        /// </summary>
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (!TouchScreenKeyboard.isSupported)
            {
                Keyboard.Instance.Close();
                Keyboard.Instance.PresentKeyboard(text, KeyboardLayout);

                if (KeyboardSpawnPoint != null)
                {
                    Keyboard.Instance.RepositionKeyboard(KeyboardSpawnPoint, null, KeyBoardPositionOffset);
                }
                else
                {
                    Keyboard.Instance.RepositionKeyboard(transform, null, KeyBoardPositionOffset);
                }

                // Subscribe to keyboard delegates
                Keyboard.Instance.OnTextUpdated += Keyboard_OnTextUpdated;
                Keyboard.Instance.OnClosed += Keyboard_OnClosed;
            }
        }

        /// <summary>
        /// Delegate function for getting keyboard input
        /// </summary>
        /// <param name="newText"></param>
        private void Keyboard_OnTextUpdated(string newText)
        {
            text = newText;
        }

        /// <summary>
        /// Delegate function for getting keyboard input
        /// </summary>
        /// <param name="sender"></param>
        private void Keyboard_OnClosed(object sender, EventArgs e)
        {
            // Unsubscribe from delegate functions
            Keyboard.Instance.OnTextUpdated -= Keyboard_OnTextUpdated;
            Keyboard.Instance.OnClosed -= Keyboard_OnClosed;
        }
    }
}
