// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Mixed reality platform capabilities.
    /// </summary>
    public enum MixedRealityCapability
    {
        /// <summary>
        /// Articulated hand input
        /// </summary>
        ArticulatedHand = 0,

        /// <summary>
        /// Gaze-Gesture-Voice hand input
        /// </summary>
        GGVHand,

        /// <summary>
        /// Motion controller input
        /// </summary>
        MotionController,

        /// <summary>
        /// Eye gaze targeting
        /// </summary>
        EyeTracking,

        /// <summary>
        /// Voice commands using app defined keywords
        /// </summary>
        VoiceCommand,

        /// <summary>
        /// Voice to text dication
        /// </summary>
        VoiceDictation,

        /// <summary>
        /// Spatial meshes
        /// </summary>
        SpatialAwarenessMesh,

        /// <summary>
        /// Spatial planes
        /// </summary>
        SpatialAwarenessPlane,

        /// <summary>
        /// Spatial points
        /// </summary>
        SpatialAwarenessPoint
    }
}
