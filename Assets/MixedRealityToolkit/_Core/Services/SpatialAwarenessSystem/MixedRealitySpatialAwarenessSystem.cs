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
        public GameObject SpatialAwarenessRootParent => spatialAwarenessParent != null ? spatialAwarenessParent : (spatialAwarenessParent = CreateSpatialAwarenessParent);

        /// <summary>
        /// Creates the parent for spatial awareness objects so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see cref="GameObject"/> to which spatial awareness created objects will be parented.
        /// </returns>
        private GameObject CreateSpatialAwarenessParent => new GameObject("Spatial Awareness System");

        private GameObject meshParent = null;

        /// <inheritdoc />
        public GameObject SpatialMeshesParent => meshParent != null ? meshParent : (meshParent = CreateSecondGenerationParent("Meshes"));

        private GameObject surfaceParent = null;

        /// <inheritdoc />
        public GameObject SurfacesParent => surfaceParent != null ? surfaceParent : (surfaceParent = CreateSecondGenerationParent("Surfaces"));

        /// <summary>
        /// Creates a parent that is a child of the Spatial Awareness System parent so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see cref="GameObject"/> to which spatial awareness objects will be parented.
        /// </returns>
        private GameObject CreateSecondGenerationParent(string name)
        {
            var secondGeneration = new GameObject(name);

            secondGeneration.transform.parent = SpatialAwarenessRootParent.transform;

            return secondGeneration;
        }

        #region IMixedRealitySpatialAwarenessSystem Implementation

        /// <inheritdoc />
        public HashSet<IMixedRealitySpatialObserverDataProvider> DetectedSpatialObservers { get; } = new HashSet<IMixedRealitySpatialObserverDataProvider>();

        /// <inheritdoc />
        public bool IsObserverRunning(IMixedRealitySpatialObserverDataProvider observer)
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
        public void ResumeObserver(IMixedRealitySpatialObserverDataProvider observer)
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
        public void SuspendObserver(IMixedRealitySpatialObserverDataProvider observer)
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
        public void RaiseSpatialAwarenessObserverDetected(IMixedRealitySpatialObserverDataProvider observer)
        {
            DetectedSpatialObservers.Add(observer);
        }

        /// <inheritdoc />
        public void RaiseSpatialAwarenessObserverLost(IMixedRealitySpatialObserverDataProvider observer)
        {
            DetectedSpatialObservers.Remove(observer);
        }

        #endregion IMixedRealitySpatialAwarenessSystem Implementation

        #region IMixedRealityToolkit Implementation

        private MixedRealitySpatialAwarenessEventData<SpatialMeshObject> meshEventData = null;
        private MixedRealitySpatialAwarenessEventData<GameObject> surfaceFindingEventData = null;

        /// <inheritdoc/>
        public override void Initialize()
        {
            meshEventData = new MixedRealitySpatialAwarenessEventData<SpatialMeshObject>(EventSystem.current);
            surfaceFindingEventData = new MixedRealitySpatialAwarenessEventData<GameObject>(EventSystem.current);
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
        public void RaiseMeshAdded(IMixedRealitySpatialMeshObserver observer, SpatialMeshObject spatialMeshObject)
        {
            // Parent the mesh object
            spatialMeshObject.GameObject.transform.parent = SpatialMeshesParent.transform;

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
        public void RaiseMeshUpdated(IMixedRealitySpatialMeshObserver observer, SpatialMeshObject spatialMeshObject)
        {
            // Parent the mesh object
            spatialMeshObject.GameObject.transform.parent = SpatialMeshesParent.transform;

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
        public void RaiseMeshRemoved(IMixedRealitySpatialMeshObserver observer, SpatialMeshObject spatialMeshObject)
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
        public void RaiseSurfaceAdded(IMixedRealitySurfaceObserver observer, int surfaceId, GameObject surfaceObject)
        {
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
        public void RaiseSurfaceUpdated(IMixedRealitySurfaceObserver observer, int surfaceId, GameObject surfaceObject)
        {
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
        public void RaiseSurfaceRemoved(IMixedRealitySurfaceObserver observer, int surfaceId)
        {
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
