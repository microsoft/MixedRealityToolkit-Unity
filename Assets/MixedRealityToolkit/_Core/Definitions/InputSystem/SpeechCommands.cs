// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    /// <summary>
    /// Data structure for mapping Voice and Keyboard input to <see cref="InputAction"/>s that can be raised by the Input System.
    /// </summary>
    [Serializable]
    public struct SpeechCommands : ISerializationCallbackReceiver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="keyword">The Keyword.</param>
        /// <param name="keyCode">The KeyCode.</param>
        /// <param name="action">The Action to perform when Keyword or KeyCode is recognized.</param>
        public SpeechCommands(string keyword, KeyCode keyCode, InputAction action)
        {
            this.keyword = keyword;
            this.keyCode = keyCode;
            this.action = action;
            Action = action;
        }

        [SerializeField]
        [Tooltip("The Keyword to listen for.")]
        private string keyword;

        /// <summary>
        /// The Keyword to listen for.
        /// </summary>
        public string Keyword => keyword;

        [SerializeField]
        [Tooltip("The corresponding KeyCode that also raises the same action as the Keyword.")]
        private KeyCode keyCode;

        /// <summary>
        /// The corresponding KeyCode that also raises the same action as the Keyword.
        /// </summary>
        public KeyCode KeyCode => keyCode;

        [SerializeField]
        [Tooltip("The Action that is raised by either the Keyword or KeyCode.")]
        private InputAction action;

        /// <summary>
        /// The <see cref="InputAction"/> that is raised by either the Keyword or KeyCode.
        /// </summary>
        public IMixedRealityInputAction Action { get; private set; }

        public void OnBeforeSerialize()
        {
            action = (InputAction)Action;
        }

        public void OnAfterDeserialize()
        {
            Action = action;
        }
    }
}