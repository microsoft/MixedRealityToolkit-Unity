// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Scripting;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// A subsystem that exposes information about the user's PhraseRecognition.
    /// </summary>
    [Preserve]
    public class PhraseRecognitionSubsystem :
        MRTKSubsystem<PhraseRecognitionSubsystem, PhraseRecognitionSubsystemDescriptor, PhraseRecognitionSubsystem.Provider>,
        IPhraseRecognitionSubsystem
    {
        /// <summary>
        /// Construct the <c>PhraseRecognitionSubsystem</c>.
        /// </summary>
        public PhraseRecognitionSubsystem()
        { }

        /// <summary>
        /// Interface for providing recognition functionality for the implementation.
        /// </summary>
        public abstract class Provider : MRTKSubsystemProvider<PhraseRecognitionSubsystem>, IPhraseRecognitionSubsystem
        {
            #region IPhraseRecognitionSubsystem implementation

            /// <inheritdoc/>
            public abstract UnityEvent CreateOrGetEventForPhrase(string phrase);

            /// <inheritdoc/>
            public abstract void RemovePhrase(string phrase);

            /// <inheritdoc/>
            public abstract void RemoveAllPhrases();

            /// <inheritdoc/>
            public abstract IReadOnlyDictionary<string, UnityEvent> GetAllPhrases();

            #endregion IPhraseRecognitionSubsystem implementation

            /// <summary>
            /// The dictionary storing the current phrases and their associated actions.
            /// </summary>
            protected Dictionary<string, UnityEvent> phraseDictionary = new Dictionary<string, UnityEvent>();
        }

        #region IPhraseRecognitionSubsystem implementation

        /// <inheritdoc/>
        public UnityEvent CreateOrGetEventForPhrase(string phrase) => provider.CreateOrGetEventForPhrase(phrase);

        /// <inheritdoc/>
        public void RemovePhrase(string phrase) => provider.RemovePhrase(phrase);

        /// <inheritdoc/>
        public void RemoveAllPhrases() => provider.RemoveAllPhrases();

        ///<inheritdoc/>
        public IReadOnlyDictionary<string, UnityEvent> GetAllPhrases() => provider.GetAllPhrases();

        #endregion IPhraseRecognitionSubsystem implementation

        /// <summary>
        /// Registers a PhraseRecognition subsystem implementation based on the given subsystem parameters.
        /// </summary>
        /// <param name="PhraseRecognitionSubsystemParams">The parameters defining the PhraseRecognition subsystem functionality implemented
        /// by the subsystem provider.</param>
        /// <returns>
        /// <c>true</c> if the subsystem implementation is registered. Otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentException">Thrown when the values specified in the
        /// <see cref="PhraseRecognitionSubsystemCinfo"/> parameter are invalid. Typically, this will occur
        /// <list type="bullet">
        /// <item>
        /// <description>if <see cref="PhraseRecognitionSubsystemCinfo.id"/> is <c>null</c> or empty</description>
        /// </item>
        /// <item>
        /// <description>if <see cref="PhraseRecognitionSubsystemCinfo.implementationType"/> is <c>null</c></description>
        /// </item>
        /// <item>
        /// <description>if <see cref="PhraseRecognitionSubsystemCinfo.implementationType"/> does not derive from the
        /// <see cref="PhraseRecognitionSubsystem"/> class
        /// </description>
        /// </item>
        /// </list>
        /// </exception>
        public static bool Register(PhraseRecognitionSubsystemCinfo PhraseRecognitionSubsystemParams)
        {
            PhraseRecognitionSubsystemDescriptor PhraseRecognitionSubsystemDescriptor = PhraseRecognitionSubsystemDescriptor.Create(PhraseRecognitionSubsystemParams);
            SubsystemDescriptorStore.RegisterDescriptor(PhraseRecognitionSubsystemDescriptor);
            return true;
        }
    }

}
