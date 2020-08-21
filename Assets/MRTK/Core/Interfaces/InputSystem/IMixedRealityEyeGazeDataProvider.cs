// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Provides eye tracking information.
    /// </summary>
    public interface IMixedRealityEyeGazeDataProvider : IMixedRealityInputDeviceManager
    {
        IMixedRealityEyeSaccadeProvider SaccadeProvider { get; }

        bool SmoothEyeTracking { get; set; }
    }
}