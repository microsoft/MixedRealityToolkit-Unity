// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

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
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SpatialAwareness/SpatialAwarenessGettingStarted.html")]
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
        { }

        private bool sendObservations = true;

        private GameObject spatialMeshObject = null;

        private MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> meshEventData = null;

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

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        bool IMixedRealityCapabilityCheck.CheckCapability(MixedRealityCapability capability)
        {
            if (capability == MixedRealityCapability.SpatialAwarenessMesh) { return true; }

            return false;
        }

        #endregion IMixedRealityCapabilityCheck Implementation

        #region IMixedRealityDataProvider implementation

        bool autoResume = false;

        /// <inheritdoc />
        public override void Initialize()
        {
            meshEventData = new MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject>(EventSystem.current);

            ReadProfile();

            if (StartupBehavior == AutoStartBehavior.AutoStart)
            {
                Resume();
            }
        }

        public override void Update()
        {
            if (!IsRunning) { return; }

            SendMeshObjects();
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
            // Resume iff we are not running and had been disabled while running.
            if (!IsRunning && autoResume)
            {
                Resume();
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            // Remember if we are currently running when Disable is called.
            autoResume = IsRunning;

            // If we are disabled while running...
            if (IsRunning)
            {
                // Suspend the observer
                Suspend();
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            Disable();
            CleanupObserver();
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

        private int currentMeshId = 0;

        /// <inheritdoc />
        public override void Resume()
        {
            if (IsRunning) { return; }
            IsRunning = true;
        }

        /// <summary>
        /// Event sent whenever a mesh is added.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject>> OnMeshAdded =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject> handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject>>(eventData);
                handler.OnObservationAdded(spatialEventData);
            };

        /// <inheritdoc />
        public override void Suspend()
        {
            if (!IsRunning) { return; }
            IsRunning = false;
        }

        /// <summary>
        /// Sends the observations using the mesh data contained within the configured 3D model.
        /// </summary>
        private void SendMeshObjects()
        {
            if (!sendObservations) { return; }

            if (spatialMeshObject != null)
            {
                MeshFilter[] meshFilters = spatialMeshObject.GetComponentsInChildren<MeshFilter>();
                for (int i = 0; i < meshFilters.Length; i++)
                {
                    SpatialAwarenessMeshObject meshObject = SpatialAwarenessMeshObject.Create(
                        meshFilters[i].sharedMesh,
                        MeshPhysicsLayer,
                        $"Spatial Object Mesh {currentMeshId}",
                        currentMeshId,
                        ObservedObjectParent);

                    meshObject.GameObject.transform.localPosition = meshFilters[i].transform.position;
                    meshObject.GameObject.transform.localRotation = meshFilters[i].transform.rotation;

                    ApplyMeshMaterial(meshObject);

                    meshes.Add(currentMeshId, meshObject);

                    meshEventData.Initialize(this, currentMeshId, meshObject);
                    SpatialAwarenessSystem?.HandleEvent(meshEventData, OnMeshAdded);

                    currentMeshId++;
                }
            }

            sendObservations = false;
        }

        /// <summary>
        /// Removes an observation.
        /// </summary>
        private void RemoveMeshObject(int meshId)
        {
            SpatialAwarenessMeshObject meshObject = null;

            if (meshes.TryGetValue(meshId, out meshObject))
            {
                // Remove the mesh object from the collection.
                meshes.Remove(meshId);
                if (meshObject != null)
                {
                    SpatialAwarenessMeshObject.Cleanup(meshObject);
                }

                // Send the mesh removed event
                meshEventData.Initialize(this, meshId, null);
                SpatialAwarenessSystem?.HandleEvent(meshEventData, OnMeshRemoved);
            }
        }

        /// <summary>
        /// Event sent whenever a mesh is discarded.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject>> OnMeshRemoved =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessMeshObject> handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessMeshObject>>(eventData);
                handler.OnObservationRemoved(spatialEventData);
            };

        #endregion IMixedRealitySpatialAwarenessObserver implementation

        #region IMixedRealitySpatialAwarenessMeshObserver implementation

        private SpatialAwarenessMeshDisplayOptions displayOption = SpatialAwarenessMeshDisplayOptions.Visible;
        
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
        public IReadOnlyDictionary<int, SpatialAwarenessMeshObject> Meshes => new Dictionary<int, SpatialAwarenessMeshObject>(meshes);

        private int meshPhysicsLayer = 31;

        /// <inheritdoc />
        public int MeshPhysicsLayer
        {
            get => meshPhysicsLayer;

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
            get => occlusionMaterial;

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
            get => visibleMaterial;

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

        /// <summary>
        /// Stop the observer and releases resources.
        /// </summary>
        private void CleanupObserver()
        {
            if (IsRunning)
            {
                Suspend();
            }

            ClearObservations();
        }

        /// <summary>
        /// Applies the appropriate material, based on the current of the <see cref="SpatialAwarenessMeshDisplayOptions"/> property. 
        /// </summary>
        /// <param name="meshObject">The <see cref="SpatialAwarenessMeshObject"/> for which the material is to be applied.</param>
        private void ApplyMeshMaterial(SpatialAwarenessMeshObject meshObject)
        {
            if (meshObject?.Renderer == null) { return; }

            bool enable = (DisplayOption != SpatialAwarenessMeshDisplayOptions.None);

            if (enable)
            {
                meshObject.Renderer.sharedMaterial = (DisplayOption == SpatialAwarenessMeshDisplayOptions.Visible) ?
                    VisibleMaterial :
                    OcclusionMaterial;
            }

            meshObject.Renderer.enabled = enable;
        }

        /// <summary>
        /// Updates the material for each observed mesh,.
        /// </summary>
        /// <param name="option">
        /// The <see cref="SpatialAwarenessMeshDisplayOptions"/> value to be used to determine the appropriate material.
        /// </param>
        private void ApplyUpdatedMeshDisplayOption(SpatialAwarenessMeshDisplayOptions option)
        {
            bool enable = (option != SpatialAwarenessMeshDisplayOptions.None);

            foreach (SpatialAwarenessMeshObject meshObject in Meshes.Values)
            {
                if ((meshObject?.Renderer == null)) { continue; }

                if (enable)
                {
                    meshObject.Renderer.sharedMaterial = (option == SpatialAwarenessMeshDisplayOptions.Visible) ?
                        VisibleMaterial :
                        OcclusionMaterial;
                }

                meshObject.Renderer.enabled = enable;
            }
        }

        /// <summary>
        /// Updates the mesh physics layer for current mesh observations.
        /// </summary>
        private void ApplyUpdatedPhysicsLayer()
        {
            foreach (SpatialAwarenessMeshObject meshObject in Meshes.Values)
            {
                if (meshObject?.GameObject == null) { continue; }

                meshObject.GameObject.layer = MeshPhysicsLayer;

            }
        }

        #endregion IMixedRealitySpatialAwarenessMeshObserver implementation
    }
}
