// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    public interface IMixedRealityCameraSystem : IMixedRealityEventSystem, IMixedRealityEventSource, IMixedRealityDataProvider
    {
        /// <summary>
        /// Is the current camera displaying on an Opaque (AR) device or a VR / immersive device
        /// </summary>
        bool IsOpaque { get; }
    }
}