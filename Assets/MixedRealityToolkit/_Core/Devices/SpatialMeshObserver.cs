// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Observers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Devices
{
    public class SpatialMeshObserver : BaseSpatialObserver, IMixedRealitySpatialAwarenessMeshObserver
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public SpatialMeshObserver(string name, uint priority) : base(name, priority)
        {
            SourceId = MixedRealityToolkit.SpatialAwarenessSystem.GenerateNewSourceId();
            SourceName = name;
        }

        #region IMixedRealitySpatialAwarenessMeshObserver implementation

        /// <summary>
        /// The collection of mesh <see cref="GameObject"/>s that have been observed.
        /// </summary>
        public virtual IDictionary<int, GameObject> Meshes => new Dictionary<int, GameObject>();

        /// <inheritdoc />
        public int MeshPhysicsLayer { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <inheritdoc />
        public int MeshPhysicsLayerMask => throw new System.NotImplementedException();

        /// <inheritdoc />
        public SpatialAwarenessMeshLevelOfDetail MeshLevelOfDetail { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <inheritdoc />
        public int MeshTrianglesPerCubicMeter { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <inheritdoc />
        public bool RecalculateNormals { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <inheritdoc />
        public SpatialObjectDisplayOptions DisplayOption { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <inheritdoc />
        public Material VisibleMaterial { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <inheritdoc />
        public Material OcclusionMaterial { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <summary>
        /// The collection of meshes being managed by the observer.
        /// </summary>
        IReadOnlyDictionary<int, SpatialAwarenessMeshObject> IMixedRealitySpatialAwarenessMeshObserver.Meshes => new Dictionary<int, SpatialAwarenessMeshObject>();

        /// <summary>
        /// Start | Resume the observer.
        /// </summary>
        public override void Resume() { }

        /// <summary>
        /// Stop | Pause the observer.
        /// </summary>
        public override void Suspend() { }

        /// <summary>
        /// When a mesh is created we will need to create a game object with a minimum 
        /// set of components to contain the mesh.  These are the required component types.
        /// </summary>
        private System.Type[] requiredMeshComponents =
        {
            typeof(MeshFilter),
            typeof(MeshRenderer),
            typeof(MeshCollider)
        };

        protected Dictionary<int, BaseSpatialAwarenessObject> meshObjects = new Dictionary<int, BaseSpatialAwarenessObject>();

        /// <summary>
        /// Creates a <see cref="SpatialMeshObject"/>.
        /// </summary>
        /// <param name="mesh"></param> todo: add comments
        /// <param name="name"></param>
        /// <param name="meshId"></param>
        /// <returns>
        /// SpatialMeshObject containing the fields that describe the mesh.
        /// </returns>
        protected override BaseSpatialAwarenessObject CreateSpatialObject(
            Mesh mesh,
            string name,
            int meshId)
        {
            BaseSpatialAwarenessObject newMesh = new BaseSpatialAwarenessObject();

            newMesh.Id = meshId;
            newMesh.GameObject = new GameObject(name, requiredMeshComponents);
            newMesh.GameObject.layer = MixedRealityToolkit.SpatialAwarenessSystem.MeshPhysicsLayer;

            newMesh.Filter = newMesh.GameObject.GetComponent<MeshFilter>();
            newMesh.Filter.sharedMesh = mesh;

            newMesh.Renderer = newMesh.GameObject.GetComponent<MeshRenderer>();

            // Reset the surface mesh collider to fit the updated mesh. 
            // Unity tribal knowledge indicates that to change the mesh assigned to a
            // mesh collider, the mesh must first be set to null.  Presumably there
            // is a side effect in the setter when setting the shared mesh to null.
            newMesh.Collider = newMesh.GameObject.GetComponent<MeshCollider>();
            newMesh.Collider.sharedMesh = null;
            newMesh.Collider.sharedMesh = newMesh.Filter.sharedMesh;

            return newMesh;
        }

        /// <summary>
        /// Cleans up mesh objects managed by the observer.
        /// </summary>
        protected override void CleanUpSpatialObjectList()
        {
            // Clean up mesh objects.
            // NOTE: We use foreach here since Dictionary<key, value>.Values is an IEnumerable.
            foreach (SpatialAwarenessMeshObject meshObject in meshObjects.Values)
            {
                // Cleanup mesh object.
                // Destroy the game object, destroy the meshes.
                CleanUpSpatialObject(meshObject);
            }
            meshObjects.Clear();
        }

        /// <summary>
        /// Clean up the resources associated with the surface.
        /// </summary>
        /// <param name="meshObject">The <see cref="SpatialAwarenessMeshObject"/> whose resources will be cleaned up.</param>
        /// <param name="destroyGameObject"></param>
        /// <param name="destroyMeshes"></param>
        protected override void CleanUpSpatialObject(
            BaseSpatialAwarenessObject spatialObject,
            bool destroyGameObject = true)
        {
            SpatialAwarenessMeshObject meshObject = spatialObject as SpatialAwarenessMeshObject;

            if (destroyGameObject && (meshObject.GameObject != null))
            {
                Object.Destroy(meshObject.GameObject);
                meshObject.GameObject = null;
            }

            Mesh filterMesh = meshObject.Filter.sharedMesh;
            Mesh colliderMesh = meshObject.Collider.sharedMesh;

            if (filterMesh != null)
            {
                Object.Destroy(filterMesh);
                meshObject.Filter.sharedMesh = null;
            }

            if ((colliderMesh != null) && (colliderMesh != filterMesh))
            {
                Object.Destroy(colliderMesh);
                meshObject.Collider.sharedMesh = null;
            }
        }

        //Old implmementation still using destroy meshes.. not sure if its needed?
        /*
         protected override void CleanUpSpatialObject(
            BaseSpatialAwarenessObject meshObject,
            bool destroyGameObject = true,
            bool destroyMeshes = true)
        {
            if (destroyGameObject && (meshObject.GameObject != null))
            {
                Object.Destroy(meshObject.GameObject);
                meshObject.GameObject = null;
            }

            Mesh filterMesh = meshObject.Filter.sharedMesh;
            Mesh colliderMesh = meshObject.Collider.sharedMesh;

            if (destroyMeshes)
            {
                if (filterMesh != null)
                {
                    Object.Destroy(filterMesh);
                    meshObject.Filter.sharedMesh = null;
                }

                if ((colliderMesh != null) && (colliderMesh != filterMesh))
                {
                    Object.Destroy(colliderMesh);
                    meshObject.Collider.sharedMesh = null;
                }
            }
        }
        */

        #endregion IMixedRealitySpatialAwarenessObserver implementation
    }
}
