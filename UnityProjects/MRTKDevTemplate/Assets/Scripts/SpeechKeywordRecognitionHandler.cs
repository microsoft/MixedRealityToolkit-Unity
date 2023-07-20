// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// A helper for registering keywords a <see cref="KeywordRecognitionSubsystem"/>.
    /// </summary>
    /// <remarks>
    /// When a <see cref="KeywordRecognitionSubsystem"/> recognizes one of the specified keywords, this
    /// will raise Unity events that consumers can respond too. See <see cref="KeywordEvent"/> for more
    /// details.
    /// </remarks>
    public class SpeechKeywordRecognitionHandler : MonoBehaviour
    {
        /// <summary>
        /// A structure holding a Unity event that will be raised when the corresponding keyword is recognized.
        /// </summary>
        [Serializable]
        public struct KeywordEvent
        {
            /// <summary>
            /// The keyword that a <see cref="KeywordRecognitionSubsystem"/> will listen for.
            /// </summary>
            [SerializeField] 
            [Tooltip("The keyword that the KeywordRecognitionSubsystem will listen for.")]
            public string Keyword;

            /// <summary>
            /// The event raised when a <see cref="KeywordRecognitionSubsystem"/> recognizes the <see cref="Keyword"/>.
            /// </summary>
            [SerializeField]             
            [Tooltip("The event raised when a KeywordRecognitionSubsystem recognizes the Keyword.")]
            public UnityEvent Event;
        }

        [SerializeField]
        [Tooltip("Get or set the list of keywords that a KeywordRecognitionSubsystem will listen for.")]
        private List<KeywordEvent> keywords = new List<KeywordEvent>();

        /// <summary>
        /// Get or set the list of keywords that a <see cref="KeywordRecognitionSubsystem"/> will listen for. 
        /// </summary>
        public List<KeywordEvent> Keywords
        {
            get => keywords;
            set
            {
                keywords = value;
                UpdateKeywords();
            }
        }

        [SerializeField] 
        [Tooltip("A Unity event that will be raised when any keyword is recognized.")]
        private UnityEvent globalEvent;

        private KeywordRecognitionSubsystem keywordRecognitionSubsystem;

        /// <summary>
        /// A Unity event function that is called on the frame when a script is enabled just before any of the update methods are called the first time.
        /// </summary> 
        private void Start()
        {
            keywordRecognitionSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<KeywordRecognitionSubsystem>();
            if (keywordRecognitionSubsystem == null)
            {
                Debug.LogWarning("No keyword subsystem detected.");
            }

            UpdateKeywords();
        }

        private void UpdateKeywords()
        {
            foreach (var data in keywords)
            {
                keywordRecognitionSubsystem.CreateOrGetEventForKeyword(data.Keyword).AddListener(() =>
                {
                    globalEvent?.Invoke();
                    data.Event?.Invoke();
                });
            }
        }
    }
}
