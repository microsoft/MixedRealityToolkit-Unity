// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem
{
    public interface IMixedRealitySpatialAwarenessSystem : IMixedRealityEventSystem, IMixedRealityEventSource
    {
        /// <summary>
        /// Indicates the developer's intended startup behavior.
        /// </summary>
        AutoStartBehavior StartupBehavior { get; set; }

        // todo: ObserverVolumeType

        // todo?: bool StationaryObserver

        /// <summary>
        /// Gets or sets the size of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        Vector3 ObservationExtents { get; set; }

        /// <summary>
        /// Should the observer remain stationary in the scene?
        /// </summary>
        /// <remarks>
        /// Set IsStationaryObserver set to false, to move the volume with the user. 
        /// If set to true, the origin will be 0,0,0 or the last known location.
        /// </remarks>
        bool IsStationaryObserver { get; set; }

        /// <summary>
        /// Gets or sets the origin of the observer.
        /// </summary>
        /// <remarks>
        /// Moving the observer origin allows the spatial awareness system to locate and discard meshes as the user
        /// navigates the environment.
        /// </remarks>
        Vector3 ObserverOrigin { get; set; }

        // todo Quaternion ObserverOrientation { get; set }

        /// <summary>
        /// Gets or sets the frequency, in seconds, at which the spatial observer updates.
        /// </summary>
        float UpdateInterval { get; set; }

        /// <summary>
        /// Indicates the current running state of the spatial awareness observer.
        /// </summary>
        bool IsObserverRunning { get; }

        /// <summary>
        /// Starts / restarts the spatial observer.
        /// </summary>
        /// <remarks>This will cause spatial awareness events to resume.</remarks>
        void ResumeObserver();

        /// <summary>
        /// Stops / pauses the spatial observer.
        /// </summary>
        /// <remarks>This will cause spatial awareness events to be suspended until ResumeObserver is called.</remarks>
        void SuspendObserver();

        #region Mesh Handling

        /// <summary>
        /// Indicates if the spatial mesh subsystem is in use by the application.
        /// </summary>
        /// <remarks>
        /// Setting this to false will suspend all mesh events and cause the subsystem to return an empty collection
        /// when the GetMeshes method is called.
        /// </remarks>
        bool UseMeshSystem { get; set; }

        /// <summary>
        /// Get or sets the desired Unity Physics Layer on which to set the spatial mesh.
        /// </summary>
        int MeshPhysicsLayer { get; set; }

        /// <summary>
        /// Gets the bit mask that corresponds to the value specified in <see cref="MeshPhysicsLayer"/>.
        /// </summary>
        int MeshPhysicsLayerMask { get; }

        /// <summary>
        /// Gets or sets the level of detail, as a MixedRealitySpatialAwarenessMeshLevelOfDetail value, for the returned spatial mesh.
        /// Setting this value to Custom, implies that the developer is specifying a custom value for MeshTrianglesPerCubicMeter. 
        /// </summary>
        /// <remarks>Specifying any other value will cause <see cref="MeshTrianglesPerCubicMeter"/> to be overwritten.</remarks>
        SpatialAwarenessMeshLevelOfDetail MeshLevelOfDetail { get; set; }

        /// <summary>
        /// Gets or sets the level of detail, in triangles per cubic meter, for the returned spatial mesh.
        /// </summary>
        /// <remarks>
        /// When specifying Coarse or Fine for the <see cref="MeshLevelOfDetail"/>, this value will be automatically overwritten with system default values.
        /// </remarks>
        int MeshTrianglesPerCubicMeter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the spatial awareness system to generate normal for the returned meshes
        /// as some platforms may not support returning normal along with the spatial mesh. 
        /// </summary>
        bool MeshRecalculateNormals { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how the mesh subsystem is to display surface meshes within the application.
        /// </summary>
        /// <remarks>
        /// Applications that wish to process the <see cref="Mesh"/>es should set this value to None.
        /// </remarks>
        SpatialMeshDisplayOptions MeshDisplayOption { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when displaying <see cref="Mesh"/>es.
        /// </summary>
        Material MeshVisibleMaterial { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when spatial <see cref="Mesh"/>es should occlude other objects.
        /// </summary>
        Material MeshOcclusionMaterial { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="GameObject"/>s being managed by the spatial awareness mesh subsystem.
        /// </summary>
        IDictionary<int, GameObject> Meshes { get; }

        #region Mesh Events

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler.OnMeshAdded"/> method to indicate a mesh has been added.
        /// </summary>
        /// <param name="meshId">Value identifying the mesh.</param>
        /// <param name="meshObject">The mesh <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshAdded(int meshId, GameObject meshObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler.OnMeshUpdated"/> method to indicate an existing mesh has been updated.
        /// </summary>
        /// <param name="meshId">Value identifying the mesh.</param>
        /// <param name="meshObject">The mesh <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshUpdated(int meshId, GameObject meshObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler.OnMeshUpdated"/> method to indicate an existing mesh has been removed.
        /// </summary>
        /// <param name="meshId">Value identifying the mesh.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshRemoved(int meshId);

        #endregion Mesh Events

        #endregion Mesh Handling

        #region Surface Finding Handling

        /// <summary>
        /// Indicates if the surface finding subsystem is in use by the application.
        /// </summary>
        /// <remarks>
        /// Setting this to false will suspend all surface finding events and cause the subsystem to return an empty collection
        /// when the GetSurfaces method is called.
        /// </remarks>
        bool UseSurfaceFindingSystem { get; set; }

        /// <summary>
        /// Get or sets the desired Unity Physics Layer on which to set the planar surfaces.
        /// </summary>
        int SurfacePhysicsLayer { get; set; }

        /// <summary>
        /// Gets the bit mask that corresponds to the value specified in <see cref="SurfacePhysicsLayer"/>.
        /// </summary>
        int SurfacePhysicsLayerMask { get; }

        /// <summary>
        /// Gets or sets the minimum surface area, in square meters, that must be satisfied before a surface is identified.
        /// </summary>
        float SurfaceFindingMinimumArea { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// floor surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="FloorSurfaceMaterial"/>.
        /// </summary>
        bool DisplayFloorSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering floor surfaces.
        /// </summary>
        Material FloorSurfaceMaterial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// ceiling surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="CeilingSurfaceMaterial"/>.
        /// </summary>
        bool DisplayCeilingSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering ceiling surfaces.
        /// </summary>
        Material CeilingSurfaceMaterial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// wall surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="WallSurfaceMaterial"/>.
        /// </summary>
        bool DisplayWallSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering wall surfaces.
        /// </summary>
        Material WallSurfaceMaterial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// horizontal platform surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="PlatformSurfaceMaterial"/>.
        /// </summary>
        bool DisplayPlatformSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering horizontal platform surfaces.
        /// </summary>
        Material PlatformSurfaceMaterial { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="GameObject"/>s being managed by the spatial awareness surface finding subsystem.
        /// </summary>
        IDictionary<int, GameObject> PlanarSurfaces { get; }

        #region Surface Finding Events

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler.OnSurfaceAdded"/> method to indicate a planar surface has been added.
        /// </summary>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <param name="surfaceObject">The surface <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceAdded(int surfaceId, GameObject surfaceObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler.OnSurfaceUpdated"/> method to indicate an existing planar surface has been updated.
        /// </summary>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <param name="surfaceObject">The surface <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceUpdated(int surfaceId, GameObject surfaceObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler.OnSurfaceUpdated"/> method to indicate an existing planar surface has been removed.
        /// </summary>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceRemoved(int surfaceId);

        #endregion Surface Finding Events

        #endregion Surface Finding Handling
    }
}
