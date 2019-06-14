// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Mixed Reality Toolkit input device definition, used to instantiate and manage one or more input devices
    /// </summary>
    public interface IMixedRealityInputDeviceManager : IMixedRealityDataProvider
    {
        /// <summary>
        /// Checks to see if the device manager supports the requested capability on the current platform.
        /// </summary>
        /// <param name="capability">The capability to be checked.</param>
        /// <returns>True if the capability is supported, fale otherwise.</returns>
        bool CheckCapability(MixedRealityInputCapabilities capability);

        /// <summary>
        /// Retrieve all controllers currently registered with this device at runtime (if direct access is required)
        /// </summary>
        /// <returns></returns>
        IMixedRealityController[] GetActiveControllers();
    }
}