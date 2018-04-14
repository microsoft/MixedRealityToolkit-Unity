// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.MixedReality.Toolkit.InputSystem.InputMapping;
using Microsoft.MixedReality.Toolkit.InputSystem.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;

#if UNITY_WSA || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif

namespace Microsoft.MixedReality.Toolkit.InputSystem.InputSources
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
    public class SpeechInputSource : BaseInputSource
    {
        /// <summary>
        /// This enumeration gives the manager two different ways to handle the recognizer. Both will
        /// set up the recognizer and add all keywords. The first causes the recognizer to start
        /// immediately. The second allows the recognizer to be manually started at a later time.
        /// </summary>
        private enum RecognizerStartBehavior { AutoStart, ManualStart }

        /// <summary>
        /// Keywords are persistent across all scenes.  This Speech Input Source instance will not be destroyed when loading a new scene.
        /// </summary>
        [Tooltip("Keywords are persistent across all scenes.  This Speech Input Source instance will not be destroyed when loading a new scene.")]
        [SerializeField]
        private bool persistentKeywords;

        [Tooltip("Whether the recognizer should be activated on start.")]
        [SerializeField]
        private RecognizerStartBehavior recognizerStart;

        [Tooltip("The keywords to be recognized and optional keyboard shortcuts.")]
        [SerializeField]
        private KeywordAndKeyCode[] keywords;
        /// <summary>
        /// The keywords to be recognized and optional keyboard shortcuts.
        /// </summary>
        public KeywordAndKeyCode[] Keywords => keywords;

#if UNITY_WSA || UNITY_STANDALONE_WIN
        /// <summary>
        /// The serialized data of this field will be lost when switching between platforms and re-serializing this class.
        /// </summary>
        [SerializeField]
        [Tooltip("The confidence level for the keyword recognizer.")]
        private ConfidenceLevel recognitionConfidenceLevel = ConfidenceLevel.Medium;

        private KeywordRecognizer keywordRecognizer;

        #region Monobehaviour Implementations

        protected virtual void OnEnable()
        {
            if (keywordRecognizer != null && recognizerStart == RecognizerStartBehavior.AutoStart)
            {
                StartKeywordRecognizer();
            }
        }

        protected virtual void Start()
        {
            if (persistentKeywords)
            {
                DontDestroyOnLoad(gameObject);
            }

            SourceId = MixedRealityInputManager.GenerateNewSourceId();

            int keywordCount = keywords.Length;
            if (keywordCount > 0)
            {
                var keywords = new string[keywordCount];

                for (int index = 0; index < keywordCount; index++)
                {
                    keywords[index] = this.keywords[index].Keyword;
                }

                keywordRecognizer = new KeywordRecognizer(keywords, recognitionConfidenceLevel);
                keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

                if (recognizerStart == RecognizerStartBehavior.AutoStart)
                {
                    keywordRecognizer.Start();
                }
            }
            else
            {
                DebugUtilities.DebugLogError("Must have at least one keyword specified in the Inspector on " + gameObject.name + ".");
            }
        }

        protected virtual void Update()
        {
            if (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                for (int index = keywords.Length; --index >= 0;)
                {
                    if (Input.GetKeyDown(keywords[index].KeyCode))
                    {
                        OnPhraseRecognized(recognitionConfidenceLevel, TimeSpan.Zero, DateTime.Now, null, keywords[index].Keyword);
                    }
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (keywordRecognizer != null)
            {
                StopKeywordRecognizer();
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

        #endregion Monobehaviour Implementations

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            OnPhraseRecognized(args.confidence, args.phraseDuration, args.phraseStartTime, args.semanticMeanings, args.text);
        }

        protected virtual void OnPhraseRecognized(ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, SemanticMeaning[] semanticMeanings, string text)
        {
            MixedRealityInputManager.RaiseSpeechKeywordPhraseRecognized(this, confidence, phraseDuration, phraseStartTime, semanticMeanings, text);
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

#endif // UNITY_WSA || UNITY_STANDALONE_WIN

        #region Base Input Source Methods

        public override SupportedInputInfo GetSupportedInputInfo()
        {
            return SupportedInputInfo.Voice;
        }

        #endregion Base Input Source Methods
    }
}