// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.SpatialAwareness;
//using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
//using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Events;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.SpatialAwarenessSystem
{
    public interface IMixedRealitySpatialAwarenessSystem // : IMixedRealityEventSystem, IMixedRealityEventSource
    {
        /// <summary>
        /// Indicates whether or not the spatial awareness system is to be automatically started.
        /// </summary>
        bool AutoStart { get; set; }

        /// <summary>
        /// The size of the observation volume.
        /// </summary>
        Vector3 Extents { get; set; }

        /// <summary>
        /// The physics layer to which identified meshes and surfaces should be attached.
        /// </summary>
        int PhysicsLayer { get; set; }

        /// <summary>
        /// The interval, in seconds, between observation updates.
        /// </summary>
        int UpdateInterval { get; set; }

        #region Mesh settings

        /// <summary>
        /// The number of triangles to calculate per cubic meter. 
        /// </summary>
        int TrianglesPerCubicMeter { get; set; }

        /// <summary>
        /// Indicates whether or not normals should be recalculated when observations are updated.
        /// </summary>
        bool RecalculateNormals { get; set; }

        #endregion Mesh settings

        #region Surface settings

        /// <summary>
        /// The minimum size, in square meters, threshold before a surface plane will be identified.
        /// </summary>
        float MinimumSurfaceSize { get; set; }

        /// <summary>
        /// The types of surfaces to identify.
        /// </summary>
        SpatialAwarenessSurfaceTypes SurfaceTypes { get; set; }

        #endregion Surface settings
    }
}
