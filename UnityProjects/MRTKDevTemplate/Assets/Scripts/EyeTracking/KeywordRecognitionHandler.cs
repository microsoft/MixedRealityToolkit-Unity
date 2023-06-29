// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using Microsoft.MixedReality.Toolkit.Subsystems;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Sample for binding speech recognition keywords to Unity Event functions.
    /// </summary>
    public class KeywordRecognitionHandler : MonoBehaviour
    {
        [Serializable]
        public struct KeywordEvent
        {
            [SerializeField]
            public string Keyword;

            [SerializeField]
            public UnityEvent Event;
        }

        [Tooltip("List of speech recognition keywords and events to trigger.")]
        [SerializeField]
        private List<KeywordEvent> _keywords = new List<KeywordEvent>();

        public List<KeywordEvent> Keywords
        {
            get => _keywords;
            set
            {
                _keywords = value;
                UpdateKeywords();
            }
        }

        private IKeywordRecognitionSubsystem _keywordRecognitionSubsystem;

        private void Start()
        {
            _keywordRecognitionSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<IKeywordRecognitionSubsystem>();
            if (_keywordRecognitionSubsystem == null)
            {
                Debug.LogWarning("No keyword subsystem detected.");
            }

            UpdateKeywords();
        }

        private void UpdateKeywords()
        {
            foreach (var data in _keywords)
            {
                _keywordRecognitionSubsystem.CreateOrGetEventForKeyword(data.Keyword).AddListener(() =>
                {
                    data.Event?.Invoke();
                });
            }
        }
    }
}
