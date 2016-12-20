// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HoloToolkit.Unity.InputModule
{
    public class SpeechInputHandler : MonoBehaviour, ISpeechHandler
    {
        [System.Serializable]
        public struct KeywordAndResponse
        {
            [Tooltip("The keyword to recognize.")]
            public string Keyword;
            [Tooltip("The UnityEvent to be invoked when the keyword is recognized.")]
            public UnityEvent Response;
        }

        [Tooltip("An array of string keywords and UnityEvents, to be set in the Inspector.")]
        public KeywordAndResponse[] keywords;

        private readonly Dictionary<string, UnityEvent> responses = new Dictionary<string, UnityEvent>();

        // Use this for initialization
        protected virtual void Start()
        {
            int keywordCount = keywords.Length;
            if (keywordCount > 0)
            {
                try
                {
                    // Convert the struct array into a dictionary, with the keywords and the keys and the methods as the values.
                    // This helps easily link the keyword recognized to the UnityEvent to be invoked.
                    for (int index = 0; index < keywordCount; index++)
                    {
                        KeywordAndResponse keywordAndResponse = keywords[index];
                        responses[keywordAndResponse.Keyword.ToLower()] = keywordAndResponse.Response;
                    }
                }
                catch (ArgumentException)
                {
                    Debug.LogError("Duplicate keywords specified in the Inspector on " + gameObject.name + ".");
                }
            }
            else
            {
                Debug.LogError("Must have at least one keyword specified in the Inspector on " + gameObject.name + ".");
            }
        }

        void ISpeechHandler.OnSpeechKeywordRecognized(SpeechKeywordRecognizedEventData eventData)
        {
            UnityEvent keywordResponse;

            // Check to make sure the recognized keyword exists in the methods dictionary, then invoke the corresponding method.
            if (enabled && responses.TryGetValue(eventData.RecognizedText.ToLower(), out keywordResponse))
            {
                keywordResponse.Invoke();
            }
        }
    }
}
