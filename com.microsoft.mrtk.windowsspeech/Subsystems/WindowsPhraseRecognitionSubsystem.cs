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
    [Preserve]
    [MRTKSubsystem(
        Name = "com.microsoft.mixedreality.windowsphraserecognition",
        DisplayName = "MRTK Windows PhraseRecognition Subsystem",
        Author = "Microsoft",
        ProviderType = typeof(WindowsPhraseRecognitionProvider),
        SubsystemTypeOverride = typeof(WindowsPhraseRecognitionSubsystem))]
    public class WindowsPhraseRecognitionSubsystem : PhraseRecognitionSubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<WindowsPhraseRecognitionSubsystem, PhraseRecognitionSubsystemCinfo>();

            if (!PhraseRecognitionSubsystem.Register(cinfo))
            {
                Debug.LogError($"Failed to register the {cinfo.Name} subsystem.");
            }
        }

        [Preserve]
        class WindowsPhraseRecognitionProvider : Provider
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            private KeywordRecognizer keywordRecognizer;
#if MSFT_OPENXR_1_5_0_OR_NEWER
            private SelectKeywordRecognizer selectKeywordRecognizer;
#endif // MSFT_OPENXR_1_5_0_OR_NEWER
            private ConcurrentQueue<UnityEvent> eventQueue;
            private bool keywordListChanged;
#endif

            /// <summary>
            /// Constructor of WindowsPhraseRecognitionProvider.
            /// </summary>
            public WindowsPhraseRecognitionProvider()
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                eventQueue = new ConcurrentQueue<UnityEvent>();
                keywordListChanged = false;
#else
                Debug.LogError("Cannot create WindowsPhraseRecognitionProvider because WindowsPhraseRecognitionProvider is only supported on Windows Editor, Standalone Windows and UWP.");
#endif
            }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            /// <inheritdoc/>
            public override void Start()
            {
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
                new ProfilerMarker("[MRTK] WindowsPhraseRecognitionSubsystem.Update");

            /// <inheritdoc/>
            public override void Update()
            {
                using (UpdatePerfMarker.Auto())
                {
                    if (keywordListChanged)
                    {
                        keywordListChanged = false;
                        Destroy();
                        keywordRecognizer = new KeywordRecognizer(phraseDictionary.Keys.ToArray());
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

            #region IPhraseRecognitionSubsystem implementation

            /// <inheritdoc/>
            public override UnityEvent CreateOrGetEventForPhrase(string phrase)
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                if (phraseDictionary.TryGetValue(phrase, out UnityEvent e))
                {
                    return e;
                }
                else
                {
                    keywordListChanged = true;
                    UnityEvent unityEvent = new UnityEvent();
                    phraseDictionary.Add(phrase, unityEvent);
                    return unityEvent;
                }
#else
                Debug.LogError("Cannot call CreateOrGetEventForPhrase because WindowsPhraseRecognitionProvider is only supported on Windows Editor, Standalone Windows and UWP.");
                return null;
#endif
            }

            /// <inheritdoc/>
            public override void RemovePhrase(string phrase)
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                phraseDictionary.Remove(phrase);
                keywordListChanged = true;
#else
                Debug.LogError("Cannot call RemovePhrase because WindowsPhraseRecognitionProvider is only supported on Windows Editor, Standalone Windows and UWP.");
#endif
            }

            /// <inheritdoc/>
            public override void RemoveAllPhrases()
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                phraseDictionary.Clear();
                Destroy();
#else
                Debug.LogError("Cannot call RemoveAllPhrases because WindowsPhraseRecognitionProvider is only supported on Windows Editor, Standalone Windows and UWP.");
#endif
            }

            /// <inheritdoc/>
            public override IReadOnlyDictionary<string, UnityEvent> GetAllPhrases()
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
                return phraseDictionary;
#else
                Debug.LogError("Cannot call GetAllPhrases because WindowsPhraseRecognitionProvider is only supported on Windows Editor, Standalone Windows and UWP.");
                return null;
#endif
            }

            #endregion IPhraseRecognitionSubsystem implementation

            #region Helpers

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA
            private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
            {
                if (phraseDictionary.TryGetValue(args.text, out UnityEvent e))
                {
                    eventQueue.Enqueue(e);
                }
            }
#endif

            #endregion Helpers
        }
    }
}
