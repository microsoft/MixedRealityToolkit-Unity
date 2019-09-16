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
        /// Retrieve all controllers currently registered with this device at runtime (if direct access is required)
        /// </summary>
        IMixedRealityController[] GetActiveControllers();
    }
}