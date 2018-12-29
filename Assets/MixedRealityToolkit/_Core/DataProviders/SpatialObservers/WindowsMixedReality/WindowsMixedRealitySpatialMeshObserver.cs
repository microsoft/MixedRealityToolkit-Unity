// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.DataProviders.SpatialObservers.WindowsMixedReality.Profiles;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System;

#if UNITY_WSA
using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Extensions;
using Microsoft.MixedReality.Toolkit.Core.Utilities;
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
            if (MixedRealityToolkit.SpatialAwarenessSystem == null)
            {
                throw new Exception("Missing a registered spatial awareness system!");
            }

#if UNITY_WSA
            observer = new SurfaceObserver();

            // Apply the initial observer volume settings.
            ConfigureObserverVolume(ObserverOrigin, ObservationExtents);
#endif // UNITY_WSA
        }

#if UNITY_WSA

        #region IMixedRealityService implementation

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            // Only update the observer if it is running.
            if (!Application.isPlaying || !IsRunning) { return; }

            // If enough time has passed since the previous observer update...
            if (Time.time - lastUpdated >= UpdateInterval)
            {
                // Update the observer location if it is not stationary
                if (!IsStationaryObserver)
                {
                    ObserverOrigin = CameraCache.Main.transform.position;
                }

                // The application can update the observer volume at any time, make sure we are using the latest.
                ConfigureObserverVolume(ObserverOrigin, ObservationExtents);

                observer.Update(SurfaceObserver_OnSurfaceChanged);
                lastUpdated = Time.time;
            }
        }

        /// <inheritdoc />
        protected override void OnDispose(bool finalizing)
        {
            observer.Dispose();

            base.OnDispose(finalizing);
        }

        #endregion IMixedRealityService implementation

        #region IMixedRealitySpatialMeshObserver implementation

        /// <summary>
        /// The surface observer providing the spatial data.
        /// </summary>
        private static SurfaceObserver observer = null;

        /// <summary>
        /// The current location of the surface observer.
        /// </summary>
        private Vector3 currentObserverOrigin = Vector3.zero;

        /// <summary> 
        /// The observation extents that are currently in use by the surface observer. 
        /// </summary> 
        private Vector3 currentObserverExtents = Vector3.zero;

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

            base.StartObserving();
        }

        /// <summary>
        /// Applies the configured observation extents.
        /// </summary>
        private void ConfigureObserverVolume(Vector3 newOrigin, Vector3 newExtents)
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
        /// <param name="surfaceId">The identifier assigned to the surface which has changed.</param>
        /// <param name="changeType">The type of change that occurred on the surface.</param>
        /// <param name="bounds">The bounds of the surface.</param>
        /// <param name="updateTime">The date and time at which the change occurred.</param>
        private async void SurfaceObserver_OnSurfaceChanged(SurfaceId surfaceId, SurfaceChange changeType, Bounds bounds, DateTime updateTime)
        {
            // If we're adding or updating a mesh
            if (changeType != SurfaceChange.Removed)
            {
                var spatialMeshObject = await RequestSpatialMeshObject(surfaceId.handle);
                spatialMeshObject.GameObject.name = $"SpatialMesh_{surfaceId.handle.ToString()}";
                var worldAnchor = spatialMeshObject.GameObject.EnsureComponent<WorldAnchor>();
                var surfaceData = new SurfaceData(surfaceId, spatialMeshObject.Filter, worldAnchor, spatialMeshObject.Collider, MeshTrianglesPerCubicMeter, true);

                if (!observer.RequestMeshAsync(surfaceData, OnDataReady))
                {
                    Debug.LogError($"Mesh request failed for spatial observer with Id {surfaceId.handle.ToString()}");
                    RaiseMeshRemoved(spatialMeshObject);
                }

                void OnDataReady(SurfaceData cookedData, bool outputWritten, float elapsedCookTimeSeconds)
                {
                    if (!outputWritten)
                    {
                        Debug.LogWarning($"No output for {cookedData.id.handle}");
                        return;
                    }

                    if (!SpatialMeshObjects.TryGetValue(cookedData.id.handle, out SpatialMeshObject meshObject))
                    {
                        // Likely it was removed before data could be cooked.
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

                    meshObject.GameObject.SetActive(true);

                    switch (changeType)
                    {
                        case SurfaceChange.Added:
                            RaiseMeshAdded(meshObject);
                            break;
                        case SurfaceChange.Updated:
                            RaiseMeshUpdated(meshObject);
                            break;
                    }
                }
            }
            else if (SpatialMeshObjects.TryGetValue(surfaceId.handle, out SpatialMeshObject meshObject))
            {
                RaiseMeshRemoved(meshObject);
            }
        }

        #endregion IMixedRealitySpatialMeshObserver implementation

#endif // UNITY_WSA
    }
}