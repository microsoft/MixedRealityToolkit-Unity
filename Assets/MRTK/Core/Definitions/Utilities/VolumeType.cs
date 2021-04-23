// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// todo
    /// </summary>
    public enum VolumeType
    {
        /// <summary>
        /// No Specified type.
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