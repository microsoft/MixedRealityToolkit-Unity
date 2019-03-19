// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// Enumeration defining levels of detail for the spatial awareness mesh subsystem.
    /// </summary>
    /// <remarks>
    /// The integral values for these levels of detail generally map to triangle density, in triangles per cubic meter.
    /// </remarks>
    public enum SpatialAwarenessMeshLevelOfDetail
    {
        /// <summary>
        /// The custom level of detail allows specifying a custom value for
        /// MeshTrianglesPerCubicMeter.
        /// </summary>
        Custom = -1,

        /// <summary>
        /// The coarse level of detail is well suited for identifying large
        /// environmental features, such as floors and walls.
        /// </summary>
        Coarse = 0,

        /// <summary>
        /// The fine level of detail is well suited for using as an occlusion
        /// mesh.
        /// </summary>
        Fine = 2000
    }
}
