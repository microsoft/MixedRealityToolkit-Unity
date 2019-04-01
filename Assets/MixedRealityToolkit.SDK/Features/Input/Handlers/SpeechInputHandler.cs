// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// This component handles the speech input events raised form the <see cref="IMixedRealityInputSystem"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public class SpeechInputHandler : BaseInputHandler, IMixedRealitySpeechHandler
    {
        /// <summary>
        /// The keywords to be recognized and optional keyboard shortcuts.
        /// </summary>
        public KeywordAndResponse[] Keywords => keywords;

        [SerializeField]
        [Tooltip("The keywords to be recognized and optional keyboard shortcuts.")]
        private KeywordAndResponse[] keywords = new KeywordAndResponse[0];

        [SerializeField]
        [Tooltip("Keywords are persistent across all scenes.  This Speech Input Handler instance will not be destroyed when loading a new scene.")]
        private bool persistentKeywords = false;

        private readonly Dictionary<string, UnityEvent> responses = new Dictionary<string, UnityEvent>();

        #region MonoBehaviour Implementation

        protected override void Start()
        {
            base.Start();

            if (persistentKeywords)
            {
                Debug.Assert(gameObject.transform.parent == null, "Persistent keyword GameObject must be at the root level of the scene hierarchy.");
                DontDestroyOnLoad(gameObject);
            }

            // Convert the struct array into a dictionary, with the keywords and the methods as the values.
            // This helps easily link the keyword recognized to the UnityEvent to be invoked.
            int keywordCount = keywords.Length;
            for (int index = 0; index < keywordCount; index++)
            {
                KeywordAndResponse keywordAndResponse = keywords[index];
                string keyword = keywordAndResponse.Keyword.ToLower();

                if (responses.ContainsKey(keyword))
                {
                    Debug.LogError($"Duplicate keyword \'{keyword}\' specified in \'{gameObject.name}\'.");
                }
                else
                {
                    responses.Add(keyword, keywordAndResponse.Response);
                }
            }
        }

        #endregion MonoBehaviour Implementation

        #region SpeechInputHandler public methods
        public void AddResponse(string keyword, UnityAction action)
        {
            string lowerKeyword = keyword.ToLower();
            if (!responses.ContainsKey(lowerKeyword))
            {
                responses[lowerKeyword] = new UnityEvent();
            }

            responses[lowerKeyword].AddListener(action);
        }

        public void RemoveResponse(string keyword, UnityAction action)
        {
            string lowerKeyword = keyword.ToLower();
            if(responses.ContainsKey(lowerKeyword))
            {
                responses[lowerKeyword].RemoveListener(action);
            }
        }
        #endregion SpeechInputHandler public methods

        #region IMixedRealitySpeechHandler Implementation

        void IMixedRealitySpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            UnityEvent keywordResponse;

            // Check to make sure the recognized keyword exists in the methods dictionary, then invoke the corresponding method.
            if (enabled && responses.TryGetValue(eventData.Command.Keyword.ToLower(), out keywordResponse))
            {
                keywordResponse.Invoke();
            }
        }

        #endregion  IMixedRealitySpeechHandler Implementation
    }
}
