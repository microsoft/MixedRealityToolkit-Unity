// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//#if WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Microsoft.MixedReality.Toolkit.WindowsSpeech
{
    [Preserve]
    [MRTKSubsystem(
        Name = "com.microsoft.mixedreality.windowsspeech",
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
                Config = XRSubsystemHelpers.GetConfiguration<TextToSpeechSubsystemConfig, WindowsUWPTextToSpeechSubsystem>();

                rateOfSpeech = Config.RateOfSpeech;
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

            /// <inheritdoc/>
            public override void Speak(string phrase, AudioSource audioSource)
            {
                throw new NotImplementedException();
            }

            #endregion TextToSpeechSubsystem implementation
        }
    }
}
//#endif WINDOWS_UWP
