// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async.AwaitYieldInstructions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.SpatialObservers
{
    /// <summary>
    /// Base class for spatial awareness observers.
    /// </summary>
    public abstract class BaseMixedRealitySpatialMeshObserver : BaseMixedRealitySpatialObserverDataProvider, IMixedRealitySpatialMeshObserver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="profile"></param>
        protected BaseMixedRealitySpatialMeshObserver(string name, uint priority, BaseMixedRealitySpatialMeshObserverProfile profile)
            : base(name, priority, profile)
        {
            MeshPhysicsLayerOverride = profile.MeshPhysicsLayerOverride;
            MeshLevelOfDetail = profile.MeshLevelOfDetail;
            MeshTrianglesPerCubicMeter = profile.MeshTrianglesPerCubicMeter;
            MeshRecalculateNormals = profile.MeshRecalculateNormals;
            MeshDisplayOption = profile.MeshDisplayOption;
            MeshVisibleMaterial = profile.MeshVisibleMaterial;
            MeshOcclusionMaterial = profile.MeshOcclusionMaterial;
            ObservationExtents = profile.ObservationExtents;
            IsStationaryObserver = profile.IsStationaryObserver;
            spatialMeshObjectPool = new Stack<SpatialMeshObject>();
        }

        private readonly WaitForUpdate NextUpdate = new WaitForUpdate();

        /// <summary>
        /// When a mesh is created we will need to create a game object with a minimum 
        /// set of components to contain the mesh.  These are the required component types.
        /// </summary>
        private readonly System.Type[] requiredMeshComponents =
        {
            typeof(MeshFilter),
            typeof(MeshRenderer),
            typeof(MeshCollider)
        };

        #region IMixedRealityService Implementation

        public override void Initialize()
        {
            base.Initialize();

            // Only update the observer if it is running.
            if (!Application.isPlaying) { return; }

            for (int i = 0; i < 10; i++)
            {
                spatialMeshObjectPool.Push(new SpatialMeshObject(-1, CreateBlankSpatialMeshGameObject()));
            }
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            // Only update the observer if it is running.
            if (!Application.isPlaying) { return; }

            // If we've got some spatial meshes and were disabled previously, turn them back on.
            foreach (SpatialMeshObject meshObject in spatialMeshObjects.Values)
            {
                meshObject.GameObject.SetActive(true);
            }
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            // Only update the observer if it is running.
            if (!Application.isPlaying || !IsRunning) { return; }

            lock (spatialMeshObjectPool)
            {
                // if we get low in our object pool, then create a few more.
                if (spatialMeshObjectPool.Count < 5)
                {
                    spatialMeshObjectPool.Push(new SpatialMeshObject(-1, CreateBlankSpatialMeshGameObject()));
                }
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            // Only update the observer if it is running.
            if (!Application.isPlaying) { return; }

            // Disable any spatial meshes we might have.
            foreach (SpatialMeshObject meshObject in spatialMeshObjects.Values)
            {
                if (meshObject.GameObject != null)
                {
                    meshObject.GameObject.SetActive(false);
                }
            }
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            if (!Application.isPlaying) { return; }

            // Cleanup the spatial meshes that are being managed by this observer.
            foreach (SpatialMeshObject meshObject in spatialMeshObjects.Values)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(meshObject.GameObject);
                }
                else
                {
                    Object.Destroy(meshObject.GameObject);
                }
            }

            spatialMeshObjects.Clear();

            lock (spatialMeshObjectPool)
            {
                foreach (SpatialMeshObject meshObject in spatialMeshObjectPool)
                {
                    if (Application.isEditor)
                    {
                        Object.DestroyImmediate(meshObject.GameObject);
                    }
                    else
                    {
                        Object.Destroy(meshObject.GameObject);
                    }
                }

                spatialMeshObjectPool.Clear();
            }
        }

        #endregion IMixedRealityService Implementation

        #region IMixedRealitySpatialMeshObserver Implementation

        private SpatialAwarenessMeshLevelOfDetail meshLevelOfDetail = SpatialAwarenessMeshLevelOfDetail.Coarse;

        /// <inheritdoc />
        public override int PhysicsLayer => MeshPhysicsLayerOverride == -1 ? base.PhysicsLayer : MeshPhysicsLayerOverride;

        /// <inheritdoc />
        public int MeshPhysicsLayerOverride { get; }

        /// <inheritdoc />
        public SpatialAwarenessMeshLevelOfDetail MeshLevelOfDetail
        {
            get => meshLevelOfDetail;
            set
            {
                if (meshLevelOfDetail != value)
                {
                    // Non-custom values automatically modify MeshTrianglesPerCubicMeter
                    if (value != SpatialAwarenessMeshLevelOfDetail.Custom)
                    {
                        MeshTrianglesPerCubicMeter = (uint)value;
                    }

                    meshLevelOfDetail = value;
                }
            }
        }

        /// <inheritdoc />
        public uint MeshTrianglesPerCubicMeter { get; private set; }

        /// <inheritdoc />
        public bool MeshRecalculateNormals { get; }

        /// <inheritdoc />
        public SpatialMeshDisplayOptions MeshDisplayOption { get; }

        /// <inheritdoc />
        public Material MeshVisibleMaterial { get; }

        /// <inheritdoc />
        public Material MeshOcclusionMaterial { get; }

        /// <inheritdoc />
        public Vector3 ObservationExtents { get; }

        /// <inheritdoc />
        public bool IsStationaryObserver { get; }

        /// <inheritdoc />
        public Vector3 ObserverOrigin { get; protected set; }

        /// <inheritdoc />
        public Quaternion ObserverOrientation { get; protected set; } = Quaternion.identity;

        private readonly Dictionary<int, SpatialMeshObject> spatialMeshObjects = new Dictionary<int, SpatialMeshObject>();

        /// <inheritdoc />
        /// <remarks>
        /// This method returns a copy of the collection maintained by the observer so that application
        /// code can iterate through the collection without concern for changes to the backing data.
        /// </remarks>
        public IReadOnlyDictionary<int, SpatialMeshObject> SpatialMeshObjects => new Dictionary<int, SpatialMeshObject>(spatialMeshObjects);

        private readonly Stack<SpatialMeshObject> spatialMeshObjectPool;

        /// <inheritdoc />
        public virtual void RaiseMeshAdded(SpatialMeshObject spatialMeshObject)
        {
            MixedRealityToolkit.SpatialAwarenessSystem.RaiseMeshAdded(this, spatialMeshObject);
        }

        /// <inheritdoc />
        public virtual void RaiseMeshUpdated(SpatialMeshObject spatialMeshObject)
        {
            MixedRealityToolkit.SpatialAwarenessSystem.RaiseMeshUpdated(this, spatialMeshObject);
        }

        /// <inheritdoc />
        public virtual void RaiseMeshRemoved(SpatialMeshObject spatialMeshObject)
        {
            SpatialMeshObject spatialMesh;
            if (spatialMeshObjects.TryGetValue(spatialMeshObject.Id, out spatialMesh))
            {
                spatialMeshObjects.Remove(spatialMesh.Id);

                // Raise mesh removed if it was prev enabled.
                // spatialMesh.GameObject's only get enabled when successfully cooked.
                // If it's disabled then likely the mesh was removed before cooking completed.
                if (spatialMesh.GameObject.activeInHierarchy)
                {
                    MixedRealityToolkit.SpatialAwarenessSystem.RaiseMeshRemoved(this, spatialMeshObject);
                }

                spatialMesh.GameObject.SetActive(false);
                // Recycle this spatial mesh object and add it back to the pool.
                spatialMesh.GameObject.name = "Reclaimed Spatial Mesh Object";
                spatialMesh.Mesh = null;
                spatialMesh.Id = -1;

                lock (spatialMeshObjectPool)
                {
                    spatialMeshObjectPool.Push(spatialMesh);
                }
            }
            else
            {
                Debug.LogError($"{spatialMeshObject.Id} is missing from known spatial objects!");
            }
        }

        /// <summary>
        /// Request a <see cref="SpatialMeshObject"/> from the collection of known spatial objects. If that object doesn't exist take one from our pool.
        /// </summary>
        /// <param name="meshId">The id of the <see cref="SpatialMeshObject"/>.</param>
        /// <returns>A <see cref="SpatialMeshObject"/></returns>
        protected async Task<SpatialMeshObject> RequestSpatialMeshObject(int meshId)
        {
            SpatialMeshObject spatialMesh;
            if (spatialMeshObjects.TryGetValue(meshId, out spatialMesh))
            {
                return spatialMesh;
            }

            lock (spatialMeshObjectPool)
            {
                if (spatialMeshObjectPool.Count > 0)
                {
                    spatialMesh = spatialMeshObjectPool.Pop();
                    spatialMesh.Id = meshId;
                    spatialMeshObjects.Add(spatialMesh.Id, spatialMesh);
                    return spatialMesh;
                }
            }

            await NextUpdate;
            return await RequestSpatialMeshObject(meshId);
        }

        private GameObject CreateBlankSpatialMeshGameObject()
        {
            var newGameObject = new GameObject($"Blank Spatial Mesh GameObject", requiredMeshComponents)
            {
                layer = MeshPhysicsLayerOverride == -1 ? base.PhysicsLayer : MeshPhysicsLayerOverride
            };

            newGameObject.transform.parent = MixedRealityToolkit.SpatialAwarenessSystem.SpatialMeshesParent.transform;
            newGameObject.SetActive(false);
            return newGameObject;
        }

        #endregion IMixedRealitySpatialMeshObserver Implementation
    }
}