// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    /// <summary>
    /// todo
    /// </summary>
    public interface IMixedRealityCameraSettingsProvider : IMixedRealityDataProvider
    {
        /// <summary>
        /// todo
        /// </summary>
        DisplayType DisplayType { get; }

        /// <summary>
        /// Returns whether or not the current display rendering mode is opaque.
        /// </summary>
        bool IsOpaque { get; }

        /// <summary>
        /// Applies the display settings (background color, clipping plane distances, etc) based on the
        /// appropriate <see cref="DisplayType"/>.
        /// </summary>
        void ApplyDisplaySettings();
    }
}
