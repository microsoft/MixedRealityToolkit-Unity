// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem
{
    /// <summary>
    /// The interface definition for Spatial Awareness features in the Mixed Reality Toolkit.
    /// </summary>
    public interface IMixedRealitySpatialAwarenessSystem : IMixedRealityEventSystem
    {
        /// <summary>
        /// Indicates the current running state of the spatial awareness observer.
        /// </summary>
        bool IsObserverRunning(IMixedRealitySpatialAwarenessObserver observer);

        /// <summary>
        /// Generates a new unique observer id.<para/>
        /// <remarks>All <see cref="IMixedRealitySpatialAwarenessObserver"/>s are required to call this method in their initialization.</remarks>
        /// </summary>
        /// <returns>a new unique Id for the observer.</returns>
        uint GenerateNewObserverId();

        /// <summary>
        /// Starts / restarts the spatial observer.
        /// </summary>
        /// <remarks>This will cause spatial awareness events to resume.</remarks>
        void ResumeObserver(IMixedRealitySpatialAwarenessObserver observer);

        /// <summary>
        /// Stops / pauses the spatial observer.
        /// </summary>
        /// <remarks>This will cause spatial awareness events to be suspended until ResumeObserver is called.</remarks>
        void SuspendObserver(IMixedRealitySpatialAwarenessObserver observer);

        /// <summary>
        /// List of the spatial observers as detected by the spatial awareness system.
        /// </summary>
        HashSet<IMixedRealitySpatialAwarenessObserver> DetectedSpatialObservers { get; }

        /// <summary>
        /// Raise the event that a <see cref="IMixedRealitySpatialAwarenessObserver"/> has been detected.
        /// </summary>
        void RaiseSpatialAwarenessObserverDetected(IMixedRealitySpatialAwarenessObserver observer);

        /// <summary>
        /// Raise the event that a <see cref="IMixedRealitySpatialAwarenessObserver"/> has been lost.
        /// </summary>
        void RaiseSpatialAwarenessObserverLost(IMixedRealitySpatialAwarenessObserver observer);

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
        /// Gets or sets the <see cref="Material"/> to be used when displaying <see cref="SpatialMeshObject"/>s.
        /// </summary>
        Material MeshVisibleMaterial { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when <see cref="SpatialMeshObject"/>s should occlude other objects.
        /// </summary>
        Material MeshOcclusionMaterial { get; set; }

        #region Mesh Events

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler{T}.OnMeshAdded"/> method to indicate a mesh has been added.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="meshObject">The mesh <see cref="SpatialMeshObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialAwarenessObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshAdded(IMixedRealitySpatialAwarenessObserver observer, SpatialMeshObject meshObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler{T}.OnMeshUpdated"/> method to indicate an existing mesh has been updated.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="meshObject">The mesh <see cref="SpatialMeshObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialAwarenessObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshUpdated(IMixedRealitySpatialAwarenessObserver observer, SpatialMeshObject meshObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler{T}.OnMeshUpdated"/> method to indicate an existing mesh has been removed.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="meshObject">The mesh <see cref="SpatialMeshObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialAwarenessObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshRemoved(IMixedRealitySpatialAwarenessObserver observer, SpatialMeshObject meshObject);

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
        /// Gets or Sets the layer mask for the surface finding physics layer.
        /// </summary>
        int SurfaceFindingPhysicsLayer { get; set; }

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
        IReadOnlyDictionary<int, GameObject> PlanarSurfaces { get; }

        #region Surface Finding Events

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler{T}.OnSurfaceAdded"/> method to indicate a planar surface has been added.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <param name="surfaceObject">The surface <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialAwarenessObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceAdded(IMixedRealitySpatialAwarenessObserver observer, int surfaceId, GameObject surfaceObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler{T}.OnSurfaceUpdated"/> method to indicate an existing planar surface has been updated.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <param name="surfaceObject">The surface <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialAwarenessObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceUpdated(IMixedRealitySpatialAwarenessObserver observer, int surfaceId, GameObject surfaceObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler{T}.OnSurfaceUpdated"/> method to indicate an existing planar surface has been removed.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialAwarenessObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseSurfaceRemoved(IMixedRealitySpatialAwarenessObserver observer, int surfaceId);

        #endregion Surface Finding Events

        #endregion Surface Finding Handling
    }
}
