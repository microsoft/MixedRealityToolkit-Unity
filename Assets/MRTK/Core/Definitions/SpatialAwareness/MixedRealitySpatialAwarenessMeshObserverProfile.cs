// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// Configuration profile settings for spatial awareness mesh observers.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Spatial Awareness Mesh Observer Profile", fileName = "MixedRealitySpatialAwarenessMeshObserverProfile", order = (int)CreateProfileMenuItemIndices.SpatialAwarenessMeshObserver)]
    [MixedRealityServiceProfile(typeof(IMixedRealitySpatialAwarenessMeshObserver))]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/spatial-awareness/configuring-spatial-awareness-mesh-observer")]
    public class MixedRealitySpatialAwarenessMeshObserverProfile : BaseSpatialAwarenessObserverProfile
    {
        #region IMixedRealitySpatialAwarenessMeshObserver settings

        [PhysicsLayer]
        [SerializeField]
        [Tooltip("Physics layer on which to set observed meshes.")]
        private int meshPhysicsLayer = BaseSpatialObserver.DefaultSpatialAwarenessLayer;

        /// <summary>
        /// The Unity Physics Layer on which to set observed meshes.
        /// </summary>
        public int MeshPhysicsLayer => meshPhysicsLayer;

        [SerializeField]
        [Tooltip("Level of detail used when creating the mesh")]
        private SpatialAwarenessMeshLevelOfDetail levelOfDetail = SpatialAwarenessMeshLevelOfDetail.Coarse;

        /// <summary>
        /// The level of detail used when creating the mesh.
        /// </summary>
        public SpatialAwarenessMeshLevelOfDetail LevelOfDetail => levelOfDetail;

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

        [SerializeField]
        [Tooltip("Optional prefab that is added to the runtime spatial mesh hierarchy.  This prefab will only" +
            " be instantiated and appended to the hierarchy in a build, the behavior associated with this property is not" + 
            " enabled in editor.\n" +
            "\n" +
            "Default Runtime Spatial Awareness Hierarchy: \n" +
            "\n" +
            "Spatial Awareness System \n" +
            "   Windows Mixed Reality Spatial Mesh Observer\n" +
            "       Spatial Mesh - ID\n" +
            "       Spatial Mesh - ID\n" +
            "       ...\n" +
            "\n" +
            "Runtime Spatial Awareness Hierarchy with this prefab:\n" +
            "\n" +
            "Spatial Awareness System \n" +
            "   Runtime Spatial Mesh Prefab\n" +
            "       Windows Mixed Reality Spatial Mesh Observer\n" +
            "           Spatial Mesh - ID\n" +
            "           Spatial Mesh - ID\n" +
            "            ...\n")]
        private GameObject runtimeSpatialMeshPrefab = null;

        /// <summary>
        /// Optional prefab that is added to the runtime spatial mesh hierarchy.  This prefab will only
        /// be instantiated and appended to the hierarchy in a build, the behavior associated with this property is not 
        /// enabled in editor.
        /// 
        /// Runtime Spatial Awareness Hierarchy without this prefab:
        /// 
        /// Spatial Awareness System 
        ///     Windows Mixed Reality Spatial Mesh Observer
        ///         Spatial Mesh - ID
        ///         Spatial Mesh - ID
        ///         ...
        ///         
        /// Runtime Spatial Awareness Hierarchy with this prefab:
        /// 
        /// Spatial Awareness System 
        ///         Runtime Spatial Mesh Prefab
        ///             Windows Mixed Reality Spatial Mesh Observer
        ///                 Spatial Mesh - ID
        ///                 Spatial Mesh - ID
        ///                 ...
        /// </summary>
        public GameObject RuntimeSpatialMeshPrefab => runtimeSpatialMeshPrefab;

        #endregion IMixedRealitySpatialAwarenessMeshObserver settings
    }
}
