// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
using UnityEngine.Windows.Speech;
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

namespace Microsoft.MixedReality.Toolkit.Speech.Windows
{
    /// <summary>
    /// The recognition confidence level for WindowsDictationSubsystem and WindowsKeywordRecognitionSubsystem.
    /// Mirrors UnityEngine.Windows.Speech.ConfidenceLevel
    /// </summary>
    internal enum WindowsSpeechConfidenceLevel
    {
        High,
        Medium,
        Low,
        Rejected
    }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
    internal static class WindowsSpeechConfidenceLevelExtensions
    {
        internal static ConfidenceLevel ToUnityConfidenceLevel(this WindowsSpeechConfidenceLevel windowsSpeechConfidenceLevel)
        {
            return windowsSpeechConfidenceLevel switch
            {
                WindowsSpeechConfidenceLevel.Rejected => ConfidenceLevel.Rejected,
                WindowsSpeechConfidenceLevel.Low => ConfidenceLevel.Low,
                WindowsSpeechConfidenceLevel.Medium => ConfidenceLevel.Medium,
                WindowsSpeechConfidenceLevel.High => ConfidenceLevel.High,
                _ => ConfidenceLevel.Medium
            };
        }

        internal static WindowsSpeechConfidenceLevel ToWindowsSpeechConfidenceLevel(this ConfidenceLevel confidenceLevel)
        {
            return confidenceLevel switch
            {
                ConfidenceLevel.Rejected => WindowsSpeechConfidenceLevel.Rejected,
                ConfidenceLevel.Low => WindowsSpeechConfidenceLevel.Low,
                ConfidenceLevel.Medium => WindowsSpeechConfidenceLevel.Medium,
                ConfidenceLevel.High => WindowsSpeechConfidenceLevel.High,
                _ => WindowsSpeechConfidenceLevel.Medium
            };
        }
    }
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
}
