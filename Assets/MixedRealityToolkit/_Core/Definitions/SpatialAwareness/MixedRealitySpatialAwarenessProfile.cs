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
    public class MixedRealitySpatialAwarenessProfile : ScriptableObject
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
        private Vector3 observationExtents = new Vector3(10f, 10f, 10f);

        /// <summary>
        /// The size of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        public Vector3 ObservationExtents => observationExtents;

        [SerializeField]
        [Tooltip("How often, in seconds, should the spatial observer update?")]
        private float updateInterval = 3.5f;

        /// <summary>
        /// Rhe frequency, in seconds, at which the spatial observer updates.
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
        private MixedRealitySpatialAwarenessMeshLevelOfDetail meshLevelOfDetail = MixedRealitySpatialAwarenessMeshLevelOfDetail.Coarse;

        /// <summary>
        /// The desired Unity Physics Layer on which to set the spatial mesh.
        /// </summary>
        public MixedRealitySpatialAwarenessMeshLevelOfDetail MeshLevelOfDetail => meshLevelOfDetail;

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
        [Tooltip("Automatically display spatial meshes?")]
        private bool displayMeshes = false;

        /// <summary>
        /// Indicates if the mesh subsystem is to automatically display surface meshes within the application.
        /// </summary>
        public bool DisplayMeshes => displayMeshes;

        [SerializeField]
        [Tooltip("Material to use when displaying meshes")]
        private Material meshMaterial = null;

        /// <summary>
        /// The material to be used when automatically displaying spatial meshes.
        /// </summary>
        public Material MeshMaterial => meshMaterial;

        #endregion Mesh settings

        #region Surface Finding settings

        // todo

        #endregion Surface Finding settings
    }
}
