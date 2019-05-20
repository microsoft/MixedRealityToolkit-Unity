// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
using System.Text;
using UnityEngine.Windows.Speech;
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

namespace Microsoft.MixedReality.Toolkit.Windows.Input
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.WindowsUniversal | SupportedPlatforms.WindowsEditor,
        "Windows Dictation Input")]
    [DocLink("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/Input/Dictation.html")]
    public class WindowsDictationInputProvider : BaseInputDeviceManager, IMixedRealityDictationSystem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsDictationInputProvider(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(registrar, inputSystem, name, priority, profile) { }

        /// <inheritdoc />
        public bool IsListening { get; private set; } = false;

        /// <inheritdoc />
        public async void StartRecording(GameObject listener, float initialSilenceTimeout = 5, float autoSilenceTimeout = 20, int recordingTime = 10, string micDeviceName = "")
        {
            await StartRecordingAsync(listener, initialSilenceTimeout, autoSilenceTimeout, recordingTime, micDeviceName);
        }

        /// <inheritdoc />
        public async void StopRecording()
        {
            await StopRecordingAsync();
        }

        /// <inheritdoc />
        public async Task StartRecordingAsync(GameObject listener = null, float initialSilenceTimeout = 5f, float autoSilenceTimeout = 20f, int recordingTime = 10, string micDeviceName = "")
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

            if (IsListening || isTransitioning || inputSystem == null || !Application.isPlaying)
            {
                Debug.LogWarning("Unable to start recording");
                return;
            }

            hasFailed = false;
            IsListening = true;
            isTransitioning = true;

            if (listener != null)
            {
                hasListener = true;
                inputSystem.PushModalInputHandler(listener);
            }

            if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
            {
                PhraseRecognitionSystem.Shutdown();
            }

            await waitUntilPhraseRecognitionSystemHasStopped;
            Debug.Assert(PhraseRecognitionSystem.Status == SpeechSystemStatus.Stopped);

            // Query the maximum frequency of the default microphone.
            int minSamplingRate; // Not used.
            deviceName = micDeviceName;
            Microphone.GetDeviceCaps(deviceName, out minSamplingRate, out samplingRate);

            dictationRecognizer.InitialSilenceTimeoutSeconds = initialSilenceTimeout;
            dictationRecognizer.AutoSilenceTimeoutSeconds = autoSilenceTimeout;
            dictationRecognizer.Start();

            await waitUntilDictationRecognizerHasStarted;
            Debug.Assert(dictationRecognizer.Status == SpeechSystemStatus.Running);

            if (dictationRecognizer.Status == SpeechSystemStatus.Failed)
            {
                inputSystem.RaiseDictationError(inputSource, "Dictation recognizer failed to start!");
                return;
            }

            // Start recording from the microphone.
            dictationAudioClip = Microphone.Start(deviceName, false, recordingTime, samplingRate);
            textSoFar = new StringBuilder();
            isTransitioning = false;
#else
            await Task.CompletedTask;
#endif
        }

        /// <inheritdoc />
        public async Task<AudioClip> StopRecordingAsync()
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            if (!IsListening || isTransitioning || !Application.isPlaying)
            {
                Debug.LogWarning("Unable to stop recording");
                return null;
            }

            IsListening = false;
            isTransitioning = true;

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

            if (hasListener)
            {
                inputSystem?.PopModalInputHandler();
                hasListener = false;
            }

            Microphone.End(deviceName);

            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
            }

            await waitUntilDictationRecognizerHasStopped;
            Debug.Assert(dictationRecognizer.Status == SpeechSystemStatus.Stopped);

            PhraseRecognitionSystem.Restart();

            await waitUntilPhraseRecognitionSystemHasStarted;
            Debug.Assert(PhraseRecognitionSystem.Status == SpeechSystemStatus.Running);

            isTransitioning = false;
            return dictationAudioClip;
#else
            await Task.CompletedTask;
            return null;
#endif
        }

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
        private bool hasFailed;
        private bool hasListener;
        private bool isTransitioning;

        private IMixedRealityInputSource inputSource = null;

        /// <summary>
        /// Caches the text currently being displayed in dictation display text.
        /// </summary>
        private StringBuilder textSoFar;

        private string deviceName = string.Empty;

        /// <summary>
        /// The device audio sampling rate.
        /// </summary>
        /// <remarks>Set by UnityEngine.Microphone.<see cref="Microphone.GetDeviceCaps"/></remarks>
        private int samplingRate;

        /// <summary>
        /// String result of the current dictation.
        /// </summary>
        private string dictationResult;

        /// <summary>
        /// Audio clip of the last dictation session.
        /// </summary>
        private AudioClip dictationAudioClip;

        private static DictationRecognizer dictationRecognizer;

        private readonly WaitUntil waitUntilPhraseRecognitionSystemHasStarted = new WaitUntil(() => PhraseRecognitionSystem.Status != SpeechSystemStatus.Stopped);
        private readonly WaitUntil waitUntilPhraseRecognitionSystemHasStopped = new WaitUntil(() => PhraseRecognitionSystem.Status != SpeechSystemStatus.Running);

        private readonly WaitUntil waitUntilDictationRecognizerHasStarted = new WaitUntil(() => dictationRecognizer.Status != SpeechSystemStatus.Stopped);
        private readonly WaitUntil waitUntilDictationRecognizerHasStopped = new WaitUntil(() => dictationRecognizer.Status != SpeechSystemStatus.Running);

#if UNITY_EDITOR
        /// <inheritdoc />
        public override void Initialize()
        {
            if (!UnityEditor.PlayerSettings.WSA.GetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone))
            {
                UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone, true);
            }

            if (!UnityEditor.PlayerSettings.WSA.GetCapability(UnityEditor.PlayerSettings.WSACapability.InternetClient))
            {
                UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.InternetClient, true);
            }
        }
#endif // UNITY_EDITOR

        /// <inheritdoc />
        public override void Enable()
        {
            if (!Application.isPlaying) { return; }

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

            if (inputSystem == null)
            {
                Debug.LogError($"Unable to start {Name}. An Input System is required for this feature.");
                return;
            }

            inputSource = inputSystem.RequestNewGenericInputSource(Name, sourceType: InputSourceType.Voice);
            dictationResult = string.Empty;

            try
            {
                if (dictationRecognizer == null)
                {
                    dictationRecognizer = new DictationRecognizer();

                    dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
                    dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
                    dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
                    dictationRecognizer.DictationError += DictationRecognizer_DictationError;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to start dictation recognizer. Are microphone permissions granted? Exception: {ex}");
                Disable();
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

            if (!Application.isPlaying || inputSystem == null) { return; }

            if (!isTransitioning && IsListening && !Microphone.IsRecording(deviceName) && dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                // If the microphone stops as a result of timing out, make sure to manually stop the dictation recognizer.
                StopRecording();
            }

            if (!hasFailed && dictationRecognizer.Status == SpeechSystemStatus.Failed)
            {
                hasFailed = true;
                inputSystem.RaiseDictationError(inputSource, "Dictation recognizer has failed!");
            }
        }

        /// <inheritdoc />
        public override async void Disable()
        {
            if (Application.isPlaying && dictationRecognizer != null)
            {
                if (!isTransitioning && IsListening) { await StopRecordingAsync(); }

                dictationRecognizer.DictationHypothesis -= DictationRecognizer_DictationHypothesis;
                dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
                dictationRecognizer.DictationComplete -= DictationRecognizer_DictationComplete;
                dictationRecognizer.DictationError -= DictationRecognizer_DictationError;
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
#if UNITY_EDITOR
            if (UnityEditor.PlayerSettings.WSA.GetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone))
            {
                UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone, false);
            }

            if (UnityEditor.PlayerSettings.WSA.GetCapability(UnityEditor.PlayerSettings.WSACapability.InternetClient))
            {
                UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.InternetClient, false);
            }
#endif // UNITY_EDITOR
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dictationRecognizer?.Dispose();
            }
        }

        /// <summary>
        /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
        /// </summary>
        /// <param name="text">The currently hypothesized recognition.</param>
        private void DictationRecognizer_DictationHypothesis(string text)
        {
            // We don't want to append to textSoFar yet, because the hypothesis may have changed on the next event.
            dictationResult = $"{textSoFar} {text}...";

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
            inputSystem?.RaiseDictationHypothesis(inputSource, dictationResult);
        }

        /// <summary>
        /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
        /// </summary>
        /// <param name="text">The text that was heard by the recognizer.</param>
        /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
        private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
        {
            textSoFar.Append($"{text}. ");

            dictationResult = textSoFar.ToString();

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
            inputSystem?.RaiseDictationResult(inputSource, dictationResult);
        }

        /// <summary>
        /// This event is fired when the recognizer stops, whether from StartRecording() being called, a timeout occurring, or some other error.
        /// Typically, this will simply return "Complete". In this case, we check to see if the recognizer timed out.
        /// </summary>
        /// <param name="cause">An enumerated reason for the session completing.</param>
        private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
        {
            // If Timeout occurs, the user has been silent for too long.
            if (cause == DictationCompletionCause.TimeoutExceeded)
            {
                Microphone.End(deviceName);

                dictationResult = "Dictation has timed out. Please try again.";
            }

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
            inputSystem?.RaiseDictationComplete(inputSource, dictationResult, dictationAudioClip);
            textSoFar = null;
            dictationResult = string.Empty;
        }

        /// <summary>
        /// This event is fired when an error occurs.
        /// </summary>
        /// <param name="error">The string representation of the error reason.</param>
        /// <param name="hresult">The int representation of the hresult.</param>
        private void DictationRecognizer_DictationError(string error, int hresult)
        {
            dictationResult = $"{error}\nHRESULT: {hresult}";

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
            inputSystem?.RaiseDictationError(inputSource, dictationResult);
            textSoFar = null;
            dictationResult = string.Empty;
        }
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
    }
}
