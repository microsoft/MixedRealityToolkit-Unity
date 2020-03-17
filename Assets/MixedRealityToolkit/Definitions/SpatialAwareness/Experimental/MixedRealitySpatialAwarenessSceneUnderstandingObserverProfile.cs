// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    /// <summary>
    /// Configuration profile settings for spatial awareness mesh observers.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Scene Understanding Observer Profile", fileName = "MixedRealitySceneUnderstandingObserverProfile", order = (int)CreateProfileMenuItemIndices.SceneUnderstandingObserver)]
    [MixedRealityServiceProfile(typeof(IMixedRealitySpatialAwarenessSceneUnderstandingObserver))]
    public class MixedRealitySpatialAwarenessSceneUnderstandingObserverProfile : BaseSpatialAwarenessObserverProfile
    {
        #region IMixedRealityOnDemandObserver settings

        [SerializeField]
        [Tooltip("Observer will update once after initialization then require manual update thereafter.")]
        private bool updateOnceOnLoad = false;
        /// <summary>
        /// Observer will update once after initialization then require manual update thereafter. Uses <see cref="FirstUpdateDelay"/> to determine when.
        /// </summary>
        public bool UpdateOnceOnLoad => updateOnceOnLoad;

        [SerializeField]
        [Tooltip("Should the observer update on an interval or on demand?")]
        private bool autoUpdate = false;

        /// <summary>
        /// Indicates if the observer is to start immediately or wait for manual startup.
        /// </summary>
        /// <remarks>
        /// When false, the observer will only update when UpdateOnDemand() is called.
        /// </remarks>
        public bool AutoUpdate => autoUpdate;

        #endregion IMixedRealityOnDemandObserver settings

        [PhysicsLayer]
        [SerializeField]
        [Tooltip("Physics layer on which to set understood planes.")]
        private int defaultPhysicsLayer = 31;
        /// <summary>
        /// Physics layer on which to set understood planes
        /// </summary>
        public int DefaultPhysicsLayer => defaultPhysicsLayer;

        [EnumFlags]
        [SerializeField]
        [Tooltip("Which plane types the observer should generate.")]
        private SpatialAwarenessSurfaceTypes surfaceTypes = SpatialAwarenessSurfaceTypes.Floor | SpatialAwarenessSurfaceTypes.Ceiling | SpatialAwarenessSurfaceTypes.Wall | SpatialAwarenessSurfaceTypes.Platform;
        /// <summary>
        /// Which plane types the observer should generate.
        /// </summary>
        public SpatialAwarenessSurfaceTypes SurfaceTypes => surfaceTypes;

        [SerializeField]
        [Tooltip("The number of planes to render per interval. Setting this too high can cause performance problems.")]
        private int instantiationBatchRate = 1;
        /// <summary>
        /// Number of planes to spawn per frame. Used to throttle object creation for performance.
        /// </summary>
        public int InstantiationBatchRate => instantiationBatchRate;

        [SerializeField]
        [Tooltip("Material to use when displaying understood planes and meshes")]
        private Material defaultMaterial = null;
        /// <summary>
        /// The material to be used when displaying understood planes.
        /// </summary>
        public Material DefaultMaterial => defaultMaterial;

        [SerializeField]
        [Tooltip("Material to use when displaying the world mesh")]
        private Material defaultWorldMeshMaterial = null;
        /// <summary>
        /// The material to be used when displaying understood planes.
        /// </summary>
        public Material DefaultWorldMeshMaterial => defaultWorldMeshMaterial;

        [SerializeField]
        [Tooltip("Load saved scan data from a file instead of device")]
        private bool shouldLoadFromFile = false;
        /// <summary>
        /// Load saved scan into scene understanding.
        /// </summary>
        public bool ShouldLoadFromFile => shouldLoadFromFile;

        [SerializeField]
        [Tooltip("The path to the saved scene understanding file.")]
        private TextAsset serializedScene = null;
        /// <summary>
        /// The path to the saved scene understanding file.
        /// </summary>
        public TextAsset SerializedScene => serializedScene;

        [SerializeField]
        [Tooltip("Create game objects for observed surface types. When off, only events will be sent to subscribers.")]
        private bool createGameObjects = true;
        /// <summary>
        /// When updating planes, save data to file.
        /// </summary>
        public bool CreateGameObjects => createGameObjects;

        [SerializeField]
        [Tooltip("Generate plane objects for surface types.")]
        private bool requestPlaneData = true;
        /// <summary>
        /// When updating planes, save data to file.
        /// </summary>
        public bool RequestPlaneData => requestPlaneData;

        [SerializeField]
        [Tooltip("Generate mesh objects for surface types.")]
        private bool requestMeshData = false;
        /// <summary>
        /// Generate mesh objects from surfaces.
        /// </summary>
        public bool RequestMeshData => requestMeshData;

        [SerializeField]
        [Tooltip("Fills in the gaps for unobserved data.")]
        private bool inferRegions = true;

        /// <summary>
        /// When enabled, renders observed and inferred regions for scene objects.
        /// When disabled, renders only the observed regions for scene objects.
        /// </summary>
        public bool InferRegions => inferRegions;

        [SerializeField]
        [Tooltip("The amount of delay before the scene is updated the first time")]
        private float firstUpdateDelay = 1.0f;
        /// <summary>
        /// The amount of delay before the scene us updated the first time
        /// </summary>
        public float FirstUpdateDelay => firstUpdateDelay;

        [SerializeField]
        [Tooltip("Controls the amount of polygons returned for the mesh")]
        private SpatialAwarenessMeshLevelOfDetail worldMeshLevelOfDetail = SpatialAwarenessMeshLevelOfDetail.Medium;
        /// <summary>
        /// The amount of detail applied to the <see cref="BoundlessSRMesh"/> and/or <see cref="GeneratePlanarMeshes"/>.
        /// </summary>
        public SpatialAwarenessMeshLevelOfDetail WorldMeshLevelOfDetail => worldMeshLevelOfDetail;

        [SerializeField]
        [Tooltip("Keep previously observed objects when updating the scene")]
        private bool usePersistentObjects = true;
        /// <summary>
        /// Keep previously observed objects when updating the scene
        /// </summary>
        public bool UsePersistentObjects => usePersistentObjects;

        [SerializeField]
        [Tooltip("Calculate surfaces upto radius distance")]
        private float queryRadius = 5.0f;
        /// <summary>
        /// Keep previously observed objects when updating the scene
        /// </summary>
        public float QueryRadius => queryRadius;

        [SerializeField]
        [Tooltip("When instantiating quads, show the occlusion mask texture")]
        private bool requestOcclusionMask = true;
        /// <summary>
        /// When instantiating quads, show the occlusion mask texture
        /// </summary>
        public bool RequestOcclusionMask => requestOcclusionMask;

        [SerializeField]
        [Tooltip("Sets pixel resolution of occlusion mask")]
        private Vector2Int occlusionMaskResolution = new Vector2Int(128, 128);
        /// <summary>
        /// When instantiating quads, show the occlusion mask texture
        /// </summary>
        public Vector2Int OcclusionMaskResolution => occlusionMaskResolution;

        [SerializeField]
        [Tooltip("Attempt to align scene to largest found floor's normal. Does nothing on HoloLens.")]
        private bool orientScene = true;
        /// <summary>
        /// When NOT on HoloLens, attempt to align scene to largest found floor's normal
        /// </summary>
        public bool OrientScene => orientScene;
    }
}