// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
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
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SpatialAwareness/SpatialAwarenessGettingStarted.html")]
    public class SpatialObjectMeshObserver :
        BaseSpatialMeshObserver,
        IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public SpatialObjectMeshObserver(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : this(spatialAwarenessSystem, name, priority, profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public SpatialObjectMeshObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        { }

        private bool sendObservations = true;

        private GameObject spatialMeshObject = null;

        #region BaseSpatialMeshObserver Implementation

        /// <summary>
        /// Reads the settings from the configuration profile.
        /// </summary>
        protected override void ReadProfile()
        {
            base.ReadProfile();

            SpatialObjectMeshObserverProfile profile = ConfigurationProfile as SpatialObjectMeshObserverProfile;
            if (profile == null) { return; }

            // SpatialObjectMeshObserver settings
            spatialMeshObject = profile.SpatialMeshObject;
        }

        #endregion BaseSpatialMeshObserver Implementation

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        bool IMixedRealityCapabilityCheck.CheckCapability(MixedRealityCapability capability)
        {
            return capability == MixedRealityCapability.SpatialAwarenessMesh;
        }

        #endregion IMixedRealityCapabilityCheck Implementation

        #region IMixedRealityDataProvider Implementation

        /// <inheritdoc />
        public override void Update()
        {
            if (!IsRunning) 
            {
                return;
            }

            SendMeshObjects();
        }

        #endregion IMixedRealityDataProvider Implementation

        #region BaseSpatialObserver Implementation

        /// <inheritdoc />
        protected override void CreateObserver()
        {
            if (StartupBehavior == AutoStartBehavior.AutoStart)
            {
                Resume();
            }
        }

        /// <inheritdoc />
        protected override void CleanupObserver()
        {
            if (IsRunning)
            {
                Suspend();
            }
        }

        #endregion BaseSpatialObserver Implementation

        #region IMixedRealitySpatialAwarenessObserver Implementation

        private static readonly ProfilerMarker ClearObservationsPerfMarker = new ProfilerMarker("[MRTK] SpatialObjectMeshObserver.ClearObservations");

        /// <inheritdoc />
        public override void ClearObservations()
        {
            using (ClearObservationsPerfMarker.Auto())
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
        }

        /// <inheritdoc />
        public override void Resume()
        {
            if (IsRunning) { return; }
            IsRunning = true;
        }

        /// <inheritdoc />
        public override void Suspend()
        {
            if (!IsRunning) { return; }
            IsRunning = false;
        }

        #endregion IMixedRealitySpatialAwarenessObserver Implementation

        #region Helpers
        
        private int currentMeshId = 0;

        private static readonly ProfilerMarker SendMeshObjectsPerfMarker = new ProfilerMarker("[MRTK] SpatialObjectMeshObserver.SendMeshObjects");

        /// <summary>
        /// Sends the observations using the mesh data contained within the configured 3D model.
        /// </summary>
        private void SendMeshObjects()
        {
            if (!sendObservations) { return; }

            using (SendMeshObjectsPerfMarker.Auto())
            {
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
        }

        private static readonly ProfilerMarker RemoveMeshObjectPerfMarker = new ProfilerMarker("[MRTK] SpatialObjectMeshObserver.RemoveMeshObject");

        /// <summary>
        /// Removes an observation.
        /// </summary>
        private void RemoveMeshObject(int meshId)
        {
            using (RemoveMeshObjectPerfMarker.Auto())
            {
                if (meshes.TryGetValue(meshId, out SpatialAwarenessMeshObject meshObject))
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

        #endregion Helpers
    }
}
