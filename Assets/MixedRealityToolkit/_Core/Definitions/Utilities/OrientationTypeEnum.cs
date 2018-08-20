// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities
{
    /// <summary>
    /// Orientation type enum
    /// </summary>
    public enum OrientationTypeEnum
    {
        /// <summary>
        /// Don't rotate at all
        /// </summary>
        None,
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
        FaceFoward,
        /// <summary>
        /// Zero rotation + 180 degrees
        /// </summary>
        FaceForwardReversed,
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
        FaceCenterAxisReversed,


    }
}