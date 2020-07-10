// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Default options for how to distribute interpolated points in a line renderer
    /// </summary>
    public enum InterpolationMode
    {
        /// <summary>
        /// Specify the number of interpolation steps manually
        /// </summary>
        FromSteps = 0,
        /// <summary>
        /// Create steps based on total length of line + manually specified length
        /// </summary>
        FromLength,
        /// <summary>
        /// Create steps based on total length of line + animation curve
        /// </summary>
        FromCurve
    }
}