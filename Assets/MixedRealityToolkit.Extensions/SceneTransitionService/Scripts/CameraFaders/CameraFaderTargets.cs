// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    public enum CameraFaderTargets
    {
        Main,       // Only target the main camera
        UI,         // Only target UI cameras (cameras that are used by canvases)
        All,        // Target ALL cameras in the scene
        Custom,     // Target a custom set of cameras provided by user
    }
}