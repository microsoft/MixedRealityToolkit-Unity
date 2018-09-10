// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem
{
    /// <summary>
    /// Data structure for mapping Voice and Keyboard input to <see cref="MixedRealityInputAction"/>s that can be raised by the Input System.
    /// </summary>
    [Serializable]
    public struct SpeechCommands
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="keyword">The Keyword.</param>
        /// <param name="keyCode">The KeyCode.</param>
        /// <param name="action">The Action to perform when Keyword or KeyCode is recognized.</param>
        public SpeechCommands(string keyword, KeyCode keyCode, MixedRealityInputAction action)
        {
            this.keyword = keyword;
            this.keyCode = keyCode;
            this.action = action;
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
        private MixedRealityInputAction action;

        /// <summary>
        /// The <see cref="MixedRealityInputAction"/> that is raised by either the Keyword or KeyCode.
        /// </summary>
        public MixedRealityInputAction Action => action;
    }
}