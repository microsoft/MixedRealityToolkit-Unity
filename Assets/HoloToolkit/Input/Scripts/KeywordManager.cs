// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// KeywordManager allows you to specify keywords in the Unity
    /// Inspector, instead of registering them explicitly in code.
    /// This also includes a setting to either automatically start the
    /// keyword recognizer or allow your code to start it.
    ///
    /// IMPORTANT: Please make sure to add the microphone capability in your app, in Unity under
    /// Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities
    /// or in your Visual Studio Package.appxmanifest capabilities.
    /// </summary>
    public partial class KeywordManager : Singleton<KeywordManager>
    {
        /// <summary>
        /// Occurs when a registered keyword is spoken
        /// </summary>
        /// <param name="keyword">keyword</param>
        public delegate void OnKeywordRecognizedEvent(string keyword);
        public event OnKeywordRecognizedEvent OnKeywordRecognized;

        [ System.Serializable]
        public struct KeywordAndResponse
        {
            [Tooltip("The keyword to recognize.")]
            public string Keyword;
            [Tooltip("The KeyCode to recognize.")]
            public KeyCode KeyCode;
        }

        // This enumeration gives the manager two different ways to handle the recognizer. Both will
        // set up the recognizer and add all keywords. The first causes the recognizer to start
        // immediately. The second allows the recognizer to be manually started at a later time.
        public enum RecognizerStartBehavior { AutoStart, ManualStart }

        [Tooltip("An enumeration to set whether the recognizer should start on or off.")]
        public RecognizerStartBehavior RecognizerStart;

        [Tooltip("An array of string keywords and UnityEvents, to be set in the Inspector.")]
        public KeywordAndResponse[] KeywordsAndResponses;

        private KeywordRecognizer keywordRecognizer;
        private List<string> responses = null;

        private void Start()
        {
            if (KeywordsAndResponses.Length == 0)
            {
                Debug.LogError( "Must have at least one keyword specified in the Inspector on " + gameObject.name + "." );
                return;
            }

            for (int i = 0; i < KeywordsAndResponses.Length; i++)
            {
                responses.Add( KeywordsAndResponses[i].Keyword);
            }

            keywordRecognizer = new KeywordRecognizer(responses.ToArray());
            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

            if (RecognizerStart == RecognizerStartBehavior.AutoStart)
            {
                keywordRecognizer.Start();
            }
        }

        private void Update()
        {
            ProcessKeyBindings();
        }

        private void OnDestroy()
        {
            if (keywordRecognizer != null)
            {
                StopKeywordRecognizer();
                keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
                keywordRecognizer.Dispose();
            }
        }

        /// <summary>
        /// Listens for key presses and calls the corrisponding keyword event
        /// </summary>
        private void ProcessKeyBindings()
        {
            for (int i = 0; i < KeywordsAndResponses.Length; i++)
            {
                if (Input.GetKeyDown(KeywordsAndResponses[i].KeyCode) && OnKeywordRecognized != null)
                {
                    OnKeywordRecognized(KeywordsAndResponses[i].Keyword);
                    return;
                }
            }
        }

        /// <summary>
        /// Throws the OnKeywordRecognized event if the keyword is registered in the responses list.
        /// </summary>
        /// <param name="args">The recognized keyword.</param>
        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            if (OnKeywordRecognized != null && responses.Contains(args.text))
            {
                OnKeywordRecognized(args.text.ToLower());
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