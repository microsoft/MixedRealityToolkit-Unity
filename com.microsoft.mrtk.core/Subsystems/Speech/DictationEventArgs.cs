// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Event data associated with the result of dictation.
    /// </summary>
    public readonly struct DictationResultEventArgs
    {
        /// <summary>
        /// The result of dictation.
        /// </summary>
        public string Result { get; }

        /// <summary>
        /// The confidence of the dictation result on a scale of 0 to 1.
        /// Null when the confidence is not available.
        /// </summary>
        public float? Confidence { get; }

        /// <summary>
        /// Construct the <c>DictationResultEventArgs</c>.
        /// </summary>
        public DictationResultEventArgs(string result, float? confidence)
        {
            Confidence = confidence;
            Result = result;
        }
    }

    /// <summary>
    ///  Event data associated with the session of dictation.
    /// </summary>
    public readonly struct DictationSessionEventArgs
    {
        /// <summary>
        /// The reason for this dictation session event.
        /// </summary>
        public DictationEventReason Reason { get; }
        /// <summary>
        /// The reason for this dictation session event in string.
        /// </summary>
        public string ReasonString { get; }

        /// <summary>
        /// Construct the <c>DictationSessionEventArgs</c>.
        /// </summary>
        public DictationSessionEventArgs(DictationEventReason reason, string reasonString)
        {
            Reason = reason;
            ReasonString = reasonString;
        }
    }
}
