// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
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
        Name = "com.microsoft.mixedreality.speech",
        DisplayName = "Windows UWP Text-To-Speech Subsystem",
        Author = "Microsoft",
        ProviderType = typeof(WindowsUWPTextToSpeechSubsystemProvider),
        SubsystemTypeOverride = typeof(WindowsUWPTextToSpeechSubsystem),
        ConfigType = typeof(TextToSpeechSubsystemConfig))]
    public class WindowsUWPTextToSpeechSubsystem : TextToSpeechSubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<WindowsUWPTextToSpeechSubsystem, TextToSpeechSubsystemCinfo>();

            if (!WindowsUWPTextToSpeechSubsystem.Register(cinfo))
            {
                Debug.LogError($"Failed to register the {cinfo.Name} subsystem.");
            }
        }

        [Preserve]
        class WindowsUWPTextToSpeechSubsystemProvider : Provider
        {
            protected TextToSpeechSubsystemConfig Config { get; }

            public WindowsUWPTextToSpeechSubsystemProvider() : base()
            {
#if WINDOWS_UWP
                Config = XRSubsystemHelpers.GetConfiguration<TextToSpeechSubsystemConfig, WindowsUWPTextToSpeechSubsystem>();

                rateOfSpeech = Config.RateOfSpeech;
                speechSynthesizer = new SpeechSynthesizer();
#else
                Debug.Log("The Windows UWP Text-To-Speech Subsystem requires building as a UWP target.");
#endif // WINDOWS_UWP
            }

            public override void Destroy()
            {
#if WINDOWS_UWP
                speechSynthesizer.Dispose();
                speechSynthesizer = null;
#endif // WINDOWS_UWP
            }

#region ITextToSpeechSubsystem implementation

            private int rateOfSpeech = 0;

            /// <inheritdoc/>
            public override int RateOfSpeech
            {
                get => rateOfSpeech;
                set
                {
                    if (rateOfSpeech != value)
                    {
                        rateOfSpeech = value;
                        RaiseRateOfSpeechChanged(value);
                    }
                }
            }

            /// <inheritdoc/>
            public override event Action<int> RateOfSpeechChanged;

            /// <summary>
            /// Sends a <see cref="RateOfSpeechChanged"/> event to registered listeners.
            /// </summary>
            /// <param name="rate">The new rate of speech value.</param>
            private void RaiseRateOfSpeechChanged(int rate)
            {
                RateOfSpeechChanged?.Invoke(rate);
            }

#if WINDOWS_UWP
            private SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
#endif // WINDOWS_UWP

#if !WINDOWS_UWP
// On non-UWP platforms, there will be no awaits in this method, suppress the warning to avoid
// cluttering the console window in the Editor.
#pragma warning disable 1998
#endif // !WINDOWS_UWP

            /// <inheritdoc/>
            public override async void Speak(string phrase, AudioSource audioSource)
            {
#if WINDOWS_UWP
                if (string.IsNullOrWhiteSpace(phrase))
                {
                    Debug.LogWarning("Nothing to speek");
                    return;
                }

                if (audioSource == null)
                {
                    Debug.LogError("Must specify the AudioSource object on which the text to speech data should be applied.");
                    return;
                }

                // By specifying to use the default synthesizer voice, we will speak in
                // the voice selected, in Settings, by the user.
                speechSynthesizer.Voice = SpeechSynthesizer.DefaultVoice;
                // todo: apply rate of speech (via SpeechSynthesizerOptions)

                SpeechSynthesisStream synthStream = await speechSynthesizer.SynthesizeTextToStreamAsync(phrase);

                // Allocate a byte array to receive the wave data
                byte[] waveBytes = new byte[(uint)synthStream.Size];

                // Read the wave data.
                using (IInputStream stream = synthStream.GetInputStreamAt(0))
                {
                    // We can safely close the synthesis stream.
                    synthStream.Dispose();

                    using (DataReader reader = new DataReader(stream))
                    {
                        await reader.LoadAsync((uint)waveBytes.Length);
                        reader.ReadBytes(waveBytes);
                    }
                }

                // Convert from bytes to floats and create an AudioClip.
                if (!TextToSpeechHelpers.TryConvertWaveData(
                        waveBytes,
                        out int samples,
                        out int sampleRate,
                        out int channels,
                        out float[] audioFloats))
                {
                    Debug.LogError("Failed to convert speech audio format.");
                    return;
                }

                audioSource.clip = TextToSpeechHelpers.CreateAudioClip(
                    "SyntgesizedText",
                    audioFloats,
                    samples,
                    channels,
                    sampleRate);
#endif //WINDOWS_UWP
            }
#if !WINDOWS_UWP
            // On non-UWP platforms, restore the missing await warnings.
#pragma warning restore 1998
#endif // !WINDOWS_UWP

            #endregion TextToSpeechSubsystem implementation
        }
    }
}
