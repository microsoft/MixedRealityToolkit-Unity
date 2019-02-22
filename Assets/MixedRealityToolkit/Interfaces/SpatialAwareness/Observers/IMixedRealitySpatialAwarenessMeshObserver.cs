// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Devices;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Observers
{
    /// <summary>
    /// The interface for defining an <see cref="IMixedRealitySpatialAwarenessObserver"/> which provides mesh data.
    /// </summary>
    public interface IMixedRealitySpatialAwarenessMeshObserver : IMixedRealitySpatialAwarenessObserver
    {
        /// <summary>
        /// Gets or sets a value indicating how the mesh subsystem is to display surface meshes within the application.
        /// </summary>
        /// <remarks>
        /// Applications that wish to process the <see cref="Mesh"/>es should set this value to None.
        /// </remarks>
        SpatialAwarenessMeshDisplayOptions DisplayOption { get; set; }

        /// <summary>
        /// Gets or sets the level of detail, as a MixedRealitySpatialAwarenessMeshLevelOfDetail value, for the returned spatial mesh.
        /// Setting this value to Custom, implies that the developer is specifying a custom value for MeshTrianglesPerCubicMeter. 
        /// </summary>
        /// <remarks>
        /// Specifying any other value will cause <see cref="MeshTrianglesPerCubicMeter"/> to be overwritten.
        /// </remarks>
        SpatialAwarenessMeshLevelOfDetail LevelOfDetail { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="SpatialAwarenessMeshObject"/>s being managed by the observer.
        /// </summary>
        IReadOnlyDictionary<int, SpatialAwarenessMeshObject> Meshes { get; }

        /// <summary>
        /// Get or sets the desired Unity Physics Layer on which to set the spatial mesh.
        /// </summary>
        /// <remarks>
        /// If not explicitly set, it is recommended that implementations return <see cref="IMixedRealitySpatialAwarenessObserver.DefaultPhysicsLayer"/>.
        /// </remarks>
        int MeshPhysicsLayer { get; set; }

        /// <summary>
        /// Gets the bit mask that corresponds to the value specified in <see cref="MeshPhysicsLayer"/>.
        /// </summary>
        int MeshPhysicsLayerMask { get; }

        /// <summary>
        /// Indicates whether or not mesh normals should be recalculated by the observer.
        /// </summary>
        bool RecalculateNormals { get; set; }

        /// <summary>
        /// Gets or sets the level of detail, in triangles per cubic meter, for the returned spatial mesh.
        /// </summary>
        /// <remarks>
        /// When specifying Coarse or Fine for the <see cref="MeshLevelOfDetail"/>, this value will be automatically overwritten with system default values.
        /// </remarks>
        int TrianglesPerCubicMeter { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when spatial <see cref="Mesh"/>es should occlude other objects.
        /// </summary>
        Material OcclusionMaterial { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when displaying <see cref="Mesh"/>es.
        /// </summary>
        Material VisibleMaterial { get; set; }
   }
}