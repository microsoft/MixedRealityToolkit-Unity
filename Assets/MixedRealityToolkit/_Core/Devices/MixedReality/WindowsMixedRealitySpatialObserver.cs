// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_WSA
using UnityEngine.XR.WSA;
#endif // UNITY_WSA

using UnityEngine.UI;

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

        #region IMixedRealityManager implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            // Only initialize if the Spatial Awareness system has been enabled in the configuration profile.
            if (!MixedRealityManager.Instance.ActiveProfile.IsSpatialAwarenessSystemEnabled) { return; }

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

        #endregion IMixedRealityManager implementation

        #region IMixedRealitySpatialAwarenessObserver implementation

#if UNITY_WSA
        /// <summary>
        /// The surface observer providing the spatial data.
        /// </summary>
        private SurfaceObserver observer = null;
#endif // UNITY_WSA

        /// <summary>
        /// The observation extents that are currently in use by the surface observer.
        /// </summary>
        private Vector3 currentObserverExtents = Vector3.zero;

        /// <summary>
        /// A queue of <see cref="SurfaceId"/> that need their meshes created (or updated).
        /// </summary>
        private readonly Queue<SurfaceId> meshWorkQueue = new Queue<SurfaceId>();

        /// <summary>
        /// Identifies the meshes, by id, that are being baked by Unity and if it has been added (true) 
        /// or is being updated (false). 
        /// </summary>
        private Dictionary<int, bool> meshAddStatus = new Dictionary<int, bool>();

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
        private SpatialMeshObject? spareMeshObject = null;

        /// <summary>
        /// The time at which the surface observer was last asked for updated data.
        /// </summary>
        private float lastUpdated = 0;

        private Dictionary<int, SpatialMeshObject> meshObjects = new Dictionary<int, SpatialMeshObject>();

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
            if (observer == null)
            {
                observer = new SurfaceObserver();
                ConfigureObserverVolume();

                if (MixedRealityManager.Instance.ActiveProfile.SpatialAwarenessProfile.StartupBehavior == AutoStartBehavior.AutoStart)
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
                // Clean up mesh objects.
                // NOTE: We use foreach here since Dictionary<key, value>.Values is an IEnumerable.
                foreach (SpatialMeshObject meshObject in meshObjects.Values)
                {
                    // Cleanup mesh object.
                    // Destroy the game object, destroy the meshes.
                    CleanupMeshObject(meshObject);
                }
                meshObjects.Clear();

                // Cleanup the oustanding mesh object.
                if (outstandingMeshObject.HasValue)
                {
                    // Destroy the game object, destroy the meshes.
                    CleanupMeshObject(outstandingMeshObject.Value);
                }

                // Cleanup the spare mesh object
                if (spareMeshObject.HasValue)
                {
                    // Destroy the game object, destroy the meshes.
                    CleanupMeshObject(spareMeshObject.Value);
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
                else if (Time.time - lastUpdated >= MixedRealityManager.Instance.ActiveProfile.SpatialAwarenessProfile.UpdateInterval)
                {
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

            // todo:
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

                worldAnchor = newMesh.GameObject.GetComponent<WorldAnchor>();
            }

            Debug.Assert(worldAnchor != null);

            SurfaceData surfaceData = new SurfaceData(
                surfaceId,
                newMesh.Filter,
                worldAnchor,
                newMesh.Collider,
                MixedRealityManager.SpatialAwarenessSystem.MeshTrianglesPerCubicMeter,
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
        /// Applies the configured observation extents.
        /// </summary>
        private void ConfigureObserverVolume()
        {
            Vector3 newExtents = MixedRealityManager.SpatialAwarenessSystem.ObservationExtents;

            if (currentObserverExtents.Equals(newExtents)) { return; }
            observer.SetVolumeAsAxisAlignedBox(Vector3.zero, newExtents);
        }
#endif // UNITY_WSA

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
                    meshAddStatus.Add(id.handle, (changeType == SurfaceChange.Added));
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

            // todo: investigate
            //if (outstandingMeshObject.Value.Id != cookedData.id.handle)
            //{
            //    Debug.LogWarning($"OnDataReady called for for mesh id {cookedData.id.handle} while request for mesh id {outstandingMeshObject.Value.Id} was outstanding.");
            //    ReclaimMeshObject(outstandingMeshObject.Value);
            //    outstandingMeshObject = null;
            //    return;
            //}

            // Apply the appropriate material to the mesh.
            SpatialMeshDisplayOptions displayOption = MixedRealityManager.SpatialAwarenessSystem.MeshDisplayOption;
            if (displayOption != SpatialMeshDisplayOptions.None)
            {
                outstandingMeshObject.Value.Renderer.sharedMaterial = (displayOption == SpatialMeshDisplayOptions.Visible) ?
                    MixedRealityManager.SpatialAwarenessSystem.MeshVisibleMaterial :
                    MixedRealityManager.SpatialAwarenessSystem.MeshOcclusionMaterial;
            }

            // Recalculate the mesh normals if requested.
            if (MixedRealityManager.SpatialAwarenessSystem.MeshRecalculateNormals)
            {
                outstandingMeshObject.Value.Filter.sharedMesh.RecalculateNormals();
            }

            // Add / update the mesh to our collection
            if (!meshObjects.ContainsKey(cookedData.id.handle))
            {
                meshObjects.Add(cookedData.id.handle, outstandingMeshObject.Value);
            }
            else
            {
                meshObjects[cookedData.id.handle] = outstandingMeshObject.Value;
            }

            // Send the appropriate mesh event (added or updated)
            bool isNewMesh = false;
            if (meshAddStatus.TryGetValue(cookedData.id.handle, out isNewMesh))
            {
                GameObject mesh = outstandingMeshObject.Value.GameObject;
                if (isNewMesh)
                {
                    MixedRealityManager.SpatialAwarenessSystem.RaiseMeshAdded(cookedData.id.handle, mesh);
                }
                else
                {
                    MixedRealityManager.SpatialAwarenessSystem.RaiseMeshUpdated(cookedData.id.handle, mesh);
                }

                meshAddStatus.Remove(cookedData.id.handle);
            }

            // We are done with the outstanding mesh object, reset it's value.
            outstandingMeshObject = null;
        }

        /// <summary>
        /// Removes the <see cref="SpatialMeshObject"/> associated with the specified id.
        /// </summary>
        /// <param name="id">The id of the mesh to be removed.</param>
        private void RemoveMeshObject(int id)
        {
            SpatialMeshObject mesh;

            if (meshObjects.TryGetValue(id, out mesh))
            {
                // Remove the mesh object from the collection.
                meshObjects.Remove(id);

                // Cleanup the mesh.
                // Do not destroy the game object, destroy the meshes.
                CleanupMeshObject(mesh, false);

                // Reclaim the mesh object for future use.
                ReclaimMeshObject(mesh);

                // Send the mesh removed event
                MixedRealityManager.SpatialAwarenessSystem.RaiseMeshRemoved(id);
            }
        }

        /// <summary>
        /// Reclaims the <see cref="SpatialMeshObject"/> to allow for later reuse.
        /// </summary>
        /// <param name="availableMeshObject"></param>
        private void ReclaimMeshObject(SpatialMeshObject availableMeshObject)
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

        // todo: SetObserverOrigin

        #endregion IMixedRealitySpatialAwarenessObserver implementation
    }
}
