// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Async;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Async.AwaitYieldInstructions;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
using System.Text;
using UnityEngine.Windows.Speech;
#endif

namespace Microsoft.MixedReality.Toolkit.InputSystem.Sources
{
    /// <summary>
    /// Singleton class that implements the DictationRecognizer to convert the user's speech to text.
    /// The DictationRecognizer exposes dictation functionality and supports registering and listening for hypothesis and phrase completed events.
    /// </summary>
    public class DictationInputSource : BaseGenericInputSource
    {
        /// <summary>
        /// Is the Dictation Manager currently running?
        /// </summary>
        public static bool IsListening { get; private set; } = false;

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        /// <summary>
        /// Constructor.
        /// </summary>
        public DictationInputSource() : base("Dictation")
        {
            if (!Application.isPlaying) { return; }

            source = this;
            dictationResult = string.Empty;

            dictationRecognizer = new DictationRecognizer();
            dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
            dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
            dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
            dictationRecognizer.DictationError += DictationRecognizer_DictationError;

            // Query the maximum frequency of the default microphone.
            int minSamplingRate; // Not used.
            Microphone.GetDeviceCaps(DeviceName, out minSamplingRate, out samplingRate);

            Run();
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            if (dictationRecognizer != null)
            {
                dictationRecognizer.DictationHypothesis -= DictationRecognizer_DictationHypothesis;
                dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
                dictationRecognizer.DictationComplete -= DictationRecognizer_DictationComplete;
                dictationRecognizer.DictationError -= DictationRecognizer_DictationError;
                dictationRecognizer?.Dispose();
            }
        }

        private static IMixedRealityInputSource source;
        private static DictationRecognizer dictationRecognizer;

        private static bool hasFailed;
        private static bool hasListener;
        private static bool isTransitioning;

        /// <summary>
        /// Caches the text currently being displayed in dictation display text.
        /// </summary>
        private static StringBuilder textSoFar;

        /// <summary>
        /// <remarks>Using an empty string specifies the default microphone.</remarks>
        /// </summary>
        private static readonly string DeviceName = string.Empty;

        /// <summary>
        /// The device audio sampling rate.
        /// <remarks>Set by UnityEngine.Microphone.<see cref="Microphone.GetDeviceCaps"/></remarks>
        /// </summary>
        private static int samplingRate;

        /// <summary>
        /// String result of the current dictation.
        /// </summary>
        private static string dictationResult;

        /// <summary>
        /// Audio clip of the last dictation session.
        /// </summary>
        private static AudioClip dictationAudioClip;

        private static readonly WaitForUpdate NextUpdate = new WaitForUpdate();

        private static async void Run()
        {
            while (true)
            {
                if (IsListening && !Microphone.IsRecording(DeviceName) && dictationRecognizer.Status == SpeechSystemStatus.Running)
                {
                    // If the microphone stops as a result of timing out, make sure to manually stop the dictation recognizer.
                    await StopRecordingAsync();
                }

                if (!hasFailed && dictationRecognizer.Status == SpeechSystemStatus.Failed)
                {
                    hasFailed = true;
                    InputSystem.RaiseDictationError(source, "Dictation recognizer has failed!");
                }

                await NextUpdate;
            }
        }

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        /// <summary>
        /// Turns on the dictation recognizer and begins recording audio from the default microphone.
        /// </summary>
        /// <param name="listener">GameObject listening for the dictation input.</param>
        /// <param name="initialSilenceTimeout">The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.</param>
        /// <param name="autoSilenceTimeout">The time length in seconds before dictation recognizer session ends due to lack of audio input.</param>
        /// <param name="recordingTime">Length in seconds for the manager to listen.</param>
        /// <returns></returns>
        public static async Task StartRecordingAsync(GameObject listener = null, float initialSilenceTimeout = 5f, float autoSilenceTimeout = 20f, int recordingTime = 10)
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            if (IsListening || isTransitioning)
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
                InputSystem.PushModalInputHandler(listener);
            }

            if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
            {
                PhraseRecognitionSystem.Shutdown();
            }

            await new WaitUntil(() => PhraseRecognitionSystem.Status != SpeechSystemStatus.Running);

            dictationRecognizer.InitialSilenceTimeoutSeconds = initialSilenceTimeout;
            dictationRecognizer.AutoSilenceTimeoutSeconds = autoSilenceTimeout;
            dictationRecognizer.Start();

            await new WaitUntil(() => dictationRecognizer.Status != SpeechSystemStatus.Stopped);

            if (dictationRecognizer.Status == SpeechSystemStatus.Failed)
            {
                InputSystem.RaiseDictationError(source, "Dictation recognizer failed to start!");
                return;
            }

            // Start recording from the microphone.
            dictationAudioClip = Microphone.Start(DeviceName, false, recordingTime, samplingRate);
            textSoFar = new StringBuilder();
            isTransitioning = false;
#else

            Debug.LogError("Unable to start recording!  Dictation is unsupported for this platform.");
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
        }

        /// <summary>
        /// Ends the recording session.
        /// </summary>
        public static async Task StopRecordingAsync()
        {
#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
            if (!IsListening || isTransitioning)
            {
                Debug.LogWarning("Unable to stop recording");
                return;
            }

            IsListening = false;
            isTransitioning = true;

            if (hasListener)
            {
                InputSystem.PopModalInputHandler();
                hasListener = false;
            }

            Microphone.End(DeviceName);

            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
            }

            await new WaitUntil(() => PhraseRecognitionSystem.Status != SpeechSystemStatus.Running);

            PhraseRecognitionSystem.Restart();

            await new WaitUntil(() => PhraseRecognitionSystem.Status == SpeechSystemStatus.Running);

            isTransitioning = false;
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
        }

        #region Dictation Recognizer Callbacks

#if UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        /// <summary>
        /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
        /// </summary>
        /// <param name="text">The currently hypothesized recognition.</param>
        private static void DictationRecognizer_DictationHypothesis(string text)
        {
            // We don't want to append to textSoFar yet, because the hypothesis may have changed on the next event.
            dictationResult = $"{textSoFar} {text}...";

            InputSystem.RaiseDictationHypothesis(source, dictationResult);
        }

        /// <summary>
        /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
        /// </summary>
        /// <param name="text">The text that was heard by the recognizer.</param>
        /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
        private static void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
        {
            textSoFar.Append($"{text}. ");

            dictationResult = textSoFar.ToString();

            InputSystem.RaiseDictationResult(source, dictationResult);
        }

        /// <summary>
        /// This event is fired when the recognizer stops, whether from StartRecording() being called, a timeout occurring, or some other error.
        /// Typically, this will simply return "Complete". In this case, we check to see if the recognizer timed out.
        /// </summary>
        /// <param name="cause">An enumerated reason for the session completing.</param>
        private static void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
        {
            // If Timeout occurs, the user has been silent for too long.
            if (cause == DictationCompletionCause.TimeoutExceeded)
            {
                Microphone.End(DeviceName);

                dictationResult = "Dictation has timed out. Please try again.";
            }

            InputSystem.RaiseDictationComplete(source, dictationResult, dictationAudioClip);
            textSoFar = null;
            dictationResult = string.Empty;
        }

        /// <summary>
        /// This event is fired when an error occurs.
        /// </summary>
        /// <param name="error">The string representation of the error reason.</param>
        /// <param name="hresult">The int representation of the hresult.</param>
        private static void DictationRecognizer_DictationError(string error, int hresult)
        {
            dictationResult = $"{error}\nHRESULT: {hresult}";

            InputSystem.RaiseDictationError(source, dictationResult);
            textSoFar = null;
            dictationResult = string.Empty;
        }

#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

        #endregion Dictation Recognizer Callbacks
    }
}
