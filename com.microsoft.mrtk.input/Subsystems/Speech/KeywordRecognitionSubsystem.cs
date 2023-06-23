// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Scripting;
using UnityEngine.SubsystemsImplementation;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// A subsystem that exposes information about the user's KeywordRecognition.
    /// </summary>
    [Preserve]
    public class KeywordRecognitionSubsystem :
        MRTKSubsystem<KeywordRecognitionSubsystem, KeywordRecognitionSubsystemDescriptor, KeywordRecognitionSubsystem.Provider>,
        IKeywordRecognitionSubsystem
    {
        /// <summary>
        /// Construct the <c>KeywordRecognitionSubsystem</c>.
        /// </summary>
        public KeywordRecognitionSubsystem()
        { }

        /// <summary>
        /// Interface for providing recognition functionality for the implementation.
        /// </summary>
        public abstract class Provider : MRTKSubsystemProvider<KeywordRecognitionSubsystem>, IKeywordRecognitionSubsystem
        {
            #region IKeywordRecognitionSubsystem implementation

            /// <inheritdoc/>
            public abstract UnityEvent CreateOrGetEventForKeyword(string keyword);

            /// <inheritdoc/>
            public abstract void RemoveKeyword(string keyword);

            /// <inheritdoc/>
            public abstract void RemoveAllKeywords();

            /// <inheritdoc/>
            public abstract IReadOnlyDictionary<string, UnityEvent> GetAllKeywords();

            #endregion IKeywordRecognitionSubsystem implementation

            /// <summary>
            /// The dictionary storing the current keywords and their associated actions.
            /// </summary>
            protected Dictionary<string, UnityEvent> keywordDictionary = new Dictionary<string, UnityEvent>();
        }

        #region IKeywordRecognitionSubsystem implementation

        /// <inheritdoc/>
        public UnityEvent CreateOrGetEventForKeyword(string keyword) => provider.CreateOrGetEventForKeyword(keyword);

        /// <inheritdoc/>
        public void RemoveKeyword(string keyword) => provider.RemoveKeyword(keyword);

        /// <inheritdoc/>
        public void RemoveAllKeywords() => provider.RemoveAllKeywords();

        ///<inheritdoc/>
        public IReadOnlyDictionary<string, UnityEvent> GetAllKeywords() => provider.GetAllKeywords();

        #endregion IKeywordRecognitionSubsystem implementation

        /// <summary>
        /// Registers a KeywordRecognition subsystem implementation based on the given subsystem parameters.
        /// </summary>
        /// <param name="keywordRecognitionSubsystemParams">The parameters defining the KeywordRecognition subsystem functionality implemented
        /// by the subsystem provider.</param>
        /// <returns>
        /// <c>true</c> if the subsystem implementation is registered. Otherwise, <c>false</c>.
        /// </returns>
        public static bool Register(KeywordRecognitionSubsystemCinfo keywordRecognitionSubsystemParams)
        {
            var descriptor = KeywordRecognitionSubsystemDescriptor.Create(keywordRecognitionSubsystemParams);
            SubsystemDescriptorStore.RegisterDescriptor(descriptor);
            return true;
        }
    }

}
