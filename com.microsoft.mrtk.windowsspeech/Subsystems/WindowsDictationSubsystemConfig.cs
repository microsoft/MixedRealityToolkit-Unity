// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
using UnityEngine.Windows.Speech;
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

namespace Microsoft.MixedReality.Toolkit.Speech.Windows
{
    /// <summary>
    /// The configuration object for <see cref="Microsoft.MixedReality.Toolkit.Speech.Windows.WindowsDictationSubsystem">WindowsDictationSubsystem</see>.
    /// </summary>
    [CreateAssetMenu(
        fileName = "WindowsDictationSubsystemConfig.asset",
        menuName = "MRTK/Subsystems/Windows Dictation Subsystem Config")]
    public class WindowsDictationSubsystemConfig : BaseSubsystemConfig
    {
        /// <summary>
        /// The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.
        /// </summary>
        [field: SerializeField, Tooltip("The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.")]
        public float InitialSilenceTimeoutSeconds { get; set; } = 5f;

        /// <summary>
        /// The time length in seconds before dictation recognizer session ends due to lack of audio input.
        /// </summary>
        [field: SerializeField, Tooltip("The time length in seconds before dictation recognizer session ends due to lack of audio input.")]
        public float AutoSilenceTimeout { get; set; } = 20f;

        [SerializeField, Tooltip("The confidence threshold for the recognizer to return its result.")]
        private WindowsSpeechConfidenceLevel confidenceLevel = WindowsSpeechConfidenceLevel.Medium;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
        /// <summary>
        /// The confidence threshold for the recognizer to return its result.
        /// </summary>
        public ConfidenceLevel ConfidenceLevel
        {
            get => confidenceLevel.ToUnityConfidenceLevel();
            set => confidenceLevel = value.ToWindowsSpeechConfidenceLevel();
        }
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN
    }
}
