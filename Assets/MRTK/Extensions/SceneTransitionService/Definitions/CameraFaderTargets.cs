// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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