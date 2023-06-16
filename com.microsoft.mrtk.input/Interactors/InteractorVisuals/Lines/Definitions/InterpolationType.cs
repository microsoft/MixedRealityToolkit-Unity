// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Defines the type of interpolation to use when calculating a spline.
    /// </summary>
    public enum InterpolationType
    {
        /// <summary>
        /// Interpolation using Bézier parametric curve.
        /// </summary>
        Bezier = 0,

        /// <summary>
        /// Interpolation using a Catmull–Rom spline.
        /// </summary>
        CatmullRom
    }
}
