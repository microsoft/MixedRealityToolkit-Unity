// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class KeywordRecognitionHandler : MonoBehaviour
    {
        [Serializable]
        public struct KeywordEvent
        {
            [SerializeField] public string Keyword;
            [SerializeField] public UnityEvent Event;
        }

        [SerializeField] private List<KeywordEvent> _keywords = new List<KeywordEvent>();

        public List<KeywordEvent> Keywords
        {
            get => _keywords;
            set
            {
                _keywords = value;
                UpdateKeywords();
            }
        }

        [SerializeField] private UnityEvent GlobalEvent;
        private KeywordRecognitionSubsystem _keywordRecognitionSubsystem;

        private void Start()
        {
            _keywordRecognitionSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<KeywordRecognitionSubsystem>();
            if (_keywordRecognitionSubsystem == null)
            {
                // TODO log warning
            }

            UpdateKeywords();
        }

        private void UpdateKeywords()
        {
            foreach (var data in _keywords)
            {
                _keywordRecognitionSubsystem.CreateOrGetEventForKeyword(data.Keyword).AddListener(() =>
                {
                    GlobalEvent?.Invoke();
                    data.Event?.Invoke();
                });
            }
        }
    }
}
