// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using UnityEngine;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
using UnityEngine.Windows.Speech;
#endif

namespace Microsoft.MixedReality.Toolkit.InputSystem.Sources
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

        [SerializeField]
        [Tooltip("Keywords are persistent across all scenes.  This Speech Input Source instance will not be destroyed when loading a new scene.")]
        private bool persistentKeywords = false;

        [SerializeField]
        [Tooltip("Whether the recognizer should be activated on start.")]
        private RecognizerStartBehavior recognizerStart = RecognizerStartBehavior.AutoStart;

        [SerializeField]
        [Tooltip("The keywords to be recognized and optional keyboard shortcuts.")]
        private KeywordAndKeyCode[] keywords = null;

        /// <summary>
        /// The keywords to be recognized and optional keyboard shortcuts.
        /// </summary>
        public KeywordAndKeyCode[] Keywords => keywords;

        public override InteractionDefinition[] Interactions => new[] { new InteractionDefinition(1, AxisType.None, InputType.Voice) };

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        [SerializeField]
        [Tooltip("The confidence level for the keyword recognizer.")]
        // NOTE: The serialized data of this field will be lost when switching between platforms and re-serializing this class.
        private ConfidenceLevel recognitionConfidenceLevel = ConfidenceLevel.Medium;

        private KeywordRecognizer keywordRecognizer;

        #region Monobehaviour Implementations

        private void OnEnable()
        {
            if (keywordRecognizer != null && recognizerStart == RecognizerStartBehavior.AutoStart)
            {
                StartKeywordRecognizer();
            }
        }

        private void Start()
        {
            if (persistentKeywords)
            {
                DontDestroyOnLoad(gameObject);
            }

            int keywordCount = keywords.Length;
            if (keywordCount > 0)
            {
                var newKeywords = new string[keywordCount];

                for (int index = 0; index < keywordCount; index++)
                {
                    newKeywords[index] = keywords[index].Keyword;
                }

                keywordRecognizer = new KeywordRecognizer(newKeywords, recognitionConfidenceLevel);
                keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

                if (recognizerStart == RecognizerStartBehavior.AutoStart)
                {
                    keywordRecognizer.Start();
                }
            }
            else
            {
                Debug.LogError($"Must have at least one keyword specified in the Inspector on {gameObject.name}.");
            }
        }

        private void Update()
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

        private void OnDisable()
        {
            if (keywordRecognizer != null)
            {
                StopKeywordRecognizer();
            }
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

        #endregion Monobehaviour Implementations

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            OnPhraseRecognized(args.confidence, args.phraseDuration, args.phraseStartTime, args.semanticMeanings, args.text);
        }

        private void OnPhraseRecognized(ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, SemanticMeaning[] semanticMeanings, string text)
        {
            InputSystem.RaiseSpeechKeywordPhraseRecognized(this, confidence, phraseDuration, phraseStartTime, semanticMeanings, text);
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

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

    }
}
