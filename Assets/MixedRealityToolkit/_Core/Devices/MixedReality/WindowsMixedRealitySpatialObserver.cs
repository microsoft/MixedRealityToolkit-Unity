// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_WSA
using UnityEngine.XR.WSA;
#endif // UNITY_WSA

namespace Microsoft.MixedReality.Toolkit.Core.Devices.SpatialAwareness
{
    public class WindowsMixedRealitySpatialObserver : IMixedRealitySpatialAwarenessObserver
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public WindowsMixedRealitySpatialObserver(string name, uint priority)
        {
            Name = name;
            Priority = priority;
        }

        #region IMixedRealityManager implementation

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public uint Priority { get; }

        /// <inheritdoc />
        public void Initialize()
        {
            InitializeInternal();
        }

        /// <summary>
        /// Platform specific initialization
        /// </summary>
        private void InitializeInternal()
        {
            // Only initialize if the Spatial Awareness system has been enabled in the configuration profile.
            if (!MixedRealityManager.Instance.ActiveProfile.IsSpatialAwarenessSystemEnabled) { return; }

#if UNITY_WSA
            CreateObserver();
#endif // UNITY_WSA
        }

        /// <inheritdoc />
        public void Reset()
        {
            // todo: cleanup created objects?
#if UNITY_WSA
            CleanupObserver();
#endif // UNITY_WSA
            InitializeInternal();
        }

        /// <inheritdoc />
        public void Enable()
        {
            // todo
        }

        /// <inheritdoc />
        public void Update()
        {
#if UNITY_WSA
            UpdateObserver();
#endif // UNITY_WSA
        }

        /// <inheritdoc />
        public void Disable()
        {
            // todo
        }

        /// <inheritdoc />
        public void Destroy()
        {
            // Cleanup any implementation specific objects.
            DestroyInternal();

            // Cleanup objects created during execution.
            if (Application.isPlaying)
            {
                // todo
            }
        }

        /// <summary>
        /// Platform specific cleanup.
        /// </summary>
        private void DestroyInternal()
        {
#if UNITY_WSA
            CleanupObserver();
#endif // UNITY_WSA
        }

        #endregion IMixedRealityManager implementation

        #region IMixedRealitySpatialAwarenessObserver implementation

        /// <summary>
        /// A queue of clean mesh GameObjects ready to be reused.
        /// </summary>
        private Queue<GameObject> availableMeshObjects = new Queue<GameObject>();

        /// <summary>
        /// A dictionary of mesh GameObjects which need to be cleaned up and readded for reuse.
        /// Key: ID of the surface currently updating
        /// Value: GameObject encapsulating Visual and Collider components to be cleaned up
        /// </summary>
        private Dictionary<int, GameObject> meshesPendingCleanup = new Dictionary<int, GameObject>();

        /// <summary>
        /// To prevent too many meshes from being generated at the same time, we will
        /// only request one mesh to be created at a time.  This variable will track
        /// if a mesh creation request is in flight.
        /// </summary>
        private bool meshWorkOutstanding = false;

        /// <summary>
        /// Collection of mesh <see cref="GameObject"/>s to be baked.
        /// </summary>
        private Dictionary<int, GameObject> meshesToBake = new Dictionary<int, GameObject>();

        /// <summary>
        /// Identifies the meshes, by id, that are being baked by Unity and if it has been added (true) or is being updated (false). 
        /// </summary>
        private Dictionary<int, bool> meshAddUpdateStatus = new Dictionary<int, bool>();

        /// <summary>
        /// Collection of mesh <see cref="GameObject"/>s that can be used in a scene.
        /// </summary>
        private Dictionary<int, GameObject> meshes = new Dictionary<int, GameObject>();

        public IDictionary<int, GameObject> Meshes
        {
            get
            {
                // We return a copy of the meshes collection to preserve the integrity of our internal dictionary.
                return new Dictionary<int, GameObject>(meshes);
            }
        }

#if UNITY_WSA
        /// <summary>
        /// The surface observer providing the spatial data.
        /// </summary>
        private SurfaceObserver observer = null;

        /// <summary>
        /// A queue of <see cref="SurfaceData"/> objects. These objects are sent to the
        /// SurfaceObserver to generate meshes of the environment.
        /// </summary>
        private Queue<SurfaceData> meshWorkQueue = new Queue<SurfaceData>();
#endif // UNITY_WSA

        /// <summary>
        /// When a mesh is created we will need to create a game object with a minimum 
        /// set of components to contain the mesh.  These are the required component types.
        /// </summary>
        private System.Type[] componentsRequiredForSurfaceMesh =
        {
            typeof(MeshFilter),
            typeof(MeshRenderer),
            typeof(MeshCollider)
        };

        /// <summary>
        /// The observation extents that are currently in use by the surface observer.
        /// </summary>
        private Vector3 currentExtents = Vector3.zero;

        /// <summary>
        /// The time at which the surface observer was last asked for updated data.
        /// </summary>
        private float lastUpdated = 0;

        /// <inheritdoc/>
        public bool IsRunning { get; private set; }

        private Dictionary<uint, IMixedRealitySpatialAwarenessMeshDescription> meshDescriptions = new Dictionary<uint, IMixedRealitySpatialAwarenessMeshDescription>();

        /// <inheritdoc/>
        public void StartObserving()
        {
#if UNITY_WSA
            if (IsRunning)
            {
                Debug.Log("The observer is currently running.");
                return;
            }

            // UpdateObserver keys off of this value to start observing.
            IsRunning = true;
#endif // UNITY_WSA
        }

        /// <inheritdoc/>
        public void StopObserving()
        {
#if UNITY_WSA
            if (!IsRunning)
            {
                Debug.Log("The observer is currently stopped.");
                return;
            }

            // UpdateObserver keys off of this value to stop observing.
            IsRunning = false;
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
                ApplyObservationExtents();

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
        }

        /// <summary>
        /// Requests updates from the surface observer.
        /// </summary>
        private void UpdateObserver()
        {
            // Only update the observer if it is running.
            if (IsRunning)
            {
                // If we don't have mesh creation in flight...
                if (!meshWorkOutstanding)
                {
                    // If we have a mesh to work on...
                    if (meshWorkQueue.Count > 0)
                    {
                        // Pop the SurfaceData off the workl queue.
                        SurfaceData surfaceMeshData = meshWorkQueue.Dequeue();

                        // If RequestMeshAsync succeeds, then we have successfully scheduled mesh creation.
                        meshWorkOutstanding = observer.RequestMeshAsync(surfaceMeshData, SurfaceObserver_OnDataReady);
                    }
                    // If enough time has passed since the previous observer update...
                    else if (Time.time - lastUpdated >= MixedRealityManager.Instance.ActiveProfile.SpatialAwarenessProfile.UpdateInterval)
                    {
                        // The application can update the observation extents at any time.
                        ApplyObservationExtents();

                        observer.Update(SurfaceObserver_OnSurfaceChanged);
                        lastUpdated = Time.time;
                    }
                }
            }
        }

        /// <summary>
        /// Applys the configured observation extents
        /// </summary>
        private void ApplyObservationExtents()
        {
            Vector3 newExtents = MixedRealityManager.Instance.ActiveProfile.SpatialAwarenessProfile.ObservationExtents;

            if (currentExtents.Equals(newExtents)) { return; }
            observer.SetVolumeAsAxisAlignedBox(Vector3.zero, newExtents);
        }

        /// <summary>
        /// Calls GetMeshAsync to update the SurfaceData and re-activate the surface object when ready.
        /// </summary>
        /// <param name="id">Identifier of the SurfaceData object to update.</param>
        /// <param name="surface">The SurfaceData object to update.</param>
        private void QueueMeshDataRequest(SurfaceId id, GameObject mesh)
        {
            SurfaceData surfaceData = new SurfaceData(id,
                                                    mesh.GetComponent<MeshFilter>(),
                                                    mesh.GetComponent<WorldAnchor>(),
                                                    mesh.GetComponent<MeshCollider>(),
                                                    MixedRealityManager.Instance.ActiveProfile.SpatialAwarenessProfile.MeshTrianglesPerCubicMeter,
                                                    true);

            meshWorkQueue.Enqueue(surfaceData);
        }

        /// <summary>
        /// Handles the SurfaceObserver's OnDataReady event.
        /// </summary>
        /// <param name="cookedData">Struct containing output data.</param>
        /// <param name="outputWritten">Set to true if output has been written.</param>
        /// <param name="elapsedCookTimeSeconds">Seconds between mesh cook request and propagation of this event.</param>
        private void SurfaceObserver_OnDataReady(SurfaceData cookedData, bool outputWritten, float elapsedCookTimeSeconds)
        {
            // Cleanup existing mesh with the same id.
            GameObject meshToCleanup = null;
            if (meshesPendingCleanup.TryGetValue(cookedData.id.handle, out meshToCleanup))
            {
                CleanupMeshObject(meshToCleanup);
                meshesPendingCleanup.Remove(cookedData.id.handle);
            }

            GameObject mesh = null;
            if (meshesToBake.TryGetValue(cookedData.id.handle, out mesh))
            {
                // Set the appropriate material on the mesh.
                SpatialMeshDisplayOptions displayOption = MixedRealityManager.Instance.ActiveProfile.SpatialAwarenessProfile.MeshDisplayOption;
                if (displayOption != SpatialMeshDisplayOptions.None)
                {
                    Renderer renderer = mesh.GetComponent<Renderer>();

                    if (renderer != null)
                    {
                        renderer.sharedMaterial = (displayOption == SpatialMeshDisplayOptions.Visible) ?
                            MixedRealityManager.Instance.ActiveProfile.SpatialAwarenessProfile.MeshMaterial :
                            MixedRealityManager.Instance.ActiveProfile.SpatialAwarenessProfile.MeshOcclusionMaterial;
                    }
                }

                // Recalculate the normals, as appropriate.
                if (MixedRealityManager.Instance.ActiveProfile.SpatialAwarenessProfile.MeshRecalculateNormals)
                {
                    MeshFilter filter = mesh.GetComponent<MeshFilter>();
                    filter?.sharedMesh.RecalculateNormals();
                }
            }

            // Inform the spatial awareness system of the mesh.
            bool isNewMesh = false;
            if (meshAddUpdateStatus.TryGetValue(cookedData.id.handle, out isNewMesh))
            {
                if (isNewMesh)
                {
                    MixedRealityManager.SpatialAwarenessSystem.RaiseMeshAdded(cookedData.id.handle, mesh);
                }
                else
                {
                    MixedRealityManager.SpatialAwarenessSystem.RaiseMeshUpdated(cookedData.id.handle, mesh);
                }

                meshAddUpdateStatus.Remove(cookedData.id.handle);
            }

            meshWorkOutstanding = false;
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
            // Verify that the client of the Surface Observer is expecting updates.
            if (!IsRunning) { return; }

            GameObject mesh;

            switch (changeType)
            {
                // Adding and updating are nearly identical.
                // The only difference is if a new mesh description needs to be created.
                case SurfaceChange.Added:
                case SurfaceChange.Updated:
                    // NOTE: Notification to the spatial awareness system is deferred until baking is complete.
                    if (meshesToBake.TryGetValue(id.handle, out mesh))
                    {
                        meshesPendingCleanup.Add(id.handle, mesh);
                        meshesToBake.Remove(id.handle);
                    }

                    // Get an available mesh GameObject ready to be used
                    mesh = GetMeshObject(id.handle);

                    // Add the mesh to our dictionary of known meshes so we can interact with it later.
                    meshesToBake.Add(id.handle, mesh);

                    // Add the mesh ID to the in bake collection so that we can tell the spatial awarenesss
                    // system to send the correct event (added or updated) once baking is complete.
                    meshAddUpdateStatus.Add(id.handle, (changeType == SurfaceChange.Added));

                    // Add the request to create the mesh to our work queue.
                    QueueMeshDataRequest(id, mesh);
                    break;

                case SurfaceChange.Removed:
                    // If the mesh is tracked, remove it and inform the spatial awareness system immediately.
                    if (meshesToBake.TryGetValue(id.handle, out mesh))
                    {
                        MixedRealityManager.SpatialAwarenessSystem.RaiseMeshRemoved(id.handle);
                        meshesToBake.Remove(id.handle);
                    }
                    break;
            }
        }
#endif // UNITY_WSA

        /// <summary>
        /// Gets a configured spatial mesh object.
        /// </summary>
        /// <param name="meshId">The id of the mesh.</param>
        /// <returns>A <see cref="GameObject"/> representing the mesh.</returns>
        private GameObject GetMeshObject(int meshId)
        {
            GameObject mesh = null;
            string meshObjectName = $"SpatialMesh-{meshId}";

            // If we have surfaces ready for reuse, use those first.
            if (availableMeshObjects.Count > 0)
            {
                mesh = availableMeshObjects.Dequeue();
            }
            else
            {
                // We are adding a new mesh, construct a GameObject to represent it.
                mesh = new GameObject(meshObjectName, componentsRequiredForSurfaceMesh);
            }

            return mesh;
        }

        /// <summary>
        /// Cleans up a <see cref="GameObject"/> representing a spatial mesh.
        /// </summary>
        /// <param name="mesh">The mesh object</param>
        private void CleanupMeshObject(GameObject mesh)
        {
            if (mesh == null)
            {
                Debug.LogWarning("Cannot cleanup a null mesh object.");
                return;
            }

            // Destroy the meshes, and add the surface back for reuse
            CleanupMeshes(mesh.GetComponent<MeshFilter>()?.sharedMesh, mesh.GetComponent<MeshCollider>()?.sharedMesh);
            availableMeshObjects.Enqueue(mesh);
            mesh.name = "SpatialMesh-Unused";
            mesh.SetActive(false);
        }

        /// <summary>
        /// Destroys the specified mesh objects.
        /// </summary>
        /// <param name="visualMesh">The visible mesh.</param>
        /// <param name="colliderMesh">The mesh used for collision detection.</param>
        private void CleanupMeshes(Mesh visualMesh, Mesh colliderMesh)
        {
            if (colliderMesh != null && colliderMesh != visualMesh)
            {
                Object.Destroy(colliderMesh);
            }

            if (visualMesh != null)
            {
                Object.Destroy(visualMesh);
            }
        }

        #endregion IMixedRealitySpatialAwarenessObserver implementation
    }
}
