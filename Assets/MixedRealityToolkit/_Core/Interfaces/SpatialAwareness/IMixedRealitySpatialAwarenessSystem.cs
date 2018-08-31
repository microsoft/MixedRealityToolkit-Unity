// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem
{
    public interface IMixedRealitySpatialAwarenessSystem : IMixedRealityEventSystem, IMixedRealityEventSource
    {
        /// <summary>
        /// Indicates if the developer intends for the spatial awareness observer to not return data until explicitly resumed.
        /// </summary>
        /// <remarks>
        /// Setting this to true allows the application to decide precisely when it wishes to begin receiving spatial data notifications. 
        /// </remarks>
        bool StartObserverSuspended { get; set; }

        /// <summary>
        /// Gets or sets the size of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        /// <remarks>This is not the total size of the observable space.</remarks>
        Vector3 ObservationExtents { get; set; }

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
        MixedRealitySpatialAwarenessMeshLevelOfDetail MeshLevelOfDetail { get; set; }

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
        bool RecalculateNormals { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the mesh subsystem is to automatically display surface meshes within the application.
        /// When enabled, the meshes will be added to the scene and rendered using the configured <see cref="MeshMaterial"/>.
        /// </summary>
        /// <remarks>
        /// Applications that wish to process the <see cref="Mesh"/>es should set this value to false.
        /// </remarks>
        bool RenderMeshes { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering <see cref="Mesh"/>es.
        /// </summary>
        Material MeshMaterial { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="IMixedRealitySpatialAwarenessMeshDescription"/>s being tracked by the spatial awareness mesh subsystem.
        /// </summary>
        /// <returns>Dictionary of <see cref="IMixedRealitySpatialAwarenessMeshDescription"/>s, indexed by the mesh id.</returns>
        Dictionary<uint, IMixedRealitySpatialAwarenessMeshDescription> GetMeshes();

        /// <summary>
        /// Gets the collection of <see cref="GameObject"/>s being managed by the spatial awareness mesh subsystem.
        /// </summary>
        /// <returns>Dictionary of <see cref="GameObject"/>s, indexed by the mesh id.</returns>
        Dictionary<uint, GameObject> GetMeshObjects();

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
        bool RenderFloorSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering floor surfaces.
        /// </summary>
        Material FloorSurfaceMaterial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// ceiling surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="CeilingSurfaceMaterial"/>.
        /// </summary>
        bool RenderCeilingSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering ceiling surfaces.
        /// </summary>
        Material CeilingSurfaceMaterial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// wall surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="WallSurfaceMaterial"/>.
        /// </summary>
        bool RenderWallSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering wall surfaces.
        /// </summary>
        Material WallSurfaceMaterial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the surface finding subsystem is to automatically display
        /// horizontal platform surfaces within the application. When enabled, the surfaces will be added to the scene
        /// and rendered using the configured <see cref="PlatformSurfaceMaterial"/>.
        /// </summary>
        bool RenderPlatformSurfaces { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when rendering horizontal platform surfaces.
        /// </summary>
        Material PlatformSurfaceMaterial { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="IMixedRealitySpatialAwarenessPlanarSurfaceDescription"/>s being tracked by the spatial awareness surface finding subsystem.
        /// </summary>
        /// <returns>Dictionary of <see cref="IMixedRealitySpatialAwarenessPlanarSurfaceDescription"/>s, indexed by the surface id.</returns>
        Dictionary<uint, IMixedRealitySpatialAwarenessPlanarSurfaceDescription> GetSurfaces();

        /// <summary>
        /// Gets the collection of <see cref="GameObject"/>s being managed by the spatial awareness surface finding subsystem.
        /// </summary>
        /// <returns>Dictionary of <see cref="GameObject"/>s, indexed by the surface id.</returns>
        Dictionary<uint, GameObject> GetSurfaceObjects();

        #endregion Surface Finding Handling
    }
}