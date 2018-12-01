// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Services.SpatialAwarenessSystem
{
    /// <summary>
    /// Class providing the default implementation of the <see cref="IMixedRealitySpatialAwarenessSystem"/> interface.
    /// </summary>
    public class MixedRealitySpatialAwarenessSystem : BaseEventSystem, IMixedRealitySpatialAwarenessSystem
    {
        private GameObject spatialAwarenessParent = null;

        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the spatial awareness system created scene objects.
        /// </summary>
        private GameObject SpatialAwarenessParent => spatialAwarenessParent != null ? spatialAwarenessParent : (spatialAwarenessParent = CreateSpatialAwarenessParent);

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
            var secondGeneration = new GameObject(name);

            secondGeneration.transform.parent = SpatialAwarenessParent.transform;

            return secondGeneration;
        }

        #region IMixedRealitySpatialAwarenessSystem Implementation

        /// <inheritdoc />
        public HashSet<IMixedRealitySpatialAwarenessObserver> DetectedSpatialObservers { get; } = new HashSet<IMixedRealitySpatialAwarenessObserver>();

        /// <inheritdoc />
        public bool IsObserverRunning(IMixedRealitySpatialAwarenessObserver observer)
        {
            return false;
        }

        /// <inheritdoc />
        public uint GenerateNewObserverId()
        {
            var newId = (uint)Random.Range(1, int.MaxValue);

            foreach (var observer in DetectedSpatialObservers)
            {
                if (observer.SourceId == newId)
                {
                    return GenerateNewObserverId();
                }
            }

            return newId;
        }

        /// <inheritdoc />
        public void ResumeObserver(IMixedRealitySpatialAwarenessObserver observer)
        {
            foreach (var spatialObserver in DetectedSpatialObservers)
            {
                if (spatialObserver.SourceId == observer.SourceId)
                {
                    spatialObserver.StartObserving();
                    break;
                }
            }
        }

        /// <inheritdoc />
        public void SuspendObserver(IMixedRealitySpatialAwarenessObserver observer)
        {
            foreach (var spatialObserver in DetectedSpatialObservers)
            {
                if (spatialObserver.SourceId == observer.SourceId)
                {
                    spatialObserver.StopObserving();
                    break;
                }
            }
        }

        /// <inheritdoc />
        public void RaiseSpatialAwarenessObserverDetected(IMixedRealitySpatialAwarenessObserver observer)
        {
            DetectedSpatialObservers.Add(observer);
        }

        /// <inheritdoc />
        public void RaiseSpatialAwarenessObserverLost(IMixedRealitySpatialAwarenessObserver observer)
        {
            DetectedSpatialObservers.Remove(observer);
        }

        #region Mesh Handling implementation

        /// <inheritdoc />
        public bool UseMeshSystem { get; set; } = true;

        /// <inheritdoc />
        public int MeshPhysicsLayer { get; set; }

        private SpatialAwarenessMeshLevelOfDetail meshLevelOfDetail = SpatialAwarenessMeshLevelOfDetail.Coarse;

        /// <inheritdoc />
        public SpatialAwarenessMeshLevelOfDetail MeshLevelOfDetail
        {
            get
            {
                return meshLevelOfDetail;
            }

            set
            {
                if (meshLevelOfDetail != value)
                {
                    // Non-custom values automatically modify MeshTrianglesPerCubicMeter
                    if (value != SpatialAwarenessMeshLevelOfDetail.Custom)
                    {
                        MeshTrianglesPerCubicMeter = (int)value;
                    }

                    meshLevelOfDetail = value;
                }
            }
        }

        /// <inheritdoc />
        public int MeshTrianglesPerCubicMeter { get; set; } = (int)SpatialAwarenessMeshLevelOfDetail.Coarse;

        /// <inheritdoc />
        public bool MeshRecalculateNormals { get; set; } = true;

        /// <inheritdoc />
        public SpatialMeshDisplayOptions MeshDisplayOption { get; set; } = SpatialMeshDisplayOptions.None;

        /// <inheritdoc />
        public Material MeshVisibleMaterial { get; set; } = null;

        /// <inheritdoc />
        public Material MeshOcclusionMaterial { get; set; } = null;

        #endregion Mesh Handling implementation

        #region Surface Finding Handling implementation

        /// <inheritdoc />
        public bool UseSurfaceFindingSystem { get; set; } = false;

        /// <inheritdoc />
        public int SurfaceFindingPhysicsLayer { get; set; }

        /// <inheritdoc />
        public float SurfaceFindingMinimumArea { get; set; } = 0.025f;

        /// <inheritdoc />
        public bool DisplayFloorSurfaces { get; set; } = false;

        /// <inheritdoc />
        public Material FloorSurfaceMaterial { get; set; } = null;

        /// <inheritdoc />
        public bool DisplayCeilingSurfaces { get; set; } = false;

        /// <inheritdoc />
        public Material CeilingSurfaceMaterial { get; set; } = null;

        /// <inheritdoc />
        public bool DisplayWallSurfaces { get; set; } = false;

        /// <inheritdoc />
        public Material WallSurfaceMaterial { get; set; } = null;

        /// <inheritdoc />
        public bool DisplayPlatformSurfaces { get; set; } = false;

        /// <inheritdoc />
        public Material PlatformSurfaceMaterial { get; set; } = null;

        /// <inheritdoc />
        public IReadOnlyDictionary<int, GameObject> PlanarSurfaces => new Dictionary<int, GameObject>(0);

        #endregion Surface Finding Handling implementation

        #endregion IMixedRealitySpatialAwarenessSystem Implementation

        #region IMixedRealityToolkit Implementation

        private MixedRealitySpatialAwarenessEventData<SpatialMeshObject> meshEventData = null;
        private MixedRealitySpatialAwarenessEventData<GameObject> surfaceFindingEventData = null;

        /// <inheritdoc/>
        public override void Initialize()
        {
            meshEventData = new MixedRealitySpatialAwarenessEventData<SpatialMeshObject>(EventSystem.current);
            surfaceFindingEventData = new MixedRealitySpatialAwarenessEventData<GameObject>(EventSystem.current);

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
            SurfaceFindingPhysicsLayer = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.SurfaceFindingPhysicsLayer;
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
        public override void Destroy()
        {
            // Cleanup game objects created during execution.
            if (Application.isPlaying)
            {
                // Detach the child objects and clean up the parent.
                if (spatialAwarenessParent != null)
                {
                    spatialAwarenessParent.transform.DetachChildren();
                    Object.Destroy(spatialAwarenessParent);
                    spatialAwarenessParent = null;
                }

                // Detach the mesh objects (they are to be cleaned up by the observer) and cleanup the parent
                if (meshParent != null)
                {
                    meshParent.transform.DetachChildren();
                    Object.Destroy(meshParent);
                    meshParent = null;
                }

                // Detach the surface objects (they are to be cleaned up by the observer) and cleanup the parent
                if (surfaceParent != null)
                {
                    surfaceParent.transform.DetachChildren();
                    Object.Destroy(surfaceParent);
                    surfaceParent = null;
                }
            }
        }

        #region Mesh Events

        /// <inheritdoc />
        public void RaiseMeshAdded(IMixedRealitySpatialAwarenessObserver observer, SpatialMeshObject spatialMeshObject)
        {
            // Parent the mesh object
            spatialMeshObject.GameObject.transform.parent = MeshParent.transform;

            meshEventData.Initialize(observer, spatialMeshObject.Id, spatialMeshObject);
            HandleEvent(meshEventData, OnMeshAdded);
        }

        /// <summary>
        /// Event sent whenever a mesh is added.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler<SpatialMeshObject>> OnMeshAdded =
            delegate (IMixedRealitySpatialAwarenessMeshHandler<SpatialMeshObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialMeshObject>>(eventData);
                handler.OnMeshAdded(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseMeshUpdated(IMixedRealitySpatialAwarenessObserver observer, SpatialMeshObject spatialMeshObject)
        {
            // Parent the mesh object
            spatialMeshObject.GameObject.transform.parent = MeshParent.transform;

            meshEventData.Initialize(observer, spatialMeshObject.Id, spatialMeshObject);
            HandleEvent(meshEventData, OnMeshUpdated);
        }

        /// <summary>
        /// Event sent whenever a mesh is updated.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler<SpatialMeshObject>> OnMeshUpdated =
            delegate (IMixedRealitySpatialAwarenessMeshHandler<SpatialMeshObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialMeshObject>>(eventData);
                handler.OnMeshUpdated(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseMeshRemoved(IMixedRealitySpatialAwarenessObserver observer, SpatialMeshObject spatialMeshObject)
        {
            meshEventData.Initialize(observer, spatialMeshObject.Id, spatialMeshObject);
            HandleEvent(meshEventData, OnMeshRemoved);
        }

        /// <summary>
        /// Event sent whenever a mesh is discarded.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler<SpatialMeshObject>> OnMeshRemoved =
            delegate (IMixedRealitySpatialAwarenessMeshHandler<SpatialMeshObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialMeshObject>>(eventData);
                handler.OnMeshRemoved(spatialEventData);
            };

        #endregion Mesh Events

        #region Surface Finding Events

        /// <inheritdoc />
        public void RaiseSurfaceAdded(IMixedRealitySpatialAwarenessObserver observer, int surfaceId, GameObject surfaceObject)
        {
            if (!UseSurfaceFindingSystem) { return; }

            surfaceFindingEventData.Initialize(observer, surfaceId, surfaceObject);
            HandleEvent(surfaceFindingEventData, OnSurfaceAdded);
        }

        /// <summary>
        /// Event sent whenever a planar surface is added.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessSurfaceFindingHandler<GameObject>> OnSurfaceAdded =
            delegate (IMixedRealitySpatialAwarenessSurfaceFindingHandler<GameObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<GameObject>>(eventData);
                handler.OnSurfaceAdded(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseSurfaceUpdated(IMixedRealitySpatialAwarenessObserver observer, int surfaceId, GameObject surfaceObject)
        {
            if (!UseSurfaceFindingSystem) { return; }

            surfaceFindingEventData.Initialize(observer, surfaceId, surfaceObject);
            HandleEvent(surfaceFindingEventData, OnSurfaceUpdated);
        }

        /// <summary>
        /// Event sent whenever a planar surface is updated.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessSurfaceFindingHandler<GameObject>> OnSurfaceUpdated =
            delegate (IMixedRealitySpatialAwarenessSurfaceFindingHandler<GameObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<GameObject>>(eventData);
                handler.OnSurfaceUpdated(spatialEventData);
            };

        /// <inheritdoc />
        public void RaiseSurfaceRemoved(IMixedRealitySpatialAwarenessObserver observer, int surfaceId)
        {
            if (!UseSurfaceFindingSystem) { return; }

            surfaceFindingEventData.Initialize(observer, surfaceId, null);
            HandleEvent(surfaceFindingEventData, OnSurfaceRemoved);
        }

        /// <summary>
        /// Event sent whenever a planar surface is discarded.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessSurfaceFindingHandler<GameObject>> OnSurfaceRemoved =
            delegate (IMixedRealitySpatialAwarenessSurfaceFindingHandler<GameObject> handler, BaseEventData eventData)
            {
                var spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<GameObject>>(eventData);
                handler.OnSurfaceRemoved(spatialEventData);
            };

        #endregion Surface Finding Events

        #endregion IMixedRealityToolkit Implementation
    }
}
