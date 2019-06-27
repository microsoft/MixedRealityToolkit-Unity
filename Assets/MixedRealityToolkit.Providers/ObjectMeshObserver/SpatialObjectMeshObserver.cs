// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialObjectMeshObserver
{
    /// <summary>
    /// Spatial awareness mesh observer that provides mesh data from a 3D model imported as a Unity asset.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        SupportedPlatforms.WindowsEditor | SupportedPlatforms.MacEditor | SupportedPlatforms.LinuxEditor,
        "Spatial Object Mesh Observer",
        "ObjectMeshObserver/Profiles/DefaultObjectMeshObserverProfile.asset",
        "MixedRealityToolkit.Providers")]
    // [DocLink("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SpatialAwareness/SpatialAwarenessGettingStarted.html")]
    public class SpatialObjectMeshObserver : 
        BaseSpatialObserver, 
        IMixedRealitySpatialAwarenessMeshObserver, 
        IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public SpatialObjectMeshObserver(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(registrar, spatialAwarenessSystem, name, priority, profile)
        {
            int i = 0;
            i++;
        }

        private bool sendObservations = true;

        private GameObject spatialMeshObject = null;

        private List<Mesh> observedMeshes = new List<Mesh>();

        /// <summary>
        /// Reads the settings from the configuration profile.
        /// </summary>
        private void ReadProfile()
        {
            SpatialObjectMeshObserverProfile profile = ConfigurationProfile as SpatialObjectMeshObserverProfile;
            if (profile == null) { return; }

            // SpatialObjectMeshObserver settings
            spatialMeshObject = profile.SpatialMeshObject;

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

        // todo: create 0 or more SpatialAwarenessMeshObjects from the model
        private void LoadObject()
        {
            //// Loading starts with a fresh mesh collection.
            //observedMeshes.Clear();

            //// todo error handling
            //if (spatialMeshObject == null) { return; }

            //MeshFilter filter = spatialMeshObject.GetComponentInChildren<MeshFilter>();
            //if (filter == null) { return; }

            //Mesh mesh = filter.sharedMesh;
            //if (mesh == null) { return; }

            //int subMeshCount = mesh.subMeshCount;
        }

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        bool IMixedRealityCapabilityCheck.CheckCapability(MixedRealityCapability capability)
        {
            if (capability == MixedRealityCapability.SpatialAwarenessMesh) { return true; }

            return false;
        }

        #endregion IMixedRealityCapabilityCheck Implementation

        #region IMixedRealityDataProvider implementation

        bool wasRunning = false;

        /// <inheritdoc />
        public override void Initialize()
        {
            ReadProfile();
            LoadObject();

            if (StartupBehavior == AutoStartBehavior.AutoStart)
            {
                wasRunning = true;
            }
        }

        /// <inheritdoc />
        public override void Reset()
        {
            CleanupObserver();
            Initialize();
        }

        /// <inheritdoc />
        public override void Enable()
        {
            if (wasRunning)
            {
                Resume();
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            // todo UpdateObserver();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            wasRunning = IsRunning;
            Suspend();
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            Disable();
            CleanupObserver();
        }

        private void CleanupObserver()
        {
            if(IsRunning)
            {
                Suspend();
            }

            ClearObservations();
            // todo
            //fileContents.Clear();
            //fileContents = null;
        }

        #endregion IMixedRealityDataProvider implementation

        #region IMixedRealitySpatialAwarenessObserver implementation

        private GameObject observedObjectParent = null;

        /// <inheritdoc />
        protected virtual GameObject ObservedObjectParent => observedObjectParent != null ? observedObjectParent : (observedObjectParent = SpatialAwarenessSystem?.CreateSpatialAwarenessObservationParent(Name));

        /// <inheritdoc />
        public override void ClearObservations()
        {
            if (IsRunning)
            {
                Debug.Log("Cannot clear observations while the observer is running. Suspending this observer.");
                Suspend();
            }

            foreach (int id in Meshes.Keys)
            {
                RemoveMeshObject(id);
            }

            // Resend file observations when resumed.
            sendObservations = true;
        }

        private int baseMeshId = 7000000;
        /// <inheritdoc />
        public override void Resume()
        {
            if (!sendObservations) { return; }

            IsRunning = true;

            // Get the collection of MeshFilters
            MeshFilter[] meshFilters= spatialMeshObject.GetComponentsInChildren<MeshFilter>();
            for (int i = 0; i < meshFilters.Length; i++)
            {
                SpatialAwarenessMeshObject meshObject = SpatialAwarenessMeshObject.Create(
                    meshFilters[i].sharedMesh,
                    MeshPhysicsLayer,
                    $"Spatial Object Mesh {baseMeshId}",
                    baseMeshId,
                    ObservedObjectParent);
                ApplyMeshMaterial(meshObject);

                meshes.Add(baseMeshId, meshObject);

                baseMeshId++;
            }

            sendObservations = false;
        }

        /// <inheritdoc />
        public override void Suspend()
        {
            IsRunning = false;
        }

        private void RemoveMeshObject(int id)
        {
            SpatialAwarenessMeshObject meshObject = null;

            if (meshes.TryGetValue(id, out meshObject))
            {
                // Remove the mesh object from the collection.
                meshes.Remove(id);
                if (meshObject != null)
                {
                    SpatialAwarenessMeshObject.Cleanup(meshObject);
                }

                // Send the mesh removed event
                SpatialAwarenessSystem?.RaiseMeshRemoved(this, id);
            }
        }

        #endregion IMixedRealitySpatialAwarenessObserver implementation

        #region IMixedRealitySpatialAwarenessMeshObserver implementation

        public SpatialAwarenessMeshDisplayOptions displayOption = SpatialAwarenessMeshDisplayOptions.Visible;
        
        /// <inheritdoc />
        public SpatialAwarenessMeshDisplayOptions DisplayOption
        {
            get => displayOption;

            set
            {
                displayOption = value;
                ApplyUpdatedMeshDisplayOption(displayOption);
            }
        }

        /// <inheritdoc />
        public SpatialAwarenessMeshLevelOfDetail LevelOfDetail { get; set; } = SpatialAwarenessMeshLevelOfDetail.Coarse;

        private Dictionary<int, SpatialAwarenessMeshObject> meshes = new Dictionary<int, SpatialAwarenessMeshObject>();

        /// <inheritdoc />
        public IReadOnlyDictionary<int, SpatialAwarenessMeshObject> Meshes =>
            new Dictionary<int, SpatialAwarenessMeshObject>(meshes) as IReadOnlyDictionary<int, SpatialAwarenessMeshObject>;

        private int meshPhysicsLayer = 31;

        /// <inheritdoc />
        public int MeshPhysicsLayer
        {
            get => meshPhysicsLayer;

            set
            {
                if ((value < 0) || (value > 31))
                {
                    Debug.LogError("Specified MeshPhysicsLayer is out of bounds. Please use a value between 0 and 31, inclusive.");
                    return;
                }
                meshPhysicsLayer = value;
            }
        }

        /// <inheritdoc />
        public int MeshPhysicsLayerMask => (1 << MeshPhysicsLayer);

        /// <inheritdoc />
        public bool RecalculateNormals { get; set; } = true;

        /// <inheritdoc />
        public int TrianglesPerCubicMeter { get; set; } = 0;

        /// <inheritdoc />
        public Material OcclusionMaterial { get; set; } = null;

        /// <inheritdoc />
        public Material VisibleMaterial { get; set; } = null;

        // todo
        protected void ApplyMeshMaterial(SpatialAwarenessMeshObject meshObject)
        {
            bool enable = (DisplayOption != SpatialAwarenessMeshDisplayOptions.None);

            if (enable)
            {
                meshObject.Renderer.sharedMaterial = (DisplayOption == SpatialAwarenessMeshDisplayOptions.Visible) ?
                    VisibleMaterial :
                    OcclusionMaterial;
            }

            meshObject.Renderer.enabled = enable;
        }

        // todo
        protected void ApplyUpdatedMeshDisplayOption(SpatialAwarenessMeshDisplayOptions option)
        {
            bool enable = (option != SpatialAwarenessMeshDisplayOptions.None);

            foreach (SpatialAwarenessMeshObject meshObject in Meshes.Values)
            {
                if ((meshObject == null) || (meshObject.Renderer == null)) { continue; }

                if (enable)
                {
                    meshObject.Renderer.sharedMaterial = (option == SpatialAwarenessMeshDisplayOptions.Visible) ?
                        VisibleMaterial :
                        OcclusionMaterial;
                }

                meshObject.Renderer.enabled = enable;
            }
        }

        #endregion IMixedRealitySpatialAwarenessMeshObserver implementation
    }
}
