// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// The possible shapes of bounding volumes for spatial awareness of the user's surroundings.
    /// </summary>
    public enum VolumeType
    {
        /// <summary>
        /// No specified type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Cubic volume aligned with the coordinate axes.
        /// </summary>
        AxisAlignedCube,

        /// <summary>
        /// Cubic volume aligned with the user.
        /// </summary>
        UserAlignedCube,

        /// <summary>
        /// Spherical volume.
        /// </summary>
        Sphere
    }
}