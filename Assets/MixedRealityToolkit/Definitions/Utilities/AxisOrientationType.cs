// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// AxisOrientationType identifies orientation in relation to cartesian axes.
    /// </summary>
    public enum AxisOrientationType
    {
        /// <summary>
        /// X+, right
        /// </summary>
        PositiveX = 0,
        /// <summary>
        /// X-, left
        /// </summary>
        NegativeX,
        /// <summary>
        /// Y+, up
        /// </summary>
        PositiveY,
        /// <summary>
        /// Y-, down
        /// </summary>
        NegativeY,
        /// <summary>
        /// Z+, forward
        /// </summary>
        PositiveZ,
        /// <summary>
        /// Z-, backward
        /// </summary>
        NegativeZ
    }
}