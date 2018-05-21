// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    [Serializable]
    public struct KeywordAndResponse
    {
        [Tooltip("The keyword to handle.")]
        public string Keyword;

        [Tooltip("The handler to be invoked.")]
        public UnityEvent Response;
    }
}