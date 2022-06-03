// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Collection of utility methods to simplify working with the Speech subsystem(s).
    /// </summary>
    public static class SpeechUtils
    {
        /// <summary>
        /// Gets the first running <see cref="PhraseRecognitionSubsystem"/> instance.
        /// </summary>
        /// <returns>The running <see cref="PhraseRecognitionSubsystemInstance"/>, or null.</returns>
        public static PhraseRecognitionSubsystem GetSubsystem()
        {
            return XRSubsystemHelpers.GetFirstRunningSubsystem<PhraseRecognitionSubsystem>() as PhraseRecognitionSubsystem;
        }
    }
}
