// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.Utilities
{
    [Serializable]
    public struct KeywordAndKeyCode
    {
        [Tooltip("The keyword to recognize.")]
        public string Keyword;
        [Tooltip("The KeyCode to recognize.")]
        public KeyCode KeyCode;
    }
}