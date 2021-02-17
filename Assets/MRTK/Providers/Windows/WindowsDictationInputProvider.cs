// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading.Tasks;
using Unity.Profiling;
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
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/input/dictation")]
    public class WindowsDictationInputProvider : BaseInputDeviceManager, IMixedRealityDictationSystem, IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public WindowsDictationInputProvider(
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
        public WindowsDictationInputProvider(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

        /// <inheritdoc />
        public bool IsListening { get; private set; } = false;

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            return (capability == MixedRealityCapability.VoiceDictation);
        }

        #endregion IMixedRealityCapabilityCheck Implementation

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

        private static readonly ProfilerMarker StartRecordingAsyncPerfMarker = new ProfilerMarker("[MRTK] WindowsDictationInputProvider.StartRecordingAsync");

        /// <inheritdoc />
        public async Task StartRecordingAsync(GameObject listener = null, float initialSilenceTimeout = 5f, float autoSilenceTimeout = 20f, int recordingTime = 10, string micDeviceName = "")
        {
            using (StartRecordingAsyncPerfMarker.Auto())
            {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
                if (IsListening || isTransitioning || Service == null || !Application.isPlaying)
                {
                    Debug.LogWarning("Unable to start recording");
                    return;
                }

                if (dictationRecognizer == null && InputSystemProfile.SpeechCommandsProfile.SpeechRecognizerStartBehavior == AutoStartBehavior.ManualStart)
                {
                    InitializeDictationRecognizer();
                }

                hasFailed = false;
                IsListening = true;
                isTransitioning = true;

                if (listener != null)
                {
                    hasListener = true;
                    Service.PushModalInputHandler(listener);
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
                    Service.RaiseDictationError(inputSource, "Dictation recognizer failed to start!");
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
        }

        private static readonly ProfilerMarker StopRecordingAsyncPerfMarker = new ProfilerMarker("[MRTK] WindowsDictationInputProvider.StopRecordingAsync");

        /// <inheritdoc />
        public async Task<AudioClip> StopRecordingAsync()
        {
            using (StopRecordingAsyncPerfMarker.Auto())
            {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
                if (!IsListening || isTransitioning || !Application.isPlaying)
                {
                    Debug.LogWarning("Unable to stop recording");
                    return null;
                }

                IsListening = false;
                isTransitioning = true;

                if (hasListener)
                {
                    Service?.PopModalInputHandler();
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

#if UNITY_EDITOR && UNITY_WSA
        /// <inheritdoc />
        public override void Initialize()
        {
            Toolkit.Utilities.Editor.UWPCapabilityUtility.RequireCapability(
                UnityEditor.PlayerSettings.WSACapability.InternetClient,
                this.GetType());

            Toolkit.Utilities.Editor.UWPCapabilityUtility.RequireCapability(
                UnityEditor.PlayerSettings.WSACapability.Microphone,
                this.GetType());
        }
#endif

        /// <inheritdoc />
        public override void Enable()
        {
            if (!Application.isPlaying) { return; }

            if (Service == null)
            {
                Debug.LogError($"Unable to start {Name}. An Input System is required for this feature.");
                return;
            }

            inputSource = Service.RequestNewGenericInputSource(Name, sourceType: InputSourceType.Voice);
            dictationResult = string.Empty;

            if (dictationRecognizer == null && InputSystemProfile.SpeechCommandsProfile.SpeechRecognizerStartBehavior == AutoStartBehavior.AutoStart)
            {
                InitializeDictationRecognizer();
            }

            // Call the base here to ensure any early exits do not
            // artificially declare the service as enabled.
            base.Enable();
        }

        private void InitializeDictationRecognizer()
        {
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
                // Don't log if the application is currently running in batch mode (for example, when running tests). This failure is expected in this case.
                if (!Application.isBatchMode)
                {
                    Debug.LogWarning($"Failed to start dictation recognizer. Are microphone permissions granted? Exception: {ex}");
                }
                Disable();
                dictationRecognizer = null;
            }
        }

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] WindowsDictationInputProvider.Update");

        /// <inheritdoc />
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                if (!Application.isPlaying || Service == null || dictationRecognizer == null) { return; }

                base.Update();

                if (!isTransitioning && IsListening && !Microphone.IsRecording(deviceName) && dictationRecognizer.Status == SpeechSystemStatus.Running)
                {
                    // If the microphone stops as a result of timing out, make sure to manually stop the dictation recognizer.
                    StopRecording();
                }

                if (!hasFailed && dictationRecognizer.Status == SpeechSystemStatus.Failed)
                {
                    hasFailed = true;
                    Service.RaiseDictationError(inputSource, "Dictation recognizer has failed!");
                }
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

                dictationRecognizer.Dispose();
            }

            base.Disable();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dictationRecognizer?.Dispose();
            }
        }

        private static readonly ProfilerMarker DictationHypothesisPerfMarker = new ProfilerMarker("[MRTK] WindowsDictationInputProvider.DictationRecognizer_DictationHypothesis");

        /// <summary>
        /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
        /// </summary>
        /// <param name="text">The currently hypothesized recognition.</param>
        private void DictationRecognizer_DictationHypothesis(string text)
        {
            using (DictationHypothesisPerfMarker.Auto())
            {
                // We don't want to append to textSoFar yet, because the hypothesis may have changed on the next event.
                dictationResult = $"{textSoFar} {text}...";

                Service?.RaiseDictationHypothesis(inputSource, dictationResult);
            }
        }

        private static readonly ProfilerMarker DictationResultPerfMarker = new ProfilerMarker("[MRTK] WindowsDictationInputProvider.DictationRecognizer_DictationResult");

        /// <summary>
        /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
        /// </summary>
        /// <param name="text">The text that was heard by the recognizer.</param>
        /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
        private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
        {
            using (DictationResultPerfMarker.Auto())
            {
                textSoFar.Append($"{text}. ");

                dictationResult = textSoFar.ToString();

                Service?.RaiseDictationResult(inputSource, dictationResult);
            }
        }

        private static readonly ProfilerMarker DictationCompletePerfMarker = new ProfilerMarker("[MRTK] WindowsDictationInputProvider.DictationRecognizer_DictationComplete");

        /// <summary>
        /// This event is fired when the recognizer stops, whether from StartRecording() being called, a timeout occurring, or some other error.
        /// Typically, this will simply return "Complete". In this case, we check to see if the recognizer timed out.
        /// </summary>
        /// <param name="cause">An enumerated reason for the session completing.</param>
        private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
        {
            using (DictationCompletePerfMarker.Auto())
            {
                // If Timeout occurs, the user has been silent for too long.
                if (cause == DictationCompletionCause.TimeoutExceeded)
                {
                    Microphone.End(deviceName);

                    dictationResult = "Dictation has timed out. Please try again.";
                }

                Service?.RaiseDictationComplete(inputSource, dictationResult, dictationAudioClip);
                textSoFar = null;
                dictationResult = string.Empty;
            }
        }

        private static readonly ProfilerMarker DictationErrorPerfMarker = new ProfilerMarker("[MRTK] WindowsDictationInputProvider.DictationRecognizer_DictationError");

        /// <summary>
        /// This event is fired when an error occurs.
        /// </summary>
        /// <param name="error">The string representation of the error reason.</param>
        /// <param name="hresult">The int representation of the hresult.</param>
        private void DictationRecognizer_DictationError(string error, int hresult)
        {
            using (DictationErrorPerfMarker.Auto())
            {
                dictationResult = $"{error}\nHRESULT: {hresult}";

                Service?.RaiseDictationError(inputSource, dictationResult);
                textSoFar = null;
                dictationResult = string.Empty;
            }
        }
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        /// <inheritdoc />	 	 
        public AudioClip AudioClip
        {
            get
            {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
                return dictationAudioClip;
#else
                return null;
#endif
            }
        }
    }
}
