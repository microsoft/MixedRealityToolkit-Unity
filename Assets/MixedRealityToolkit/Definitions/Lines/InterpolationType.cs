// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Lines
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