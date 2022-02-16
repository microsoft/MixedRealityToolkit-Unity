﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Keyword/UnityEvent pair that ties voice input to UnityEvents wired up in the inspector.
    /// </summary>
    [Serializable]
    public struct KeywordAndResponse
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="keyword">The keyword to listen for.</param>
        /// <param name="response">The handler to be invoked when the keyword is recognized.</param>
        public KeywordAndResponse(string keyword, UnityEvent response)
        {
            this.keyword = keyword;
            this.response = response;
        }

        [SerializeField]
        [Tooltip("The keyword to listen for.")]
        [SpeechKeyword]
        private string keyword;

        /// <summary>
        /// The keyword to listen for.
        /// </summary>
        public string Keyword => keyword;

        [SerializeField]
        [Tooltip("The handler to be invoked when the keyword is recognized.")]
        private UnityEvent response;

        /// <summary>
        /// The handler to be invoked when the keyword is recognized.
        /// </summary>
        public UnityEvent Response => response;
    }
}