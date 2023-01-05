// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    /// <summary>
    /// Demonstration script showing how to subscribe to and handle
    /// events fired by SpeechRecognitionSubsystem.
    /// </summary>
    [AddComponentMenu("MRTK/Examples/Speech Recognition Handler")]
    public class SpeechRecognitionHandler : MonoBehaviour
    {

        /// <summary>
        /// Wrapper of UnityEvent&lt;string&gt; for serialization.
        /// </summary>
        [System.Serializable]
        public class StringUnityEvent : UnityEvent<string> { }

        /// <summary>
        /// Event raised while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
        /// </summary>
        public StringUnityEvent OnSpeechRecognizing;

        /// <summary>
        /// Event raised after the user pauses, typically at the end of a sentence. Contains the full recognized string so far.
        /// </summary>
        public StringUnityEvent OnSpeechRecognized;

        /// <summary>
        /// Event raised when the recognizer stops. Contains the final recognized string.
        /// </summary>
        public StringUnityEvent OnRecognitionFinished;

        /// <summary>
        /// Event raised when an error occurs. Contains the string representation of the error reason.
        /// </summary>
        public StringUnityEvent OnRecognitionFaulted;

        private SpeechRecognitionSubsystem speechRecognitionSubsystem;

        /// <summary>
        /// Start speech recognition on a SpeechRecognitionSubsystem.
        /// </summary>
        public void StartRecognition()
        {
            // Make sure there isn't an ongoing recognition session
            StopRecognition();

            speechRecognitionSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<SpeechRecognitionSubsystem>();
            if (speechRecognitionSubsystem != null)
            {
                speechRecognitionSubsystem.Recognizing += SpeechRecognitionSubsystem_Recognizing;
                speechRecognitionSubsystem.Recognized += SpeechRecognitionSubsystem_Recognized;
                speechRecognitionSubsystem.RecognitionFinished += SpeechRecognitionSubsystem_RecognitionFinished;
                speechRecognitionSubsystem.RecognitionFaulted += SpeechRecognitionSubsystem_RecognitionFaulted;
                speechRecognitionSubsystem.StartRecognition();
            }
            else
            {
                Debug.LogError("Cannot find a running SpeechRecognitionSubsystem. Please check the MRTK profile settings " +
                    "(Project Settings -> MRTK3) and/or ensure a SpeechRecognitionSubsystem is running.");
            }
        }

        private void SpeechRecognitionSubsystem_RecognitionFaulted(SpeechRecognitionSessionEventArgs obj)
        {
            OnRecognitionFaulted.Invoke("Recognition faulted. Reason: " + obj.ReasonString);
        }

        private void SpeechRecognitionSubsystem_RecognitionFinished(SpeechRecognitionSessionEventArgs obj)
        {
            OnRecognitionFinished.Invoke("Recognition finished. Reason: " + obj.ReasonString);
        }

        private void SpeechRecognitionSubsystem_Recognized(SpeechRecognitionResultEventArgs obj)
        {
            OnSpeechRecognized.Invoke("Recognized:" + obj.Result);
        }

        private void SpeechRecognitionSubsystem_Recognizing(SpeechRecognitionResultEventArgs obj)
        {
            OnSpeechRecognizing.Invoke("Recognizing:" + obj.Result);
        }

        /// <summary>
        /// Stop speech recognition on the current SpeechRecognitionSubsystem.
        /// </summary>
        public void StopRecognition()
        {
            if (speechRecognitionSubsystem != null)
            {
                speechRecognitionSubsystem.StopRecognition();
                speechRecognitionSubsystem.Recognizing += SpeechRecognitionSubsystem_Recognizing;
                speechRecognitionSubsystem.Recognized += SpeechRecognitionSubsystem_Recognized;
                speechRecognitionSubsystem.RecognitionFinished += SpeechRecognitionSubsystem_RecognitionFinished;
                speechRecognitionSubsystem.RecognitionFaulted += SpeechRecognitionSubsystem_RecognitionFaulted;
                speechRecognitionSubsystem = null;
            }
        }
    }
}
