// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices
{
    /// <summary>
    /// Mixed Reality Toolkit controller definition, used to manage a specific controller type
    /// </summary>
    public interface IMixedRealitySpeechController
    {
        /// <summary>
        /// Prepare the speech recognition for operation
        /// Autostart if the profile is configured to do so
        /// </summary>
        void Initialize();

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

        /// <summary>
        /// Cleanup any speech recognition references / services
        /// </summary>
        void Dispose();
    }
}