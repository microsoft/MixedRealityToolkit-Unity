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
        int PlanarPhysicsLayer { get; set; }

        /// <summary>
        /// Gets the bit mask that corresponds to the value specified in <see cref="MeshPhysicsLayer"/>.
        /// </summary>
        int PlanarPhysicsLayerMask { get; }

        /// <summary>
        /// Gets or sets a value indicating if the spatial awareness system to generate normal for the returned meshes
        /// as some platforms may not support returning normal along with the spatial mesh. 
        /// </summary>
        bool RecalculateNormals { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how the mesh subsystem is to display surface meshes within the application.
        /// </summary>
        /// <remarks>
        /// Applications that wish to process the <see cref="Plane"/>es should set this value to None.
        /// </remarks>
        SpatialObjectDisplayOptions DisplayOption { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when displaying <see cref="Planes"/>es.
        /// </summary>
        Material VisibleMaterial { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when spatial <see cref="Planes"/>es should occlude other objects.
        /// </summary>
        Material OcclusionMaterial { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="SpatialAwarenessPlanarObject"/>s being managed by the observer.
        /// </summary>
        IReadOnlyDictionary<int, SpatialAwarenessPlanarObject> Planes { get; }
    }
}