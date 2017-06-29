// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace HoloToolkit.Unity.InputModule
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

        protected virtual void Start()
        {
            if (PersistentKeywords)
            {
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
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

            if (IsGlobalListener)
            {
                InputManager.Instance.AddGlobalListener(gameObject);
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

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (IsGlobalListener)
            {
                StartCoroutine(AttachToInputManagerInstance());
            }
        }

        private IEnumerator AttachToInputManagerInstance()
        {
            while (InputManager.Instance == null)
            {
                yield return null;
            }

            InputManager.Instance.AddGlobalListener(gameObject);
        }
    }
}
