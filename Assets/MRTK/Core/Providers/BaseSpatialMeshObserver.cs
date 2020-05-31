// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// Class providing a base implementation of the <see cref="IMixedRealitySpatialAwarenessMeshObserver"/> interface.
    /// </summary>
    public abstract class BaseSpatialMeshObserver : BaseSpatialObserver, IMixedRealitySpatialAwarenessMeshObserver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="spatialAwarenessSystem">The <see cref="SpatialAwareness.IMixedRealitySpatialAwarenessSystem"/> to which the observer is providing data.</param>
        /// <param name="name">The friendly name of the data provider.</param>
        /// <param name="priority">The registration priority of the data provider.</param>
        /// <param name="profile">The configuration profile for the data provider.</param>
        protected BaseSpatialMeshObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        {
        }

        #region BaseSpatialMeshObserver Implementation

        protected MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> meshEventData = null;

        private GameObject observedObjectParent = null;

        /// <summary>
        /// The parent GameObject for all observed meshes to be placed under.
        /// </summary>
        protected virtual GameObject ObservedObjectParent => observedObjectParent != null ? observedObjectParent : (observedObjectParent = SpatialAwarenessSystem?.CreateSpatialAwarenessObservationParent(Name));

        protected virtual void ReadProfile()
        {
            if (ConfigurationProfile == null)
            {
                Debug.LogError($"{Name} requires a configuration profile to run properly.");
                return;
            }

            MixedRealitySpatialAwarenessMeshObserverProfile profile = ConfigurationProfile as MixedRealitySpatialAwarenessMeshObserverProfile;
            if (profile == null)
            {
                Debug.LogError($"{Name}'s configuration profile must be a MixedRealitySpatialAwarenessMeshObserverProfile.");
                return;
            }

            // IMixedRealitySpatialAwarenessObserver settings
            StartupBehavior = profile.StartupBehavior;
            IsStationaryObserver = profile.IsStationaryObserver;
            ObservationExtents = profile.ObservationExtents;
            ObserverVolumeType = profile.ObserverVolumeType;
            UpdateInterval = profile.UpdateInterval;

            // IMixedRealitySpatialAwarenessMeshObserver settings
            DisplayOption = profile.DisplayOption;
            LevelOfDetail = profile.LevelOfDetail;
            MeshPhysicsLayer = profile.MeshPhysicsLayer;
            OcclusionMaterial = profile.OcclusionMaterial;
            RecalculateNormals = profile.RecalculateNormals;
            TrianglesPerCubicMeter = profile.TrianglesPerCubicMeter;
            VisibleMaterial = profile.VisibleMaterial;
        }

        private static readonly ProfilerMarker ApplyUpdatedMeshDisplayOptionPerfMarker = new ProfilerMarker("[MRTK] BaseSpatialMeshObserver.ApplyUpdatedMeshDisplayOption");

        /// <summary>
        /// Applies the mesh display option to existing meshes when modified at runtime.
        /// </summary>
        /// <param name="option">The <see cref="SpatialAwarenessMeshDisplayOptions"/> value to be used to determine the appropriate material.</param>
        protected virtual void ApplyUpdatedMeshDisplayOption(SpatialAwarenessMeshDisplayOptions option)
        {
            using (ApplyUpdatedMeshDisplayOptionPerfMarker.Auto())
            {
                bool enable = (option != SpatialAwarenessMeshDisplayOptions.None);

                foreach (SpatialAwarenessMeshObject meshObject in Meshes.Values)
                {
                    if (meshObject?.Renderer == null) { continue; }

                    if (enable)
                    {
                        meshObject.Renderer.sharedMaterial = (option == SpatialAwarenessMeshDisplayOptions.Visible) ?
                            VisibleMaterial :
                            OcclusionMaterial;
                    }

                    meshObject.Renderer.enabled = enable;
                }
            }
        }

        /// <summary>
        /// Maps <see cref="SpatialAwarenessMeshLevelOfDetail"/> to <see cref="TrianglesPerCubicMeter"/>.
        /// </summary>
        /// <param name="levelOfDetail">The desired level of density for the spatial mesh.</param>
        /// <returns>
        /// The number of triangles per cubic meter that will result in the desired level of density.
        /// </returns>
        protected virtual int LookupTriangleDensity(SpatialAwarenessMeshLevelOfDetail levelOfDetail)
        {
            // By default, returns the existing value. This will be custom defined for each platform, if necessary.
            return TrianglesPerCubicMeter;
        }

        private static readonly ProfilerMarker ApplyUpdatedPhysicsLayerPerfMarker = new ProfilerMarker("[MRTK] BaseSpatialMeshObserver.ApplyUpdatedPhysicsLayer");

        /// <summary>
        /// Updates the mesh physics layer for current mesh observations.
        /// </summary>
        protected virtual void ApplyUpdatedPhysicsLayer()
        {
            using (ApplyUpdatedPhysicsLayerPerfMarker.Auto())
            {
                foreach (SpatialAwarenessMeshObject meshObject in Meshes.Values)
                {
                    if (meshObject?.GameObject == null) { continue; }

                    meshObject.GameObject.layer = MeshPhysicsLayer;
                }
            }
        }

        private static readonly ProfilerMarker OnMeshAddedPerfMarker = new ProfilerMarker("[MRTK] BaseSpatialMeshObserver.OnMeshAdded - Raising OnObservationAdded");

        /// <summary>
        /// Event sent whenever a mesh is added.
        /// </summary>
        protected static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject>> OnMeshAdded =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject> handler, BaseEventData eventData)
            {
                using (OnMeshAddedPerfMarker.Auto())
                {
                    MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject>>(eventData);
                    handler.OnObservationAdded(spatialEventData);
                }
            };

        private static readonly ProfilerMarker OnMeshUpdatedPerfMarker = new ProfilerMarker("[MRTK] BaseSpatialMeshObserver.OnMeshUpdated - Raising OnObservationUpdated");

        /// <summary>
        /// Event sent whenever a mesh is updated.
        /// </summary>
        protected static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject>> OnMeshUpdated =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject> handler, BaseEventData eventData)
            {
                using (OnMeshUpdatedPerfMarker.Auto())
                {
                    MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject>>(eventData);
                    handler.OnObservationUpdated(spatialEventData);
                }
            };

        private static readonly ProfilerMarker OnMeshRemovedPerfMarker = new ProfilerMarker("[MRTK] BaseSpatialMeshObserver.OnMeshRemoved - Raising OnObservationRemoved");

        /// <summary>
        /// Event sent whenever a mesh is discarded.
        /// </summary>
        protected static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject>> OnMeshRemoved =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject> handler, BaseEventData eventData)
            {
                using (OnMeshRemovedPerfMarker.Auto())
                {
                    MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject>>(eventData);
                    handler.OnObservationRemoved(spatialEventData);
                }
            };

        #endregion BaseSpatialMeshObserver Implementation

        #region IMixedRealityDataProvider Implementation

        /// <summary>
        /// Initializes event data and creates the observer.
        /// </summary>
        public override void Initialize()
        {
            meshEventData = new MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject>(EventSystem.current);

            ReadProfile();

            base.Initialize();
        }

        #endregion IMixedRealityDataProvider Implementation

        #region IMixedRealitySpatialMeshObserver Implementation

        private SpatialAwarenessMeshDisplayOptions displayOption = SpatialAwarenessMeshDisplayOptions.Visible;

        /// <inheritdoc />
        public SpatialAwarenessMeshDisplayOptions DisplayOption
        {
            get { return displayOption; }
            set
            {
                displayOption = value;
                ApplyUpdatedMeshDisplayOption(displayOption);
            }
        }

        private SpatialAwarenessMeshLevelOfDetail levelOfDetail = SpatialAwarenessMeshLevelOfDetail.Coarse;

        /// <inheritdoc />
        public SpatialAwarenessMeshLevelOfDetail LevelOfDetail
        {
            get { return levelOfDetail; }
            set
            {
                if (value != SpatialAwarenessMeshLevelOfDetail.Custom)
                {
                    TrianglesPerCubicMeter = LookupTriangleDensity(value);
                }

                levelOfDetail = value;
            }
        }

        /// <summary>
        /// The backing field for Meshes, to allow the mesh observer implementation to track its meshes.
        /// </summary>
        protected readonly Dictionary<int, SpatialAwarenessMeshObject> meshes = new Dictionary<int, SpatialAwarenessMeshObject>();

        /// <inheritdoc />
        public IReadOnlyDictionary<int, SpatialAwarenessMeshObject> Meshes => new Dictionary<int, SpatialAwarenessMeshObject>(meshes);

        private int meshPhysicsLayer = 31;

        /// <inheritdoc />
        public int MeshPhysicsLayer
        {
            get { return meshPhysicsLayer; }
            set
            {
                if ((value < 0) || (value > 31))
                {
                    Debug.LogError("Specified MeshPhysicsLayer is out of bounds. Please set a value between 0 and 31, inclusive.");
                    return;
                }

                meshPhysicsLayer = value;
                ApplyUpdatedPhysicsLayer();
            }
        }

        /// <inheritdoc />
        public int MeshPhysicsLayerMask => (1 << MeshPhysicsLayer);

        /// <inheritdoc />
        public bool RecalculateNormals { get; set; } = true;

        /// <inheritdoc />
        public int TrianglesPerCubicMeter { get; set; } = 0;

        private Material occlusionMaterial = null;

        /// <inheritdoc />
        public Material OcclusionMaterial
        {
            get { return occlusionMaterial; }
            set
            {
                if (value != occlusionMaterial)
                {
                    occlusionMaterial = value;

                    if (DisplayOption == SpatialAwarenessMeshDisplayOptions.Occlusion)
                    {
                        ApplyUpdatedMeshDisplayOption(SpatialAwarenessMeshDisplayOptions.Occlusion);
                    }
                }
            }
        }

        private Material visibleMaterial = null;

        /// <inheritdoc />
        public Material VisibleMaterial
        {
            get { return visibleMaterial; }
            set
            {
                if (value != visibleMaterial)
                {
                    visibleMaterial = value;

                    if (DisplayOption == SpatialAwarenessMeshDisplayOptions.Visible)
                    {
                        ApplyUpdatedMeshDisplayOption(SpatialAwarenessMeshDisplayOptions.Visible);
                    }
                }
            }
        }

        #endregion IMixedRealitySpatialMeshObserver Implementation
    }
}
