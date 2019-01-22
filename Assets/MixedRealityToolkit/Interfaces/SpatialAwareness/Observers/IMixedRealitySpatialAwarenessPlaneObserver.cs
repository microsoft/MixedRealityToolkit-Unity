// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Observers
{
    /// <summary>
    /// The interface for defining an <see cref="IMixedRealitySpatialAwarenessObserver"/> which provides mesh data.
    /// </summary>
    public interface IMixedRealitySpatialAwarenessPlaneObserver : IMixedRealitySpatialAwarenessObserver
    {
        /// <summary>
        /// Get or sets the desired Unity Physics Layer on which to set the spatial mesh.
        /// </summary>
        /// <remarks>
        /// If not explicitly set, it is recommended that implementations return <see cref="IMixedRealitySpatialAwarenessObserver.DefaultPhysicsLayer"/>.
        /// </remarks>
        int SurfacePhysicsLayer { get; set; }

        /// <summary>
        /// Gets the bit mask that corresponds to the value specified in <see cref="MeshPhysicsLayer"/>.
        /// </summary>
        int SurfacePhysicsLayerMask { get; }

        /// <summary>
        /// todo
        /// </summary>
        bool DisplayFloorSurfaces { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        Material FloorSurfaceMaterial { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        bool DisplayCeilingSurfaces { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        Material CeilingSurfaceMaterial { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        bool DisplayWallSurfaces { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        Material WallSurfaceMaterial { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        bool DisplayPlatformSurfaces { get; set; }

        /// <summary>
        /// todoW
        /// </summary>
        Material PlatformSurfaceMaterial { get; set; }

        /// <summary>
        /// todo
        /// </summary>
        GameObject SurfaceParent { get; }

        /// <summary>
        /// Gets the collection of <see cref="SpatialAwarenessPlanarObject"/>s being managed by the observer.
        /// </summary>
        IReadOnlyDictionary<int, SpatialAwarenessPlanarObject> Planes { get; }
    }
}