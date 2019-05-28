// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_WSA
using UnityEngine.XR.WSA;
#endif // UNITY_WSA

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.SpatialAwareness
{
    [MixedRealityDataProvider(
        typeof(IMixedRealitySpatialAwarenessSystem),
        SupportedPlatforms.WindowsUniversal,
        "Windows Mixed Reality Spatial Mesh Observer",
        "Profiles/DefaultMixedRealitySpatialAwarenessMeshObserverProfile.asset", 
        "MixedRealityToolkit.SDK")]
    [DocLink("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/SpatialAwareness/SpatialAwarenessGettingStarted.html")]
    public class WindowsMixedRealitySpatialMeshObserver : BaseSpatialObserver, IMixedRealitySpatialAwarenessMeshObserver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsMixedRealitySpatialMeshObserver(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(registrar, spatialAwarenessSystem, name, priority, profile)
        {
            ReadProfile();
        }

        private void ReadProfile()
        {
            if (ConfigurationProfile == null)
            {
                Debug.LogError("Windows Mixed Reality Spatial Mesh Observer requires a configuration profile to run properly.");
                return;
            }

            MixedRealitySpatialAwarenessMeshObserverProfile profile = ConfigurationProfile as MixedRealitySpatialAwarenessMeshObserverProfile;
            if (profile == null)
            {
                Debug.LogError("Windows Mixed Reality Spatial Mesh Observer's configuration profile must be a MixedRealitySpatialAwarenessMeshObserverProfile.");
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

        #region IMixedRealityToolkit implementation

#if UNITY_WSA

        /// <inheritdoc />
        public override void Initialize()
        {
            CreateObserver();

            // Apply the initial observer volume settings.
            ConfigureObserverVolume();
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
            // todo
        }

        /// <inheritdoc />
        public override void Update()
        {
            UpdateObserver();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            // todo
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            CleanupObserver();
        }

#endif // UNITY_WSA

        #endregion IMixedRealityToolkit implementation

        #region IMixedRealitySpatialAwarenessObserver implementation

        private GameObject observedObjectParent = null;

        /// <summary>
        /// The <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> to which observed objects are parented.
        /// </summary>
        private GameObject ObservedObjectParent => observedObjectParent != null ? observedObjectParent : (observedObjectParent = SpatialAwarenessSystem?.CreateSpatialAwarenessObjectParent("WindowsMixedRealitySpatialMeshObserver"));

#if UNITY_WSA
        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposed) { return; }

            base.Dispose(disposing);

            if (IsRunning)
            {
                Suspend();
            }

            if (disposing)
            {
                CleanupObservedObjects();
            }

            DisposeObserver();

            disposed = true;
        }

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

        public SpatialAwarenessMeshDisplayOptions displayOption = SpatialAwarenessMeshDisplayOptions.Visible;
        /// <inheritdoc />
        public SpatialAwarenessMeshDisplayOptions DisplayOption
        {
            get
            {
                return displayOption;
            }

            set
            {
                if (value != displayOption)
                {
                    displayOption = value;
                    ApplyUpdatedMeshDisplayOption(displayOption);
                }
            }
        }

        public SpatialAwarenessMeshLevelOfDetail levelOfDetail = SpatialAwarenessMeshLevelOfDetail.Coarse;

        /// <inheritdoc />
        public SpatialAwarenessMeshLevelOfDetail LevelOfDetail
        {
            get { return levelOfDetail; }

            set
            {
                if (value != SpatialAwarenessMeshLevelOfDetail.Custom)
                {
                    // For non-custom levels, the enum value is the appropriate triangles per cubic meter.
                    TrianglesPerCubicMeter = (int)value;
                }

                levelOfDetail = value;
            }
        }

        private Dictionary<int, SpatialAwarenessMeshObject> meshes = new Dictionary<int, SpatialAwarenessMeshObject>();

        /// <inheritdoc />
        public IReadOnlyDictionary<int, SpatialAwarenessMeshObject> Meshes => new Dictionary<int, SpatialAwarenessMeshObject>(meshes) as IReadOnlyDictionary<int, SpatialAwarenessMeshObject>;

        /// <inheritdoc />
        public int MeshPhysicsLayer { get; set; }

        /// <inheritdoc />
        public int MeshPhysicsLayerMask => (1 << MeshPhysicsLayer);

        /// <inheritdoc />
        public Material OcclusionMaterial { get; set; } = null;

        /// <inheritdoc />
        public bool RecalculateNormals { get; set; } = true;

        public int TrianglesPerCubicMeter { get; set; } = 0;

        /// <inheritdoc />
        public Material VisibleMaterial { get; set; } = null;

        /// <inheritdoc/>
        public override void Resume()
        {
#if UNITY_WSA
            if (IsRunning)
            {
                Debug.LogWarning("The Windows Mixed Reality spatial observer is currently running.");
                return;
            }
            
            // We want the first update immediately.
            lastUpdated = 0;

            // UpdateObserver keys off of this value to start observing.
            IsRunning = true;

#endif // UNITY_WSA
        }

        /// <inheritdoc/>
        public override void Suspend()
        {
#if UNITY_WSA
            if (!IsRunning)
            {
                Debug.LogWarning("The Windows Mixed Reality spatial observer is currently stopped.");
                return;
            }

            // UpdateObserver keys off of this value to stop observing.
            IsRunning = false;

            // Clear any pending work.
            meshWorkQueue.Clear();
#endif // UNITY_WSA
        }

#if UNITY_WSA
        /// <summary>
        /// Creates the surface observer and handles the desired startup behavior.
        /// </summary>
        private void CreateObserver()
        {
            if (SpatialAwarenessSystem == null) { return; }

            if (observer == null)
            {
                observer = new SurfaceObserver();
                ConfigureObserverVolume();

                if (StartupBehavior == AutoStartBehavior.AutoStart)
                {
                    Resume();
                }
            }
        }

        /// <summary>
        /// Ensures that the surface observer has been stopped and destroyed.
        /// </summary>
        private void CleanupObserver()
        {
            // Destroys all observed objects and the observer.
            Dispose(true);
        }

        /// <summary>
        /// Cleans up the objects created during observation.
        /// </summary>
        private void CleanupObservedObjects()
        {
            if (Application.isPlaying)
            {
                // Cleanup the scene objects are managing
                if (observedObjectParent != null)
                {
                    observedObjectParent.transform.DetachChildren();
                }

                foreach (SpatialAwarenessMeshObject meshObject in meshes.Values)
                {
                    if (meshObject != null)
                    {
                        // Destroy the game object, destroy the meshes.
                        SpatialAwarenessMeshObject.Cleanup(meshObject);
                    }
                }
                meshes.Clear();

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
            }
        }

        /// <summary>
        /// Implements proper cleanup of the SurfaceObserver.
        /// </summary>
        private void DisposeObserver()
        {
            if (observer != null)
            {
                observer.Dispose();
                observer = null;
            }
        }

        /// <summary>
        /// Requests updates from the surface observer.
        /// </summary>
        private void UpdateObserver()
        {
            if (SpatialAwarenessSystem == null)
            {
                return;
            }

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
            /// <returns></returns>
            private static Pose Concatenate(Pose lhs, Pose rhs)
            {
                return rhs.GetTransformedBy(lhs);
            }

            /// <summary>
            /// Compute and set the parent's transform.
            /// </summary>
            private void Update()
            {
                Pose worldFromPlayspace = new Pose(MixedRealityPlayspace.Position, MixedRealityPlayspace.Rotation);
                Pose anchorPose = new Pose(transform.position, transform.rotation);
                Pose parentPose = Concatenate(worldFromPlayspace, anchorPose);
                transform.parent.position = parentPose.position;
                transform.parent.rotation = parentPose.rotation;
            }
        }

        /// <summary>
        /// Issue a request to the Surface Observer to begin baking the mesh.
        /// </summary>
        /// <param name="surfaceId">ID of the mesh to bake.</param>
        private void RequestMesh(SurfaceId surfaceId)
        {
            string meshName = ("SpatialMesh - " + surfaceId.handle);

            SpatialAwarenessMeshObject newMesh;
            WorldAnchor worldAnchor;

            if (spareMeshObject == null)
            {
                newMesh = SpatialAwarenessMeshObject.Create(null, MeshPhysicsLayer, meshName, surfaceId.handle);

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


        /// <summary>
        /// Removes the <see cref="SpatialAwarenessMeshObject"/> associated with the specified id.
        /// </summary>
        /// <param name="id">The id of the mesh to be removed.</param>
        protected void RemoveMeshObject(int id)
        {
            SpatialAwarenessMeshObject mesh;

            if (meshes.TryGetValue(id, out mesh))
            {
                // Remove the mesh object from the collection.
                meshes.Remove(id);

                // Reclaim the mesh object for future use.
                ReclaimMeshObject(mesh);

                // Send the mesh removed event
                SpatialAwarenessSystem?.RaiseMeshRemoved(this, id);
            }
        }

        /// <summary>
        /// Reclaims the <see cref="SpatialAwarenessMeshObject"/> to allow for later reuse.
        /// </summary>
        /// <param name="availableMeshObject"></param>
        protected void ReclaimMeshObject(SpatialAwarenessMeshObject availableMeshObject)
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

        /// <summary>
        /// Applies the configured observation extents.
        /// </summary>
        private void ConfigureObserverVolume()
        {
            // Update the observer
            switch(ObserverVolumeType)
            {
                case VolumeType.AxisAlignedCube:
                    observer.SetVolumeAsAxisAlignedBox(ObserverOrigin, ObservationExtents);
                    break;

                case VolumeType.Sphere:
                    // We use the x value of the extents as the sphere radius
                    observer.SetVolumeAsSphere(ObserverOrigin, ObservationExtents.x);
                    break;

                case VolumeType.UserAlignedCube:
                    observer.SetVolumeAsOrientedBox(ObserverOrigin, ObservationExtents, ObserverRotation);
                    break;

                default:
                    Debug.LogError($"Unsupported ObserverVolumeType value {ObserverVolumeType}");
                    break;
            }
        }

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

        /// <summary>
        /// Handles the SurfaceObserver's OnDataReady event.
        /// </summary>
        /// <param name="cookedData">Struct containing output data.</param>
        /// <param name="outputWritten">Set to true if output has been written.</param>
        /// <param name="elapsedCookTimeSeconds">Seconds between mesh cook request and propagation of this event.</param>
        private void SurfaceObserver_OnDataReady(SurfaceData cookedData, bool outputWritten, float elapsedCookTimeSeconds)
        {
            if (!IsRunning) { return; }

            if (outstandingMeshObject == null)
            {
                Debug.LogWarning($"OnDataReady called for mesh id {cookedData.id.handle} while no request was outstanding.");
                return;
            }

            if (!outputWritten)
            {
                Debug.LogWarning($"OnDataReady reported no data written for mesh id {cookedData.id.handle}");
                ReclaimMeshObject(outstandingMeshObject);
                outstandingMeshObject = null;
                return;
            }

            // Since there is only one outstanding mesh object, update the id to match
            // the one received after baking.
            SpatialAwarenessMeshObject meshObject = outstandingMeshObject;
            meshObject.Id = cookedData.id.handle;
            outstandingMeshObject = null;

            // Apply the appropriate material to the mesh.
            SpatialAwarenessMeshDisplayOptions displayOption = DisplayOption;
            if (displayOption != SpatialAwarenessMeshDisplayOptions.None)
            {
                meshObject.Renderer.enabled = true;
                meshObject.Renderer.sharedMaterial = (displayOption == SpatialAwarenessMeshDisplayOptions.Visible) ?
                    VisibleMaterial :
                    OcclusionMaterial;
            }
            else
            {
                meshObject.Renderer.enabled = false;
            }

            // Recalculate the mesh normals if requested.
            if (RecalculateNormals)
            {
                meshObject.Filter.sharedMesh.RecalculateNormals();
            }

            // Add / update the mesh to our collection
            bool sendUpdatedEvent = false;
            if (meshes.ContainsKey(cookedData.id.handle))
            {
                // Reclaim the old mesh object for future use.
                ReclaimMeshObject(meshes[cookedData.id.handle]);
                meshes.Remove(cookedData.id.handle);

                sendUpdatedEvent = true;
            }
            meshes.Add(cookedData.id.handle, meshObject);

            meshObject.GameObject.transform.parent = (ObservedObjectParent.transform != null) ? ObservedObjectParent.transform : null;

            if (sendUpdatedEvent)
            {
                SpatialAwarenessSystem?.RaiseMeshUpdated(this, cookedData.id.handle, meshObject);
            }
            else
            {
                SpatialAwarenessSystem?.RaiseMeshAdded(this, cookedData.id.handle, meshObject);
            }
        }
#endif // UNITY_WSA

        /// <summary>
        /// Applies the mesh display option to existing meshes when modified at runtime.
        /// </summary>
        /// <param name="option">The <see cref="SpatialAwarenessMeshDisplayOptions"/> to apply to the meshes.</param>
        private void ApplyUpdatedMeshDisplayOption(SpatialAwarenessMeshDisplayOptions option)
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

        #endregion IMixedRealitySpatialAwarenessObserver implementation
    }
}
