// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

#if XR_MANAGEMENT_ENABLED
using UnityEngine.XR.Management;
#endif // XR_MANAGEMENT_ENABLED

namespace Microsoft.MixedReality.Toolkit.XRSDK
{
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        (SupportedPlatforms)(-1) ^ SupportedPlatforms.WindowsUniversal, // All platforms supported by Unity except UWP
        "XR SDK Spatial Mesh Observer",
        "Profiles/DefaultMixedRealitySpatialAwarenessMeshObserverProfile.asset",
        "MixedRealityToolkit.SDK",
        true,
        SupportedUnityXRPipelines.XRSDK)]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/spatial-awareness/spatial-awareness-getting-started")]
    public class GenericXRSDKSpatialMeshObserver :
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
        public GenericXRSDKSpatialMeshObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        { }

        protected virtual bool? IsActiveLoader => true;

        /// <inheritdoc />
        public override void Enable()
        {
            if (!IsActiveLoader.HasValue)
            {
                IsEnabled = false;
                EnableIfLoaderBecomesActive();
                return;
            }
            else if (!IsActiveLoader.Value)
            {
                IsEnabled = false;
                return;
            }

            base.Enable();
        }

        private async void EnableIfLoaderBecomesActive()
        {
            await new WaitUntil(() => IsActiveLoader.HasValue);
            if (IsActiveLoader.Value)
            {
                Enable();
            }
        }

        #region BaseSpatialObserver Implementation

        private XRMeshSubsystem meshSubsystem;

        /// <summary>
        /// Creates the XRMeshSubsystem and handles the desired startup behavior.
        /// </summary>
        protected override void CreateObserver()
        {
            if (Service == null
#if XR_MANAGEMENT_ENABLED
                || XRGeneralSettings.Instance == null || XRGeneralSettings.Instance.Manager == null || XRGeneralSettings.Instance.Manager.activeLoader == null
#endif // XR_MANAGEMENT_ENABLED
                ) { return; }

#if XR_MANAGEMENT_ENABLED
            meshSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRMeshSubsystem>();
#else
            meshSubsystem = XRSubsystemHelpers.MeshSubsystem;
#endif // XR_MANAGEMENT_ENABLED

            if (meshSubsystem != null)
            {
                ConfigureObserverVolume();
            }
        }

        /// <summary>
        /// Implements proper cleanup of the SurfaceObserver.
        /// </summary>
        protected override void CleanupObserver()
        {
            if (IsRunning)
            {
                Suspend();
            }

            // Since we don't handle the mesh subsystem's lifecycle, we don't do anything more here.
        }

        #endregion BaseSpatialObserver Implementation

        #region BaseSpatialMeshObserver Implementation

        /// <inheritdoc />
        protected override int LookupTriangleDensity(SpatialAwarenessMeshLevelOfDetail levelOfDetail)
        {
            // For non-custom levels, the enum value is the appropriate triangles per cubic meter.
            int level = (int)levelOfDetail;
            if (meshSubsystem != null)
            {
                if (levelOfDetail == SpatialAwarenessMeshLevelOfDetail.Unlimited)
                {
                    meshSubsystem.meshDensity = 1;
                }
                else
                {
                    meshSubsystem.meshDensity = level / (float)SpatialAwarenessMeshLevelOfDetail.Fine; // For now, map Coarse to 0.0 and Fine to 1.0
                }
            }
            return level;
        }

        #endregion BaseSpatialMeshObserver Implementation

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            if (capability != MixedRealityCapability.SpatialAwarenessMesh)
            {
                return false;
            }

            var descriptors = new List<XRMeshSubsystemDescriptor>();
            SubsystemManager.GetSubsystemDescriptors(descriptors);

            return descriptors.Count > 0 && (IsActiveLoader ?? false);
        }

        #endregion IMixedRealityCapabilityCheck Implementation

        #region IMixedRealityDataProvider Implementation

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKSpatialMeshObserver.Update");

        /// <inheritdoc />
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                if (!IsEnabled)
                {
                    return;
                }

                base.Update();
                UpdateObserver();
            }
        }

        #endregion IMixedRealityDataProvider Implementation

        #region IMixedRealitySpatialAwarenessObserver Implementation

        /// <summary>
        /// A queue of MeshId that need their meshes created (or updated).
        /// </summary>
        private readonly Queue<MeshId> meshWorkQueue = new Queue<MeshId>();

        private readonly List<MeshInfo> meshInfos = new List<MeshInfo>();

        /// <summary> 
        /// To prevent too many meshes from being generated at the same time, we will 
        /// only request one mesh to be created at a time. This variable will track 
        /// if a mesh creation request is in flight.
        /// </summary> 
        private SpatialAwarenessMeshObject outstandingMeshObject = null;

        /// <summary>
        /// When surfaces are replaced or removed, rather than destroying them, we'll keep
        /// one as a spare for use in outstanding mesh requests. That way, we'll have fewer
        /// game object create/destroy cycles, which should help performance.
        /// </summary>
        protected SpatialAwarenessMeshObject spareMeshObject = null;

        /// <summary>
        /// The time at which the surface observer was last asked for updated data.
        /// </summary>
        private float lastUpdated = 0;

        private static readonly ProfilerMarker ResumePerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKSpatialMeshObserver.Resume");

        /// <inheritdoc/>
        public override void Resume()
        {
            if (IsRunning)
            {
                Debug.LogWarning("The XR SDK spatial observer is currently running.");
                return;
            }

            using (ResumePerfMarker.Auto())
            {
                if (meshSubsystem != null && !meshSubsystem.running)
                {
                    meshSubsystem.Start();
                }

                // We want the first update immediately.
                lastUpdated = 0;

                // UpdateObserver keys off of this value to start observing.
                IsRunning = true;
            }
        }

        private static readonly ProfilerMarker SuspendPerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKSpatialMeshObserver.Suspend");

        /// <inheritdoc/>
        public override void Suspend()
        {
            if (!IsRunning)
            {
                Debug.LogWarning("The XR SDK spatial observer is currently stopped.");
                return;
            }

            using (SuspendPerfMarker.Auto())
            {
                if (meshSubsystem != null && meshSubsystem.running)
                {
                    meshSubsystem.Stop();
                }

                // UpdateObserver keys off of this value to stop observing.
                IsRunning = false;

                // Clear any pending work.
                meshWorkQueue.Clear();
            }
        }

        private static readonly ProfilerMarker ClearObservationsPerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKSpatialMeshObserver.ClearObservations");

        /// <inheritdoc />
        public override void ClearObservations()
        {
            using (ClearObservationsPerfMarker.Auto())
            {
                bool wasRunning = false;

                if (IsRunning)
                {
                    wasRunning = true;
                    Suspend();
                }

                IReadOnlyList<int> observations = new List<int>(Meshes.Keys);
                foreach (int meshId in observations)
                {
                    RemoveMeshObject(meshId);
                }

                if (wasRunning)
                {
                    Resume();
                }
            }
        }

        #endregion IMixedRealitySpatialAwarenessObserver Implementation

        #region Helpers

        private static readonly ProfilerMarker UpdateObserverPerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKSpatialMeshObserver.UpdateObserver");

        /// <summary>
        /// Requests updates from the surface observer.
        /// </summary>
        private void UpdateObserver()
        {
            if (Service == null || meshSubsystem == null) { return; }

            using (UpdateObserverPerfMarker.Auto())
            {
                // Only update the observer if it is running.
                if (IsRunning && (outstandingMeshObject == null))
                {
                    // If we have a mesh to work on...
                    if (meshWorkQueue.Count > 0)
                    {
                        // We're using a simple first-in-first-out rule for requesting meshes, but a more sophisticated algorithm could prioritize
                        // the queue based on distance to the user or some other metric.
                        RequestMesh(meshWorkQueue.Dequeue());
                    }
                    // If enough time has passed since the previous observer update...
                    else if (Time.time - lastUpdated >= UpdateInterval)
                    {
                        // Update the observer orientation if user aligned
                        if (ObserverVolumeType == VolumeType.UserAlignedCube)
                        {
                            ObserverRotation = CameraCache.Main.transform.rotation;
                        }

                        // Update the observer location if it is not stationary
                        if (!IsStationaryObserver)
                        {
                            ObserverOrigin = CameraCache.Main.transform.position;
                        }

                        // The application can update the observer volume at any time, make sure we are using the latest.
                        ConfigureObserverVolume();

                        if (meshSubsystem.TryGetMeshInfos(meshInfos))
                        {
                            UpdateMeshes(meshInfos);
                        }

                        lastUpdated = Time.time;
                    }
                }
            }
        }

        private static readonly ProfilerMarker RequestMeshPerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKSpatialMeshObserver.RequestMesh");

        /// <summary>
        /// Issue a request to the Surface Observer to begin baking the mesh.
        /// </summary>
        /// <param name="meshId">ID of the mesh to bake.</param>
        private void RequestMesh(MeshId meshId)
        {
            using (RequestMeshPerfMarker.Auto())
            {
                string meshName = ("SpatialMesh - " + meshId);

                SpatialAwarenessMeshObject newMesh;

                if (spareMeshObject == null)
                {
                    newMesh = SpatialAwarenessMeshObject.Create(
                        null,
                        MeshPhysicsLayer,
                        meshName,
                        meshId.GetHashCode());
                }
                else
                {
                    newMesh = spareMeshObject;
                    spareMeshObject = null;

                    newMesh.GameObject.name = meshName;
                    newMesh.Id = meshId.GetHashCode();
                    newMesh.GameObject.SetActive(true);
                }

                meshSubsystem.GenerateMeshAsync(meshId, newMesh.Filter.mesh, newMesh.Collider, MeshVertexAttributes.Normals, (MeshGenerationResult meshGenerationResult) => MeshGenerationAction(meshGenerationResult));
                outstandingMeshObject = newMesh;
            }
        }

        private static readonly ProfilerMarker RemoveMeshObjectPerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKSpatialMeshObserver.RemoveMeshObject");

        /// <summary>
        /// Removes the <see cref="SpatialAwareness.SpatialAwarenessMeshObject"/> associated with the specified id.
        /// </summary>
        /// <param name="id">The id of the mesh to be removed.</param>
        protected void RemoveMeshObject(int id)
        {
            using (RemoveMeshObjectPerfMarker.Auto())
            {
                SpatialAwarenessMeshObject mesh;
                if (meshes.TryGetValue(id, out mesh))
                {
                    // Remove the mesh object from the collection.
                    meshes.Remove(id);

                    // Reclaim the mesh object for future use.
                    ReclaimMeshObject(mesh);

                    // Send the mesh removed event
                    meshEventData.Initialize(this, id, null);
                    Service?.HandleEvent(meshEventData, OnMeshRemoved);
                }
            }
        }

        private static readonly ProfilerMarker ReclaimMeshObjectPerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKSpatialMeshObserver.ReclaimMeshObject");

        /// <summary>
        /// Reclaims the <see cref="SpatialAwareness.SpatialAwarenessMeshObject"/> to allow for later reuse.
        /// </summary>
        protected void ReclaimMeshObject(SpatialAwarenessMeshObject availableMeshObject)
        {
            using (ReclaimMeshObjectPerfMarker.Auto())
            {
                if (spareMeshObject == null)
                {
                    // Cleanup the mesh object.
                    // Do not destroy the game object, destroy the meshes.
                    SpatialAwarenessMeshObject.Cleanup(availableMeshObject, false);

                    availableMeshObject.GameObject.name = "Unused Spatial Mesh";
                    availableMeshObject.GameObject.SetActive(false);

                    spareMeshObject = availableMeshObject;
                }
                else
                {
                    // Cleanup the mesh object.
                    // Destroy the game object, destroy the meshes.
                    SpatialAwarenessMeshObject.Cleanup(availableMeshObject);
                }
            }
        }

        private static readonly ProfilerMarker ConfigureObserverVolumePerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKSpatialMeshObserver.ConfigureObserverVolume");

        private Vector3 oldObserverOrigin = Vector3.zero;
        private Vector3 oldObservationExtents = Vector3.zero;
        private VolumeType oldObserverVolumeType = VolumeType.None;

        /// <summary>
        /// Applies the configured observation extents.
        /// </summary>
        protected virtual void ConfigureObserverVolume()
        {
            if (meshSubsystem == null
                || (oldObserverOrigin == ObserverOrigin
                && oldObservationExtents == ObservationExtents
                && oldObserverVolumeType == ObserverVolumeType))
            {
                return;
            }

            using (ConfigureObserverVolumePerfMarker.Auto())
            {
                // Update the observer
                switch (ObserverVolumeType)
                {
                    case VolumeType.AxisAlignedCube:
                        meshSubsystem.SetBoundingVolume(ObserverOrigin, ObservationExtents);
                        break;

                    default:
                        Debug.LogError($"Unsupported ObserverVolumeType value {ObserverVolumeType}");
                        break;
                }

                oldObserverOrigin = ObserverOrigin;
                oldObservationExtents = ObservationExtents;
                oldObserverVolumeType = ObserverVolumeType;
            }
        }

        private static readonly ProfilerMarker UpdateMeshesPerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKSpatialMeshObserver.UpdateMeshes");

        /// <summary>
        /// Updates meshes based on the result of the MeshSubsystem.TryGetMeshInfos method.
        /// </summary>
        private void UpdateMeshes(List<MeshInfo> meshInfos)
        {
            if (!IsRunning) { return; }

            using (UpdateMeshesPerfMarker.Auto())
            {
                foreach (MeshInfo meshInfo in meshInfos)
                {
                    switch (meshInfo.ChangeState)
                    {
                        case MeshChangeState.Added:
                        case MeshChangeState.Updated:
                            meshWorkQueue.Enqueue(meshInfo.MeshId);
                            break;

                        case MeshChangeState.Removed:
                            RemoveMeshObject(meshInfo.MeshId.GetHashCode());
                            break;
                    }
                }
            }
        }

        private static readonly ProfilerMarker MeshGenerationActionPerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKSpatialMeshObserver.MeshGenerationAction");

        private void MeshGenerationAction(MeshGenerationResult meshGenerationResult)
        {
            if (!IsRunning) { return; }

            using (MeshGenerationActionPerfMarker.Auto())
            {
                if (outstandingMeshObject == null)
                {
                    Debug.LogWarning($"MeshGenerationAction called for mesh id {meshGenerationResult.MeshId} while no request was outstanding.");
                    return;
                }

                switch (meshGenerationResult.Status)
                {
                    case MeshGenerationStatus.InvalidMeshId:
                    case MeshGenerationStatus.Canceled:
                    case MeshGenerationStatus.UnknownError:
                        outstandingMeshObject = null;
                        break;
                    case MeshGenerationStatus.Success:
                        // Since there is only one outstanding mesh object, update the id to match
                        // the one received after baking.
                        SpatialAwarenessMeshObject meshObject = outstandingMeshObject;
                        meshObject.Id = meshGenerationResult.MeshId.GetHashCode();
                        outstandingMeshObject = null;

                        // Check to see if this is a new or updated mesh.
                        bool isMeshUpdate = meshes.ContainsKey(meshObject.Id);

                        // We presume that if the display option is not occlusion, that we should 
                        // default to the visible material. 
                        // Note: We check explicitly for a display option of none later in this method.
                        Material material = (DisplayOption == SpatialAwarenessMeshDisplayOptions.Occlusion) ?
                            OcclusionMaterial : VisibleMaterial;

                        // If this is a mesh update, we want to preserve the mesh's previous material.
                        material = isMeshUpdate ? meshes[meshObject.Id].Renderer.sharedMaterial : material;

                        // Apply the appropriate material.
                        meshObject.Renderer.sharedMaterial = material;

                        // Recalculate the mesh normals if requested.
                        if (RecalculateNormals)
                        {
                            meshObject.Filter.sharedMesh.RecalculateNormals();
                        }

                        // Check to see if the display option is set to none. If so, we disable
                        // the renderer.
                        meshObject.Renderer.enabled = (DisplayOption != SpatialAwarenessMeshDisplayOptions.None);

                        // Set the physics material
                        if (meshObject.Renderer.enabled)
                        {
                            meshObject.Collider.material = PhysicsMaterial;
                        }

                        // Add / update the mesh to our collection
                        if (isMeshUpdate)
                        {
                            // Reclaim the old mesh object for future use.
                            ReclaimMeshObject(meshes[meshObject.Id]);
                            meshes.Remove(meshObject.Id);
                        }
                        meshes.Add(meshObject.Id, meshObject);

                        // This is important. We need to preserve the mesh's local transform here, not its global pose.
                        // Think of it like this. If we set the camera's coordinates 3 meters to the left, the physical camera
                        // hasn't moved, only its coordinates have changed. Likewise, the physical room hasn't moved (relative to
                        // the physical camera), so we also want to set its coordinates 3 meters to the left.
                        Transform meshObjectParent = (ObservedObjectParent.transform != null) ? ObservedObjectParent.transform : null;
                        meshObject.GameObject.transform.SetParent(meshObjectParent, false);

                        meshEventData.Initialize(this, meshObject.Id, meshObject);
                        if (isMeshUpdate)
                        {
                            Service?.HandleEvent(meshEventData, OnMeshUpdated);
                        }
                        else
                        {
                            Service?.HandleEvent(meshEventData, OnMeshAdded);
                        }
                        break;
                }
            }
        }

        #endregion Helpers

        public override void Initialize()
        {
            base.Initialize();

            if (Service == null || meshSubsystem == null) { return; }

            if (RuntimeSpatialMeshPrefab != null)
            {
                AddRuntimeSpatialMeshPrefabToHierarchy();
            }
        }
    }
}
