// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    [Serializable]
    public struct SpeechCommands
    {
        public SpeechCommands(string keyword, KeyCode keyCode, InputAction action)
        {
            this.keyword = keyword;
            this.keyCode = keyCode;
            this.action = action;
        }

        [SerializeField]
        [Tooltip("The Keyword to recognize.")]
        private string keyword;
        public string Keyword => keyword;

        [SerializeField]
        [Tooltip("The KeyCode to recognize.")]
        private KeyCode keyCode;
        public KeyCode KeyCode => keyCode;

        [SerializeField]
        [Tooltip("The Action associated to the Speech Command.")]
        private InputAction action;
        public InputAction Action => action;
    }
}