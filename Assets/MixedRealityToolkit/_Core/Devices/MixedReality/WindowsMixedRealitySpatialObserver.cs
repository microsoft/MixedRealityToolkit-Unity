// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_WSA
using UnityEngine.XR.WSA;
#endif // UNITY_WSA

namespace Microsoft.MixedReality.Toolkit.Core.Devices.SpatialAwareness
{
    public class WindowsMixedRealitySpatialObserver : BaseSpatialObserver
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public WindowsMixedRealitySpatialObserver(string name, uint priority) : base(name, priority)
        { }

        #region IMixedRealityToolkit implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            // Only initialize if the Spatial Awareness system has been enabled in the configuration profile.
            if (!MixedRealityToolkit.Instance.ActiveProfile.IsSpatialAwarenessSystemEnabled) { return; }

#if UNITY_WSA
            CreateObserver();

            // Apply the initial observer volume settings.
            ConfigureObserverVolume();
#endif // UNITY_WSA
        }

        /// <inheritdoc />
        public override void Reset()
        {
#if UNITY_WSA
            CleanupObserver();
#endif // UNITY_WSA
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
#if UNITY_WSA
            UpdateObserver();
#endif // UNITY_WSA
        }

        /// <inheritdoc />
        public override void Disable()
        {
            // todo
        }

        /// <inheritdoc />
        public override void Destroy()
        {
#if UNITY_WSA
            CleanupObserver();
#endif // UNITY_WSA
        }

        #endregion IMixedRealityToolkit implementation

        #region IMixedRealitySpatialAwarenessObserver implementation

        private IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem = null;

        /// <summary>
        /// The currently active instance of <see cref="IMixedRealitySpatialAwarenessSystem"/>.
        /// </summary>
        private IMixedRealitySpatialAwarenessSystem SpatialAwarenessSystem => spatialAwarenessSystem ?? (spatialAwarenessSystem = MixedRealityToolkit.SpatialAwarenessSystem);

#if UNITY_WSA
        /// <summary>
        /// The surface observer providing the spatial data.
        /// </summary>
        private SurfaceObserver observer = null;

        /// <summary>
        /// The current location of the surface observer.
        /// </summary>
        private Vector3 currentObserverOrigin = Vector3.zero;

        /// <summary> 
        /// The observation extents that are currently in use by the surface observer. 
        /// </summary> 
        private Vector3 currentObserverExtents = Vector3.zero;

        /// <summary>
        /// A queue of <see cref="SurfaceId"/> that need their meshes created (or updated).
        /// </summary>
        private readonly Queue<SurfaceId> meshWorkQueue = new Queue<SurfaceId>();

        /// <summary> 
        /// To prevent too many meshes from being generated at the same time, we will 
        /// only request one mesh to be created at a time.  This variable will track 
        /// if a mesh creation request is in flight. 
        /// </summary> 
        private SpatialMeshObject? outstandingMeshObject = null;

        /// <summary>
        /// When surfaces are replaced or removed, rather than destroying them, we'll keep
        /// one as a spare for use in outstanding mesh requests. That way, we'll have fewer
        /// game object create/destroy cycles, which should help performance.
        /// </summary>
        protected SpatialMeshObject? spareMeshObject = null;

        /// <summary>
        /// The time at which the surface observer was last asked for updated data.
        /// </summary>
        private float lastUpdated = 0;
#endif // UNITY_WSA

        /// <inheritdoc />
        public override IDictionary<int, GameObject> Meshes
        {
            get
            {
                Dictionary<int, GameObject> meshes = new Dictionary<int, GameObject>();
                // NOTE: We use foreach here since Dictionary<key, value>.Values is an IEnumerable.
                foreach (int id in meshObjects.Keys)
                {
                    meshes.Add(id, meshObjects[id].GameObject);
                }
                return meshes;
            }
        }

        /// <inheritdoc/>
        public override void StartObserving()
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
        public override void StopObserving()
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

                if (SpatialAwarenessSystem.StartupBehavior == AutoStartBehavior.AutoStart)
                {
                    StartObserving();
                }
            }
        }

        /// <summary>
        /// Ensures that the surface observer has been stopped and destroyed.
        /// </summary>
        private void CleanupObserver()
        {
            if (observer != null)
            {
                if (IsRunning)
                {
                    StopObserving();
                }
                observer.Dispose();
                observer = null;
            }

            if (Application.isPlaying)
            {
                // Cleanup the mesh objects that are being manaaged by this observer.
                base.CleanupMeshes();

                // Cleanup the outstanding mesh object.
                if (outstandingMeshObject.HasValue)
                {
                    // Destroy the game object, destroy the meshes.
                    CleanupMeshObject(outstandingMeshObject.Value);
                    outstandingMeshObject = null;
                }

                // Cleanup the spare mesh object
                if (spareMeshObject.HasValue)
                {
                    // Destroy the game object, destroy the meshes.
                    CleanupMeshObject(spareMeshObject.Value);
                    spareMeshObject = null;
                }

                // Clean up planar surface objects
                // todo
            }
        }

        /// <summary>
        /// Requests updates from the surface observer.
        /// </summary>
        private void UpdateObserver()
        {
            if (SpatialAwarenessSystem == null) { return; }

            // Only update the observer if it is running.
            if (IsRunning && !outstandingMeshObject.HasValue)
            {
                // If we have a mesh to work on...
                if (meshWorkQueue.Count > 0)
                {
                    // We're using a simple first-in-first-out rule for requesting meshes, but a more sophisticated algorithm could prioritize
                    // the queue based on distance to the user or some other metric.
                    RequestMesh(meshWorkQueue.Dequeue());
                }
                // If enough time has passed since the previous observer update...
                else if (Time.time - lastUpdated >= SpatialAwarenessSystem.UpdateInterval)
                {
                    // Update the observer location if it is not stationary
                    if (!SpatialAwarenessSystem.IsStationaryObserver)
                    {
                        SpatialAwarenessSystem.ObserverOrigin = CameraCache.Main.transform.position;
                    }

                    // The application can update the observer volume at any time, make sure we are using the latest.
                    ConfigureObserverVolume();

                    observer.Update(SurfaceObserver_OnSurfaceChanged);
                    lastUpdated = Time.time;
                }
            }
        }

        /// <summary>
        /// Issue a request to the Surface Observer to begin baking the mesh.
        /// </summary>
        /// <param name="surfaceId">ID of the mesh to bake.</param>
        private void RequestMesh(SurfaceId surfaceId)
        {
            string meshName = ("SpatialMesh - " + surfaceId.handle);

            SpatialMeshObject newMesh;
            WorldAnchor worldAnchor;

            if (!spareMeshObject.HasValue)
            {
                newMesh = CreateSpatialMeshObject(null, meshName, surfaceId.handle);

                worldAnchor = newMesh.GameObject.AddComponent<WorldAnchor>();
            }
            else
            {
                newMesh = spareMeshObject.Value;
                spareMeshObject = null;

                newMesh.GameObject.name = meshName;
                newMesh.Id = surfaceId.handle;
                newMesh.GameObject.SetActive(true);

                worldAnchor = newMesh.GameObject.GetComponent<WorldAnchor>();
            }

            Debug.Assert(worldAnchor != null);

            SurfaceData surfaceData = new SurfaceData(
                surfaceId,
                newMesh.Filter,
                worldAnchor,
                newMesh.Collider,
                SpatialAwarenessSystem.MeshTrianglesPerCubicMeter,
                true);

            if (observer.RequestMeshAsync(surfaceData, SurfaceObserver_OnDataReady))
            {
                outstandingMeshObject = newMesh;
            }
            else
            {
                Debug.LogError($"Mesh request failed for Id == surfaceId.handle");
                Debug.Assert(outstandingMeshObject == null);
                ReclaimMeshObject(newMesh);
            }
        }


        /// <summary>
        /// Removes the <see cref="SpatialMeshObject"/> associated with the specified id.
        /// </summary>
        /// <param name="id">The id of the mesh to be removed.</param>
        protected void RemoveMeshObject(int id)
        {
            SpatialMeshObject mesh;

            if (meshObjects.TryGetValue(id, out mesh))
            {
                // Remove the mesh object from the collection.
                meshObjects.Remove(id);

                // Reclaim the mesh object for future use.
                ReclaimMeshObject(mesh);

                // Send the mesh removed event
                SpatialAwarenessSystem.RaiseMeshRemoved(id);
            }
        }

        /// <summary>
        /// Reclaims the <see cref="SpatialMeshObject"/> to allow for later reuse.
        /// </summary>
        /// <param name="availableMeshObject"></param>
        protected void ReclaimMeshObject(SpatialMeshObject availableMeshObject)
        {
            if (!spareMeshObject.HasValue)
            {
                // Cleanup the mesh object.
                // Do not destroy the game object, destroy the meshes.
                CleanupMeshObject(availableMeshObject, false);

                availableMeshObject.GameObject.name = "Unused Spatial Mesh";
                availableMeshObject.GameObject.SetActive(false);

                spareMeshObject = availableMeshObject;
            }
            else
            {
                // Cleanup the mesh object.
                // Destroy the game object, destroy the meshes.
                CleanupMeshObject(availableMeshObject);
            }
        }

        /// <summary>
        /// Applies the configured observation extents.
        /// </summary>
        private void ConfigureObserverVolume()
        {
            Vector3 newExtents = SpatialAwarenessSystem.ObservationExtents;
            Vector3 newOrigin = SpatialAwarenessSystem.ObserverOrigin;

            if (currentObserverExtents.Equals(newExtents) &&
                currentObserverOrigin.Equals(newOrigin))
            {
                return;
            }
            observer.SetVolumeAsAxisAlignedBox(newOrigin, newExtents);

            currentObserverExtents = newExtents;
            currentObserverOrigin = newOrigin;
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

            if (!outstandingMeshObject.HasValue)
            {
                Debug.LogWarning($"OnDataReady called for mesh id {cookedData.id.handle} while no request was outstanding.");
                return;
            }

            if (!outputWritten)
            {
                Debug.LogWarning($"OnDataReady reported no data written for mesh id {cookedData.id.handle}");
                ReclaimMeshObject(outstandingMeshObject.Value);
                outstandingMeshObject = null;
                return;
            }

            // Since there is only one outstanding mesh object, update the id to match
            // the one received after baking.
            SpatialMeshObject meshObject = outstandingMeshObject.Value;
            meshObject.Id = cookedData.id.handle;
            outstandingMeshObject = null;

            // Apply the appropriate material to the mesh.
            SpatialMeshDisplayOptions displayOption = SpatialAwarenessSystem.MeshDisplayOption;
            if (displayOption != SpatialMeshDisplayOptions.None)
            {
                meshObject.Renderer.enabled = true;
                meshObject.Renderer.sharedMaterial = (displayOption == SpatialMeshDisplayOptions.Visible) ?
                    SpatialAwarenessSystem.MeshVisibleMaterial :
                    SpatialAwarenessSystem.MeshOcclusionMaterial;
            }
            else
            {
                meshObject.Renderer.enabled = false;
            }

            // Recalculate the mesh normals if requested.
            if (SpatialAwarenessSystem.MeshRecalculateNormals)
            {
                meshObject.Filter.sharedMesh.RecalculateNormals();
            }

            // Add / update the mesh to our collection
            bool sendUpdatedEvent = false;
            if (meshObjects.ContainsKey(cookedData.id.handle))
            {
                // Reclaim the old mesh object for future use.
                ReclaimMeshObject(meshObjects[cookedData.id.handle]);
                meshObjects.Remove(cookedData.id.handle);

                sendUpdatedEvent = true;
            }
            meshObjects.Add(cookedData.id.handle, meshObject);

            if (sendUpdatedEvent)
            {
                SpatialAwarenessSystem.RaiseMeshUpdated(cookedData.id.handle, meshObject.GameObject);
            }
            else
            {
                SpatialAwarenessSystem.RaiseMeshAdded(cookedData.id.handle, meshObject.GameObject);
            }
        }
#endif // UNITY_WSA

        #endregion IMixedRealitySpatialAwarenessObserver implementation
    }
}
