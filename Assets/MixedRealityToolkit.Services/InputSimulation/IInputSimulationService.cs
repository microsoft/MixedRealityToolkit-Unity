// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Input
{ 
    public interface IInputSimulationService : IMixedRealityInputDeviceManager
    {
        MixedRealityInputSimulationProfile InputSimulationProfile { get; }
    }
}