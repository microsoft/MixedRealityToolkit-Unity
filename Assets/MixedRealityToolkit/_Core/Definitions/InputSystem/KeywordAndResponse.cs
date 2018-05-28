// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem
{
    /// <summary>
    /// Key to Unity event mapping definition
    /// </summary>
    [Serializable]
    public struct KeywordAndResponse
    {
        /// <summary>
        /// Keyword to look for and link to
        /// </summary>
        [Tooltip("The keyword to handle.")]
        public string Keyword;

        /// <summary>
        /// The UnityEvent to be invoked when the keyword is recognized
        /// </summary>
        [Tooltip("The handler to be invoked.")]
        public UnityEvent Response;
    }
}