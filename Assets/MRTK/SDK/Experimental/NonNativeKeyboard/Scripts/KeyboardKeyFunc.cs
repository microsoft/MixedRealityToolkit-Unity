// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Experimental.UI
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
        [Experimental]
        [SerializeField, FormerlySerializedAs("m_ButtonFunction")] private Function buttonFunction = Function.UNDEFINED;

        public Function ButtonFunction => buttonFunction;

        /// <summary>
        /// Subscribe to the onClick event.
        /// </summary>
        private void Start()
        {
            Button m_Button = GetComponent<Button>();
            m_Button.onClick.RemoveAllListeners();
            m_Button.onClick.AddListener(FireFunctionKey);
        }

        /// <summary>
        /// Method injected into the button's onClick listener.
        /// </summary>
        private void FireFunctionKey()
        {
            NonNativeKeyboard.Instance.FunctionKey(this);
        }
    }
}
