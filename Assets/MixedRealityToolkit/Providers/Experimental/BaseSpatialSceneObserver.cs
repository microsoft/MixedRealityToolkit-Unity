// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    public class BaseSpatialSceneObserver : BaseSpatialObserver, IMixedRealitySpatialAwarenessSceneUnderstandingObserver
    {
        public BaseSpatialSceneObserver(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            string name = null,
            uint priority = 10,
            BaseMixedRealityProfile profile = null) : base(spatialAwarenessSystem, name, priority, profile)
        { }

        private MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> sceneEventData = null;

        protected Dictionary<System.Guid, SpatialAwarenessSceneObject> sceneObjects = new Dictionary<System.Guid, SpatialAwarenessSceneObject>();

        /// <inheritdoc />
        public IReadOnlyDictionary<System.Guid, SpatialAwarenessSceneObject> SceneObjects => throw new System.NotImplementedException();

        /// <inheritdoc />
        public SpatialAwarenessMeshLevelOfDetail LevelOfDetail{ get; set; }

        #region SceneObject event broadcasting

        /// <summary>
        /// Event sent whenever a SceneObject is added.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>> OnSceneObjectAdded =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject> handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject>>(eventData);
                handler.OnObservationAdded(spatialEventData);
            };

        /// <inheritdoc />
        public override void Initialize()
        {
            sceneEventData = new MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject>(EventSystem.current);
        }

        /// <summary>
        /// Sends SceneObject Added event via <see cref="IMixedRealitySpatialAwarenessObservationHandler{T}"/>
        /// </summary>
        /// <param name="sceneObj">The <see cref="SpatialAwarenessSceneObject"/> being created</param>
        /// <param name="id">the id associated with the <paramref name="sceneObj"/></param>
        protected virtual void SendSceneObjectAdded(SpatialAwarenessSceneObject sceneObj, System.Guid id)
        {
            // Send the mesh removed event
            sceneEventData.Initialize(this, id, sceneObj);
            SpatialAwarenessSystem?.HandleEvent(sceneEventData, OnSceneObjectAdded);
        }

        /// <summary>
        /// Event sent whenever a SceneObject is updated.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>> OnSceneObjectUpdated =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject> handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject>>(eventData);
                handler.OnObservationUpdated(spatialEventData);
            };

        /// <summary>
        /// Sends SceneObject Updated event via <see cref="IMixedRealitySpatialAwarenessObservationHandler{T}"/>
        /// </summary>
        /// <param name="sceneObj">The <see cref="SpatialAwarenessSceneObject"/> being updated</param>
        /// <param name="id">the id associated with the <paramref name="sceneObj"/></param>
        protected virtual void SendSceneObjectUpdated(SpatialAwarenessSceneObject sceneObj, int id)
        {

            // Send the mesh removed event
            sceneEventData.Initialize(this, id, sceneObj);
            SpatialAwarenessSystem?.HandleEvent(sceneEventData, OnSceneObjectUpdated);
        }

        /// <summary>
        /// Event sent whenever a SceneObject is removed.
        /// </summary>
        private static readonly ExecuteEvents.EventFunction<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>> OnSceneObjectRemoved =
            delegate (IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject> handler, BaseEventData eventData)
            {
                MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> spatialEventData = ExecuteEvents.ValidateEventData<MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject>>(eventData);
                handler.OnObservationRemoved(spatialEventData);
            };

        /// <summary>
        /// Sends SceneObject Removed event via <see cref="IMixedRealitySpatialAwarenessObservationHandler{T}"/>
        /// </summary>
        /// <param name="sceneObj">The <see cref="SpatialAwarenessSceneObject"/> being removed</param>
        /// <param name="id">the id associated with the <paramref name="sceneObj"/></param>
        protected virtual void SendSceneObjectRemoved(SpatialAwarenessSceneObject sceneObj, int id)
        {
            // Send the mesh removed event
            sceneEventData.Initialize(this, id, null);
            SpatialAwarenessSystem?.HandleEvent(sceneEventData, OnSceneObjectRemoved);
        }

        public virtual void LoadScene(string filename)
        {
            throw new System.NotImplementedException();
        }

        public virtual void SaveScene(string filename)
        {
            throw new System.NotImplementedException();
        }

        #endregion SceneObject event broadcasting

        public SpatialAwarenessSurfaceTypes SurfaceTypes { get; set; }
        public int PhysicsLayer { get; set; }
        public bool ShouldLoadFromFile { get; set; }
        public int InstantiationBatchRate { get; set; }
        public bool RenderInferredRegions { get; set; }
        public bool GenerateEnvironmentMesh { get; set; }
        public bool GenerateMeshes { get; set; }
        public bool GeneratePlanes { get; set; }
        public bool UsePersistentObjects { get; set; }
        public float QueryRadius { get; set; }
        public bool VisualizeOcclusionMask { get; set; }
        public Vector2Int OcclusionMaskResolution { get; set; }

    }
}