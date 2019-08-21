// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    /// <summary>
    /// Manager interface for a camera system in the Mixed Reality Toolkit.
    /// The camera system is expected to manage settings on the main camera.
    /// It should update the camera's clear settings, render mask, etc based on platform.
    /// </summary>
    public interface IMixedRealityCameraSystem : IMixedRealityEventSystem, IMixedRealityEventSource, IMixedRealityService
    {
        /// <summary>
        /// Typed representation of the ConfigurationProfile property.
        /// </summary>
        MixedRealityCameraProfile CameraProfile { get; }

        /// <summary>
        /// Is the current camera displaying on an Opaque (AR) device or a VR / immersive device
        /// </summary>
        bool IsOpaque { get; }
    }
}