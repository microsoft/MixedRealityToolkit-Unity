// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// Mixed Reality Toolkit device definition, used to instantiate and manage a specific device / SDK
    /// </summary>
    public interface IMixedRealityDevice
    {
        /// <summary>
        /// Initialize function to setup the device, called after all managers are initialized by the Mixed Reality Manager
        /// </summary>
        void Initialize();

        /// <summary>
        /// Function to enable the device if disabled
        /// </summary>
        void Enable();

        /// <summary>
        /// Function to disable the device if enabled
        /// </summary>
        void Disable();

        /// <summary>
        /// Cleanup function, called by the Mixed Reality Manager when it is being teared down
        /// </summary>
        void Destroy();

        /// <summary>
        /// Retrieve all controllers currently registered with this device at runtime (if direct access is required)
        /// </summary>
        /// <returns></returns>
        IMixedRealityController[] GetActiveControllers();
    }
}