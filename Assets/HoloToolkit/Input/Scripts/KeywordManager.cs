// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows.Speech;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// KeywordManager allows you to specify keywords and methods in the Unity
    /// Inspector, instead of registering them explicitly in code.
    /// This also includes a setting to either automatically start the
    /// keyword recognizer or allow your code to start it.
    ///
    /// IMPORTANT: Please make sure to add the microphone capability in your app, in Unity under
    /// Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities
    /// or in your Visual Studio Package.appxmanifest capabilities.
    /// </summary>
    public partial class KeywordManager : MonoBehaviour
    {
        [System.Serializable]
        public struct KeywordAndResponse
        {
            [Tooltip("The keyword to recognize.")]
            public string Keyword;
            [Tooltip("The KeyCode to recognize.")]
            public KeyCode KeyCode;
            [Tooltip("The UnityEvent to be invoked when the keyword is recognized.")]
            public UnityEvent Response;
        }

        // This enumeration gives the manager two different ways to handle the recognizer. Both will
        // set up the recognizer and add all keywords. The first causes the recognizer to start
        // immediately. The second allows the recognizer to be manually started at a later time.
        public enum RecognizerStartBehavior { AutoStart, ManualStart };

        [Tooltip("An enumeration to set whether the recognizer should start on or off.")]
        public RecognizerStartBehavior RecognizerStart;

        [Tooltip("An array of string keywords and UnityEvents, to be set in the Inspector.")]
        public KeywordAndResponse[] KeywordsAndResponses;

        private KeywordRecognizer keywordRecognizer;
        private Dictionary<string, UnityEvent> responses;

        void Start()
        {
            if (KeywordsAndResponses.Length > 0)
            {
                // Convert the struct array into a dictionary, with the keywords and the keys and the methods as the values.
                // This helps easily link the keyword recognized to the UnityEvent to be invoked.
                responses = KeywordsAndResponses.ToDictionary(keywordAndResponse => keywordAndResponse.Keyword,
                                                              keywordAndResponse => keywordAndResponse.Response);

                keywordRecognizer = new KeywordRecognizer(responses.Keys.ToArray());
                keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

                if (RecognizerStart == RecognizerStartBehavior.AutoStart)
                {
                    keywordRecognizer.Start();
                }
            }
            else
            {
                Debug.LogError("Must have at least one keyword specified in the Inspector on " + gameObject.name + ".");
            }
        }

        void Update()
        {
            ProcessKeyBindings();
        }

        void OnDestroy()
        {
            if (keywordRecognizer != null)
            {
                StopKeywordRecognizer();
                keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
                keywordRecognizer.Dispose();
            }
        }

        private void ProcessKeyBindings()
        {
            foreach (var kvp in KeywordsAndResponses)
            {
                if (Input.GetKeyDown(kvp.KeyCode))
                {
                    kvp.Response.Invoke();
                    return;
                }
            }
        }

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            UnityEvent keywordResponse;

            // Check to make sure the recognized keyword exists in the methods dictionary, then invoke the corresponding method.
            if (responses.TryGetValue(args.text, out keywordResponse))
            {
                keywordResponse.Invoke();
            }
        }

        /// <summary>
        /// Make sure the keyword recognizer is off, then start it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public void StartKeywordRecognizer()
        {
            if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Start();
            }
        }

        /// <summary>
        /// Make sure the keyword recognizer is on, then stop it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public void StopKeywordRecognizer()
        {
            if (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Stop();
            }
        }
    }
}