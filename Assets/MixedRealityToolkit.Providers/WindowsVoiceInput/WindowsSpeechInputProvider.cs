// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;
using UInput = UnityEngine.Input;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
using UnityEngine.Windows.Speech;
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

namespace Microsoft.MixedReality.Toolkit.Windows.Input
{
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.WindowsUniversal | SupportedPlatforms.WindowsEditor,
        "Windows Speech Input")]
    public class WindowsSpeechInputProvider : BaseInputDeviceManager, IMixedRealitySpeechSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsSpeechInputProvider(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            MixedRealityInputSystemProfile inputSystemProfile,
            Transform playspace,
            string name = null, 
            uint priority = DefaultPriority, 
            BaseMixedRealityProfile profile = null) : base(registrar, inputSystem, inputSystemProfile, playspace, name, priority, profile) { }

        /// <summary>
        /// The keywords to be recognized and optional keyboard shortcuts.
        /// </summary>
        // todo: fix...
        private static SpeechCommands[] Commands => MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile.SpeechCommands;

        /// <summary>
        /// The Input Source for Windows Speech Input.
        /// </summary>
        public IMixedRealityInputSource InputSource = null;

        private KeywordRecognizer keywordRecognizer;

        /// <inheritdoc />
        public bool IsRecognitionActive
        {
            get { return keywordRecognizer != null && keywordRecognizer.IsRunning; }
        }

        public RecognitionConfidenceLevel RecognitionConfidenceLevel { get; set; }

        /// <inheritdoc />
        public override void Enable()
        {
            if (!Application.isPlaying || Commands.Length == 0) { return; }
            
            if (InputSystemProfile == null) { return; }

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

            InputSource = inputSystem?.RequestNewGenericInputSource("Windows Speech Input Source", sourceType: InputSourceType.Voice);

            var newKeywords = new string[Commands.Length];

            for (int i = 0; i < Commands.Length; i++)
            {
                newKeywords[i] = Commands[i].LocalizedKeyword;
            }

            RecognitionConfidenceLevel = InputSystemProfile.SpeechCommandsProfile.SpeechRecognitionConfidenceLevel;

            if (keywordRecognizer == null)
            {
                keywordRecognizer = new KeywordRecognizer(newKeywords, (ConfidenceLevel)RecognitionConfidenceLevel);
                keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
            }

            if (InputSystemProfile.SpeechCommandsProfile.SpeechRecognizerStartBehavior == AutoStartBehavior.AutoStart)
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
                    if (UInput.GetKeyDown(Commands[i].KeyCode))
                    {
                        OnPhraseRecognized((ConfidenceLevel)RecognitionConfidenceLevel, TimeSpan.Zero, DateTime.UtcNow, Commands[i].LocalizedKeyword);
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
        public void StartRecognition()
        {
            if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Start();
            }
        }

        /// <inheritdoc />
        public void StopRecognition()
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
            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

            for (int i = 0; i < Commands?.Length; i++)
            {
                if (Commands[i].LocalizedKeyword == text)
                {
                    inputSystem?.RaiseSpeechCommandRecognized(InputSource, (RecognitionConfidenceLevel)confidence, phraseDuration, phraseStartTime, Commands[i]);
                    break;
                }
            }
        }
    }
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
}
