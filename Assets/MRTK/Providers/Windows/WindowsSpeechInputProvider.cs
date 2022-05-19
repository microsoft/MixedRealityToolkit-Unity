// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using Unity.Profiling;
using UnityEngine;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
using UnityEngine.Windows.Speech;
using UInput = UnityEngine.Input;
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

namespace Microsoft.MixedReality.Toolkit.Windows.Input
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.WindowsUniversal | SupportedPlatforms.WindowsEditor,
        "Windows Speech Input")]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/input/speech")]
    public class WindowsSpeechInputProvider : BaseInputDeviceManager, IMixedRealitySpeechSystem, IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        [Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public WindowsSpeechInputProvider(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : this(inputSystem, name, priority, profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsSpeechInputProvider(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

        /// <summary>
        /// The keywords to be recognized and optional keyboard shortcuts.
        /// </summary>
        private SpeechCommands[] Commands => InputSystemProfile.SpeechCommandsProfile.SpeechCommands;

        /// <summary>
        /// The Input Source for Windows Speech Input.
        /// </summary>
        public IMixedRealityInputSource InputSource => globalInputSource;

        /// <summary>
        /// The minimum confidence level for the recognizer to fire an event.
        /// </summary>
        public RecognitionConfidenceLevel RecognitionConfidenceLevel { get; set; }

        /// <summary>
        /// The global input source used by the the speech input provider to raise events.
        /// </summary>
        private BaseGlobalInputSource globalInputSource = null;

        /// <inheritdoc />
        public bool IsRecognitionActive =>
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            keywordRecognizer?.IsRunning ??
#endif
            false;

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            return capability == MixedRealityCapability.VoiceCommand;
        }

        #endregion IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public void StartRecognition()
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            // try to initialize the keyword recognizer if it is null
            if (keywordRecognizer == null && InputSystemProfile.SpeechCommandsProfile.SpeechRecognizerStartBehavior == AutoStartBehavior.ManualStart)
            {
                InitializeKeywordRecognizer();
            }

            if (keywordRecognizer != null && !keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Start();
            }
#endif
        }

        /// <inheritdoc />
        public void StopRecognition()
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            if (keywordRecognizer != null && keywordRecognizer.IsRunning)
            {
                keywordRecognizer.Stop();
            }
#endif
        }

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
        private KeywordRecognizer keywordRecognizer;

#if UNITY_EDITOR && UNITY_WSA
        /// <inheritdoc />
        public override void Initialize()
        {
            Toolkit.Utilities.Editor.UWPCapabilityUtility.RequireCapability(
                    UnityEditor.PlayerSettings.WSACapability.Microphone,
                    this.GetType());
        }
#endif

        /// <inheritdoc />
        public override void Enable()
        {
            if (InputSystemProfile.SpeechCommandsProfile.SpeechRecognizerStartBehavior == AutoStartBehavior.AutoStart)
            {
                InitializeKeywordRecognizer();
                StartRecognition();
            }

            // Call the base here to ensure any early exits do not
            // artificially declare the service as enabled.
            base.Enable();
        }

        private void InitializeKeywordRecognizer()
        {
            if (!Application.isPlaying ||
                InputSystemProfile == null ||
                keywordRecognizer != null)
            {
                return;
            }

            SpeechCommands[] commands = Commands;
            int commandsCount = commands?.Length ?? 0;

            if (commandsCount == 0)
            {
                return;
            }

            globalInputSource = Service?.RequestNewGlobalInputSource("Windows Speech Input Source", sourceType: InputSourceType.Voice);

            var newKeywords = new string[commandsCount];

            for (int i = 0; i < commandsCount; i++)
            {
                newKeywords[i] = commands[i].LocalizedKeyword;
            }

            RecognitionConfidenceLevel = InputSystemProfile.SpeechCommandsProfile.SpeechRecognitionConfidenceLevel;

            try
            {
                keywordRecognizer = new KeywordRecognizer(newKeywords, (ConfidenceLevel)RecognitionConfidenceLevel);
            }
            catch (Exception ex)
            {
                // Don't log if the application is currently running in batch mode (for example, when running tests). This failure is expected in this case.
                if (!Application.isBatchMode)
                {
                    Debug.LogWarning($"Failed to start keyword recognizer. Are microphone permissions granted? Exception: {ex}");
                }
                keywordRecognizer = null;
                return;
            }

            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        }

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] WindowsSpeechInputProvider.Update");

        /// <inheritdoc />
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                base.Update();

                if (keywordRecognizer != null && keywordRecognizer.IsRunning)
                {
                    SpeechCommands[] commands = Commands;
                    int commandsCount = commands?.Length ?? 0;
                    for (int i = 0; i < commandsCount; i++)
                    {
                        SpeechCommands command = commands[i];
                        if (UInput.GetKeyDown(command.KeyCode))
                        {
                            OnPhraseRecognized((ConfidenceLevel)RecognitionConfidenceLevel, TimeSpan.Zero, DateTime.UtcNow, command.LocalizedKeyword);
                        }
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

            keywordRecognizer = null;

            base.Disable();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                keywordRecognizer?.Dispose();
            }
        }

        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            OnPhraseRecognized(args.confidence, args.phraseDuration, args.phraseStartTime, args.text);
        }

        private static readonly ProfilerMarker OnPhraseRecognizedPerfMarker = new ProfilerMarker("[MRTK] WindowsSpeechInputProvider.OnPhraseRecognized");

        private void OnPhraseRecognized(ConfidenceLevel confidence, TimeSpan phraseDuration, DateTime phraseStartTime, string text)
        {
            using (OnPhraseRecognizedPerfMarker.Auto())
            {
                SpeechCommands[] commands = Commands;
                int commandsCount = commands?.Length ?? 0;
                for (int i = 0; i < commandsCount; i++)
                {
                    SpeechCommands command = commands[i];
                    if (command.LocalizedKeyword == text)
                    {
                        globalInputSource.UpdateActivePointers();
                        Service?.RaiseSpeechCommandRecognized(InputSource, (RecognitionConfidenceLevel)confidence, phraseDuration, phraseStartTime, command);
                        break;
                    }
                }
            }
        }
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
    }
}
