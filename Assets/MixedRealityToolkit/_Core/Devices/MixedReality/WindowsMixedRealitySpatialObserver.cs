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
        private SpatialMeshObject? spareMeshObject = null;

        /// <summary>
        /// The time at which the surface observer was last asked for updated data.
        /// </summary>
        private float lastUpdated = 0;

        private Dictionary<int, GameObject> meshes = new Dictionary<int, GameObject>();

        /// <inheritdoc />
        public IDictionary<int, GameObject> Meshes
        {
            get
            {
                // We return a copy of the meshes collection to preserve the integrity of our internal dictionary.
                return new Dictionary<int, GameObject>(meshes);
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
                //    ApplyObservationExtents();

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
                //    // Clean up mesh objects.
                //    // NOTE: We use foreach here since Dictionary<key, value>.Values is an IEnumerable.
                //    foreach (GameObject mesh in Meshes.Values)
                //    {
                //        if (Application.isEditor)
                //        {
                //            Object.DestroyImmediate(mesh);
                //        }
                //        else
                //        {
                //            Object.Destroy(mesh);
                //        }
                //    }
                //    Meshes.Clear();

                //    // Clean up mesh objects that were to be baked.
                //    // NOTE: We use foreach here since Dictionary<key, value>.Values is an IEnumerable.
                //    foreach (GameObject mesh in meshesToBake.Values)
                //    {
                //        if (Application.isEditor)
                //        {
                //            Object.DestroyImmediate(mesh);
                //        }
                //        else
                //        {
                //            Object.Destroy(mesh);
                //        }
                //    }
                //    meshesToBake.Clear();

                //    // Clean up planar surface objects
                //    // todo
            }
        }

        /// <summary>
        /// Requests updates from the surface observer.
        /// </summary>
        private void UpdateObserver()
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
                else if (Time.time - lastUpdated >= MixedRealityManager.Instance.ActiveProfile.SpatialAwarenessProfile.UpdateInterval)
                {
                    // todo:
                    //// The application can update the observation extents at any time.
                    //ApplyObservationExtents();

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

            if (spareMeshObject == null)
            {
                newMesh = CreateSpatialMeshObject(null, meshName, surfaceId.handle);

                worldAnchor = newMesh.MeshObject.AddComponent<WorldAnchor>();
            }
            else
            {
                newMesh = spareMeshObject.Value;
                spareMeshObject = null;

                newMesh.MeshObject.name = meshName;
                newMesh.Id = surfaceId.handle;

                //    Debug.Assert(!newMesh.MeshObject.activeSelf);

                // todo: newMesh.MeshObject.SetActive(false);

                //    Debug.Assert(newMesh.Filter.sharedMesh == null);
                //    Debug.Assert(newMesh.Collider.sharedMesh == null);

                // todo: newMesh.Renderer.enabled = false;

                worldAnchor = newMesh.MeshObject.GetComponent<WorldAnchor>();
            }

            Debug.Assert(worldAnchor != null);

            //var surfaceData = new SurfaceData(
            //    surfaceID,
            //    newMesh.Filter,
            //    worldAnchor,
            //    newMesh.Collider,
            //    TrianglesPerCubicMeter,
            //    _bakeCollider: true
            //    );

            //if (observer.RequestMeshAsync(surfaceData, SurfaceObserver_OnDataReady))
            //{
            //    outstandingMeshRequest = newSurface;
            //}
            //else
            //{
            //    Debug.LogErrorFormat("Mesh request for failed. Is {0} a valid Surface ID?", surfaceID.handle);

            //    Debug.Assert(outstandingMeshRequest == null);
            //    ReclaimSurface(newSurface);
            //}
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
                    meshWorkQueue.Enqueue(id);
                    break;

                case SurfaceChange.Removed:
                    // todo
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

            // todo
        }

        // todo: SetObserverOrigin
        // todo: SetObserverVolume

        #endregion IMixedRealitySpatialAwarenessObserver implementation
    }
}