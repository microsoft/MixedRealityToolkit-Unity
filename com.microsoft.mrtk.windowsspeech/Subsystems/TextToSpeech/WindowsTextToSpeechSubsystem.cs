// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

#if WINDOWS_UWP
using Windows.Foundation;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.Speech.Windows
{
    [Preserve]
    [MRTKSubsystem(
        Name = "com.microsoft.mrtk.windowsspeech.texttospeech",
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

#if !(WINDOWS_UWP || UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
            private bool haveLogged = false;
#endif

            /// <inheritdoc/>
            public override async Task<bool> TrySpeak(string phrase, AudioSource audioSource)
            {
                if (audioSource == null)
                {
                    Debug.LogError("Must specify the AudioSource object on which the text to speech data should be applied.");
                    return false;
                }

                byte[] waveData = await Synthesize(phrase);
                if (waveData == null)
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
            /// Synthesizes the specified phrase.
            /// </summary>
            /// <param name="phrase">The phrase to be synthesized.</param>
            /// <returns>The audio (wave) data upon successful synthesis, or null.</returns>
            private async Task<byte[]> Synthesize(string phrase)
            {
                if (string.IsNullOrWhiteSpace(phrase))
                {
                    Debug.LogWarning("Nothing to speak");
                    return null;
                }

#if WINDOWS_UWP
                // By specifying to use the default synthesizer voice, we will speak in
                // the voice selected, in Settings, by the user.
                synthesizer.Voice = SpeechSynthesizer.DefaultVoice;

                SpeechSynthesisStream synthStream = await synthesizer.SynthesizeTextToStreamAsync(phrase);

                // Allocate a byte array to receive the wave data
                byte[] waveData = new byte[(uint)synthStream.Size];

                // Read the wave data.
                using (IInputStream stream = synthStream.GetInputStreamAt(0))
                {
                    // We can safely close the synthesis stream.
                    synthStream.Dispose();

                    using (DataReader reader = new DataReader(stream))
                    {
                        await reader.LoadAsync((uint)waveData.Length);
                        reader.ReadBytes(waveData);
                    }
                }

                return waveData;
#elif (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
                return await Task<byte[]>.Run(() =>
                {
                    if (!WinRTTextToSpeechPInvokes.TrySynthesizePhrase(phrase, out IntPtr nativeData, out int length))
                    {
                        Debug.LogError("Failed to synthesize the phrase");
                        return null;
                    }

                    byte[] waveData = new byte[length];
                    Marshal.Copy(nativeData, waveData, 0, length);
                    // We can safely free the native data.
                    WinRTTextToSpeechPInvokes.FreeSynthesizedData(nativeData);

                    return waveData;
                });
#else
                if (!haveLogged)
                {
                    Debug.LogError("The Windows Text-To-Speech subsystem is not supported on the current platform.");
                    haveLogged = true;
                }
                return await Task.FromResult<byte[]>(null);
#endif
            }

#if WINDOWS_UWP
            private SpeechSynthesizer synthesizer = new SpeechSynthesizer();
#endif

            #endregion TextToSpeechSubsystem implementation
        }
    }
}
