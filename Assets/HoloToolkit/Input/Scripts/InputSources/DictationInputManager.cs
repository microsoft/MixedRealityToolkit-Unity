// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

#if UNITY_WSA || UNITY_STANDALONE_WIN
using System.Text;
using UnityEngine.Windows.Speech;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Singleton class that implements the DictationRecognizer to convert the user's speech to text.
    /// The DictationRecognizer exposes dictation functionality and supports registering and listening for hypothesis and phrase completed events.
    /// </summary>
    public class DictationInputManager : Singleton<DictationInputManager>, IInputSource
    {
#if UNITY_WSA || UNITY_STANDALONE_WIN
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
        /// Is the Dictation Manager currently running?
        /// </summary>
        public static bool IsListening { get; private set; }

        /// <summary>
        /// String result of the current dictation.
        /// </summary>
        private static string dictationResult;

        /// <summary>
        /// Audio clip of the last dictation session.
        /// </summary>
        private static AudioClip dictationAudioClip;

        private static DictationRecognizer dictationRecognizer;

        private static bool isTransitioning;
        private static bool hasFailed;

#endif // UNITY_WSA || UNITY_STANDALONE_WIN

        #region IInputSource Implementation

        public uint SourceId { get; protected set; }

        public string Name { get { return "Dictation"; } }

        public SupportedInputInfo GetSupportedInputInfo()
        {
            return SupportedInputInfo.Voice;
        }

        public bool SupportsInputInfo(SupportedInputInfo inputInfo)
        {
            return (GetSupportedInputInfo() & inputInfo) == inputInfo;
        }

        #endregion IInputSource Implementation

        #region IEquality Implementation

        private bool Equals(IInputSource other)
        {
            return base.Equals(other) && SourceId == other.SourceId && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IInputSource)obj);
        }

        public static bool Equals(IInputSource left, IInputSource right)
        {
            return left.SourceId == right.SourceId;
        }

        bool IEqualityComparer.Equals(object x, object y)
        {
            var left = (IInputSource)x;
            var right = (IInputSource)y;
            if (left != null && right != null)
            {
                return Equals(left, right);
            }

            return false;
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)SourceId;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation

        #region Unity Methods

#if UNITY_WSA || UNITY_STANDALONE_WIN
        protected override void Awake()
        {
            base.Awake();

            dictationResult = string.Empty;

            dictationRecognizer = new DictationRecognizer();
            dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
            dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
            dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
            dictationRecognizer.DictationError += DictationRecognizer_DictationError;

            // Query the maximum frequency of the default microphone.
            int minSamplingRate; // Not used.
            Microphone.GetDeviceCaps(DeviceName, out minSamplingRate, out samplingRate);
        }

        private void Start()
        {
            SourceId = InputManager.GenerateNewSourceId();
        }

        private void LateUpdate()
        {
            if (IsListening && !Microphone.IsRecording(DeviceName) && dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                // If the microphone stops as a result of timing out, make sure to manually stop the dictation recognizer.
                StartCoroutine(StopRecording());
            }

            if (!hasFailed && dictationRecognizer.Status == SpeechSystemStatus.Failed)
            {
                hasFailed = true;
                InputManager.Instance.RaiseDictationError(Instance, "Dictation recognizer has failed!");
            }
        }

        protected override void OnDestroy()
        {
            dictationRecognizer.Dispose();

            base.OnDestroy();
        }
#endif

        #endregion // Unity Methods

        /// <summary>
        /// Turns on the dictation recognizer and begins recording audio from the default microphone.
        /// </summary>
        /// <param name="initialSilenceTimeout">The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.</param>
        /// <param name="autoSilenceTimeout">The time length in seconds before dictation recognizer session ends due to lack of audio input.</param>
        /// <param name="recordingTime">Length in seconds for the manager to listen.</param>
        /// <returns></returns>
        public static IEnumerator StartRecording(float initialSilenceTimeout = 5f, float autoSilenceTimeout = 20f, int recordingTime = 10)
        {
#if UNITY_WSA || UNITY_STANDALONE_WIN
            if (IsListening || isTransitioning)
            {
                Debug.LogWarning("Unable to start recording");
                yield break;
            }

            IsListening = true;
            isTransitioning = true;

            if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
            {
                PhraseRecognitionSystem.Shutdown();
            }

            while (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
            {
                yield return null;
            }

            dictationRecognizer.InitialSilenceTimeoutSeconds = initialSilenceTimeout;
            dictationRecognizer.AutoSilenceTimeoutSeconds = autoSilenceTimeout;
            dictationRecognizer.Start();

            while (dictationRecognizer.Status == SpeechSystemStatus.Failed)
            {
                InputManager.Instance.RaiseDictationError(Instance, "Dictation recognizer failed to start!");
                yield break;
            }

            while (dictationRecognizer.Status == SpeechSystemStatus.Stopped)
            {
                yield return null;
            }

            // Start recording from the microphone.
            dictationAudioClip = Microphone.Start(DeviceName, false, recordingTime, samplingRate);
            textSoFar = new StringBuilder();
            isTransitioning = false;
#else
            return null;
#endif
        }

        /// <summary>
        /// Ends the recording session.
        /// </summary>
        public static IEnumerator StopRecording()
        {
#if UNITY_WSA || UNITY_STANDALONE_WIN
            if (!IsListening || isTransitioning)
            {
                Debug.LogWarning("Unable to stop recording");
                yield break;
            }

            IsListening = false;
            isTransitioning = true;

            Microphone.End(DeviceName);

            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
            }

            while (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                yield return null;
            }

            PhraseRecognitionSystem.Restart();
            isTransitioning = false;
#else
            return null;
#endif
        }

        #region Dictation Recognizer Callbacks

#if UNITY_WSA || UNITY_STANDALONE_WIN

        /// <summary>
        /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
        /// </summary>
        /// <param name="text">The currently hypothesized recognition.</param>
        private static void DictationRecognizer_DictationHypothesis(string text)
        {
            // We don't want to append to textSoFar yet, because the hypothesis may have changed on the next event.
            dictationResult = textSoFar.ToString() + " " + text + "...";

            InputManager.Instance.RaiseDictationHypothesis(Instance, dictationResult);
        }

        /// <summary>
        /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
        /// </summary>
        /// <param name="text">The text that was heard by the recognizer.</param>
        /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
        private static void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
        {
            textSoFar.Append(text + ". ");

            dictationResult = textSoFar.ToString();

            InputManager.Instance.RaiseDictationResult(Instance, dictationResult);
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

            InputManager.Instance.RaiseDictationComplete(Instance, dictationResult, dictationAudioClip);
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
            dictationResult = error + "\nHRESULT: " + hresult.ToString();

            InputManager.Instance.RaiseDictationError(Instance, dictationResult);
            textSoFar = null;
            dictationResult = string.Empty;
        }

#endif // UNITY_WSA || UNITY_STANDALONE_WIN

        #endregion // Dictation Recognizer Callbacks
    }
}
