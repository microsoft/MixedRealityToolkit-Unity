// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Singleton class that implements the DictationRecognizer to convert the user's speech to text.
    /// The DictationRecognizer exposes dictation functionality and supports registering and listening for hypothesis and phrase completed events.
    /// </summary>
    public class DictationInputManager : Singleton<DictationInputManager>, IInputSource
    {
        /// <summary>
        /// Initial value for InitialSilenceTimeout.
        /// </summary>
        private static float initialSilenceTimeout = 5f;

        /// <summary>
        /// The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.
        /// </summary>
        public static float InitialSilenceTimeout
        {
            get
            {
                return dictationRecognizer != null ? dictationRecognizer.InitialSilenceTimeoutSeconds : initialSilenceTimeout;
            }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("value");

                initialSilenceTimeout = value;

                if (dictationRecognizer != null)
                {
                    dictationRecognizer.InitialSilenceTimeoutSeconds = initialSilenceTimeout;
                }
            }
        }

        /// <summary>
        /// Initial value for AutoSilenceTimeout.
        /// </summary>
        private static float autoSilenceTimeout = 20f;

        /// <summary>
        /// The time length in seconds before dictation recognizer session ends due to lack of audio input.
        /// </summary>
        public static float AutoSilenceTimeout
        {
            get
            {
                return dictationRecognizer != null ? dictationRecognizer.AutoSilenceTimeoutSeconds : autoSilenceTimeout;
            }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("value");

                autoSilenceTimeout = value;

                if (dictationRecognizer != null)
                {
                    dictationRecognizer.AutoSilenceTimeoutSeconds = autoSilenceTimeout;
                }
            }
        }

        /// <summary>
        /// Initial value for RecordingTime.
        /// </summary>
        private static int recordingTime = 10;

        /// <summary>
        /// Length in seconds for the manager to listen.
        /// </summary>
        public static int RecordingTime
        {
            get
            {
                return recordingTime;
            }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException("value");

                recordingTime = value;
            }
        }

        /// <summary>
        /// Caches the text currently being displayed in dictation display text.
        /// </summary>
        private static StringBuilder textSoFar;

        /// <summary>
        /// <remarks>Using an empty string specifies the default microphone.</remarks>
        /// </summary>
        private static readonly string DeviceName = string.Empty;

        /// <summary>
        /// The device audio sammpling rate.
        /// <remarks>Set by UnityEngine.Microphone.<see cref="Microphone.GetDeviceCaps"/></remarks>
        /// </summary>
        private static int samplingRate;

        /// <summary>
        /// Use this to reset the UI once the Mic is done recording.
        /// </summary>
        private static bool recordingStarted;

        /// <summary>
        /// String result of the current dictation.
        /// </summary>
        private static string dictationResult;

        /// <summary>
        /// Audio clip of the last dictation session.
        /// </summary>
        private static AudioClip dictationAudioClip;

        private static DictationRecognizer dictationRecognizer;

        #region Unity Methods

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
            int minSamplingRate; // Unsued.
            Microphone.GetDeviceCaps(DeviceName, out minSamplingRate, out samplingRate);

            // Use this string to cache the text currently displayed.
            textSoFar = new StringBuilder();

            // Use this to reset once the Microphone is done recording after it was started.
            recordingStarted = false;
        }

        private void Update()
        {
            if (recordingStarted && !Microphone.IsRecording(DeviceName) && dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                recordingStarted = false;

                // If the microphone stops as a result of timing out, make sure to manually stop the dictation recognizer.
                StopRecording();
            }
        }

        protected override void OnDestroy()
        {
            dictationRecognizer.Dispose();

            base.OnDestroy();
        }

        #endregion // Unity Methods

        /// <summary>
        /// Turns on the dictation recognizer and begins recording audio from the default microphone.
        /// </summary>
        public static IEnumerator StartRecording()
        {
            if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
            {
                PhraseRecognitionSystem.Shutdown();
            }

            yield return PhraseRecognitionSystem.Status == SpeechSystemStatus.Stopped;

            dictationRecognizer.Start();

            yield return dictationRecognizer.Status == SpeechSystemStatus.Running;

            recordingStarted = true;

            // Start recording from the microphone.
            dictationAudioClip = Microphone.Start(DeviceName, false, RecordingTime, samplingRate);
        }

        /// <summary>
        /// Ends the recording session.
        /// </summary>
        public static IEnumerator StopRecording()
        {
            Microphone.End(DeviceName);

            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
            }

            yield return dictationRecognizer.Status == SpeechSystemStatus.Stopped;

            PhraseRecognitionSystem.Restart();
        }

        #region Dictation Recognizer Callbacks

        /// <summary>
        /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
        /// </summary>
        /// <param name="text">The currently hypothesized recognition.</param>
        private static void DictationRecognizer_DictationHypothesis(string text)
        {
            // We don't want to append to textSoFar yet, because the hypothesis may have changed on the next event.
            dictationResult = textSoFar.ToString() + " " + text + "...";

            InputManager.Instance.RaiseDictationHypothesis(Instance, 0, dictationResult);
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

            InputManager.Instance.RaiseDictationResult(Instance, 0, dictationResult);
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

            InputManager.Instance.RaiseDictationComplete(Instance, 0, dictationResult, dictationAudioClip);
        }

        /// <summary>
        /// This event is fired when an error occurs.
        /// </summary>
        /// <param name="error">The string representation of the error reason.</param>
        /// <param name="hresult">The int representation of the hresult.</param>
        private static void DictationRecognizer_DictationError(string error, int hresult)
        {
            dictationResult = error + "\nHRESULT: " + hresult.ToString();

            InputManager.Instance.RaiseDictationError(Instance, 0, dictationResult);
        }

        #endregion // Dictation Recognizer Callbacks

        #region IInputSource Implementation

        public SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            return SupportedInputInfo.None;
        }

        public bool SupportsInputInfo(uint sourceId, SupportedInputInfo inputInfo)
        {
            return (GetSupportedInputInfo(sourceId) & inputInfo) != 0;
        }

        public bool TryGetPosition(uint sourceId, out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        public bool TryGetOrientation(uint sourceId, out Quaternion orientation)
        {
            orientation = Quaternion.identity;
            return false;
        }

        #endregion // IInputSource Implementation
    }
}
