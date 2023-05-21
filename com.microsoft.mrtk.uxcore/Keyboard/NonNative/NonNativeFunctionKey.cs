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
            /// <summary>
            /// No valid function key has been set.
            /// </summary>
            Undefined = 0,
            /// <summary>
            /// If SubmitOnEnter is enabled, this function key closes the keyboard. Otherwise, adds a new line. 
            /// </summary>
            Enter = 1,
            /// <summary>
            /// Adds a tab.
            /// </summary>
            Tab = 2,
            /// <summary>
            /// Switches from the symbol key section to the alpha key section.
            /// </summary>
            Alpha = 3,
            /// <summary>
            /// Switches from the alpha key section to the symbol key section.
            /// </summary>
            Symbol = 4,
            /// <summary>
            /// Moves the carat back one index.
            /// </summary>
            Previous = 5,
            /// <summary>
            /// Moves the carat forward one index.
            /// </summary>
            Next = 6,
            /// <summary>
            /// Shifts all of the NonNativeValueKeys until the next character is typed.
            /// </summary>
            Shift = 7,
            /// <summary>
            /// Shifts all of the NonNativeValueKeys until CapsLock is disabled.
            /// </summary>
            CapsLock = 8,
            /// <summary>
            /// Adds a space.
            /// </summary>
            Space = 9,
            /// <summary>
            /// Deletes the previous character, or the selected characters.
            /// </summary>
            Backspace = 10,
            /// <summary>
            /// Closes the keyboard. 
            /// </summary>
            Close = 11,
            /// <summary>
            /// Starts and ends dictation.
            /// </summary>
            Dictate = 12,
        }

        /// <summary>
        /// The function of this key.
        /// </summary>
        [field: SerializeField, Tooltip("The function of this key.")]
        public Function KeyFunction { get; set; } = Function.Undefined;

        /// <inheritdoc/>
        protected override void FireKey()
        {
            NonNativeKeyboard.Instance.ProcessFunctionKeyPress(this);
        }
    }
}
