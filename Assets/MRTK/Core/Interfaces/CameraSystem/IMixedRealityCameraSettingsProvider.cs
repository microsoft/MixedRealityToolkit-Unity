// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    /// <summary>
    /// Interface defining the a camera system settings provider.
    /// </summary>
    public interface IMixedRealityCameraSettingsProvider : IMixedRealityDataProvider
    {
        /// <summary>
        /// Returns whether or not the current display rendering mode is opaque.
        /// </summary>
        bool IsOpaque { get; }

        /// <summary>
        /// Applies provider specific configuration settings.
        /// </summary>
        void ApplyConfiguration();
    }
}
