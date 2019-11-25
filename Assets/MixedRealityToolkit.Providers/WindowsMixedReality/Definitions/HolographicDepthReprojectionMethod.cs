// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Enumeration defining how holograms are stabilized during reprojection.
    /// </summary>
    public enum HolographicDepthReprojectionMethod
    {
        /// <summary>
        /// Use the depth buffer.
        /// </summary>
        DepthReprojection = 0,

        /// <summary>
        /// Automatically placed plane.
        /// </summary>
        AutoPlanar = 1
    }
}