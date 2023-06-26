// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
#if MSFT_OPENXR_1_5_0_OR_NEWER
using Microsoft.MixedReality.OpenXR;
#endif // MSFT_OPENXR_1_5_0_OR_NEWER
using UnityEngine.Windows.Speech;
#endif // UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_EDITOR_WIN

namespace Microsoft.MixedReality.Toolkit.Speech.Windows
{
    /// <summary>
    /// A Unity subsystem that extends <see cref="Microsoft.MixedReality.Toolkit.Subsystems.KeywordRecognitionSubsystem">KeywordRecognitionSubsystem</see>
    /// so to expose the keyword recognition services available on Windows platforms. This subsystem is enabled for Windows Standalone and
    /// Universal Windows Applications. 
    /// </summary>
    /// <remarks>
    /// This subsystem can be configured using the <see cref="Microsoft.MixedReality.Toolkit.Speech.Windows.WindowsKeywordRecognitionSubsystemConfig">WindowsKeywordRecognitionSubsystemConfig</see> Unity asset.
    /// </remarks>
    [Preserve]
    [MRTKSubsystem(
        Name = "com.microsoft.mixedreality.windowskeywordrecognition",
        DisplayName = "MRTK Windows KeywordRecognition Subsystem",
        Author = "Microsoft",
        ProviderType = typeof(WindowsKeywordRecognitionProvider),
        SubsystemTypeOverride = typeof(WindowsKeywordRecognitionSubsystem),
        ConfigType = typeof(WindowsKeywordRecognitionSubsystemConfig))]
    public class WindowsKeywordRecognitionSubsystem : KeywordRecognitionSubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<WindowsKeywordRecognitionSubsystem, KeywordRecognitionSubsystemCinfo>();

            if (!Register(cinfo))
            {
                Debug.LogError($"Failed to register the {cinfo.Name} subsystem.");
            }
        }

        /// <summary>
        /// A subsystem provider used with <see cref="WindowsKeywordRecognitionSubsystem"/> that exposes methods on the
        /// <see cref="Microsoft.MixedReality.OpenXR.SelectKeywordRecognizer">SelectKeywordRecognizer</see> and Unity's 
        /// `KeywordRecognizer`. 
        /// </summary>
        [Preserve]
        class WindowsKeywordRecognitionProvider : Provider
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            /// <summary>
            /// The confidence threshold for the recognizer to return its result.
            /// </summary>
            public ConfidenceLevel ConfidenceLevel
            {
                get => confidenceLevel;
                set
                {
                    if (confidenceLevel != value)
                    {
                        confidenceLevel = value;
                        if (keywordRecognizer != null)
                        {
                            reinitRecognizerRequired = true;
                        }
                    }
                }
            }

            private WindowsKeywordRecognitionSubsystemConfig config;
            private ConfidenceLevel confidenceLevel;
            private KeywordRecognizer keywordRecognizer;
#if MSFT_OPENXR_1_5_0_OR_NEWER
            private SelectKeywordRecognizer selectKeywordRecognizer;
#endif // MSFT_OPENXR_1_5_0_OR_NEWER
            private ConcurrentQueue<UnityEvent> eventQueue;
            private bool reinitRecognizerRequired;
#endif

            /// <summary>
            /// Constructor of WindowsKeywordRecognitionProvider.
            /// </summary>
            public WindowsKeywordRecognitionProvider()
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                eventQueue = new ConcurrentQueue<UnityEvent>();
                reinitRecognizerRequired = false;
#else
                Debug.LogError("Cannot create WindowsKeywordRecognitionProvider because WindowsKeywordRecognitionProvider is only supported on Windows Editor, Standalone Windows and UWP.");
#endif
            }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            /// <inheritdoc/>
            public override void Start()
            {
                config = XRSubsystemHelpers.GetConfiguration<WindowsKeywordRecognitionSubsystemConfig, WindowsKeywordRecognitionSubsystem>();
                confidenceLevel = config.ConfidenceLevel;
                if (keywordRecognizer != null)
                {
                    keywordRecognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
                    keywordRecognizer.Start();
                }
#if MSFT_OPENXR_1_5_0_OR_NEWER
                if (selectKeywordRecognizer != null)
                {
                    selectKeywordRecognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
                    selectKeywordRecognizer.Start();
                }
#endif // MSFT_OPENXR_1_5_0_OR_NEWER
            }

            private static readonly ProfilerMarker UpdatePerfMarker =
                new ProfilerMarker("[MRTK] WindowsKeywordRecognitionSubsystem.Update");

            /// <inheritdoc/>
            public override void Update()
            {
                using (UpdatePerfMarker.Auto())
                {
                    if (reinitRecognizerRequired)
                    {
                        reinitRecognizerRequired = false;
                        Destroy();
                        keywordRecognizer = new KeywordRecognizer(keywordDictionary.Keys.ToArray(), confidenceLevel);
#if MSFT_OPENXR_1_5_0_OR_NEWER
                        if (SelectKeywordRecognizer.IsSupported)
                        {
                            selectKeywordRecognizer = new SelectKeywordRecognizer();
                        }
#endif // MSFT_OPENXR_1_5_0_OR_NEWER
                        Start();
                    }
                    while (eventQueue.TryDequeue(out UnityEvent unityEvent))
                    {
                        unityEvent.Invoke();
                    }
                }
            }

            /// <inheritdoc/>
            public override void Stop()
            {
                if (keywordRecognizer != null)
                {
                    keywordRecognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
                    keywordRecognizer.Stop();

                    // Calling stop won't shutdown the recognition system after it has started.
                    // This recognition system should be shut down when not in use, so that
                    // things like dictation can occur.
                    if (PhraseRecognitionSystem.isSupported)
                    {
                        PhraseRecognitionSystem.Shutdown();
                    }
                }
#if MSFT_OPENXR_1_5_0_OR_NEWER
                if (selectKeywordRecognizer != null)
                {
                    selectKeywordRecognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
                    selectKeywordRecognizer.Stop();
                }
#endif // MSFT_OPENXR_1_5_0_OR_NEWER
            }

            /// <inheritdoc/>
            public override void Destroy()
            {
                if (keywordRecognizer != null)
                {
                    keywordRecognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
                    keywordRecognizer.Dispose();
                    keywordRecognizer = null;
                }
#if MSFT_OPENXR_1_5_0_OR_NEWER
                if (selectKeywordRecognizer != null)
                {
                    selectKeywordRecognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
                    selectKeywordRecognizer.Dispose();
                    selectKeywordRecognizer = null;
                }
#endif // MSFT_OPENXR_1_5_0_OR_NEWER
            }
#endif

            #region IKeywordRecognitionSubsystem implementation

            /// <inheritdoc/>
            public override UnityEvent CreateOrGetEventForKeyword(string keyword)
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                if (keywordDictionary.TryGetValue(keyword, out UnityEvent e))
                {
                    return e;
                }
                else
                {
                    reinitRecognizerRequired = true;
                    UnityEvent unityEvent = new UnityEvent();
                    keywordDictionary.Add(keyword, unityEvent);
                    return unityEvent;
                }
#else
                Debug.LogError("Cannot call CreateOrGetEventForKeyword because WindowsKeywordRecognitionProvider is only supported on Windows Editor, Standalone Windows and UWP.");
                return null;
#endif
            }

            /// <inheritdoc/>
            public override void RemoveKeyword(string keyword)
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                keywordDictionary.Remove(keyword);
                reinitRecognizerRequired = true;
#else
                Debug.LogError("Cannot call RemoveKeyword because WindowsKeywordRecognitionProvider is only supported on Windows Editor, Standalone Windows and UWP.");
#endif
            }

            /// <inheritdoc/>
            public override void RemoveAllKeywords()
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                keywordDictionary.Clear();
                Destroy();
#else
                Debug.LogError("Cannot call RemoveAllKeywords because WindowsKeywordRecognitionProvider is only supported on Windows Editor, Standalone Windows and UWP.");
#endif
            }

            /// <inheritdoc/>
            public override IReadOnlyDictionary<string, UnityEvent> GetAllKeywords()
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                return keywordDictionary;
#else
                Debug.LogError("Cannot call GetAllKeywords because WindowsKeywordRecognitionProvider is only supported on Windows Editor, Standalone Windows and UWP.");
                return null;
#endif
            }

            #endregion IKeywordRecognitionSubsystem implementation

            #region Helpers

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
            {
                if (keywordDictionary.TryGetValue(args.text, out UnityEvent e))
                {
                    eventQueue.Enqueue(e);
                }
            }
#endif

            #endregion Helpers
        }
    }
}
