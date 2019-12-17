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
        private int physicsLayer = 31;
        /// <summary>
        ///
        /// </summary>
        public int PhysicsLayer => physicsLayer;

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
        [Tooltip("Material to use when displaying understood planes")]
        private Material defaultMaterial = null;
        /// <summary>
        /// The material to be used when displaying understood planes.
        /// </summary>
        public Material DefaultMaterial => defaultMaterial;

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
        [Tooltip("Generate plane objects for surface types.")]
        private bool generatePlanes = true;
        /// <summary>
        /// When updating planes, save data to file.
        /// </summary>
        public bool GeneratePlanes => generatePlanes;

        [SerializeField]
        [Tooltip("Apply validation mask texture to planes.")]
        private bool planeValidationMask = false;
        /// <summary>
        /// Apply validation mask texture to planes.
        /// </summary>
        public bool PlaneValidationMask => planeValidationMask;

        [SerializeField]
        [Tooltip("Generate mesh objects for surface types.")]
        private bool generateMeshes = false;
        /// <summary>
        /// Generate mesh objects from surfaces.
        /// </summary>
        public bool GenerateMeshes => generateMeshes;

        [SerializeField]
        [Tooltip("Generate boundless SR mesh.")]
        private bool generateEnvironmentMesh = false;
        /// <summary>
        /// Generate boundless SR mesh.
        /// </summary>
        public bool GenerateEnvironmentMesh => generateEnvironmentMesh;

        [SerializeField]
        [Tooltip("When enabled, renders observed and inferred regions for scene objects. When disabled, renders only the observed regions for scene objects.")]
        private bool renderInferredRegions = true;
        /// <summary>
        /// When enabled, renders observed and inferred regions for scene objects.
        /// When disabled, renders only the observed regions for scene objects.
        /// </summary>
        public bool RenderInferredRegions => renderInferredRegions;

        [SerializeField]
        [Tooltip("The amount of delay before the scene is updated the first time")]
        private float firstUpdateDelay = 1.0f;
        /// <summary>
        /// The amount of delay before the scene us updated the first time
        /// </summary>
        public float FirstUpdateDelay => firstUpdateDelay;

        [SerializeField]
        [Tooltip("The amount of delay before the scene us updated the first time")]
        private SpatialAwarenessMeshLevelOfDetail levelOfDetail = SpatialAwarenessMeshLevelOfDetail.Medium;
        /// <summary>
        /// The amount of detail applied to the <see cref="BoundlessSRMesh"/> and/or <see cref="GeneratePlanarMeshes"/>.
        /// </summary>
        public SpatialAwarenessMeshLevelOfDetail LevelOfDetail => levelOfDetail;

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
    }
}