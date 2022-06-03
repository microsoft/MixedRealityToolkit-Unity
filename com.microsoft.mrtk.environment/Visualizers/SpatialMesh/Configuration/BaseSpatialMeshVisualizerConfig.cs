// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Environment
{
    /// <summary>
    /// Scriptable object that contains reusable configuration values for spatial mesh visualization.
    /// </summary>
    public abstract class BaseSpatialMeshVisualizerConfig : ScriptableObject
    {

        [SerializeField]
        [Tooltip("Should the spatial observer remain in a fixed location?")]
        private bool isStationaryObserver = false;

        /// <summary>
        /// Indicates whether or not the spatial observer is to remain in a fixed location.
        /// </summary>
        public bool IsStationaryObserver => isStationaryObserver;

        [SerializeField]
        [Tooltip("The dimensions of the spatial observer volume, in meters.")]
        private Vector3 observationExtents = Vector3.one * 3;

        /// <summary>
        /// The size of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        public Vector3 ObservationExtents => observationExtents;

        [SerializeField]
        [Tooltip("How often, in seconds, should the spatial observer update?")]
        private float updateInterval = 3.5f;

        /// <summary>
        /// The frequency, in seconds, at which the spatial observer updates.
        /// </summary>
        public float UpdateInterval => updateInterval;

        #region IMixedRealitySpatialAwarenessMeshObserver settings

        [PhysicsLayer]
        [SerializeField]
        [Tooltip("Physics layer on which to set observed meshes.")]
        private int meshPhysicsLayer = BaseSpatialMeshVisualizer.DefaultSpatialAwarenessLayer;

        /// <summary>
        /// The Unity Physics Layer on which to set observed meshes.
        /// </summary>
        public int MeshPhysicsLayer => meshPhysicsLayer;

        [SerializeField]
        [Tooltip("Level of detail used when creating the mesh")]
        private SpatialMeshLevelOfDetail levelOfDetail = SpatialMeshLevelOfDetail.Unlimited;

        /// <summary>
        /// The level of detail used when creating the mesh.
        /// </summary>
        public SpatialMeshLevelOfDetail LevelOfDetail => levelOfDetail;

        [SerializeField]
        [Tooltip("Level of detail, in triangles per cubic meter.\nIgnored unless LevelOfDetail is set to Custom.")]
        private int trianglesPerCubicMeter = 0;

        /// <summary>
        /// The level of detail, in triangles per cubic meter, for the returned spatial mesh.
        /// </summary>
        /// <remarks>This value is ignored, unless <see cref="LevelOfDetail"/> is set to Custom.</remarks>
        public int TrianglesPerCubicMeter => trianglesPerCubicMeter;

        [SerializeField]
        [Tooltip("Should normals be recalculated when a mesh is added or updated?")]
        private bool recalculateNormals = true;

        /// <summary>
        /// Indicates if the spatial awareness system to generate normal for the returned meshes
        /// as some platforms may not support returning normal along with the spatial mesh.
        /// </summary>
        public bool RecalculateNormals => recalculateNormals;

        [SerializeField]
        [Tooltip("How should spatial meshes be displayed?")]
        private SpatialAwarenessMeshDisplayOptions displayOption = SpatialAwarenessMeshDisplayOptions.Visible;

        /// <summary>
        /// Indicates how the mesh subsystem is to display surface meshes within the application.
        /// </summary>
        public SpatialAwarenessMeshDisplayOptions DisplayOption => displayOption;

        [SerializeField]
        [Tooltip("Material to use when displaying observed meshes")]
        private Material visibleMaterial = null;

        /// <summary>
        /// The material to be used when displaying observed meshes.
        /// </summary>
        public Material VisibleMaterial => visibleMaterial;

        [SerializeField]
        [Tooltip("Material to use when observed meshes should occlude other objects")]
        private Material occlusionMaterial = null;

        /// <summary>
        /// The material to be used when observed meshes should occlude other objects.
        /// </summary>
        public Material OcclusionMaterial => occlusionMaterial;

        [SerializeField]
        [Tooltip("Optional physics material to apply to spatial mesh")]
        private PhysicMaterial physicsMaterial = null;

        public PhysicMaterial PhysicsMaterial => physicsMaterial;

        #endregion IMixedRealitySpatialAwarenessMeshObserver settings
    }
}

