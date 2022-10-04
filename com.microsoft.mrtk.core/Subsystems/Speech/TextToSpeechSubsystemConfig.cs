// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// The configuration object for all TextToSpeechSubsystem implementations.
    /// </summary>
    [CreateAssetMenu(
        fileName = "TextToSpeechSubsystemConfig.asset",
        menuName = "MRTK/Subsystems/TextToSpeechSubsystem Config")]
    public class TextToSpeechSubsystemConfig : BaseSubsystemConfig
    {
        [SerializeField]
        [Tooltip("The rate at which synthesized speech should be spoken.")]
        private int rateOfSpeech;

        /// <summary>
        /// The rate at which synthesized speech should be spoken.
        /// </summary>
        public int RateOfSpeech
        {
            get => rateOfSpeech;
            set => rateOfSpeech = value;    // todo: validate range
        }
    }
}
