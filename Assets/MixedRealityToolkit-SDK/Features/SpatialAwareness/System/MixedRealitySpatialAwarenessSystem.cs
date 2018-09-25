// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem
{
    /// <summary>
    /// Class poviding the default implementation of the <see cref="IMixedRealitySpatialAwarenessSystem"/> interface.
    /// </summary>
    public class MixedRealitySpatialAwarenessSystem : MixedRealityEventManager, IMixedRealitySpatialAwarenessSystem
    {
        private GameObject spatialAwarenessParent = null;

        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the spatial awareness system created scene objects.
        /// </summary>
        private GameObject SpatialAwarenessParent => spatialAwarenessParent ?? (spatialAwarenessParent = CreateSpatialAwarenessParent());

        /// <summary>
        /// Creates the parent for spatial awareness objects so that the scene heirarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see cref="GameObject"/> to which spatial awareness created objects will be parented.
        /// </returns>
        private GameObject CreateSpatialAwarenessParent()
        {
            return new GameObject("Spatial Awareness System");
        }

        private GameObject meshParent = null;

        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the system created mesh objects.
        /// </summary>
        private GameObject MeshParent => meshParent ?? (meshParent = CreateSecondGenerationParent("Meshes"));

        private GameObject surfaceParent = null;

        /// <summary>
        /// Parent <see cref="GameObject"/> which will encapsulate all of the system created mesh objects.
        /// </summary>
        private GameObject SurfaceParent => surfaceParent ?? (surfaceParent = CreateSecondGenerationParent("Surfaces"));

        /// <summary>
        /// Creates the a parent, that is a child if the Spatial Awareness System parent so that the scene heirarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see cref="GameObject"/> to whichspatial awareness objects will be parented.
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
        private IMixedRealitySpatialAwarenessObserver SpatialAwarenessObserver => spatialAwarenessObserver ?? (spatialAwarenessObserver = MixedRealityManager.Instance.GetManager<IMixedRealitySpatialAwarenessObserver>());

        #region IMixedRealityManager Implementation

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

            // todo - get the appropriate IMixedRealitySpatialAwarenessObserver and ask if it is running
        }

        /// <inheritdoc/>
        public override void Reset()
        {
            base.Reset();
            InitializeInternal();
        }

        public override void Destroy()
        {
            // Cleanup game objects created during execution.
            if (Application.isPlaying)
            {
                // Detach the child objects (we are tracking them separately) and clean up the parent.
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

                // Detatch the mesh objects (we are tracking them separately) and cleanup their parent
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

                // Detatch the surface objects (we are tracking them separately) and cleanup their parent
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

                // Cleanup mesh object
                // todo

                // Cleanup surface objects
                // todo
            }
        }

        #region Mesh Events

        /// <summary>
        /// Method called by the surface observer to inform the spatial awareness system of a new mesh.
        /// </summary>
        /// <param name="meshId">The id of the mesh</param>
        /// <param name="meshObject">The mesh <see cref="GameObject"/></param>
        public void AddMesh(int meshId, GameObject meshObject)
        {
            // todo: add the mesh to the collection

            RaiseMeshAdded(meshId, meshObject);
        }

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler.OnMeshAdded"/> method to indicate a new mesh has been added.
        /// </summary>
        /// <param name="meshId">Value identifying the mesh.</param>
        /// <param name="meshObject">The mesh <see cref="GameObject"/>.</param>
        private void RaiseMeshAdded(int meshId, GameObject meshObject)
        {
            // todo
        }

        /// <summary>
        /// Event sent whenever a mesh is added.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler> OnMeshAdded =
            delegate (IMixedRealitySpatialAwarenessMeshHandler handler, BaseEventData eventData)
            {
                // todo
                //BoundaryEventData boundaryEventData = ExecuteEvents.ValidateEventData<BoundaryEventData>(eventData);
                //handler.OnBoundaryVisualizationChanged(boundaryEventData);
            };

        /// <summary>
        /// Method called by the surface observer to inform the spatial awareness system that an existing mesh has been updated.
        /// </summary>
        /// <param name="meshId">The id of the mesh</param>
        /// <param name="meshObject">The mesh <see cref="GameObject"/></param>
        public void UpdateMesh(uint meshId, GameObject meshObject)
        {
            // todo: update the mesh in the collection

            RaiseMeshUpdated(meshId, meshObject);
        }

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler.OnMeshUpdated"/> method to indicate an exising mesh has changed.
        /// </summary>
        /// <param name="meshId">Value identifying the mesh.</param>
        /// <param name="meshObject">The mesh <see cref="GameObject"/>.</param>
        private void RaiseMeshUpdated(uint meshId, GameObject meshObject)
        {
            // todo
        }

        /// <summary>
        /// Event sent whenever a mesh is updated.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler> OnMeshUpdated =
            delegate (IMixedRealitySpatialAwarenessMeshHandler handler, BaseEventData eventData)
            {
                // todo
                //BoundaryEventData boundaryEventData = ExecuteEvents.ValidateEventData<BoundaryEventData>(eventData);
                //handler.OnBoundaryVisualizationChanged(boundaryEventData);
            };

        /// <summary>
        /// Method called by the surface observer to inform the spatial awareness system that an existing mesh has been removed.
        /// </summary>
        /// <param name="meshId">The id of the mesh</param>
        public void RemoveMesh(int meshId)
        {
            // todo: update the mesh in the collection

            RaiseMeshRemoved(meshId);
        }

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler.OnMeshUpdated"/> method to indicate an exising mesh has been removed.
        /// </summary>
        /// <param name="meshId">Value identifying the mesh.</param>
        private void RaiseMeshRemoved(int meshId)
        {
            // todo
        }

        /// <summary>
        /// Event sent whenever a mesh is discarded.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessMeshHandler> OnMeshRemoved =
            delegate (IMixedRealitySpatialAwarenessMeshHandler handler, BaseEventData eventData)
            {
                // todo
                //BoundaryEventData boundaryEventData = ExecuteEvents.ValidateEventData<BoundaryEventData>(eventData);
                //handler.OnBoundaryVisualizationChanged(boundaryEventData);
            };

        #endregion Mesh Events

        #region Surface Finding Events

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler.OnSurfaceAdded"/> method to indicate a new planar surface has been added.
        /// </summary>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <param name="position">The position, in the environment, at which the surface should be placed.</param>
        /// <param name="boundingBox">Axis aligned bounding box containing the surface.</param>
        /// <param name="normal">The normal of the surface.</param>
        /// <param name="surfaceType">The semantic (ex: Floor) associated with the surface.</param>
        /// <param name="surfaceObject">The surface finding subsystem managed <see cref="GameObject"/> for the surface.</param>
        private void RaiseSurfaceAdded(
            uint surfaceId,
            Vector3 position,
            Bounds boundingBox,
            Vector3 normal,
            SpatialAwarenessSurfaceTypes surfaceType,
            GameObject surfaceObject = null)
        {
            // todo
        }

        /// <summary>
        /// Event sent whenever a planar surface is added.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessSurfaceFindingHandler> OnSurfaceAdded =
            delegate (IMixedRealitySpatialAwarenessSurfaceFindingHandler handler, BaseEventData eventData)
            {
                // todo
                //BoundaryEventData boundaryEventData = ExecuteEvents.ValidateEventData<BoundaryEventData>(eventData);
                //handler.OnBoundaryVisualizationChanged(boundaryEventData);
            };

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler.OnSurfaceUpdated"/> method to indicate an existing planar surface has changed.
        /// </summary>
        /// <param name="surfaceId">Value identifying the surface.</param>
        /// <param name="position">The position, in the environment, at which the surface should be placed.</param>
        /// <param name="boundingBox">Axis aligned bounding box containing the surface.</param>
        /// <param name="normal">The normal of the surface.</param>
        /// <param name="surfaceType">The semantic (ex: Floor) associated with the surface.</param>
        /// <param name="surfaceObject">The surface finding subsystem managed <see cref="GameObject"/> for the surface.</param>
        private void RaiseSurfaceUpdated(
            uint surfaceId,
            Vector3 position,
            Bounds boundingBox,
            Vector3 normal,
            SpatialAwarenessSurfaceTypes surfaceType,
            GameObject surfaceObject = null)
        {
            // todo
        }

        /// <summary>
        /// Event sent whenever a planar surface is updated.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessSurfaceFindingHandler> OnSurfaceUpdated =
            delegate (IMixedRealitySpatialAwarenessSurfaceFindingHandler handler, BaseEventData eventData)
            {
                // todo
                //BoundaryEventData boundaryEventData = ExecuteEvents.ValidateEventData<BoundaryEventData>(eventData);
                //handler.OnBoundaryVisualizationChanged(boundaryEventData);
            };

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler.OnSurfaceRemoved"/> method to indicate an existing planar surface has been removed.
        /// </summary>
        /// <param name="surfaceId">Value identifying the surface.</param>
        private void RaiseSurfaceDeleted(uint surfaceId)
        {
            // todo
        }

        /// <summary>
        /// Event sent whenever a planar surface is discarded.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessSurfaceFindingHandler> OnSurfaceDeleted =
            delegate (IMixedRealitySpatialAwarenessSurfaceFindingHandler handler, BaseEventData eventData)
            {
                // todo
                //BoundaryEventData boundaryEventData = ExecuteEvents.ValidateEventData<BoundaryEventData>(eventData);
                //handler.OnBoundaryVisualizationChanged(boundaryEventData);
            };
        
        #endregion Surface Finding Events

        #endregion IMixedRealityManager Implementation

        #region IMixedRealtyEventSystem Implementation

        /// <inheritdoc />
        public override void HandleEvent<T>(BaseEventData eventData, ExecuteEvents.EventFunction<T> eventHandler)
        {
            base.HandleEvent(eventData, eventHandler);
        }

        /// <summary>
        /// Registers the <see cref="GameObject"/> to listen for boundary events.
        /// </summary>
        /// <param name="listener"></param>
        public override void Register(GameObject listener)
        {
            base.Register(listener);
        }

        /// <summary>
        /// UnRegisters the <see cref="GameObject"/> to listen for boundary events.
        /// /// </summary>
        /// <param name="listener"></param>
        public override void Unregister(GameObject listener)
        {
            base.Unregister(listener);
        }

        #endregion

        #region IMixedRealityEventSource Implementation

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object x, object y)
        {
            // There shouldn't be other Boundary Managers to compare to.
            return false;
        }

        /// <inheritdoc />
        public int GetHashCode(object obj)
        {
            return Mathf.Abs(SourceName.GetHashCode());
        }

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName { get; } = "Mixed Reality Spatial Awareness System";

        #endregion IMixedRealityEventSource Implementation

        #region IMixedRealitySpatialAwarenessSystem Implementation

        /// <inheritdoc />
        public AutoStartBehavior StartupBehavior { get; set; } = AutoStartBehavior.AutoStart;

        /// <inheritdoc />
        public Vector3 ObservationExtents { get; set; } = Vector3.one * 10;

        private float updateInterval = 3.5f;

        /// <inheritdoc />
        public float UpdateInterval
        {
            get
            {
                return updateInterval;
            }

            set
            {
                if (IsObserverRunning)
                {
                    Debug.LogError("UpdateInterval cannot be modified while the observer is running.");
                    return;
                }

                updateInterval = value;
            }
        }

        /// <inheritdoc />
        public bool IsObserverRunning
        {
            get
            {
                if (SpatialAwarenessObserver == null) { return false; }
                return SpatialAwarenessObserver.IsRunning;
            }
        }

        /// <inheritdoc />
        public void ResumeObserver()
        {
            if (SpatialAwarenessObserver == null) { return; }
            SpatialAwarenessObserver.StartObserving();
        }

        /// <inheritdoc />
        public void SuspendObserver()
        {
            if (SpatialAwarenessObserver == null) { return; }
            SpatialAwarenessObserver.StopObserving();
        }

        #region Mesh Handling implementation

        /// <inheritdoc />
        public bool UseMeshSystem { get; set; } = true;

        /// <inheritdoc />
        public int MeshPhysicsLayer { get; set; } = 31;

        /// <inheritdoc />
        public int MeshPhysicsLayerMask => 1 << MeshPhysicsLayer;

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
                if (IsObserverRunning)
                {
                    Debug.LogError("MeshLevelOfDetail cannot be modified while the observer is running.");
                    return;
                }

                if (meshLevelOfDetail != value)
                {
                    // Non-custom values automatically modify MeshTrianglesPerCubicMeter
                    if (value != SpatialAwarenessMeshLevelOfDetail.Custom)
                    {
                        meshTrianglesPerCubicMeter = (int)value;
                    }

                    meshLevelOfDetail = value;
                }
            }
        }

        private int meshTrianglesPerCubicMeter = (int)SpatialAwarenessMeshLevelOfDetail.Coarse;

        /// <inheritdoc />
        public int MeshTrianglesPerCubicMeter
        {
            get
            {
                return meshTrianglesPerCubicMeter;
            }

            set
            {
                if (IsObserverRunning)
                {
                    Debug.LogError("MeshTrianglesPerCubicMeter cannot be modified while the observer is running.");
                    return;
                }

                meshTrianglesPerCubicMeter = value;
            }
        }

        /// <inheritdoc />
        public bool MeshRecalculateNormals { get; set; } = true;

        /// <inheritdoc />
        public SpatialMeshDisplayOptions MeshDisplayOption { get; set; } = SpatialMeshDisplayOptions.None;

        /// <inheritdoc />
        public Material MeshVisibleMaterial { get; set; } = null;

        /// <inheritdoc />
        public Material MeshOcclusionMaterial { get; set; } = null;

        /// <inheritdoc />
        public Dictionary<int, GameObject> MeshObjects
        {
            get
            {
                // This implementation of the spatial awareness system manages game objects.
                // todo
                return new Dictionary<int, GameObject>(0);
            }
        }

        #endregion Mesh Handling implementation

        #region Surface Finding Handling implementation

        /// <inheritdoc />
        public bool UseSurfaceFindingSystem { get; set; } = false;

        /// <inheritdoc />
        public int SurfacePhysicsLayer { get; set; } = 31;

        /// <inheritdoc />
        public int SurfacePhysicsLayerMask => 1 << SurfacePhysicsLayer;

        /// <inheritdoc />
        public float SurfaceFindingMinimumArea { get; set; } = 0.025f;

        /// <inheritdoc />
        public bool RenderFloorSurfaces { get; set; } = false;

        /// <inheritdoc />
        public Material FloorSurfaceMaterial { get; set; } = null;

        /// <inheritdoc />
        public bool RenderCeilingSurfaces { get; set; } = false;

        /// <inheritdoc />
        public Material CeilingSurfaceMaterial { get; set; } = null;

        /// <inheritdoc />
        public bool RenderWallSurfaces { get; set; } = false;

        /// <inheritdoc />
        public Material WallSurfaceMaterial { get; set; } = null;

        /// <inheritdoc />
        public bool RenderPlatformSurfaces { get; set; } = false;

        /// <inheritdoc />
        public Material PlatformSurfaceMaterial { get; set; } = null;

        /// <inheritdoc />
        public Dictionary<int, GameObject> SurfaceObjects
        {
            get
            {
                // This implementation of the spatial awareness system manages game objects.
                // todo
                return new Dictionary<int, GameObject>(0);
            }
        }

        #endregion Surface Finding Handling implementation

        #endregion IMixedRealitySpatialAwarenessSystem Implementation
    }
}
