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

    [MixedRealityExtensionService(SupportedPlatforms.WindowsStandalone | SupportedPlatforms.WindowsUniversal | SupportedPlatforms.WindowsEditor)]
    public class WindowsSpeechInputProvider : BaseDeviceManager, IMixedRealitySpeechSystem
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
            string name = null, 
            uint priority = DefaultPriority, 
            BaseMixedRealityProfile profile = null) : base(registrar, name, priority, profile) { }

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
        public bool IsRecognitionActive
        {
            get { return keywordRecognizer != null && keywordRecognizer.IsRunning; }
        }

        public RecognitionConfidenceLevel RecognitionConfidenceLevel { get; set; }

#if UNITY_EDITOR
        /// <inheritdoc />
        public override void Initialize()
        {
            if (!UnityEditor.PlayerSettings.WSA.GetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone))
            {
                UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone, true);
            }
        }
#endif // UNITY_EDITOR

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
                    if (UInput.GetKeyDown(Commands[i].KeyCode))
                    {
                        OnPhraseRecognized((ConfidenceLevel)RecognitionConfidenceLevel, TimeSpan.Zero, DateTime.UtcNow, Commands[i].Keyword);
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

#if UNITY_EDITOR
        /// <inheritdoc />
        public override void Destroy()
        {
            if (UnityEditor.PlayerSettings.WSA.GetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone))
            {
                UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone, false);
            }
        }
#endif // UNITY_EDITOR

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
