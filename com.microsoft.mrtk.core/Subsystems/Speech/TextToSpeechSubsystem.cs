// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// A subsystem to enable text-to-speech.
    /// </summary>
    [Preserve]
    public class TextToSpeechSubsystem :
        MRTKSubsystem<TextToSpeechSubsystem, TextToSpeechSubsystemDescriptor, TextToSpeechSubsystem.Provider>,
        ITextToSpeechSubsystem
    {
        /// <summary>
        /// Construct the <c>TextToSpeechSubsystem</c>.
        /// </summary>
        public TextToSpeechSubsystem()
        { }

        /// <summary>
        /// An abstract class for the provider that will implement the ITextToSpeechSubsystem.
        /// </summary>
        [Preserve]
        public abstract class Provider : MRTKSubsystemProvider<TextToSpeechSubsystem>, ITextToSpeechSubsystem
        {
            #region ITextToSpeechSubsystem implementation

            /// <inheritdoc/>
            public abstract Task<bool> TrySpeak(string phrase, AudioSource audioSource);

            #endregion ITextToSpeechSubsystem implementation
        }

        #region ITextToSpeechSubsystem implementation

        /// <inheritdoc/>
        public virtual Task<bool> TrySpeak(string phrase, AudioSource audioSource) => provider.TrySpeak(phrase, audioSource);

        #endregion ITextToSpeechSubsystem implementation

        /// <summary>
        /// Registers a TextToSpeechSubsystem implementation based on the given subsystem parameters.
        /// </summary>
        /// <param name="subsystemParams">The parameters defining the TextToSpeechSubsystem
        /// functionality implemented by the subsystem provider.</param>
        /// <returns>
        /// <c>true</c> if the subsystem implementation is registered. Otherwise, <c>false</c>.
        /// </returns>
        public static bool Register(TextToSpeechSubsystemCinfo subsystemParams)
        {
            TextToSpeechSubsystemDescriptor Descriptor = TextToSpeechSubsystemDescriptor.Create(subsystemParams);
            SubsystemDescriptorStore.RegisterDescriptor(Descriptor);
            return true;
        }
    }
}
