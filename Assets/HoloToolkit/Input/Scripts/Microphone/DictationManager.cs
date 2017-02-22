// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace HoloToolkit.Unity.InputModule
{
    public class DictationManager : Singleton<DictationManager>
    {
        /// <summary>
        /// Dictation Audio Source. Useful for dictation playback.
        /// </summary>
        public AudioSource DictationAudioSource { get; private set; }

        /// <summary>
        /// String result of the current dictation.
        /// </summary>
        public string DictationResult { get; private set; }

        /// <summary>
        /// Initial value for InitialSilenceTimeout. Only used to initialize the DictationRecognizer's InitialSilenceTimeout value during Start.
        /// </summary>
        [SerializeField]
        [Tooltip("The default timeout with initial silence is 5 seconds.")]
        [Range(0.1f, 30f)]
        private float initialSilenceTimeout = 5f;
        public float InitialSilenceTimeout
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
        /// Initial value for AutoSilenceTimeout. Only used to initalize the DictationRecognizer's AutoSilenceTimeout value during Start.
        /// </summary>
        [SerializeField]
        [Tooltip("The default timeout after a recognition is 20 seconds.")]
        [Range(5f, 60f)]
        private float autoSilenceTimeout = 20f;
        public float AutoSilenceTimeout
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
        /// Length in seconds for the manager to listen.
        /// </summary>
        [SerializeField]
        [Tooltip("Length in seconds for the manager to listen.")]
        [Range(1f, 60f)]
        private int recordingTime = 10;

        /// <summary>
        /// Caches the text currently being displayed in dictation display text.
        /// </summary>
        private StringBuilder textSoFar;

        /// <summary>
        /// <remarks>Using an empty string specifies the default microphone.</remarks>
        /// </summary>
        private static readonly string DeviceName = string.Empty;

        /// <summary>
        /// The device audio sammpling rate.
        /// <remarks>Set by UnityEngine.Microphone.<see cref="Microphone.GetDeviceCaps"/></remarks>
        /// </summary>
        private int samplingRate;

        /// <summary>
        /// Use this to reset the UI once the Mic is done recording.
        /// </summary>
        private bool recordingStarted;

        private DictationRecognizer dictationRecognizer;

        protected override void Awake()
        {
            base.Awake();

            DictationResult = string.Empty;

            DictationAudioSource = gameObject.GetComponent<AudioSource>();

            dictationRecognizer = new DictationRecognizer();
            dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
            dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
            dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
            dictationRecognizer.DictationError += DictationRecognizer_DictationError;

            //Initialize our timeout values
            dictationRecognizer.InitialSilenceTimeoutSeconds = initialSilenceTimeout;
            dictationRecognizer.AutoSilenceTimeoutSeconds = autoSilenceTimeout;

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
            Debug.Log(dictationRecognizer.Status);

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

        /// <summary>
        /// Turns on the dictation recognizer and begins recording audio from the default microphone.
        /// </summary>
        /// <returns>The audio clip recorded from the microphone.</returns>
        public void StartRecording()
        {
            PhraseRecognitionSystem.Shutdown();

            dictationRecognizer.Start();

            recordingStarted = true;

            // Start recording from the microphone.
            DictationAudioSource.clip = Microphone.Start(DeviceName, false, recordingTime, samplingRate);
        }

        /// <summary>
        /// Ends the recording session.
        /// </summary>
        public void StopRecording()
        {
            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                dictationRecognizer.Stop();
            }

            Microphone.End(DeviceName);

            PhraseRecognitionSystem.Restart();
        }

        #region Dictation Recognizer Callbacks

        /// <summary>
        /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
        /// </summary>
        /// <param name="text">The currently hypothesized recognition.</param>
        private void DictationRecognizer_DictationHypothesis(string text)
        {
            // We don't want to append to textSoFar yet, because the hypothesis may have changed on the next event.
            DictationResult = textSoFar.ToString() + " " + text + "...";
        }

        /// <summary>
        /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
        /// </summary>
        /// <param name="text">The text that was heard by the recognizer.</param>
        /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
        private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
        {
            textSoFar.Append(text + ". ");

            DictationResult = textSoFar.ToString();
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
                Microphone.End(DeviceName);

                DictationResult = "Dictation has timed out. Please try again.";
            }
        }

        /// <summary>
        /// This event is fired when an error occurs.
        /// </summary>
        /// <param name="error">The string representation of the error reason.</param>
        /// <param name="hresult">The int representation of the hresult.</param>
        private void DictationRecognizer_DictationError(string error, int hresult)
        {
            DictationResult = error + "\nHRESULT: " + hresult.ToString();
        }

        #endregion // Dictation Recognizer Callbacks
    }
}
