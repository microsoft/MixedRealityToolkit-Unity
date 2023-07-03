// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

#if WINDOWS_UWP
using Windows.Foundation;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.Speech.Windows
{
    /// <summary>
    /// A Unity subsystem that extends <see cref="Microsoft.MixedReality.Toolkit.Subsystems.TextToSpeechSubsystem">TextToSpeechSubsystem</see>
    /// so to expose the text to speech services available on Windows platforms. This subsystem is enabled for Windows Standalone and
    /// Universal Windows Applications. 
    /// </summary>
    /// <remarks>
    /// This subsystem can be configured using the <see cref="Microsoft.MixedReality.Toolkit.Speech.Windows.WindowsKeywordRecognitionSubsystemConfig">WindowsKeywordRecognitionSubsystemConfig</see> Unity asset.
    /// </remarks>
    [Preserve]
    [MRTKSubsystem(
        Name = "com.microsoft.mrtk.windowsspeech.texttospeech",
        DisplayName = "Windows Text-To-Speech Subsystem",
        Author = "Microsoft",
        ProviderType = typeof(WindowsTextToSpeechSubsystemProvider),
        SubsystemTypeOverride = typeof(WindowsTextToSpeechSubsystem),
        ConfigType = typeof(WindowsTextToSpeechSubsystemConfig))]
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

        /// <summary>
        /// A subsystem provider for <see cref="WindowsTextToSpeechSubsystem"/> that exposes methods on the Windows
        /// speech synthesizer systems.
        /// </summary>
        [Preserve]
        class WindowsTextToSpeechSubsystemProvider : Provider
        {
            private WindowsTextToSpeechSubsystemConfig config;
#if WINDOWS_UWP
            private SpeechSynthesizer synthesizer;
            private VoiceInformation voiceInfo;
#endif

            public WindowsTextToSpeechSubsystemProvider() : base()
            { }

            public override void Start()
            {
                config = XRSubsystemHelpers.GetConfiguration<WindowsTextToSpeechSubsystemConfig, WindowsTextToSpeechSubsystem>();
#if WINDOWS_UWP
                synthesizer = new SpeechSynthesizer();
#endif
            }

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
                // Change voice?
                if (config.Voice != TextToSpeechVoice.Default)
                {
                    // See if it's never been found or is changing
                    if ((voiceInfo == null) || (!voiceInfo.DisplayName.Contains(config.VoiceName)))
                    {
                        // Search for voice info
                        voiceInfo = SpeechSynthesizer.AllVoices.Where(v => v.DisplayName.Contains(config.VoiceName)).FirstOrDefault();

                        // If found, select
                        if (voiceInfo != null)
                        {
                            synthesizer.Voice = voiceInfo;
                        }
                        else
                        {
                            Debug.LogErrorFormat("TTS voice {0} could not be found.", config.VoiceName);
                        }
                    }
                }
                else
                {
                    synthesizer.Voice = SpeechSynthesizer.DefaultVoice;
                }

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
                    if (!WinRTTextToSpeechPInvokes.TrySynthesizePhraseWithCustomVoice(phrase, config.VoiceName, out IntPtr nativeData, out int length))
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

            #endregion TextToSpeechSubsystem implementation
        }
    }
}
