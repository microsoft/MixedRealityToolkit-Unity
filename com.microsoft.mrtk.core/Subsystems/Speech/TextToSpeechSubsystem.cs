// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.Scripting;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
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

        [Preserve]
        public abstract class Provider : MRTKSubsystemProvider<TextToSpeechSubsystem>, ITextToSpeechSubsystem
        {
            #region ITextToSpeechSubsystem implementation

            // TODO: Implement abstract Provider class.

            #endregion ITextToSpeechSubsystem implementation
        }

        #region ITextToSpeechSubsystem implementation

        // TODO: Calls into abstract Provider (ex: public int MaxValue => provider.MaxValue;

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
