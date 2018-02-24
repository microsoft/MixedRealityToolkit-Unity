// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common.Extensions;
using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MixedRealityToolkit.InputModule.Utilities.Interations
{
    public class SpeechInputHandler : MonoBehaviour, ISpeechHandler
    {
        [Serializable]
        public struct KeywordAndResponse
        {
            [Tooltip("The keyword to handle.")]
            public string Keyword;

            [Tooltip("The handler to be invoked.")]
            public UnityEvent Response;
        }

        /// <summary>
        /// The keywords to be recognized and optional keyboard shortcuts.
        /// </summary>
        [Tooltip("The keywords to be recognized and optional keyboard shortcuts.")]
        public KeywordAndResponse[] Keywords;

        /// <summary>
        /// Determines if this handler is a global listener, not connected to a specific GameObject.
        /// </summary>
        [Tooltip("Determines if this handler is a global listener, not connected to a specific GameObject.")]
        public bool IsGlobalListener;

        /// <summary>
        /// Keywords are persistent across all scenes.  This Speech Input Source instance will not be destroyed when loading a new scene.
        /// </summary>
        [Tooltip("Keywords are persistent across all scenes.  This Speech Input Handler instance will not be destroyed when loading a new scene.")]
        public bool PersistentKeywords;

        [NonSerialized]
        private readonly Dictionary<string, UnityEvent> responses = new Dictionary<string, UnityEvent>();

        protected virtual void OnEnable()
        {
            if (IsGlobalListener)
            {
                InputManager.Instance.AddGlobalListener(gameObject);
            }
        }

        protected virtual void Start()
        {
            if (PersistentKeywords)
            {
                gameObject.DontDestroyOnLoad();
            }

            // Convert the struct array into a dictionary, with the keywords and the methods as the values.
            // This helps easily link the keyword recognized to the UnityEvent to be invoked.
            int keywordCount = Keywords.Length;
            for (int index = 0; index < keywordCount; index++)
            {
                KeywordAndResponse keywordAndResponse = Keywords[index];
                string keyword = keywordAndResponse.Keyword.ToLower();

                if (responses.ContainsKey(keyword))
                {
                    Debug.LogError("Duplicate keyword '" + keyword + "' specified in '" + gameObject.name + "'.");
                }
                else
                {
                    responses.Add(keyword, keywordAndResponse.Response);
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (InputManager.Instance != null && IsGlobalListener)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (InputManager.Instance != null && IsGlobalListener)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }

        void ISpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
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
