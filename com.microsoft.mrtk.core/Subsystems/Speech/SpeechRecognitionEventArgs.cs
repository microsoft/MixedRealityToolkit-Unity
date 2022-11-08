// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Event data associated with the result of speech recognition.
    /// </summary>
    public class SpeechRecognitionResultEventArgs
    {
        /// <summary>
        /// The result of speech recognition.
        /// </summary>
        public string Result { get; private set; }

        /// <summary>
        /// The confidence of the speech recognition result on a scale of 0 to 1.
        /// Null when the confidence is not available.
        /// </summary>
        public float? Confidence { get; private set; }

        /// <summary>
        /// Construct the <c>SpeechRecognitionResultEventArgs</c>.
        /// </summary>
        public SpeechRecognitionResultEventArgs(string result, float? confidence)
        {
            Confidence = confidence;
            Result = result;
        }
    }

    /// <summary>
    ///  Event data associated with the session of speech recognition.
    /// </summary>
    public class SpeechRecognitionSessionEventArgs
    {
        /// <summary>
        /// The reason for this speech recognition session event.
        /// </summary>
        public SpeechRecognitionEventReason Reason { get; private set; }
        /// <summary>
        /// The reason for this speech recognition session event in string.
        /// </summary>
        public string ReasonString { get; private set; }

        /// <summary>
        /// Construct the <c>SpeechRecognitionSessionEventArgs</c>.
        /// </summary>
        public SpeechRecognitionSessionEventArgs(SpeechRecognitionEventReason reason, string reasonString)
        {
            Reason = reason;
            ReasonString = reasonString;
        }
    }
}
