// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Interface defining the functionality of the Text-To-Speech subsystem.
    /// </summary>
    internal interface ITextToSpeechSubsystem
    {
        /// <summary>
        /// The rate at which the converted text will be spoken by the speech synthesizer;
        /// </summary>
        int RateOfSpeech { get; set; }

        /// <summary>
        /// Indicates that the value of <see cref="RateOfSpeech"/> has been changed.
        /// </summary>
        event Action<int> RateOfSpeechChanged;

        /// <summary>
        /// Synthesizes and speaks a text phrase.
        /// </summary>
        /// <param name="phrase">The phrase to be spoken.</param>
        /// <param name="audioSource">The audio source on which to play the generated audio.</param>
        void Speak(string phrase, AudioSource audioSource);
    }
}
