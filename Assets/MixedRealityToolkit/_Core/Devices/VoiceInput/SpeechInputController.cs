// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async.AwaitYieldInstructions;
using System;
using UnityEngine;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
using UnityEngine.Windows.Speech;
#endif

namespace Microsoft.MixedReality.Toolkit.Core.Devices.VoiceInput
{
    /// <summary>
    /// A Windows Mixed Reality Controller Instance.
    /// </summary>
    public class SpeechInputController : BaseController, IMixedRealitySpeechController
    {
        private readonly WaitForUpdate waitForUpdate = new WaitForUpdate();

        /// <summary>
        /// The keywords to be recognized and optional keyboard shortcuts.
        /// </summary>
        private SpeechCommands[] Commands { get { return MixedRealityManager.Instance.ActiveProfile.SpeechCommandsProfile.SpeechCommands; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public SpeechInputController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            //throw new System.NotImplementedException();
        }

        #region IMixedRealitySpeechController interface

        /// <inheritdoc />
        public void Initialize()
        {
            if (!Application.isPlaying || Commands.Length == 0) { return; }

            var newKeywords = new string[Commands.Length];

            for (int i = 0; i < Commands.Length; i++)
            {
                newKeywords[i] = Commands[i].Keyword;
            }

            Setup(newKeywords);

            if (MixedRealityManager.Instance.ActiveProfile.SpeechCommandsProfile.SpeechRecognizerStartBehavior == AutoStartBehavior.AutoStart)
            {
                StartRecognition();
            }

            Run();
        }

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

        /// <inheritdoc />
        public void Dispose()
        {
            if (keywordRecognizer == null) { return; }
            StopRecognition();
            Cleanup();
        }
        #endregion // IMixedRealitySpeechController interface

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
            RecognitionConfidenceLevel = (ConfidenceLevel)MixedRealityManager.Instance.ActiveProfile.SpeechCommandsProfile.SpeechRecognitionConfidenceLevel;
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
                    InputSystem.RaiseSpeechCommandRecognized(InputSource, Commands[i].Action, confidence, phraseDuration, phraseStartTime, semanticMeanings, text);
                    break;
                }
            }
        }

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
        #endregion UWP and Windows Standalone Implementations
    }
}