// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows.Speech;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// SpeechInputSource allows you to specify keywords and methods in the Unity
    /// Inspector, instead of registering them explicitly in code.
    /// This also includes a setting to either automatically start the
    /// keyword recognizer or allow your code to start it.
    ///
    /// IMPORTANT: Please make sure to add the microphone capability in your app, in Unity under
    /// Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities
    /// or in your Visual Studio Package.appxmanifest capabilities.
    /// </summary>
    public partial class SpeechInputSource : BaseInputSource
    {
        [Serializable]
        public struct KeywordAndKeyCode
        {
            [Tooltip("The keyword to recognize.")]
            public string Keyword;
            [Tooltip("The KeyCode to recognize.")]
            public KeyCode KeyCode;
        }

        // This enumeration gives the manager two different ways to handle the recognizer. Both will
        // set up the recognizer and add all keywords. The first causes the recognizer to start
        // immediately. The second allows the recognizer to be manually started at a later time.
        public enum RecognizerStartBehavior { AutoStart, ManualStart };

        [Tooltip("Whether the recognizer should be activated on start.")]
        public RecognizerStartBehavior RecognizerStart;

        [Tooltip("The keywords to be recognized and optional keyboard shortcuts.")]
        public KeywordAndKeyCode[] Keywords;

        private KeywordRecognizer keywordRecognizer;


        protected override void Start()
        {
            base.Start();

            int keywordCount = Keywords.Length;
            if (keywordCount > 0)
            {
                string[] keywords = new string[keywordCount];

                for (int index = 0; index < keywordCount; index++)
                {
                    keywords[index] = Keywords[index].Keyword;
                }

                keywordRecognizer = new KeywordRecognizer(keywords);
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

        protected virtual void Update()
        {
            if (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                ProcessKeyBindings();
            }
        }

        protected virtual void OnDestroy()
        {
            if (keywordRecognizer != null)
            {
                StopKeywordRecognizer();
                keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
                keywordRecognizer.Dispose();
            }
        }

        protected virtual void OnDisable()
        {
            if (keywordRecognizer != null)
            {
                StopKeywordRecognizer();
            }
        }

        protected virtual void OnEnable()
        {
            if (keywordRecognizer != null && RecognizerStart == RecognizerStartBehavior.AutoStart)
            {
                StartKeywordRecognizer();
            }
        }

        private void ProcessKeyBindings()
        {
            for (int index = Keywords.Length; --index >= 0;)
            {
                if (Input.GetKeyDown(Keywords[index].KeyCode))
                {
                    OnPhraseRecognized(ConfidenceLevel.High, TimeSpan.Zero, DateTime.Now, null, Keywords[index].Keyword);
                }
            }
        }

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            OnPhraseRecognized(args.confidence, args.phraseDuration, args.phraseStartTime, args.semanticMeanings, args.text);
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

        protected void OnPhraseRecognized(ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, SemanticMeaning[] semanticMeanings, string text)
        {
            InputManager.Instance.RaiseSpeechKeywordPhraseRecognized(this, 0, confidence, phraseDuration, phraseStartTime, semanticMeanings, text);
        }

        public override bool TryGetPosition(uint sourceId, out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        public override bool TryGetOrientation(uint sourceId, out Quaternion orientation)
        {
            orientation = Quaternion.identity;
            return false;
        }

        public override SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            return SupportedInputInfo.None;
        }
    }
}