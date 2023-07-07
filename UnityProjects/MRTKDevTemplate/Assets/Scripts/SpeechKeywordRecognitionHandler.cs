// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    public class SpeechKeywordRecognitionHandler : MonoBehaviour
    {
        [Serializable]
        public struct KeywordEvent
        {
            [SerializeField] public string Keyword;
            [SerializeField] public UnityEvent Event;
        }

        [SerializeField] private List<KeywordEvent> keywords = new List<KeywordEvent>();

        public List<KeywordEvent> Keywords
        {
            get => keywords;
            set
            {
                keywords = value;
                UpdateKeywords();
            }
        }

        [SerializeField] private UnityEvent globalEvent;

        private KeywordRecognitionSubsystem keywordRecognitionSubsystem;

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
