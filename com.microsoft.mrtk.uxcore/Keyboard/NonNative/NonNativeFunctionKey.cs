// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Class representing a function key in the non native keyboard
    /// </summary>
    public class NonNativeFunctionKey : NonNativeKey
    {
        /// <summary>
        /// Possible functionalities for a function key.
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

            UNDEFINED = 255,
        }

        /// <summary>
        /// The function of this key.
        /// </summary>
        [field: SerializeField, Tooltip("The function of this key.")]
        public Function KeyFunction { get; private set; } = Function.UNDEFINED;

        /// <inheritdoc/>
        protected override void FireKey()
        {
            NonNativeKeyboard.Instance.ProcessFunctionKeyPress(this);
        }
    }
}
