// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Defines the type of interpolation to use when calculating a spline.
    /// </summary>
    public enum InterpolationType
    {
        Bezier = 0,
        CatmullRom,
        Hermite,
    }
}