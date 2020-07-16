// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Orientation type enum
    /// </summary>
    public enum OrientationType
    {
        /// <summary>
        /// Don't rotate at all
        /// </summary>
        None = 0,
        /// <summary>
        /// Rotate towards the origin
        /// </summary>
        FaceOrigin,
        /// <summary>
        /// Rotate towards the origin + 180 degrees
        /// </summary>
        FaceOriginReversed,
        /// <summary>
        /// Zero rotation
        /// </summary>
        FaceParentFoward,
        /// <summary>
        /// Zero rotation + 180 degrees
        /// </summary>
        FaceParentForwardReversed,
        /// <summary>
        /// Parent Relative Up
        /// </summary>
        FaceParentUp,
        /// <summary>
        /// Parent Relative Down
        /// </summary>
        FaceParentDown,
        /// <summary>
        /// Lay flat on the surface, facing in
        /// </summary>
        FaceCenterAxis,
        /// <summary>
        /// Lay flat on the surface, facing out
        /// </summary>
        FaceCenterAxisReversed
    }
}