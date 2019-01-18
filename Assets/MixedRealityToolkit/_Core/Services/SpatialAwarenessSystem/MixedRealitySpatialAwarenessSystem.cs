// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Observers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Services.SpatialAwarenessSystem
{
    /// <summary>
    /// Class providing the default implementation of the <see cref="IMixedRealitySpatialAwarenessSystem"/> interface.
    /// </summary>
    public class MixedRealitySpatialAwarenessSystem : BaseEventSystem, IMixedRealitySpatialAwarenessSystem
    {
        public AutoStartBehavior DefaultObserverStartupBehavior { get; set; }

        private GameObject spatialAwarenessParent = null;

        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the spatial awareness system created scene objects.
        /// </summary>
        public GameObject SpatialAwarenessParent => spatialAwarenessParent != null ? spatialAwarenessParent : (spatialAwarenessParent = CreateSpatialAwarenessParent);

        /// <summary>
        /// Creates the parent for spatial awareness objects so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see cref="GameObject"/> to which spatial awareness created objects will be parented.
        /// </returns>
        private GameObject CreateSpatialAwarenessParent => new GameObject("Spatial Awareness System");

        private GameObject meshParent = null;

        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the system created mesh objects.
        /// </summary>
        private GameObject MeshParent => meshParent != null ? meshParent : (meshParent = CreateSecondGenerationParent("Meshes"));

        private GameObject surfaceParent = null;

        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the system created mesh objects.
        /// </summary>
        private GameObject SurfaceParent => surfaceParent != null ? surfaceParent : (surfaceParent = CreateSecondGenerationParent("Surfaces"));

        /// <summary>
        /// Creates the a parent, that is a child if the Spatial Awareness System parent so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see cref="GameObject"/> to which spatial awareness objects will be parented.
        /// </returns>
        private GameObject CreateSecondGenerationParent(string name)
        {
            GameObject secondGeneration = new GameObject(name);

            secondGeneration.transform.parent = SpatialAwarenessParent.transform;

            return secondGeneration;
        }

        private IMixedRealitySpatialAwarenessObserver spatialAwarenessObserver = null;

        /// <summary>
        /// The <see cref="IMixedRealitySpatialAwarenessObserver"/>, if any, that is active on the current platform.
        /// </summary>
        private IMixedRealitySpatialAwarenessObserver SpatialAwarenessObserver => spatialAwarenessObserver ?? (spatialAwarenessObserver = MixedRealityToolkit.Instance.GetService<IMixedRealitySpatialAwarenessObserver>());

        #region IMixedRealityToolkit Implementation

        private MixedRealitySpatialAwarenessEventData meshEventData = null;
        private MixedRealitySpatialAwarenessEventData surfaceFindingEventData = null;

        /// <inheritdoc/>
        public override void Initialize()
        {
            base.Initialize();
            InitializeInternal();
        }

        /// <summary>
        /// Performs initialization tasks for the spatial awareness system.
        /// </summary>
        private void InitializeInternal()
        {
            meshEventData = new MixedRealitySpatialAwarenessEventData(EventSystem.current);
            surfaceFindingEventData = new MixedRealitySpatialAwarenessEventData(EventSystem.current);

            // General settings
            StartupBehavior = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.StartupBehavior;
            ObservationExtents = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.ObservationExtents;
            IsStationaryObserver = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.IsStationaryObserver;
            UpdateInterval = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.UpdateInterval;

            // Mesh settings
            UseMeshSystem = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.UseMeshSystem;
            MeshPhysicsLayer = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.MeshPhysicsLayer;
            MeshLevelOfDetail = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.MeshLevelOfDetail;
            MeshTrianglesPerCubicMeter = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.MeshTrianglesPerCubicMeter;
            MeshRecalculateNormals = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.MeshRecalculateNormals;
            MeshDisplayOption = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.MeshDisplayOption;
            MeshVisibleMaterial = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.MeshVisibleMaterial;
            MeshOcclusionMaterial = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.MeshOcclusionMaterial;

            // Surface finding settings
            UseSurfaceFindingSystem = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.UseSurfaceFindingSystem;
            SurfacePhysicsLayer = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.SurfaceFindingPhysicsLayer;
            SurfaceFindingMinimumArea = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.SurfaceFindingMinimumArea;
            DisplayFloorSurfaces = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.DisplayFloorSurfaces;
            FloorSurfaceMaterial = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.FloorSurfaceMaterial;
            DisplayCeilingSurfaces = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.DisplayCeilingSurface;
            CeilingSurfaceMaterial = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.CeilingSurfaceMaterial;
            DisplayWallSurfaces = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.DisplayWallSurface;
            WallSurfaceMaterial = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.WallSurfaceMaterial;
            DisplayPlatformSurfaces = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.DisplayPlatformSurfaces;
            PlatformSurfaceMaterial = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.PlatformSurfaceMaterial;
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            base.Reset();
            // todo: cleanup some objects but not the root scene items
            InitializeInternal();
        }

        /// <inheritdoc/>
        public override void Destroy()
        {
            // Cleanup game objects created during execution.
            if (Application.isPlaying)
            {
                // Detach the child objects and clean up the parent.
                if (spatialAwarenessParent != null)
                {
                    spatialAwarenessParent.transform.DetachChildren();
                    if (Application.isEditor)
                    {
                        Object.DestroyImmediate(spatialAwarenessParent);
                    }
                    else
                    {
                        Object.Destroy(spatialAwarenessParent);
                    }
                    spatialAwarenessParent = null;
                }

                // Detach the mesh objects (they are to be cleaned up by the observer) and cleanup the parent
                if (meshParent != null)
                {
                    meshParent.transform.DetachChildren();
                    if (Application.isEditor)
                    {
                        Object.DestroyImmediate(meshParent);
                    }
                    else
                    {
                        Object.Destroy(meshParent);
                    }
                    meshParent = null;
                }

                // Detach the surface objects (they are to be cleaned up by the observer) and cleanup the parent
                if (surfaceParent != null)
                {
                    surfaceParent.transform.DetachChildren();
                    if (Application.isEditor)
                    {
                        Object.DestroyImmediate(surfaceParent);
                    }
                    else
                    {
                        Object.Destroy(surfaceParent);
                    }
                    surfaceParent = null;
                }
            }
        }

        #region Mesh Events

        /// <inheritdoc />
        public void RaiseMeshAdded(IMixedRealitySpatialAwarenessMeshObserver meshObserver, int meshId, GameObject mesh)
        {
            // Parent the mesh object
            mesh.transform.parent = MeshParent.transform;

            meshEventData.Initialize(meshObserver, meshId, mesh);
            HandleEvent(meshEventData, OnMeshAdded);
        }

        /// <summary>
        /// Event sent whenever a mesh is added.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler> OnMeshAdded =
            delegate (IMixedRealitySpatialAwarenessMeshHandler handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData>(eventData);
                handler.OnMeshAdded(spatialEventData);
            };

        ///<inheritdoc />
        public void RaiseMeshUpdated(IMixedRealitySpatialAwarenessMeshObserver meshObserver, int meshId, GameObject mesh)
        {
            // Parent the mesh object
            mesh.transform.parent = MeshParent.transform;

            meshEventData.Initialize(meshObserver, meshId, mesh);
            HandleEvent(meshEventData, OnMeshUpdated);
        }

        /// <summary>
        /// Event sent whenever a mesh is updated.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler> OnMeshUpdated =
            delegate (IMixedRealitySpatialAwarenessMeshHandler handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData>(eventData);
                handler.OnMeshUpdated(spatialEventData);
            };

        ///<inheritdoc />
        public void RaiseMeshRemoved(IMixedRealitySpatialAwarenessMeshObserver meshObserver, int meshId)
        {
            meshEventData.Initialize(meshObserver, meshId, null);
            HandleEvent(meshEventData, OnMeshRemoved);
        }

        /// <summary>
        /// Event sent whenever a mesh is discarded.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler> OnMeshRemoved =
            delegate (IMixedRealitySpatialAwarenessMeshHandler handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData>(eventData);
                handler.OnMeshRemoved(spatialEventData);
            };

        #endregion Mesh Events

        #region Surface Finding Events

        /// <inheritdoc />
        public void RaiseSurfaceAdded(IMixedRealitySpatialAwarenessObserver observer, int surfaceId, GameObject surfaceObject)
        {
            surfaceFindingEventData.Initialize(observer, surfaceId, surfaceObject);
            HandleEvent(surfaceFindingEventData, OnSurfaceAdded);
        }

        /// <summary>
        /// Event sent whenever a planar surface is added.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessSurfaceFindingHandler> OnSurfaceAdded =
            delegate (IMixedRealitySpatialAwarenessSurfaceFindingHandler handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData>(eventData);
                handler.OnSurfaceAdded(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseSurfaceUpdated(IMixedRealitySpatialAwarenessObserver observer, int surfaceId, GameObject surfaceObject)
        {
            surfaceFindingEventData.Initialize(observer, surfaceId, surfaceObject);
            HandleEvent(surfaceFindingEventData, OnSurfaceUpdated);
        }

        /// <summary>
        /// Event sent whenever a planar surface is updated.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessSurfaceFindingHandler> OnSurfaceUpdated =
            delegate (IMixedRealitySpatialAwarenessSurfaceFindingHandler handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData>(eventData);
                handler.OnSurfaceUpdated(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseSurfaceRemoved(IMixedRealitySpatialAwarenessObserver observer, int surfaceId)
        {
            surfaceFindingEventData.Initialize(observer, surfaceId, null);
            HandleEvent(surfaceFindingEventData, OnSurfaceRemoved);
        }

        /// <summary>
        /// Event sent whenever a planar surface is discarded.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessSurfaceFindingHandler> OnSurfaceRemoved =
            delegate (IMixedRealitySpatialAwarenessSurfaceFindingHandler handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData>(eventData);
                handler.OnSurfaceRemoved(spatialEventData);
            };

        #endregion Surface Finding Events

        #endregion IMixedRealityToolkit Implementation

        #region IMixedRealitySpatialAwarenessSystem Implementation

        private uint nextSourceId = 0;

        /// <inheritdoc />
        public uint GenerateNewSourceId()
        {
            return nextSourceId++;
        }

        /// <inheritdoc />
        public void ResumeObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void ResumeObserver<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
        }

        /// <inheritdoc />
        public void ResumeObserver<T>(int id) where T : IMixedRealitySpatialAwarenessObserver
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void SuspendObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            MixedRealityToolkit.Instance.GetService(T);
            //for each
        }

        /// <inheritdoc />
        public void SuspendObserver<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void SuspendObserver<T>(int id) where T : IMixedRealitySpatialAwarenessObserver
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public T GetObservers<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public T GetObserver<T>() where T : IMixedRealitySpatialAwarenessObserver
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public T GetObserver<T>(int id) where T : IMixedRealitySpatialAwarenessObserver
        {
            throw new System.NotImplementedException();
        }

        #endregion IMixedRealitySpatialAwarenessSystem Implementation
    }
}
