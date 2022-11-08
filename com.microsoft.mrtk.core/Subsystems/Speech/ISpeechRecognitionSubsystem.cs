// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Specification for what a SpeechRecognitionSubsystem needs to be able to provide.
    /// Both the SpeechRecognitionSubsystem implementation and the associated provider
    /// MUST implement this interface, preferably with a direct 1:1 mapping
    /// between the provider surface and the subsystem surface.
    /// </summary>
    public interface ISpeechRecognitionSubsystem
    {
        /// <summary>
        /// Start speech recognition with default configurations.
        /// There may be other overloads with parameters to specify configurations depending on the implementation.
        /// </summary>
        void StartRecognition();

        /// <summary>
        /// Stop speech recognition.
        /// </summary>
        void StopRecognition();

        /// <summary>
        /// Action triggered when the recognizer is processing the input and has a tentative result.
        /// </summary>
        public event Action<SpeechRecognitionResultEventArgs> Recognizing;

        /// <summary>
        /// Action triggered when the recognizer recognized the input and returns a final result.
        /// </summary>
        public event Action<SpeechRecognitionResultEventArgs> Recognized;

        /// <summary>
        /// Action triggered when the recognition session is finished.
        /// </summary>
        public event Action<SpeechRecognitionSessionEventArgs> RecognitionFinished;

        /// <summary>
        /// Action triggered when the recognition is faulted (i.e. error occurred).
        /// </summary>
        public event Action<SpeechRecognitionSessionEventArgs> RecognitionFaulted;
    }
}
