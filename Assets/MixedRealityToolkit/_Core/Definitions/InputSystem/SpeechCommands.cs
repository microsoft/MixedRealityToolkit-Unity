// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    /// <summary>
    /// Speech command definition, defining each keyword, key code and it's corresponding logical Action the command will perform.
    /// </summary>
    [Serializable]
    public struct SpeechCommands
    {
        /// <summary>
        /// Speech commands constructor
        /// </summary>
        /// <param name="keyword">Word to be recognized</param>
        /// <param name="keyCode">Corresponding keyboard input</param>
        /// <param name="action">Logical action to be performed by the Input System</param>
        public SpeechCommands(string keyword, KeyCode keyCode, InputAction action)
        {
            this.keyword = keyword;
            this.keyCode = keyCode;
            this.action = action;
        }

        /// <summary>
        /// The input string keyword that will be listened for by the SpeechInputSource
        /// </summary>
        [SerializeField]
        [Tooltip("The Keyword to recognize.")]
        private string keyword;
        public string Keyword => keyword;

        /// <summary>
        /// The corresponding keyboard key to map to
        /// </summary>
        [SerializeField]
        [Tooltip("The KeyCode to recognize.")]
        private KeyCode keyCode;
        public KeyCode KeyCode => keyCode;

        /// <summary>
        /// The logical input system action that will be performed when the keyword is recognized
        /// </summary>
        [SerializeField]
        [Tooltip("The Action associated to the Speech Command.")]
        private InputAction action;
        public InputAction Action => action;
    }
}