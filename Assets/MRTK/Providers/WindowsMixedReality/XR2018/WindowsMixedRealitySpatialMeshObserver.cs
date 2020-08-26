// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.license information.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Windows.Utilities;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

#if UNITY_WSA
using UnityEngine.XR;
using UnityEngine.XR.WSA;
#endif // UNITY_WSA

#if WINDOWS_UWP
using WindowsSpatialSurfaces = global::Windows.Perception.Spatial.Surfaces;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.SpatialAwareness
{
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        SupportedPlatforms.WindowsUniversal,
        "Windows Mixed Reality Spatial Mesh Observer",
        "Profiles/DefaultMixedRealitySpatialAwarenessMeshObserverProfile.asset",
        "MixedRealityToolkit.SDK",
        true)]
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SpatialAwareness/SpatialAwarenessGettingStarted.html")]
    public class WindowsMixedRealitySpatialMeshObserver :
        BaseSpatialMeshObserver,
        IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="spatialAwarenessSystem">The service instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public WindowsMixedRealitySpatialMeshObserver(
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
        /// <param name="spatialAwarenessSystem">The service instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsMixedRealitySpatialMeshObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        { }

        #region BaseSpatialObserver Implementation

        /// <summary>
        /// Creates the surface observer and handles the desired startup behavior.
        /// </summary>
        protected override void CreateObserver()
        {
            if (SpatialAwarenessSystem == null) { return; }

#if UNITY_WSA
            if (observer == null)
            {
                observer = new SurfaceObserver();
                ConfigureObserverVolume();

                if (StartupBehavior == AutoStartBehavior.AutoStart)
                {
                    Resume();
                }
            }
#endif // UNITY_WSA
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

#if UNITY_WSA
            if (observer != null)
            {
                observer.Dispose();
                observer = null;
            }
#endif // UNITY_WSA
        }

        #endregion BaseSpatialObserver Implementation

        #region BaseSpatialMeshObserver Implementation

        /// <inheritdoc />
        protected override int LookupTriangleDensity(SpatialAwarenessMeshLevelOfDetail levelOfDetail)
        {
            int triangleDensity;
            switch (levelOfDetail)
            {
                case SpatialAwarenessMeshLevelOfDetail.Coarse:
                    triangleDensity = 0;
                    break;

                case SpatialAwarenessMeshLevelOfDetail.Medium:
                    triangleDensity = 400;
                    break;

                case SpatialAwarenessMeshLevelOfDetail.Fine:
                    triangleDensity = 2000;
                    break;

                default:
                    Debug.LogWarning($"There is no triangle density lookup for {levelOfDetail}, defaulting to Coarse");
                    triangleDensity = 0;
                    break;
            }

            return triangleDensity;
        }

        #endregion BaseSpatialMeshObserver Implementation

        #region IMixedRealityCapabilityCheck Implementation

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            if (WindowsApiChecker.IsMethodAvailable(
                "Windows.Perception.Spatial.Surfaces",
                "SpatialSurfaceObserver",
                "IsSupported"))
            {
#if WINDOWS_UWP
                return (capability == MixedRealityCapability.SpatialAwarenessMesh) && WindowsSpatialSurfaces.SpatialSurfaceObserver.IsSupported();
#endif // WINDOWS_UWP
            }

            return false;
        }

        #endregion IMixedRealityCapabilityCheck Implementation

        #region IMixedRealityDataProvider Implementation

#if UNITY_WSA
        /// <summary>
        /// Creates and configures the spatial observer, as well as
        /// setting the required SpatialPerception capability.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

#if UNITY_EDITOR && UNITY_WSA
            Utilities.Editor.UWPCapabilityUtility.RequireCapability(
                    UnityEditor.PlayerSettings.WSACapability.SpatialPerception,
                    this.GetType());
#endif
        }

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealitySpatialMeshObserver.Update");

        /// <inheritdoc />
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                UpdateObserver();
            }
        }
#endif // UNITY_WSA

        #endregion IMixedRealityDataProvider Implementation

        #region IMixedRealitySpatialAwarenessObserver Implementation

#if UNITY_WSA
        /// <summary>
        /// The surface observer providing the spatial data.
        /// </summary>
        private SurfaceObserver observer = null;

        /// <summary>
        /// A queue of <see cref="SurfaceId"/> that need their meshes created (or updated).
        /// </summary>
        private readonly Queue<SurfaceId> meshWorkQueue = new Queue<SurfaceId>();

        /// <summary> 
        /// To prevent too many meshes from being generated at the same time, we will 
        /// only request one mesh to be created at a time.  This variable will track 
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
#endif // UNITY_WSA

#if UNITY_WSA
        private static readonly ProfilerMarker ResumePerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealitySpatialMeshObserver.Resume");
#endif // UNITY_WSA

        /// <inheritdoc/>
        public override void Resume()
        {
#if UNITY_WSA
            if (IsRunning)
            {
                Debug.LogWarning("The Windows Mixed Reality spatial observer is currently running.");
                return;
            }

            using (ResumePerfMarker.Auto())
            {
                // We want the first update immediately.
                lastUpdated = 0;

                // UpdateObserver keys off of this value to start observing.
                IsRunning = true;
            }
#endif // UNITY_WSA
        }

#if UNITY_WSA
        private static readonly ProfilerMarker SuspendPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealitySpatialMeshObserver.Suspend");
#endif // UNITY_WSA

        /// <inheritdoc/>
        public override void Suspend()
        {
#if UNITY_WSA
            if (!IsRunning)
            {
                Debug.LogWarning("The Windows Mixed Reality spatial observer is currently stopped.");
                return;
            }

            using (SuspendPerfMarker.Auto())
            {
                // UpdateObserver keys off of this value to stop observing.
                IsRunning = false;

                // Halt any outstanding work.
                if (outstandingMeshObject != null)
                {
                    ReclaimMeshObject(outstandingMeshObject);
                    outstandingMeshObject = null;
                }

                // Clear any pending work.
                meshWorkQueue.Clear();
            }
#endif // UNITY_WSA
        }

#if UNITY_WSA
        private static readonly ProfilerMarker ClearObservationsPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealitySpatialMeshObserver.ClearObservations");
#endif // UNITY_WSA

#if UNITY_WSA
        /// <inheritdoc />
        public override void ClearObservations()
        {
            using (ClearObservationsPerfMarker.Auto())
            {
                bool wasRunning = false;

                if (IsRunning)
                {
                    wasRunning = true;
                    Debug.Log("Cannot clear observations while the observer is running. Suspending this observer.");
                    Suspend();
                }

                IReadOnlyList<int> observations = new List<int>(Meshes.Keys);
                foreach (int meshId in observations)
                {
                    RemoveMeshObject(meshId);
                }

                // Cleanup the outstanding mesh object.
                if (outstandingMeshObject != null)
                {
                    // Destroy the game object, destroy the meshes.
                    SpatialAwarenessMeshObject.Cleanup(outstandingMeshObject);
                    outstandingMeshObject = null;
                }

                // Cleanup the spare mesh object	
                if (spareMeshObject != null)
                {
                    // Destroy the game object, destroy the meshes.	
                    SpatialAwarenessMeshObject.Cleanup(spareMeshObject);
                    spareMeshObject = null;
                }

                if (wasRunning)
                {
                    Resume();
                }
            }
        }
#endif // UNITY_WSA

        #endregion IMixedRealitySpatialAwarenessObserver Implementation

        #region Helpers

#if UNITY_WSA
        private static readonly ProfilerMarker UpdateObserverPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealitySpatialMeshObserver.UpdateObserver");

        /// <summary>
        /// Requests updates from the surface observer.
        /// </summary>
        private void UpdateObserver()
        {
            if (SpatialAwarenessSystem == null || HolographicSettings.IsDisplayOpaque || !XRDevice.isPresent) { return; }

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

                        observer.Update(SurfaceObserver_OnSurfaceChanged);

                        lastUpdated = Time.time;
                    }
                }
            }
        }

        /// <summary>
        /// Internal component to monitor the WorldAnchor's transform, apply the MixedRealityPlayspace transform,
        /// and apply it to its parent.
        /// </summary>
        private class PlayspaceAdapter : MonoBehaviour
        {
            /// <summary>
            /// Compute concatenation of lhs * rhs such that lhs * (rhs * v) = Concat(lhs, rhs) * v
            /// </summary>
            /// <param name="lhs">Second transform to apply</param>
            /// <param name="rhs">First transform to apply</param>
            private static Pose Concatenate(Pose lhs, Pose rhs)
            {
                return rhs.GetTransformedBy(lhs);
            }

            private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealitySpatialMeshObserver+PlayspaceAdapter.Update");

            /// <summary>
            /// Compute and set the parent's transform.
            /// </summary>
            private void Update()
            {
                using (UpdatePerfMarker.Auto())
                {
                    Pose worldFromPlayspace = new Pose(MixedRealityPlayspace.Position, MixedRealityPlayspace.Rotation);
                    Pose anchorPose = new Pose(transform.position, transform.rotation);
                    /// Propagate any global scale on the playspace into the position.
                    Vector3 playspaceScale = MixedRealityPlayspace.Transform.lossyScale;
                    anchorPose.position *= playspaceScale.x; 
                    Pose parentPose = Concatenate(worldFromPlayspace, anchorPose);
                    transform.parent.position = parentPose.position;
                    transform.parent.rotation = parentPose.rotation;
                }
            }
        }

        private static readonly ProfilerMarker RequestMeshPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealitySpatialMeshObserver+PlayspaceAdapter.RequestMesh");

        /// <summary>
        /// Issue a request to the Surface Observer to begin baking the mesh.
        /// </summary>
        /// <param name="surfaceId">ID of the mesh to bake.</param>
        private void RequestMesh(SurfaceId surfaceId)
        {
            using (RequestMeshPerfMarker.Auto())
            {
                string meshName = ("SpatialMesh - " + surfaceId.handle);

                SpatialAwarenessMeshObject newMesh;
                WorldAnchor worldAnchor;

                if (spareMeshObject == null)
                {
                    newMesh = SpatialAwarenessMeshObject.Create(
                        null,
                        MeshPhysicsLayer,
                        meshName,
                        surfaceId.handle,
                        ObservedObjectParent);

                    // The WorldAnchor component places its object where the anchor is in the same space as the camera. 
                    // But since the camera is repositioned by the MixedRealityPlayspace's transform, the meshes' transforms
                    // should also the WorldAnchor position repositioned by the MixedRealityPlayspace's transform.
                    // So rather than put the WorldAnchor on the mesh's GameObject, the WorldAnchor is placed out of the way in the scene,
                    // and its transform is concatenated with the Playspace transform to compute the transform on the mesh's object.
                    // That adapting the WorldAnchor's transform into playspace is done by the internal PlayspaceAdapter component.
                    // The GameObject the WorldAnchor is placed on is unimportant, but it is convenient for cleanup to make it a child
                    // of the GameObject whose transform will track it.
                    GameObject anchorHolder = new GameObject(meshName + "_anchor");
                    anchorHolder.AddComponent<PlayspaceAdapter>(); // replace with required component?
                    worldAnchor = anchorHolder.AddComponent<WorldAnchor>(); // replace with required component and GetComponent()? 
                    anchorHolder.transform.SetParent(newMesh.GameObject.transform, false);
                }
                else
                {
                    newMesh = spareMeshObject;
                    spareMeshObject = null;

                    newMesh.GameObject.name = meshName;
                    newMesh.Id = surfaceId.handle;
                    newMesh.GameObject.SetActive(true);

                    // There should be exactly one child on the newMesh.GameObject, and that is the GameObject added above
                    // to hold the WorldAnchor component and adapter.
                    Debug.Assert(newMesh.GameObject.transform.childCount == 1, "Expecting a single child holding the WorldAnchor");
                    worldAnchor = newMesh.GameObject.transform.GetChild(0).gameObject.GetComponent<WorldAnchor>();
                }

                Debug.Assert(worldAnchor != null);

                SurfaceData surfaceData = new SurfaceData(
                    surfaceId,
                    newMesh.Filter,
                    worldAnchor,
                    newMesh.Collider,
                    TrianglesPerCubicMeter,
                    true);

                if (observer.RequestMeshAsync(surfaceData, SurfaceObserver_OnDataReady))
                {
                    outstandingMeshObject = newMesh;
                }
                else
                {
                    Debug.LogError($"Mesh request failed for Id == surfaceId.handle");
                    outstandingMeshObject = null;
                    ReclaimMeshObject(newMesh);
                }
            }
        }

        private static readonly ProfilerMarker RemoveMeshObjectPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealitySpatialMeshObserver+PlayspaceAdapter.RemoveMeshObject");

        /// <summary>
        /// Removes the <see cref="SpatialAwarenessMeshObject"/> associated with the specified id.
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
                    SpatialAwarenessSystem?.HandleEvent(meshEventData, OnMeshRemoved);
                }
            }
        }

        private static readonly ProfilerMarker ReclaimMeshObjectPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealitySpatialMeshObserver+PlayspaceAdapter.ReclaimMeshObject");

        /// <summary>
        /// Reclaims the <see cref="SpatialAwarenessMeshObject"/> to allow for later reuse.
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

        private static readonly ProfilerMarker ConfigureObserverVolumePerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealitySpatialMeshObserver+PlayspaceAdapter.ConfigureObserverVolume");

        /// <summary>
        /// Applies the configured observation extents.
        /// </summary>
        private void ConfigureObserverVolume()
        {
            using (ConfigureObserverVolumePerfMarker.Auto())
            {
                if (MixedRealityPlayspace.Transform == null)
                {
                    Debug.LogError("Unexpected failure acquiring MixedRealityPlayspace.");
                    return;
                }

                // If we aren't using a HoloLens or there isn't an XR device present, return.
                if (observer == null || HolographicSettings.IsDisplayOpaque || !XRDevice.isPresent) { return; }

                // The observer's origin is in world space, we need it in the camera's parent's space
                // to set the volume. The MixedRealityPlayspace provides that space that the camera/head moves around in.
                Vector3 observerOriginPlayspace = MixedRealityPlayspace.InverseTransformPoint(ObserverOrigin);
                Quaternion observerRotationPlayspace = Quaternion.Inverse(MixedRealityPlayspace.Rotation) * ObserverRotation;

                // Update the observer
                switch (ObserverVolumeType)
                {
                    case VolumeType.AxisAlignedCube:
                        observer.SetVolumeAsAxisAlignedBox(observerOriginPlayspace, ObservationExtents);
                        break;

                    case VolumeType.Sphere:
                        // We use the x value of the extents as the sphere radius
                        observer.SetVolumeAsSphere(observerOriginPlayspace, ObservationExtents.x);
                        break;

                    case VolumeType.UserAlignedCube:
                        observer.SetVolumeAsOrientedBox(observerOriginPlayspace, ObservationExtents, observerRotationPlayspace);
                        break;

                    default:
                        Debug.LogError($"Unsupported ObserverVolumeType value {ObserverVolumeType}");
                        break;
                }
            }
        }

        private static readonly ProfilerMarker OnSurfaceChangedPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealitySpatialMeshObserver+PlayspaceAdapter.SurfaceObserver_OnSurfaceChanged");

        /// <summary>
        /// Handles the SurfaceObserver's OnSurfaceChanged event.
        /// </summary>
        /// <param name="id">The identifier assigned to the surface which has changed.</param>
        /// <param name="changeType">The type of change that occurred on the surface.</param>
        /// <param name="bounds">The bounds of the surface.</param>
        /// <param name="updateTime">The date and time at which the change occurred.</param>
        private void SurfaceObserver_OnSurfaceChanged(SurfaceId id, SurfaceChange changeType, Bounds bounds, System.DateTime updateTime)
        {
            if (!IsRunning) { return; }

            using (OnSurfaceChangedPerfMarker.Auto())
            {
                switch (changeType)
                {
                    case SurfaceChange.Added:
                    case SurfaceChange.Updated:
                        meshWorkQueue.Enqueue(id);
                        break;

                    case SurfaceChange.Removed:
                        RemoveMeshObject(id.handle);
                        break;
                }
            }
        }

        private static readonly ProfilerMarker OnDataReadyPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealitySpatialMeshObserver+PlayspaceAdapter.SurfaceObserver_OnDataReady");

        /// <summary>
        /// Handles the SurfaceObserver's OnDataReady event.
        /// </summary>
        /// <param name="cookedData">Struct containing output data.</param>
        /// <param name="outputWritten">Set to true if output has been written.</param>
        /// <param name="elapsedCookTimeSeconds">Seconds between mesh cook request and propagation of this event.</param>
        private void SurfaceObserver_OnDataReady(SurfaceData cookedData, bool outputWritten, float elapsedCookTimeSeconds)
        {
            if (!IsRunning) { return; }

            using (OnDataReadyPerfMarker.Auto())
            {
                if (outstandingMeshObject == null)
                {
                    return;
                }

                if (!outputWritten)
                {
                    ReclaimMeshObject(outstandingMeshObject);
                    outstandingMeshObject = null;
                    return;
                }

                // Since there is only one outstanding mesh object, update the id to match
                // the one received after baking.
                SpatialAwarenessMeshObject meshObject = outstandingMeshObject;
                meshObject.Id = cookedData.id.handle;
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

                // Preserve local transform relative to parent.
                meshObject.GameObject.transform.SetParent(ObservedObjectParent != null ?
                    ObservedObjectParent.transform: null, false);

                meshEventData.Initialize(this, meshObject.Id, meshObject);
                if (isMeshUpdate)
                {
                    SpatialAwarenessSystem?.HandleEvent(meshEventData, OnMeshUpdated);
                }
                else
                {
                    SpatialAwarenessSystem?.HandleEvent(meshEventData, OnMeshAdded);
                }
            }
        }
#endif // UNITY_WSA

        #endregion Helpers
    }
}
