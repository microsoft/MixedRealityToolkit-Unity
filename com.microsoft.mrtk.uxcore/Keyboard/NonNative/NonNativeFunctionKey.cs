// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// A simple general use keyboard that is ideal for AR/VR applications that do not provide a native keyboard.
    /// </summary>
    public class NonNativeFunctionKey : NonNativeKey
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
            Close,

            // Editing
            Shift,
            CapsLock,
            Space,
            Backspace,

            UNDEFINED,
        }

        [field: SerializeField, Tooltip("The type of this button.")]
        public Function ButtonFunction { get; private set; } = Function.UNDEFINED;

        protected override void FireKey()
        {
            NonNativeKeyboard.Instance.ProcessFunctionKeyPress(this);
        }
    }
}
