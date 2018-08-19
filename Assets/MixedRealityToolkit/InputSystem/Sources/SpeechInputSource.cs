// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Async;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Async.AwaitYieldInstructions;
using System;
using UnityEngine;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
using UnityEngine.Windows.Speech;
#endif

namespace Microsoft.MixedReality.Toolkit.InputSystem.Sources
{
    /// <summary>
    /// SpeechInputSource allows you to specify keywords for use in your application.
    /// This also includes a setting to either automatically start the
    /// keyword recognizer or allow your code to start it.
    /// </summary>
    /// <remarks>
    /// IMPORTANT:
    /// If you're targeting the UWP platform, please make sure to add the microphone capability in your app,
    /// in Unity under Edit -> Project Settings -> Player -> Settings for Windows Store -> Publishing Settings -> Capabilities
    /// or in your Visual Studio Package.appxmanifest capabilities.
    /// </remarks>
    public class SpeechInputSource : BaseGenericInputSource
    {
        /// <summary>
        /// This enumeration gives the manager two different ways to handle the recognizer. Both will
        /// set up the recognizer and add all keywords. The first causes the recognizer to start
        /// immediately. The second allows the recognizer to be manually started at a later time.
        /// </summary>
        private enum RecognizerStartBehavior { AutoStart, ManualStart }

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
        /// <summary>
        /// Constructor.
        /// </summary>
        public SpeechInputSource(SpeechCommands[] commands, ConfidenceLevel recognitionConfidenceLevel = ConfidenceLevel.Medium) : base("SpeechInput")
        {
            Commands = commands;
            RecognitionConfidenceLevel = recognitionConfidenceLevel;
            Initialize();
        }
#else
        /// <summary>
        /// Constructor.
        /// </summary>
        public SpeechInputSource(SpeechCommands[] commands) : base("SpeechInput")
        {
            this.commands = commands;
            Initialize();
        }
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        private void Initialize()
        {
            if (!Application.isPlaying || Commands.Length == 0) { return; }

            var newKeywords = new string[Commands.Length];

            for (int i = 0; i < Commands.Length; i++)
            {
                newKeywords[i] = Commands[i].Keyword;
            }

            Setup(newKeywords);

            if (recognizerStart == RecognizerStartBehavior.AutoStart)
            {
                StartRecognition();
            }

            Run();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            if (keywordRecognizer == null) { return; }
            StopRecognition();
            Cleanup();
        }

        [SerializeField]
        [Tooltip("Whether the recognizer should be activated on start.")]
        private RecognizerStartBehavior recognizerStart = RecognizerStartBehavior.AutoStart;

        /// <summary>
        /// The keywords to be recognized and optional keyboard shortcuts.
        /// </summary>
        public SpeechCommands[] Commands { get; }

        private readonly WaitForUpdate waitForUpdate = new WaitForUpdate();

        private async void Run()
        {
            while (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                for (int i = 0; i < Commands.Length; i++)
                {
                    if (Input.GetKeyDown(Commands[i].KeyCode))
                    {
                        RaiseKeywordAction(Commands[i].Keyword);
                    }
                }

                await waitForUpdate;
            }
        }

        #region Platform Routing

        private void Setup(string[] newKeywords)
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            WindowsSetup(newKeywords);
#else
            // TODO: Implement on other platforms
#endif
        }

        /// <summary>
        /// Raises events based on keyboard input.
        /// </summary>
        /// <param name="keyword"></param>
        private void RaiseKeywordAction(string keyword)
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            OnPhraseRecognized(RecognitionConfidenceLevel, TimeSpan.Zero, DateTime.Now, null, keyword);
#else
            // TODO: Implement on other platforms
#endif
        }

        private void Cleanup()
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            WindowsCleanup();
#else
            // TODO: Implement on other platforms
#endif
        }

        /// <summary>
        /// Make sure the keyword recognizer is off, then start it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public void StartRecognition()
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            WindowsStartRecognition();
#else
            // TODO: Implement on other platforms
#endif
        }

        /// <summary>
        /// Make sure the keyword recognizer is on, then stop it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        public void StopRecognition()
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            WindowsStopRecognition();
#else
            // TODO: Implement on other platforms
#endif
        }

        #endregion Platform Routing

        #region UWP and Windows Standalone Implementations
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        private KeywordRecognizer keywordRecognizer;

        public ConfidenceLevel RecognitionConfidenceLevel { get; set; }

        private void WindowsSetup(string[] newKeywords)
        {
            keywordRecognizer = new KeywordRecognizer(newKeywords, RecognitionConfidenceLevel);
            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        }

        private void WindowsCleanup()
        {
            keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
            keywordRecognizer.Dispose();
        }

        private void WindowsStartRecognition()
        {
            if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Start();
            }
        }

        private void WindowsStopRecognition()
        {
            if (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Stop();
            }
        }

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            OnPhraseRecognized(args.confidence, args.phraseDuration, args.phraseStartTime, args.semanticMeanings, args.text);
        }

        private void OnPhraseRecognized(ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, SemanticMeaning[] semanticMeanings, string text)
        {
            for (int i = 0; i < Commands?.Length; i++)
            {
                if (Commands[i].Keyword == text)
                {
                    InputSystem.RaiseSpeechCommandRecognized(this, Commands[i].Action, confidence, phraseDuration, phraseStartTime, semanticMeanings, text);
                    break;
                }
            }
        }

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
        #endregion UWP and Windows Standalone Implementations
    }
}
