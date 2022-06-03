// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Specification for what a PhraseRecognitionSubsystem needs to be able to provide.
    /// Both the PhraseRecognitionSubsystem implementation and the associated provider
    /// MUST implement this interface, preferably with a direct 1:1 mapping
    /// between the provider surface and the subsystem surface.
    /// </summary>
    public interface IPhraseRecognitionSubsystem
    {
        /// <summary>
        /// Add or update a phrase to recognize.
        /// </summary>
        UnityEvent CreateOrGetEventForPhrase(string phrase);

        /// <summary>
        /// Remove a phrase to recognize.
        /// </summary>
        void RemovePhrase(string phrase);

        /// <summary>
        /// Remove all phrases to recognize.
        /// </summary>
        /// <param name="joint">Identifier of the requested joint.</param>
        void RemoveAllPhrases();

        /// <summary>
        /// Get a read-only reference to the all phrases that are currently registered with the recognizer.
        /// </summary>
        IReadOnlyDictionary<string, UnityEvent> GetAllPhrases();
    }
}