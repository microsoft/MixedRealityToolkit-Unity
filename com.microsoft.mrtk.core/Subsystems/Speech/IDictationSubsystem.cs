// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Subsystems
{
    /// <summary>
    /// Specification for what a DictationSubsystem needs to be able to provide.
    /// Both the DictationSubsystem implementation and the associated provider
    /// MUST implement this interface, preferably with a direct 1:1 mapping
    /// between the provider surface and the subsystem surface.
    /// </summary>
    public interface IDictationSubsystem : ISubsystem
    {
        /// <summary>
        /// Start dictation with default configurations.
        /// There may be other overloads with parameters to specify configurations depending on the implementation.
        /// </summary>
        void StartDictation();

        /// <summary>
        /// Stop dictation.
        /// </summary>
        void StopDictation();

        /// <summary>
        /// Action triggered when the recognizer is processing the input and has a tentative result.
        /// </summary>
        public event Action<DictationResultEventArgs> Recognizing;

        /// <summary>
        /// Action triggered when the recognizer recognized the input and returns a final result.
        /// </summary>
        public event Action<DictationResultEventArgs> Recognized;

        /// <summary>
        /// Action triggered when the recognition session is finished.
        /// </summary>
        public event Action<DictationSessionEventArgs> RecognitionFinished;

        /// <summary>
        /// Action triggered when the recognition is faulted (i.e. error occurred).
        /// </summary>
        public event Action<DictationSessionEventArgs> RecognitionFaulted;
    }
}
