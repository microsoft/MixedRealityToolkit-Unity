// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers
{
    /// <summary>
    /// The interface contract for Mixed Reality spatial observers.
    /// </summary>
    public interface IMixedRealitySpatialMeshObserver : IMixedRealitySpatialObserverDataProvider
    {
        /// <summary>
        /// The optional Physics layer override of the base <see cref="IMixedRealitySpatialObserverDataProvider.PhysicsLayer"/>
        /// </summary>
        int MeshPhysicsLayerOverride { get; }

        /// <summary>
        /// Gets or sets the level of detail, as a MixedRealitySpatialAwarenessMeshLevelOfDetail value, for the returned spatial mesh.
        /// Setting this value to Custom, implies that the developer is specifying a custom value for MeshTrianglesPerCubicMeter. 
        /// </summary>
        /// <remarks>Specifying any other value will cause <see cref="MeshTrianglesPerCubicMeter"/> to be overwritten.</remarks>
        SpatialAwarenessMeshLevelOfDetail MeshLevelOfDetail { get; }

        /// <summary>
        /// Gets or sets the level of detail, in triangles per cubic meter, for the returned spatial mesh.
        /// </summary>
        /// <remarks>
        /// When specifying Coarse or Fine for the <see cref="MeshLevelOfDetail"/>, this value will be automatically overwritten with system default values.
        /// </remarks>
        uint MeshTrianglesPerCubicMeter { get; }

        /// <summary>
        /// Gets or sets a value indicating if the spatial awareness system to generate normal for the returned meshes
        /// as some platforms may not support returning normal along with the spatial mesh. 
        /// </summary>
        bool MeshRecalculateNormals { get; }

        /// <summary>
        /// Gets or sets a value indicating how the mesh subsystem is to display surface meshes within the application.
        /// </summary>
        /// <remarks>
        /// Applications that wish to process the <see cref="Mesh"/>es should set this value to None.
        /// </remarks>
        SpatialMeshDisplayOptions MeshDisplayOption { get; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when displaying <see cref="SpatialMeshObject"/>s.
        /// </summary>
        Material MeshVisibleMaterial { get; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when <see cref="SpatialMeshObject"/>s should occlude other objects.
        /// </summary>
        Material MeshOcclusionMaterial { get; }

        /// <summary>
        /// Gets or sets the size of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        Vector3 ObservationExtents { get; }

        /// <summary>
        /// Should the observer remain stationary in the scene?
        /// </summary>
        /// <remarks>
        /// Set IsStationaryObserver set to false, to move the volume with the user.
        /// If set to true, the origin will be 0,0,0 or the last known location.
        /// </remarks>
        bool IsStationaryObserver { get; }

        /// <summary>
        /// Gets or sets the origin of the observer.
        /// </summary>
        /// <remarks>
        /// Moving the observer origin allows the spatial awareness system to locate and discard meshes as the user
        /// navigates the environment.
        /// </remarks>
        Vector3 ObserverOrigin { get; }

        /// <summary>
        /// Gets for sets the rotation of the observer
        /// </summary>
        Quaternion ObserverOrientation { get; }

        /// <summary>
        /// The collection of mesh <see cref="SpatialMeshObject"/>s that have been observed.
        /// </summary>
        IReadOnlyDictionary<int, SpatialMeshObject> SpatialMeshObjects { get; }

        /// <summary>
        /// Forwards mesh added event to the <see cref="SpatialAwarenessSystem.IMixedRealitySpatialAwarenessSystem"/>.
        /// </summary>
        /// <param name="spatialMeshObject">The <see cref="SpatialMeshObject"/> data.</param>
        void RaiseMeshAdded(SpatialMeshObject spatialMeshObject);

        /// <summary>
        /// Forwards mesh updated event to the <see cref="SpatialAwarenessSystem.IMixedRealitySpatialAwarenessSystem"/>.
        /// </summary>
        /// <param name="spatialMeshObject">The <see cref="SpatialMeshObject"/> data.</param>
        void RaiseMeshUpdated(SpatialMeshObject spatialMeshObject);

        /// <summary>
        /// Forwards mesh removed event to the <see cref="SpatialAwarenessSystem.IMixedRealitySpatialAwarenessSystem"/>.
        /// </summary>
        /// <param name="spatialMeshObject">The <see cref="SpatialMeshObject"/> data.</param>
        void RaiseMeshRemoved(SpatialMeshObject spatialMeshObject);
    }
}