// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// Used by scene service to control how to transition from one lighting scene to another.
    /// </summary>
    public enum LightingSceneTransitionType
    {
        None,           // Previous lighting scene is unloaded, new lighting scene is loaded. No transition.
        FadeToBlack,    // Previous lighting scene fades out to black. New lighting scene is loaded, then faded up from black. Useful for smooth transitions between locations.
        CrossFade,      // Previous lighting scene fades out as new lighting scene fades in. Useful for smooth transitions between lighting setups in the same location. NOTE: This will not work with different skyboxes.
    }
}