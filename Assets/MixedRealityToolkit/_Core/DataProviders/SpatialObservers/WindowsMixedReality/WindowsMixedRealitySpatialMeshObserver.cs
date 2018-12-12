// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.DataProviders.SpatialObservers.WindowsMixedReality.Profiles;

#if UNITY_WSA
using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Extensions;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
#endif // UNITY_WSA

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.SpatialObservers.WindowsMixedReality
{
    /// <summary>
    /// The Windows Mixed Reality Spatial Mesh Observer.
    /// </summary>
    public class WindowsMixedRealitySpatialMeshObserver : BaseMixedRealitySpatialMeshObserver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        public WindowsMixedRealitySpatialMeshObserver(string name, uint priority, WindowsMixedRealitySpatialMeshObserverProfile profile) : base(name, priority, profile)
        {
        }

#if UNITY_WSA

        #region IMixedRealityService implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            // Only initialize if the Spatial Awareness system has been enabled in the configuration profile.
            if (!Application.isPlaying ||
                !MixedRealityToolkit.Instance.ActiveProfile.IsSpatialAwarenessSystemEnabled ||
                MixedRealityToolkit.SpatialAwarenessSystem == null)
            {
                return;
            }

            if (observer == null)
            {
                observer = new SurfaceObserver();
            }

            Debug.Assert(observer != null);

            // Apply the initial observer volume settings.
            ConfigureObserverVolume(ObservationExtents, ObserverOrigin);

            base.Initialize();
        }

        /// <inheritdoc />
        public override void Update()
        {
            // Only update the observer if it is running.
            if (MixedRealityToolkit.SpatialAwarenessSystem == null || !Application.isPlaying || !IsRunning) { return; }

            // If we have a mesh to work on...
            if (!pendingSpatialObject.HasValue || meshWorkQueue.Count > 0)
            {
                // We're using a simple first-in-first-out rule for requesting meshes,
                // but a more sophisticated algorithm could prioritize
                // the queue based on distance to the user or some other metric.
                RequestMesh(meshWorkQueue.Dequeue());
            }
            // If enough time has passed since the previous observer update...
            else if (Time.time - lastUpdated >= UpdateInterval)
            {
                // Update the observer location if it is not stationary
                if (!IsStationaryObserver)
                {
                    ObserverOrigin = CameraCache.Main.transform.position;
                }

                // The application can update the observer volume at any time, make sure we are using the latest.
                ConfigureObserverVolume(ObservationExtents, ObserverOrigin);

                observer.Update(SurfaceObserver_OnSurfaceChanged);
                lastUpdated = Time.time;
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (pendingSpatialObject.HasValue)
            {
                DestroyMeshObject(pendingSpatialObject.Value);
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            if (observer != null)
            {
                observer.Dispose();
                observer = null;
            }
        }

        #endregion IMixedRealityService implementation

        #region IMixedRealitySpatialMeshObserver implementation

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
        private SpatialMeshObject? pendingSpatialObject = null;

        /// <summary>
        /// The time at which the surface observer was last asked for updated data.
        /// </summary>
        private float lastUpdated = 0;

        /// <inheritdoc/>
        public override void StartObserving()
        {
            if (IsRunning)
            {
                return;
            }

            // We want the first update immediately.
            lastUpdated = 0;

            // UpdateObserver keys off of this value to start observing.
            IsRunning = true;
        }

        /// <inheritdoc/>
        public override void StopObserving()
        {
            if (!IsRunning)
            {
                return;
            }

            // UpdateObserver keys off of this value to stop observing.
            IsRunning = false;

            // Clear any pending work.
            meshWorkQueue.Clear();
        }

        /// <summary>
        /// Issue a request to the Surface Observer to begin baking the mesh.
        /// </summary>
        /// <param name="surfaceId">ID of the mesh to bake.</param>
        private void RequestMesh(SurfaceId surfaceId)
        {
            string meshName = $"SpatialMesh_{surfaceId.handle.ToString()}";

            var newSpatialMeshObject = CreateSpatialMeshObject(null, meshName, surfaceId.handle);
            var worldAnchor = newSpatialMeshObject.GameObject.EnsureComponent<WorldAnchor>();

            Debug.Assert(worldAnchor != null);

            var surfaceData = new SurfaceData(
                surfaceId,
                newSpatialMeshObject.Filter,
                worldAnchor,
                newSpatialMeshObject.Collider,
                MeshTrianglesPerCubicMeter,
                true);

            if (observer.RequestMeshAsync(surfaceData, SurfaceObserver_OnDataReady))
            {
                pendingSpatialObject = newSpatialMeshObject;
            }
            else
            {
                Debug.LogError($"Mesh request failed for spatial observer with Id {surfaceId.handle.ToString()}");
                DestroyMeshObject(newSpatialMeshObject);
            }
        }

        /// <summary>
        /// Applies the configured observation extents.
        /// </summary>
        private void ConfigureObserverVolume(Vector3 newExtents, Vector3 newOrigin)
        {
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
        private void SurfaceObserver_OnSurfaceChanged(SurfaceId id, SurfaceChange changeType, Bounds bounds, DateTime updateTime)
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

            if (!pendingSpatialObject.HasValue || pendingSpatialObject.Value.Id != cookedData.id.handle)
            {
                Debug.LogError("Pending spatial mesh object is invalid!");
                return;
            }

            SpatialMeshObject meshObject = pendingSpatialObject.Value;

            if (!outputWritten)
            {
                Debug.LogWarning($"OnDataReady reported no data written for mesh id {cookedData.id.handle.ToString()}");
                DestroyMeshObject(meshObject);
                return;
            }

            // Apply the appropriate material to the mesh.
            SpatialMeshDisplayOptions displayOption = MeshDisplayOption;

            if (displayOption != SpatialMeshDisplayOptions.None)
            {
                meshObject.Renderer.enabled = true;
                meshObject.Renderer.sharedMaterial = (displayOption == SpatialMeshDisplayOptions.Visible)
                    ? MeshVisibleMaterial
                    : MeshOcclusionMaterial;
            }
            else
            {
                meshObject.Renderer.enabled = false;
            }

            // Recalculate the mesh normals if requested.
            if (MeshRecalculateNormals)
            {
                meshObject.Filter.sharedMesh.RecalculateNormals();
            }

            if (SpatialMeshObjects.ContainsKey(cookedData.id.handle))
            {
                RaiseMeshUpdated(meshObject);
                return;
            }

            RaiseMeshAdded(meshObject);
        }

        #endregion IMixedRealitySpatialMeshObserver implementation

#endif // UNITY_WSA
    }
}