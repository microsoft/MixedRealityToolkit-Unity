// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;
#if WINDOWS_UWP
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.Speech.Windows
{
    [Preserve]
    [MRTKSubsystem(
        Name = "com.microsoft.mixedreality.speech",
        DisplayName = "Windows Text-To-Speech Subsystem",
        Author = "Microsoft",
        ProviderType = typeof(WindowsTextToSpeechSubsystemProvider),
        SubsystemTypeOverride = typeof(WindowsTextToSpeechSubsystem),
        ConfigType = typeof(BaseSubsystemConfig))]
    public class WindowsTextToSpeechSubsystem : TextToSpeechSubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<WindowsTextToSpeechSubsystem, TextToSpeechSubsystemCinfo>();

            if (!WindowsTextToSpeechSubsystem.Register(cinfo))
            {
                Debug.LogError($"Failed to register the {cinfo.Name} subsystem.");
            }
        }

        [Preserve]
        class WindowsTextToSpeechSubsystemProvider : Provider
        {
            public WindowsTextToSpeechSubsystemProvider() : base()
            { }

            public override void Destroy()
            {
#if WINDOWS_UWP
                if (synthesizer != null)
                {
                    synthesizer.Dispose();
                    synthesizer = null;
                }
#endif // WINDOWS_UWP
            }

            #region ITextToSpeechSubsystem implementation

            /// <inheritdoc/>
            public override bool TrySpeak(string phrase, AudioSource audioSource)
            {
                if (audioSource == null)
                {
                    Debug.LogError("Must specify the AudioSource object on which the text to speech data should be applied.");
                    return false;
                }

                if (!TrySynthesize(phrase, out byte[] waveData))
                {
                    return false;
                }

                // Convert from bytes to floats and create an AudioClip.
                if (!TextToSpeechHelpers.TryConvertWaveData(
                        waveData,
                        out int samples,
                        out int sampleRate,
                        out int channels,
                        out float[] audioFloats))
                {
                    Debug.LogError("Failed to convert speech audio format.");
                    return false;
                }

                audioSource.clip = TextToSpeechHelpers.CreateAudioClip(
                    "SynthesizedText",
                    audioFloats,
                    samples,
                    channels,
                    sampleRate);

                audioSource.Play();
                
                return true;
            }

            /// <summary>
            /// Attempts to synthesize the specified phrase.
            /// </summary>
            /// <param name="phrase"></param>
            /// <param name="waveData"></param>
            /// <returns>True if the phrase was successfully synthesized, or false.</returns>
            private bool TrySynthesize(string phrase, out byte[] waveData)
            {
                waveData = null;

                if (string.IsNullOrWhiteSpace(phrase))
                {
                    Debug.LogWarning("Nothing to speek");
                    return false;
                }

#if WINDOWS_UWP
                // By specifying to use the default synthesizer voice, we will speak in
                // the voice selected, in Settings, by the user.
                synthesizer.Voice = SpeechSynthesizer.DefaultVoice;

                Task<SpeechSynthesisStream> synthTask = synthesizer.SynthesizeTextToStreamAsync(phrase).AsTask<SpeechSynthesisStream>();
                while (!synthTask.IsCompleted)
                {
                    System.Threading.Thread.Sleep(0);
                }
                SpeechSynthesisStream synthStream = synthTask.Result;

                // Allocate a byte array to receive the wave data
                waveData = new byte[(uint)synthStream.Size];

                // Read the wave data.
                using (IInputStream stream = synthStream.GetInputStreamAt(0))
                {
                    // We can safely close the synthesis stream.
                    synthStream.Dispose();

                    using (DataReader reader = new DataReader(stream))
                    {
                        Task<uint> loadTask = reader.LoadAsync((uint)waveData.Length).AsTask<uint>();
                        while (!loadTask.IsCompleted)
                        {
                            System.Threading.Thread.Sleep(0);
                        }
                        // No real need to look at the Result property of the loadTask.

                        reader.ReadBytes(waveData);
                    }
                }
#elif (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
                if (!WinRTTextToSpeechPInvokes.TrySynthesizePhrase(phrase, out IntPtr nativeData, out int length))
                {
                    Debug.LogError("Failed to synthesize the phrase");
                    return false;
                }

                waveData = new byte[length];
                Marshal.Copy(nativeData, waveData, 0, length);
                // We can safely free the native data.
                WinRTTextToSpeechPInvokes.FreeSynthesizedData(nativeData);
#endif
                return true;
            }

#if WINDOWS_UWP
            private SpeechSynthesizer synthesizer = new SpeechSynthesizer();
#endif

#endregion TextToSpeechSubsystem implementation
        }
    }
}
