// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using Unity.Profiling;
using UnityEngine;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
using UnityEngine.Windows.Speech;
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
                (Commands == null) ||
                (Commands.Length == 0) ||
                InputSystemProfile == null ||
                keywordRecognizer != null
                )
            {
                return;
            }

            globalInputSource = Service?.RequestNewGlobalInputSource("Windows Speech Input Source", sourceType: InputSourceType.Voice);

            var newKeywords = new string[Commands.Length];

            for (int i = 0; i < Commands.Length; i++)
            {
                newKeywords[i] = Commands[i].LocalizedKeyword;
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
                    for (int i = 0; i < Commands.Length; i++)
                    {
#if INPUTSYSTEM_PACKAGE
                        if (UnityEngine.InputSystem.Keyboard.current[MapKeyCodeToKey(Commands[i].KeyCode)].wasPressedThisFrame)
#else
                        if (UnityEngine.Input.GetKeyDown(Commands[i].KeyCode))
#endif // INPUTSYSTEM_PACKAGE
                        {
                            OnPhraseRecognized((ConfidenceLevel)RecognitionConfidenceLevel, TimeSpan.Zero, DateTime.UtcNow, Commands[i].LocalizedKeyword);
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
                for (int i = 0; i < Commands?.Length; i++)
                {
                    if (Commands[i].LocalizedKeyword == text)
                    {
                        globalInputSource.UpdateActivePointers();

                        Service?.RaiseSpeechCommandRecognized(InputSource, (RecognitionConfidenceLevel)confidence, phraseDuration, phraseStartTime, Commands[i]);
                        break;
                    }
                }
            }
        }

#if INPUTSYSTEM_PACKAGE
        internal static UnityEngine.InputSystem.Key MapKeyCodeToKey(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Space:
                    return UnityEngine.InputSystem.Key.Space;
                case KeyCode.Return:
                    return UnityEngine.InputSystem.Key.Enter;
                case KeyCode.Tab:
                    return UnityEngine.InputSystem.Key.Tab;
                case KeyCode.BackQuote:
                    return UnityEngine.InputSystem.Key.Backquote;
                case KeyCode.Quote:
                    return UnityEngine.InputSystem.Key.Quote;
                case KeyCode.Semicolon:
                    return UnityEngine.InputSystem.Key.Semicolon;
                case KeyCode.Comma:
                    return UnityEngine.InputSystem.Key.Comma;
                case KeyCode.Period:
                    return UnityEngine.InputSystem.Key.Period;
                case KeyCode.Slash:
                    return UnityEngine.InputSystem.Key.Slash;
                case KeyCode.Backslash:
                    return UnityEngine.InputSystem.Key.Backslash;
                case KeyCode.LeftBracket:
                    return UnityEngine.InputSystem.Key.LeftBracket;
                case KeyCode.RightBracket:
                    return UnityEngine.InputSystem.Key.RightBracket;
                case KeyCode.Minus:
                    return UnityEngine.InputSystem.Key.Minus;
                case KeyCode.Equals:
                    return UnityEngine.InputSystem.Key.Equals;
                case KeyCode.A:
                    return UnityEngine.InputSystem.Key.A;
                case KeyCode.B:
                    return UnityEngine.InputSystem.Key.B;
                case KeyCode.C:
                    return UnityEngine.InputSystem.Key.C;
                case KeyCode.D:
                    return UnityEngine.InputSystem.Key.D;
                case KeyCode.E:
                    return UnityEngine.InputSystem.Key.E;
                case KeyCode.F:
                    return UnityEngine.InputSystem.Key.F;
                case KeyCode.G:
                    return UnityEngine.InputSystem.Key.G;
                case KeyCode.H:
                    return UnityEngine.InputSystem.Key.H;
                case KeyCode.I:
                    return UnityEngine.InputSystem.Key.I;
                case KeyCode.J:
                    return UnityEngine.InputSystem.Key.J;
                case KeyCode.K:
                    return UnityEngine.InputSystem.Key.K;
                case KeyCode.L:
                    return UnityEngine.InputSystem.Key.L;
                case KeyCode.M:
                    return UnityEngine.InputSystem.Key.M;
                case KeyCode.N:
                    return UnityEngine.InputSystem.Key.N;
                case KeyCode.O:
                    return UnityEngine.InputSystem.Key.O;
                case KeyCode.P:
                    return UnityEngine.InputSystem.Key.P;
                case KeyCode.Q:
                    return UnityEngine.InputSystem.Key.Q;
                case KeyCode.R:
                    return UnityEngine.InputSystem.Key.R;
                case KeyCode.S:
                    return UnityEngine.InputSystem.Key.S;
                case KeyCode.T:
                    return UnityEngine.InputSystem.Key.T;
                case KeyCode.U:
                    return UnityEngine.InputSystem.Key.U;
                case KeyCode.V:
                    return UnityEngine.InputSystem.Key.V;
                case KeyCode.W:
                    return UnityEngine.InputSystem.Key.W;
                case KeyCode.X:
                    return UnityEngine.InputSystem.Key.X;
                case KeyCode.Y:
                    return UnityEngine.InputSystem.Key.Y;
                case KeyCode.Z:
                    return UnityEngine.InputSystem.Key.Z;
                case KeyCode.Alpha1:
                    return UnityEngine.InputSystem.Key.Digit1;
                case KeyCode.Alpha2:
                    return UnityEngine.InputSystem.Key.Digit2;
                case KeyCode.Alpha3:
                    return UnityEngine.InputSystem.Key.Digit3;
                case KeyCode.Alpha4:
                    return UnityEngine.InputSystem.Key.Digit4;
                case KeyCode.Alpha5:
                    return UnityEngine.InputSystem.Key.Digit5;
                case KeyCode.Alpha6:
                    return UnityEngine.InputSystem.Key.Digit6;
                case KeyCode.Alpha7:
                    return UnityEngine.InputSystem.Key.Digit7;
                case KeyCode.Alpha8:
                    return UnityEngine.InputSystem.Key.Digit8;
                case KeyCode.Alpha9:
                    return UnityEngine.InputSystem.Key.Digit9;
                case KeyCode.Alpha0:
                    return UnityEngine.InputSystem.Key.Digit0;
                case KeyCode.LeftShift:
                    return UnityEngine.InputSystem.Key.LeftShift;
                case KeyCode.RightShift:
                    return UnityEngine.InputSystem.Key.RightShift;
                case KeyCode.LeftAlt:
                    return UnityEngine.InputSystem.Key.LeftAlt;
                case KeyCode.RightAlt:
                    return UnityEngine.InputSystem.Key.RightAlt;
                case KeyCode.AltGr:
                    return UnityEngine.InputSystem.Key.AltGr;
                case KeyCode.LeftControl:
                    return UnityEngine.InputSystem.Key.LeftCtrl;
                case KeyCode.RightControl:
                    return UnityEngine.InputSystem.Key.RightCtrl;
                case KeyCode.LeftWindows:
                case KeyCode.LeftCommand:
                    return UnityEngine.InputSystem.Key.LeftCommand;
                case KeyCode.RightWindows:
                case KeyCode.RightCommand:
                    return UnityEngine.InputSystem.Key.RightCommand;
                case KeyCode.Escape:
                    return UnityEngine.InputSystem.Key.Escape;
                case KeyCode.LeftArrow:
                    return UnityEngine.InputSystem.Key.LeftArrow;
                case KeyCode.RightArrow:
                    return UnityEngine.InputSystem.Key.RightArrow;
                case KeyCode.UpArrow:
                    return UnityEngine.InputSystem.Key.UpArrow;
                case KeyCode.DownArrow:
                    return UnityEngine.InputSystem.Key.DownArrow;
                case KeyCode.Backspace:
                    return UnityEngine.InputSystem.Key.Backspace;
                case KeyCode.PageDown:
                    return UnityEngine.InputSystem.Key.PageDown;
                case KeyCode.PageUp:
                    return UnityEngine.InputSystem.Key.PageUp;
                case KeyCode.Home:
                    return UnityEngine.InputSystem.Key.Home;
                case KeyCode.Insert:
                    return UnityEngine.InputSystem.Key.Insert;
                case KeyCode.Delete:
                    return UnityEngine.InputSystem.Key.Delete;
                case KeyCode.CapsLock:
                    return UnityEngine.InputSystem.Key.CapsLock;
                case KeyCode.Numlock:
                    return UnityEngine.InputSystem.Key.NumLock;
                case KeyCode.Print:
                    return UnityEngine.InputSystem.Key.PrintScreen;
                case KeyCode.ScrollLock:
                    return UnityEngine.InputSystem.Key.ScrollLock;
                case KeyCode.Pause:
                    return UnityEngine.InputSystem.Key.Pause;
                case KeyCode.KeypadEnter:
                    return UnityEngine.InputSystem.Key.NumpadEnter;
                case KeyCode.KeypadDivide:
                    return UnityEngine.InputSystem.Key.NumpadDivide;
                case KeyCode.KeypadMultiply:
                    return UnityEngine.InputSystem.Key.NumpadMultiply;
                case KeyCode.KeypadPlus:
                    return UnityEngine.InputSystem.Key.NumpadPlus;
                case KeyCode.KeypadMinus:
                    return UnityEngine.InputSystem.Key.NumpadMinus;
                case KeyCode.KeypadPeriod:
                    return UnityEngine.InputSystem.Key.NumpadPeriod;
                case KeyCode.KeypadEquals:
                    return UnityEngine.InputSystem.Key.NumpadEquals;
                case KeyCode.Keypad0:
                    return UnityEngine.InputSystem.Key.Numpad0;
                case KeyCode.Keypad1:
                    return UnityEngine.InputSystem.Key.Numpad1;
                case KeyCode.Keypad2:
                    return UnityEngine.InputSystem.Key.Numpad2;
                case KeyCode.Keypad3:
                    return UnityEngine.InputSystem.Key.Numpad3;
                case KeyCode.Keypad4:
                    return UnityEngine.InputSystem.Key.Numpad4;
                case KeyCode.Keypad5:
                    return UnityEngine.InputSystem.Key.Numpad5;
                case KeyCode.Keypad6:
                    return UnityEngine.InputSystem.Key.Numpad6;
                case KeyCode.Keypad7:
                    return UnityEngine.InputSystem.Key.Numpad7;
                case KeyCode.Keypad8:
                    return UnityEngine.InputSystem.Key.Numpad8;
                case KeyCode.Keypad9:
                    return UnityEngine.InputSystem.Key.Numpad9;
                case KeyCode.F1:
                    return UnityEngine.InputSystem.Key.F1;
                case KeyCode.F2:
                    return UnityEngine.InputSystem.Key.F2;
                case KeyCode.F3:
                    return UnityEngine.InputSystem.Key.F3;
                case KeyCode.F4:
                    return UnityEngine.InputSystem.Key.F4;
                case KeyCode.F5:
                    return UnityEngine.InputSystem.Key.F5;
                case KeyCode.F6:
                    return UnityEngine.InputSystem.Key.F6;
                case KeyCode.F7:
                    return UnityEngine.InputSystem.Key.F7;
                case KeyCode.F8:
                    return UnityEngine.InputSystem.Key.F8;
                case KeyCode.F9:
                    return UnityEngine.InputSystem.Key.F9;
                case KeyCode.F10:
                    return UnityEngine.InputSystem.Key.F10;
                case KeyCode.F11:
                    return UnityEngine.InputSystem.Key.F11;
                case KeyCode.F12:
                    return UnityEngine.InputSystem.Key.F12;
                default:
                    return UnityEngine.InputSystem.Key.None;
            }
        }
#endif // INPUTSYSTEM_PACKAGE
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
    }
}
