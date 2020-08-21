// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Rotational Pivot axis for orientating an object
    /// </summary>
    public enum PivotAxis
    {
        // Most common options, preserving current functionality with the same enum order.
        XY,
        Y,
        // Rotate about an individual axis.
        X,
        Z,
        // Rotate about a pair of axes.
        XZ,
        YZ,
        // Rotate about all axes.
        Free
    }
}