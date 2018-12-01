// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System;
using UnityEngine;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
using UnityEngine.Windows.Speech;
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.Controllers.WindowsMixedReality
{
    [Obsolete("Use WindowsSpeechDataProvider instead.")]
    public class WindowsSpeechInputDeviceManager { }

    /// <summary>
    /// Speech data provider for windows 10 based platforms.
    /// </summary>
    public class WindowsSpeechDataProvider : BaseSpeechDataProvider
    {
        public WindowsSpeechDataProvider(string name, uint priority) : base(name, priority) { }

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        /// <summary>
        /// The keywords to be recognized and optional keyboard shortcuts.
        /// </summary>
        private static SpeechCommands[] Commands => MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile.SpeechCommands;

        /// <summary>
        /// The Input Source for Windows Speech Input.
        /// </summary>
        public IMixedRealityInputSource InputSource = null;

        private KeywordRecognizer keywordRecognizer;

        /// <inheritdoc />
        public override bool IsRecognitionActive => keywordRecognizer != null && keywordRecognizer.IsRunning;

        public RecognitionConfidenceLevel RecognitionConfidenceLevel { get; set; }

        /// <inheritdoc />
        public override void Enable()
        {
            if (!Application.isPlaying || Commands.Length == 0) { return; }

            InputSource = MixedRealityToolkit.InputSystem?.RequestNewGenericInputSource("Windows Speech Input Source");

            var newKeywords = new string[Commands.Length];

            for (int i = 0; i < Commands.Length; i++)
            {
                newKeywords[i] = Commands[i].Keyword;
            }

            RecognitionConfidenceLevel = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile.SpeechRecognitionConfidenceLevel;
            keywordRecognizer = new KeywordRecognizer(newKeywords, (ConfidenceLevel)RecognitionConfidenceLevel);
            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;

            if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile.SpeechRecognizerStartBehavior == AutoStartBehavior.AutoStart)
            {
                StartRecognition();
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                for (int i = 0; i < Commands.Length; i++)
                {
                    if (Input.GetKeyDown(Commands[i].KeyCode))
                    {
                        OnPhraseRecognized((ConfidenceLevel)RecognitionConfidenceLevel, TimeSpan.Zero, DateTime.Now, Commands[i].Keyword);
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            if (keywordRecognizer != null)
            {
                StopRecognition();
                keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
                keywordRecognizer.Dispose();
            }
        }

        /// <inheritdoc />
        public override void StartRecognition()
        {
            if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Start();
            }
        }

        /// <inheritdoc />
        public override void StopRecognition()
        {
            if (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Stop();
            }
        }

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            OnPhraseRecognized(args.confidence, args.phraseDuration, args.phraseStartTime, args.text);
        }

        private void OnPhraseRecognized(ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, string text)
        {
            for (int i = 0; i < Commands?.Length; i++)
            {
                if (Commands[i].Keyword == text)
                {
                    MixedRealityToolkit.InputSystem.RaiseSpeechCommandRecognized(InputSource, Commands[i].Action, (RecognitionConfidenceLevel)confidence, phraseDuration, phraseStartTime, text);
                    break;
                }
            }
        }
    }
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
}