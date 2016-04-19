// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Spatial Mapping Observer states.
    /// </summary>
    public enum ObserverStates
    {
        /// <summary>
        /// The SurfaceObserver is currently running.
        /// </summary>
        Running = 0,

        /// <summary>
        /// The SurfaceObserver is currently idle.
        /// </summary>
        Stopped = 1
    }

    /// <summary>
    /// The SpatialMappingObserver class encapsulates the SurfaceObserver into an easy to use
    /// object that handles managing the observed surfaces and the rendering of surface geometry.
    /// </summary>
    public class SpatialMappingObserver : SpatialMappingSource
    {
        [Tooltip("The number of triangles to calculate per cubic meter.")]
        public float TrianglesPerCubicMeter = 500f;
        
        [Tooltip("The extents of the observation volume.")]
        public Vector3 Extents = Vector3.one * 10.0f;

        [Tooltip("How long to wait (in sec) between Spatial Mapping updates.")]
        public float TimeBetweenUpdates = 3.5f;

        /// <summary>
        /// Our Surface Observer object for generating/updating Spatial Mapping data.
        /// </summary>
        private SurfaceObserver observer;

        /// <summary>
        /// A dictionary of surfaces that our Surface Observer knows about.
        /// Key: surface id
        /// Value: GameObject containing a Mesh, a MeshRenderer and a Material
        /// </summary>
        private Dictionary<int, GameObject> surfaces = new Dictionary<int, GameObject>();

        /// <summary>
        /// A queue of SurfaceData objects. SurfaceData objects are sent to the
        /// SurfaceObserver to generate meshes of the environment.
        /// </summary>
        private Queue<SurfaceData> surfaceWorkQueue = new Queue<SurfaceData>();

        /// <summary>
        /// To prevent too many meshes from being generated at the same time, we will
        /// only request one mesh to be created at a time.  This variable will track
        /// if a mesh creation request is in flight.
        /// </summary>
        private bool surfaceWorkOutstanding = false;

        /// <summary>
        /// Used to track when the Observer was last updated.
        /// </summary>
        private float updateTime;

        /// <summary>
        /// Indicates the current state of the Surface Observer.
        /// </summary>
        public ObserverStates ObserverState { get; private set; }

        private void Awake()
        {
            observer = new SurfaceObserver();
            ObserverState = ObserverStates.Stopped;
        }

        /// <summary>
        /// Called when the GaemObject is initialized.
        /// </summary>
        private void Start()
        {
            observer.SetVolumeAsAxisAlignedBox(Vector3.zero, Extents);            
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        private void Update()
        {
            // Only do processing if the observer is running.
            if (ObserverState == ObserverStates.Running)
            {
                // If we don't have mesh creation in flight, but we could schedule mesh creation, do so.
                if (surfaceWorkOutstanding == false && surfaceWorkQueue.Count > 0)
                {
                    // Pop the SurfaceData off the queue.  A more sophisticated algorithm could prioritize
                    // the queue based on distance to the user or some other metric.
                    SurfaceData surfaceData = surfaceWorkQueue.Dequeue();

                    // If RequestMeshAsync succeeds, then we have successfully scheduled mesh creation.
                    surfaceWorkOutstanding = observer.RequestMeshAsync(surfaceData, SurfaceObserver_OnDataReady);
                }
                // If we don't have any other work to do, and enough time has passed since the previous
                // update request, request updates for the spatial mapping data.
                else if (surfaceWorkOutstanding == false && (Time.time - updateTime) >= TimeBetweenUpdates)
                {
                    observer.Update(SurfaceObserver_OnSurfaceChanged);
                    updateTime = Time.time;
                }
            }
        }

        /// <summary>
        /// Starts the Surface Observer.
        /// </summary>
        public void StartObserving()
        {
            if (ObserverState != ObserverStates.Running)
            {
                Debug.Log("Starting the observer.");
                ObserverState = ObserverStates.Running;

                // We want the first update immediately.
                updateTime = 0;
            }
        }

        /// <summary>
        /// Stops the Surface Observer.
        /// </summary>
        /// <remarks>Sets the Surface Observer state to ObserverStates.Stopped.</remarks>
        public void StopObserving()
        {
            if (ObserverState == ObserverStates.Running)
            {
                Debug.Log("Stopping the observer.");
                ObserverState = ObserverStates.Stopped;
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
            GameObject surface;
            if (surfaces.TryGetValue(cookedData.id.handle, out surface))
            {
                // Set the draw material for the renderer.
                MeshRenderer renderer = surface.GetComponent<MeshRenderer>();
                renderer.sharedMaterial = SpatialMappingManager.Instance.SurfaceMaterial;
                renderer.enabled = SpatialMappingManager.Instance.DrawVisualMeshes;

                if(SpatialMappingManager.Instance.CastShadows == false)
                {
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }

            surfaceWorkOutstanding = false;
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
            if (ObserverState != ObserverStates.Running)
            {
                return;
            }

            GameObject surface;

            switch (changeType)
            {
                // Adding and updating are nearly identical.  The only difference is if a new gameobject to contain 
                // the surface needs to be created.
                case SurfaceChange.Added:
                case SurfaceChange.Updated:
                    // Check to see if the surface is known to the observer.
                    if (!surfaces.TryGetValue(id.handle, out surface))
                    {
                        // If we are adding a new surface, construct a GameObject
                        // to represent its state and attach some Mesh-related
                        // components to it.
                        surface = AddSurfaceObject(null, string.Format("Surface-{0}", id.handle), transform);

                        surface.AddComponent<WorldAnchor>();

                        // Add the surface to our dictionary of known surfaces so
                        // we can interact with it later.
                        surfaces.Add(id.handle, surface);
                    }

                    // Add the request to create the mesh for this surface to our work queue.
                    QueueSurfaceDataRequest(id, surface);
                    break;

                case SurfaceChange.Removed:
                    // Always process surface removal events.
                    if (surfaces.TryGetValue(id.handle, out surface))
                    {
                        surfaces.Remove(id.handle);
                        Destroy(surface);
                    }
                    break;
            }
        }

        /// <summary>
        /// Calls GetMeshAsync to update the SurfaceData and re-activate the surface object when ready.
        /// </summary>
        /// <param name="id">Identifier of the SurfaceData object to update.</param>
        /// <param name="surface">The SurfaceData object to update.</param>
        private void QueueSurfaceDataRequest(SurfaceId id, GameObject surface)
        {
            SurfaceData surfaceData = new SurfaceData(id,
                                                        surface.GetComponent<MeshFilter>(),
                                                        surface.GetComponent<WorldAnchor>(),
                                                        surface.GetComponent<MeshCollider>(),
                                                        TrianglesPerCubicMeter,
                                                        true);

            surfaceWorkQueue.Enqueue(surfaceData);
        }

        /// <summary>
        /// Called when the GameObject is unloaded.
        /// </summary>
        private void OnDestroy()
        {
            // Stop the observer.
            StopObserving();

            observer.Dispose(); 
            observer = null;

            // Clear our surface mesh collection.
            surfaces.Clear();
        }
    }
}