// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.Controllers
{
    [Obsolete("use IMixedRealityDictationDataProvider instead.")]
    public interface IMixedRealitySpeechSystem { }

    /// <summary>
    /// Mixed Reality Toolkit controller definition, used to manage a specific controller type
    /// </summary>
    public interface IMixedRealitySpeechDataProvider : IMixedRealityControllerDataProvider
    {
        /// <summary>
        /// Query whether or not the speech system is active
        /// </summary>
        bool IsRecognitionActive { get; }

        /// <summary>
        /// Make sure the keyword recognizer is on, then stop it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        void StartRecognition();

        /// <summary>
        /// Make sure the keyword recognizer is on, then stop it.
        /// Otherwise, leave it alone because it's already in the desired state.
        /// </summary>
        void StopRecognition();
    }
}
