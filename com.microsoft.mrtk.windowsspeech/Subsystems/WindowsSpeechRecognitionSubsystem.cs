// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Scripting;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
using UnityEngine.Windows.Speech;
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

namespace Microsoft.MixedReality.Toolkit.Speech.Windows
{
    [Preserve]
    [MRTKSubsystem(
        Name = "com.microsoft.mixedreality.windowsspeechrecognition",
        DisplayName = "MRTK Windows SpeechRecognition Subsystem",
        Author = "Microsoft",
        ProviderType = typeof(WindowsSpeechRecognitionProvider),
        SubsystemTypeOverride = typeof(WindowsSpeechRecognitionSubsystem),
        ConfigType = typeof(WindowsSpeechRecognitionSubsystemConfig))]
    public class WindowsSpeechRecognitionSubsystem : SpeechRecognitionSubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<WindowsSpeechRecognitionSubsystem, SpeechRecognitionSubsystemCinfo>();

            if (!Register(cinfo))
            {
                Debug.LogError($"Failed to register the {cinfo.Name} subsystem.");
            }
        }

        [Preserve]
        class WindowsSpeechRecognitionProvider : Provider
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            /// <summary>
            /// The confidence threshold for the recognizer to return its result.
            /// </summary>
            public ConfidenceLevel ConfidenceLevel => confidenceLevel;

            /// <summary>
            /// The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.
            /// </summary>
            public float InitialSilenceTimeoutSeconds
            {
                get => initialSilenceTimeoutSeconds;
                set
                {
                    if (initialSilenceTimeoutSeconds != value)
                    {
                        initialSilenceTimeoutSeconds = value;
                        if (dictationRecognizer != null)
                        {
                            dictationRecognizer.InitialSilenceTimeoutSeconds = value;
                        }
                    }
                }
            }

            /// <summary>
            /// The time length in seconds before dictation recognizer session ends due to lack of audio input.
            /// </summary>
            public float AutoSilenceTimeout
            {
                get => autoSilenceTimeout;
                set
                {
                    if (autoSilenceTimeout != value)
                    {
                        autoSilenceTimeout = value;
                        if (dictationRecognizer != null)
                        {
                            dictationRecognizer.AutoSilenceTimeoutSeconds = value;
                        }
                    }
                }
            }

            private WindowsSpeechRecognitionSubsystemConfig config;
            private DictationRecognizer dictationRecognizer;
            private ConfidenceLevel confidenceLevel;
            private float initialSilenceTimeoutSeconds;
            private float autoSilenceTimeout;
#endif

            /// <summary>
            /// Constructor of WindowsSpeechRecognitionProvider.
            /// </summary>
            public WindowsSpeechRecognitionProvider()
            {
#if !(UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA)
                Debug.LogError("Cannot create WindowsSpeechRecognitionSubsystem because WindowsSpeechRecognitionSubsystem is only supported on Windows Editor, Standalone Windows and UWP.");
#endif
            }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            /// <inheritdoc/>
            public override void Start()
            {
                base.Start();
                config = XRSubsystemHelpers.GetConfiguration<WindowsSpeechRecognitionSubsystemConfig, WindowsSpeechRecognitionSubsystem>();
                confidenceLevel = config.ConfidenceLevel;
                initialSilenceTimeoutSeconds = config.InitialSilenceTimeoutSeconds;
                autoSilenceTimeout = config.AutoSilenceTimeout;
            }
#endif


            /// <inheritdoc/>
            public override void Destroy()
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                if (dictationRecognizer != null)
                {
                    DeregisterDictationEvents();
                    dictationRecognizer.Dispose();
                    dictationRecognizer = null;
                }
#endif
            }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            /// <summary>
            /// Start speech recognition using the specified ConfidenceLevel.
            /// </summary>
            public void StartRecognition(ConfidenceLevel confidenceLevel)
            {
                if (dictationRecognizer == null || this.confidenceLevel != confidenceLevel)
                {
                    Destroy();
                    this.confidenceLevel = confidenceLevel;
                    dictationRecognizer = new DictationRecognizer(confidenceLevel);
                    dictationRecognizer.InitialSilenceTimeoutSeconds = initialSilenceTimeoutSeconds;
                    dictationRecognizer.AutoSilenceTimeoutSeconds = autoSilenceTimeout;
                }
                if (dictationRecognizer.Status != SpeechSystemStatus.Running)
                {
                    dictationRecognizer.Start();
                    // Deregister first to prevent duplicate
                    DeregisterDictationEvents();
                    RegisterDictationEvents();
                }
            }
#endif

            #region ISpeechRecognitionSubsystem implementation

            /// <inheritdoc/>
            public override void StartRecognition()
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                StartRecognition(confidenceLevel);
#else
                Debug.LogError("Cannot call StartRecognition because WindowsSpeechRecognitionSubsystem is only supported on Windows Editor, Standalone Windows and UWP.");
#endif
            }

            /// <inheritdoc/>
            public override void StopRecognition()
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                if (dictationRecognizer == null)
                {
                    dictationRecognizer.Stop();
                }
#else
                Debug.LogError("Cannot call StopRecognition because WindowsSpeechRecognitionSubsystem is only supported on Windows Editor, Standalone Windows and UWP.");
#endif
            }

            #endregion ISpeechRecognitionSubsystem implementation

            #region Helpers

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA

            private void RegisterDictationEvents()
            {
                dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
                dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
                dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
                dictationRecognizer.DictationError += DictationRecognizer_DictationError;
            }

            private void DeregisterDictationEvents()
            {
                dictationRecognizer.DictationHypothesis -= DictationRecognizer_DictationHypothesis;
                dictationRecognizer.DictationResult -= DictationRecognizer_DictationResult;
                dictationRecognizer.DictationComplete -= DictationRecognizer_DictationComplete;
                dictationRecognizer.DictationError -= DictationRecognizer_DictationError;
            }

            /// <summary>
            /// This event is fired while the user is talking. As the recognizer listens, it provides text of what it's heard so far.
            /// </summary>
            /// <param name="text">The currently hypothesized recognition.</param>
            private void DictationRecognizer_DictationHypothesis(string text)
            {
                SpeechRecognitionResultEventArgs eventArgs = new SpeechRecognitionResultEventArgs(text, null);
                OnRecognizing(eventArgs);
            }

            /// <summary>
            /// This event is fired after the user pauses, typically at the end of a sentence. The full recognized string is returned here.
            /// </summary>
            /// <param name="text">The text that was heard by the recognizer.</param>
            /// <param name="confidence">A representation of how confident (rejected, low, medium, high) the recognizer is of this recognition.</param>
            private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
            {
                SpeechRecognitionResultEventArgs eventArgs = new SpeechRecognitionResultEventArgs(text, ConfidenceLevelToFloat(confidence));
                OnRecognized(eventArgs);
            }

            /// <summary>
            /// This event is fired when the recognizer stops, whether from StartRecording() being called, a timeout occurring, or some other error.
            /// Typically, this will simply return "Complete".
            /// </summary>
            /// <param name="cause">An enumerated reason for the session completing.</param>
            private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
            {
                SpeechRecognitionSessionEventArgs eventArgs = new SpeechRecognitionSessionEventArgs(ToSpeechRecognitionEventReason(cause),
                    cause.ToString());
                OnRecognitionFinished(eventArgs);
            }

            /// <summary>
            /// This event is fired when an error occurs.
            /// </summary>
            /// <param name="error">The string representation of the error reason.</param>
            /// <param name="hresult">The int representation of the hresult.</param>
            private void DictationRecognizer_DictationError(string error, int hresult)
            {
                SpeechRecognitionSessionEventArgs eventArgs = new SpeechRecognitionSessionEventArgs(SpeechRecognitionEventReason.UnknownFailure,
                    error.ToString() + "\nHRESULT: " + hresult);
                OnRecognitionFaulted(eventArgs);
            }

            private float ConfidenceLevelToFloat(ConfidenceLevel level)
            {
                return level switch
                {
                    ConfidenceLevel.Rejected => 0,
                    ConfidenceLevel.Low => 0.25f,
                    ConfidenceLevel.Medium => 0.5f,
                    ConfidenceLevel.High => 0.75f,
                    _ => 0,
                };
            }

            private SpeechRecognitionEventReason ToSpeechRecognitionEventReason(DictationCompletionCause cause)
            {
                return cause switch
                {
                    DictationCompletionCause.Complete => SpeechRecognitionEventReason.Complete,
                    DictationCompletionCause.AudioQualityFailure => SpeechRecognitionEventReason.AudioQualityFailure,
                    DictationCompletionCause.Canceled => SpeechRecognitionEventReason.Canceled,
                    DictationCompletionCause.TimeoutExceeded => SpeechRecognitionEventReason.TimeoutExceeded,
                    DictationCompletionCause.PauseLimitExceeded => SpeechRecognitionEventReason.PauseLimitExceeded,
                    DictationCompletionCause.NetworkFailure => SpeechRecognitionEventReason.NetworkFailure,
                    DictationCompletionCause.MicrophoneUnavailable => SpeechRecognitionEventReason.MicrophoneUnavailable,
                    DictationCompletionCause.UnknownError => SpeechRecognitionEventReason.UnknownFailure,
                    _ => SpeechRecognitionEventReason.Unknown,
                };
            }
#endif

            #endregion Helpers
        }
    }
}
