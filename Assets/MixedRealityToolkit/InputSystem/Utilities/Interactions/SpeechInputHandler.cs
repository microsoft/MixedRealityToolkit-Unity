// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.EventData;
using Microsoft.MixedReality.Toolkit.InputSystem.InputHandlers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Utilities.Interactions
{
    public class SpeechInputHandler : MonoBehaviour, ISpeechHandler
    {

        /// <summary>
        /// The keywords to be recognized and optional keyboard shortcuts.
        /// </summary>
        public KeywordAndResponse[] Keywords => keywords;
        [SerializeField]
        [Tooltip("The keywords to be recognized and optional keyboard shortcuts.")]
        private KeywordAndResponse[] keywords = new KeywordAndResponse[0];

        /// <summary>
        /// Determines if this handler is a global listener, not connected to a specific GameObject.
        /// </summary>
        [Tooltip("Determines if this handler is a global listener, not connected to a specific GameObject.")]
        [SerializeField]
        private bool isGlobalListener;

        /// <summary>
        /// Keywords are persistent across all scenes.  This Speech Input Source instance will not be destroyed when loading a new scene.
        /// </summary>
        [Tooltip("Keywords are persistent across all scenes.  This Speech Input Handler instance will not be destroyed when loading a new scene.")]
        [SerializeField]
        private bool persistentKeywords;

        [NonSerialized]
        private readonly Dictionary<string, UnityEvent> responses = new Dictionary<string, UnityEvent>();

        #region Monobehaviour Implementation

        protected virtual void OnEnable()
        {
            if (isGlobalListener)
            {
                MixedRealityInputManager.AddGlobalListener(gameObject);
            }
        }

        protected virtual void Start()
        {
            if (persistentKeywords)
            {
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
                    DebugUtilities.DebugLogError($"Duplicate keyword \'{keyword}\' specified in \'{gameObject.name}\'.");
                }
                else
                {
                    responses.Add(keyword, keywordAndResponse.Response);
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (isGlobalListener)
            {
                MixedRealityInputManager.RemoveGlobalListener(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (isGlobalListener)
            {
                MixedRealityInputManager.RemoveGlobalListener(gameObject);
            }
        }

        #endregion Monobehaviour Implementation

        #region ISpeechHandler Implementation

        void ISpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            UnityEvent keywordResponse;

            // Check to make sure the recognized keyword exists in the methods dictionary, then invoke the corresponding method.
            if (enabled && responses.TryGetValue(eventData.RecognizedText.ToLower(), out keywordResponse))
            {
                keywordResponse.Invoke();
            }
        }

        #endregion  ISpeechHandler Implementation
    }
}
