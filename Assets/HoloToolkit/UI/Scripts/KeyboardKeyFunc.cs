// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.UI.Keyboard
{
    /// <summary>
    /// Represents a key on the keyboard that has a function.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class KeyboardKeyFunc : MonoBehaviour
    {
        /// <summary>
        /// Possible functionality for a button.
        /// </summary>
        public enum Function
        {
            // Commands
            Enter,
            Tab,
            ABC,
            Symbol,
            Previous,
            Next,
            Close,
            Dictate,

            // Editing
            Shift,
            CapsLock,
            Space,
            Backspace,

            UNDEFINED,
        }

        /// <summary>
        /// Designer specified functionality of a keyboard button.
        /// </summary>
        public Function m_ButtonFunction = Function.UNDEFINED;

        /// <summary>
        /// Reference to gameobject's Button component.
        /// </summary>
        private Button m_Button = null;

        /// <summary>
        /// Get the button component.
        /// </summary>
        private void Awake()
        {
            m_Button = GetComponent<Button>();
        }

        /// <summary>
        /// Subscribe to the onClick event.
        /// </summary>
        private void Start()
        {
            m_Button.onClick.RemoveAllListeners();
            m_Button.onClick.AddListener(new UnityEngine.Events.UnityAction(FireFunctionKey));
        }

        /// <summary>
        /// Method injected into the button's onClick listener.
        /// </summary>
        private void FireFunctionKey()
        {
            Keyboard.Instance.FunctionKey(this);
        }
    }
}
