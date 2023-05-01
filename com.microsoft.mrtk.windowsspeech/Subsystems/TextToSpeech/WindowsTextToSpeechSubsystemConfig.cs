// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Speech.Windows
{
    /// <summary>
    /// Configuration for Windows Text To Speech Subsystem
    /// </summary>
    [CreateAssetMenu(
        fileName = "WindowsTextToSpeechSubsystemConfig.asset",
        menuName = "MRTK/Subsystems/Windows Text to Speech Subsystem Config")]
    public class WindowsTextToSpeechSubsystemConfig : BaseSubsystemConfig
    {
        [Tooltip("The voice that will be used to generate speech. To use a non en-US voice, set this to Other.")]
        [SerializeField]
        private TextToSpeechVoice voice;

        /// <summary>
        /// Gets or sets the voice that will be used to generate speech. To use a non en-US voice, set this to Other.
        /// </summary>
        /// <remarks>
        /// If a custom voice is desired (i.e. this enum is being set to Other) make sure to set the <see cref="VoiceName"/> property.
        /// </remarks>
        public TextToSpeechVoice Voice { get { return voice; } set { voice = value; } }

        [Tooltip("The custom voice that will be used to generate speech. See below for the list of available voices.")]
        [SerializeField]
        private string customVoice = string.Empty;

        /// <summary>
        /// Gets or sets the voice that will be used to generate speech.
        /// </summary>
        /// <remarks>
        /// It is required to set the voice through this property when using a custom voice.
        /// </remarks>
        public string VoiceName
        {
            get
            {
                return Voice != TextToSpeechVoice.Other ? Voice.ToString() : customVoice;
            }
            set
            {
                if (Enum.TryParse(value, out TextToSpeechVoice parsedVoice))
                {
                    Voice = parsedVoice;
                }
                else
                {
                    Voice = TextToSpeechVoice.Other;
                    customVoice = value;
                }
            }
        }
    }
}
