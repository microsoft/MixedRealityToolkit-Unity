// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// Configuration profile settings for setting up the spatial awareness system.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Spatial Awareness Profile", fileName = "MixedRealitySpatialAwarenessProfile", order = (int)CreateProfileMenuItemIndices.SpatialAwareness)]
    public class MixedRealitySpatialAwarenessProfile : BaseMixedRealityProfile
    {
        #region General settings

        [SerializeField]
        [Tooltip("How should the spatial awareness observer behave at startup?")]
        private AutoStartBehavior startupBehavior = AutoStartBehavior.AutoStart;

        /// <summary>
        /// Indicates if the developer intends for the spatial awareness observer start immediately or wait for manual startup.
        /// </summary>
        public AutoStartBehavior StartupBehavior => startupBehavior;

        [SerializeField]
        [Tooltip("The dimensions of the spatial observer volume, in meters.")]
        private Vector3 observationExtents = Vector3.one * 3;

        /// <summary>
        /// The size of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        public Vector3 ObservationExtents => observationExtents;

        [SerializeField]
        [Tooltip("Should the spatial observer remain in a fixed location?")]
        private bool isStationaryObserver = false;

        /// <summary>
        /// The size of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        public bool IsStationaryObserver => isStationaryObserver;

        [SerializeField]
        [Tooltip("How often, in seconds, should the spatial observer update?")]
        private float updateInterval = 3.5f;

        /// <summary>
        /// The frequency, in seconds, at which the spatial observer updates.
        /// </summary>
        public float UpdateInterval => updateInterval;

        #endregion General settings

        #region Mesh settings

        [SerializeField]
        [Tooltip("Is the mesh subsystem in use?")]
        private bool useMeshSystem = true;

        /// <summary>
        /// Indicates if the spatial mesh subsystem is in use by the application.
        /// </summary>
        public bool UseMeshSystem => useMeshSystem;

        [SerializeField]
        [Tooltip("Physics layer on which to set the mesh")]
        private int meshPhysicsLayer = 31;

        /// <summary>
        /// The desired Unity Physics Layer on which to set the spatial mesh.
        /// </summary>
        public int MeshPhysicsLayer => meshPhysicsLayer;

        [SerializeField]
        [Tooltip("Level of detail for the mesh")]
        private SpatialAwarenessMeshLevelOfDetail meshLevelOfDetail = SpatialAwarenessMeshLevelOfDetail.Coarse;

        /// <summary>
        /// The desired Unity Physics Layer on which to set the spatial mesh.
        /// </summary>
        public SpatialAwarenessMeshLevelOfDetail MeshLevelOfDetail => meshLevelOfDetail;

        [SerializeField]
        [Tooltip("Level of detail, in triangles per cubic meter.\nIgnored unless MeshLevelOfDetail is set to Custom.")]
        private int meshTrianglesPerCubicMeter = 0;

        /// <summary>
        /// The level of detail, in triangles per cubic meter, for the returned spatial mesh.
        /// </summary>
        /// <remarks>This value is ignored, unless <see cref="MeshLevelOfDetail"/> is set to Coarse.</remarks>
        public int MeshTrianglesPerCubicMeter => meshTrianglesPerCubicMeter;

        [SerializeField]
        [Tooltip("Should normals be recalculated when a mesh is added or updated?")]
        private bool meshRecalculateNormals = true;

        /// <summary>
        /// Indicates if the spatial awareness system to generate normal for the returned meshes
        /// as some platforms may not support returning normal along with the spatial mesh.
        /// </summary>
        public bool MeshRecalculateNormals => meshRecalculateNormals;

        [SerializeField]
        [Tooltip("How should spatial meshes be displayed?")]
        private SpatialMeshDisplayOptions meshDisplayOption = SpatialMeshDisplayOptions.None;

        /// <summary>
        /// Indicates how the mesh subsystem is to display surface meshes within the application.
        /// </summary>
        public SpatialMeshDisplayOptions MeshDisplayOption => meshDisplayOption;

        [SerializeField]
        [Tooltip("Material to use when displaying meshes")]
        private Material meshVisibleMaterial = null;

        /// <summary>
        /// The material to be used when automatically displaying spatial meshes.
        /// </summary>
        public Material MeshVisibleMaterial => meshVisibleMaterial;

        [SerializeField]
        [Tooltip("Material to use when spatial meshes should occlude other objects")]
        private Material meshOcclusionMaterial = null;

        /// <summary>
        /// The material to be used when spatial meshes should occlude other objects.
        /// </summary>
        public Material MeshOcclusionMaterial => meshOcclusionMaterial;

        #endregion Mesh settings

        #region Surface Finding settings

        [SerializeField]
        [Tooltip("Is the surface finding subsystem in use?")]
        private bool useSurfaceFindingSystem = true;

        /// <summary>
        /// Indicates if the surface finding subsystem is in use by the application.
        /// </summary>
        public bool UseSurfaceFindingSystem => useSurfaceFindingSystem;

        [SerializeField]
        [Tooltip("Physics layer on which to set the planar surfaces")]
        private int surfaceFindingPhysicsLayer = 31;

        /// <summary>
        /// The desired Unity Physics Layer on which to set the planar surfaces.
        /// </summary>
        public int SurfaceFindingPhysicsLayer => surfaceFindingPhysicsLayer;

        [SerializeField]
        [Tooltip("The minimum area, in square meters, of the planar surfaces")]
        private float surfaceFindingMinimumArea = 0.025f;

        /// <summary>
        /// The minimum area, in square meters, of the planar surfaces.
        /// </summary>
        public float SurfaceFindingMinimumArea => surfaceFindingMinimumArea;

        [SerializeField]
        [Tooltip("Automatically display floor surfaces?")]
        private bool displayFloorSurfaces = false;

        /// <summary>
        /// Indicates if the surface finding subsystem is to automatically display floor surfaces within the application.
        /// </summary>
        public bool DisplayFloorSurfaces => displayFloorSurfaces;

        [SerializeField]
        [Tooltip("Material to use when displaying floor surfaces")]
        private Material floorSurfaceMaterial = null;

        /// <summary>
        /// The material to be used when automatically displaying floor surfaces.
        /// </summary>
        public Material FloorSurfaceMaterial => floorSurfaceMaterial;

        [SerializeField]
        [Tooltip("Automatically display ceiling surfaces?")]
        private bool displayCeilingSurfaces = false;

        /// <summary>
        /// Indicates if the surface finding subsystem is to automatically display ceiling surfaces within the application.
        /// </summary>
        public bool DisplayCeilingSurface => displayCeilingSurfaces;

        [SerializeField]
        [Tooltip("Material to use when displaying ceiling surfaces")]
        private Material ceilingSurfaceMaterial = null;

        /// <summary>
        /// The material to be used when automatically displaying ceiling surfaces.
        /// </summary>
        public Material CeilingSurfaceMaterial => ceilingSurfaceMaterial;

        [SerializeField]
        [Tooltip("Automatically display wall surfaces?")]
        private bool displayWallSurfaces = false;

        /// <summary>
        /// Indicates if the surface finding subsystem is to automatically display wall surfaces within the application.
        /// </summary>
        public bool DisplayWallSurface => displayWallSurfaces;

        [SerializeField]
        [Tooltip("Material to use when displaying wall surfaces")]
        private Material wallSurfaceMaterial = null;

        /// <summary>
        /// The material to be used when automatically displaying wall surfaces.
        /// </summary>
        public Material WallSurfaceMaterial => wallSurfaceMaterial;

        [SerializeField]
        [Tooltip("Automatically display platform surfaces?")]
        private bool displayPlatformSurfaces = false;

        /// <summary>
        /// Indicates if the surface finding subsystem is to automatically display platform surfaces within the application.
        /// </summary>
        public bool DisplayPlatformSurfaces => displayPlatformSurfaces;

        [SerializeField]
        [Tooltip("Material to use when displaying platform surfaces")]
        private Material platformSurfaceMaterial = null;

        /// <summary>
        /// The material to be used when automatically displaying platform surfaces.
        /// </summary>
        public Material PlatformSurfaceMaterial => platformSurfaceMaterial;

        #endregion Surface Finding settings
    }
}
