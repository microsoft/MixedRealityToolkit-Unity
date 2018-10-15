// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities
{
    /// <summary>
    /// Used to constrain to a specific axis of movement or rotation.
    /// </summary>
    [Flags]
    public enum AxisConstraintType
    {
        /// <summary>
        /// Transform all Axes.
        /// </summary>
        None = 0 << 0,
        /// <summary>
        /// Constrain to X-Axis.
        /// </summary>
        XAxis = 1 << 0,
        /// <summary>
        /// Constrain to Y-Axis.
        /// </summary>
        YAxis = 1 << 1,
        /// <summary>
        /// Constrain to Z-Axis.
        /// </summary>
        ZAxis = 1 << 2,
        /// <summary>
        /// Constrained to the X and Y Axes.
        /// </summary>
        XYAxis = XAxis | YAxis,
        /// <summary>
        /// Constrained to the X and Z Axes.
        /// </summary>
        XZAxis = XAxis | ZAxis,
        /// <summary>
        /// Constrained to the Y and Z Axes.
        /// </summary>
        YZAxis = YAxis | ZAxis,
    }
}