// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// Enumeration defining levels of detail for the spatial awareness mesh subsystem.
    /// </summary>
    public enum SpatialAwarenessMeshLevelOfDetail
    {
        /// <summary>
        /// The custom level of detail allows specifying a custom value for
        /// TrianglesPerCubicMeter.
        /// </summary>
        Custom = -1,

        /// <summary>
        /// The coarse level of detail is well suited for identifying large
        /// environmental features, such as floors and walls.
        /// </summary>
        Coarse = 0,

        /// <summary>
        /// The medium level of detail is often useful for experiences that
        /// continually scan the environment (ex: a virtual pet).
        /// </summary>
        Medium,

        /// <summary>
        /// The fine level of detail is well suited for using as an occlusion
        /// mesh.
        /// </summary>
        Fine
    }
}
