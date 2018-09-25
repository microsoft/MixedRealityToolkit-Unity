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
        /// Collection of meshes observed.
        /// </summary>
        Dictionary<uint, Mesh> meshes = new Dictionary<uint, Mesh>();

#if UNITY_WSA
        /// <summary>
        /// The surface observer providing the spatial data.
        /// </summary>
        private SurfaceObserver observer = null;
#endif // UNITY_WSA

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
            if (IsRunning &&
                (Time.time - lastUpdated >= MixedRealityManager.Instance.ActiveProfile.SpatialAwarenessProfile.UpdateInterval))
            {
                // The application can update the observation extents at any time.
                ApplyObservationExtents();
                
                // todo

                lastUpdated = Time.time;
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
        private void QueueSpatialMeshDataRequest(SurfaceId id, IMixedRealitySpatialAwarenessMeshDescription meshDescription)
        {
            // todo
            //SurfaceData surfaceData = new SurfaceData(id,
            //                                            surface.GetComponent<MeshFilter>(),
            //                                            surface.GetComponent<WorldAnchor>(),
            //                                            surface.GetComponent<MeshCollider>(),
            //                                            TrianglesPerCubicMeter,
            //                                            true);

            //surfaceWorkQueue.Enqueue(surfaceData);
        }

        /// <summary>
        /// Handles the SurfaceObserver's OnDataReady event.
        /// </summary>
        /// <param name="cookedData">Struct containing output data.</param>
        /// <param name="outputWritten">Set to true if output has been written.</param>
        /// <param name="elapsedCookTimeSeconds">Seconds between mesh cook request and propagation of this event.</param>
        private void SurfaceObserver_OnDataReady(SurfaceData cookedData, bool outputWritten, float elapsedCookTimeSeconds)
        {
            // todo
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
                case SurfaceChange.Added:
                case SurfaceChange.Updated:
                    // Adding and updating are nearly identical.
                    // The only difference is if a new mesh description needs to be created.
                    // NOTE: Added and Updated notification to the spatial awareness system is deferred until baking is complete.
                    // todo
                    break;

                case SurfaceChange.Removed:
                    // If the mesh is tracked, remove it and inform the spatial awareness system immediately.
                    if (meshes.TryGetValue(id.handle, out mesh))
                    {
                        surfaces.Remove(id.handle);
                        // todo: use "cached" spatial system and call the RemoveMesh method
                    }
                    break;
            }
        }
#endif // UNITY_WSA

        #endregion IMixedRealitySpatialAwarenessObserver implementation
    }
}
