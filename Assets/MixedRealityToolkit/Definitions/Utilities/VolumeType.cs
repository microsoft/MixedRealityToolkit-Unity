// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities
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