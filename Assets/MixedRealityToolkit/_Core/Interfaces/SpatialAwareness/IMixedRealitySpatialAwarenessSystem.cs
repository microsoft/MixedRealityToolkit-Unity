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
        Vector3 ObservationExtents { get; set; }

        /// <summary>
        /// The physics layer to which identified meshes and surfaces should be attached.
        /// </summary>
        int PhysicsLayer { get; set; }

        /// <summary>
        /// The interval, in seconds, between observation updates.
        /// </summary>
        float UpdateInterval { get; set; }

        /// <summary>
        /// Indicates whether or not the spatial observer is currently running.
        /// </summary>
        bool IsObserverRunning { get; }
        
        /// <summary>
        /// Starts the spatial observer.
        /// </summary>
        void StartObserver();

        /// <summary>
        /// Stops the spatial observer.
        /// </summary>
        void StopObserver();

        #region Mesh

        /// <summary>
        /// The number of triangles to calculate per cubic meter. 
        /// </summary>
        int TrianglesPerCubicMeter { get; set; }

        /// <summary>
        /// Indicates whether or not normals should be recalculated when observations are updated.
        /// </summary>
        bool RecalculateNormals { get; set; }

        /// <summary>
        /// Indicates whether or not the platform supports returning spatial mesh data.
        /// </summary>
        bool IsMeshDataSupported { get; }

        #endregion Mesh

        #region Surface

        /// <summary>
        /// The minimum area, in square meters, threshold before a surface plane will be identified.
        /// </summary>
        float MinimumSurfaceArea { get; set; }

        #endregion Surface
    }
}
