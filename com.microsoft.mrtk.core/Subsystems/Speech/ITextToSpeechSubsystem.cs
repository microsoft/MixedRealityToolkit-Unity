// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Interface defining the functionality of the Text-To-Speech subsystem.
    /// </summary>
    internal interface ITextToSpeechSubsystem
    {
        /// <summary>
        /// Synthesizes and speaks a text phrase.
        /// </summary>
        /// <param name="phrase">The phrase to be spoken.</param>
        /// <param name="audioSource">The audio source on which to play the generated audio.</param>
        /// <returns>True if the phrase was successfully synthesized and audio playback has begun, otherwise false.</returns>
        Task<bool> TrySpeak(string phrase, AudioSource audioSource);
    }
}
