// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEngine;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Core.Devices
{
    public class BaseSpatialObserver : IMixedRealitySpatialAwarenessObserver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public BaseSpatialObserver(string name, uint priority)
        {
            Name = name;
            Priority = priority;
        }

        #region IMixedRealityToolkit implementation

        public string Name { get; }

        /// <inheritdoc />
        public uint Priority { get; }

        /// <inheritdoc />
        public virtual void Initialize() { }

        /// <inheritdoc />
        public virtual void Reset() { }

        /// <inheritdoc />
        public virtual void Enable() { }

        /// <inheritdoc />
        public virtual void Update() { }

        /// <inheritdoc />
        public virtual void Disable() { }

        /// <inheritdoc />
        public virtual void Destroy() { }

        #endregion IMixedRealityToolkit implementation

        /// <summary>
        /// Is the observer running (actively accumulating spatial data)?
        /// </summary>
        public bool IsRunning { get; protected set; }

        /// <summary>
        /// The collection of mesh <see cref="GameObject"/>s that have been observed.
        /// </summary>
        public virtual IDictionary<int, GameObject> Meshes => new Dictionary<int, GameObject>();

        /// <summary>
        /// Start the observer.
        /// </summary>
        public virtual void StartObserving()
        { }

        /// <summary>
        /// Stop the observer.
        /// </summary>
        public virtual void StopObserving()
        { }

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

        /// <summary>
        /// The collection of meshes being managed by the observer.
        /// </summary>
        protected Dictionary<int, SpatialMeshObject> meshObjects = new Dictionary<int, SpatialMeshObject>();

        /// <summary>
        /// Creates a <see cref="SpatialMeshObject"/>.
        /// </summary>
        /// <param name="mesh"></param> todo: add comments
        /// <param name="name"></param>
        /// <param name="meshId"></param>
        /// <returns>
        /// SpatialMeshObject containing the fields that describe the mesh.
        /// </returns>
        protected SpatialMeshObject CreateSpatialMeshObject(
            Mesh mesh,
            string name,
            int meshId)
        {
            SpatialMeshObject newMesh = new SpatialMeshObject();

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
        protected void CleanupMeshes()
        {
            // Clean up mesh objects.
            // NOTE: We use foreach here since Dictionary<key, value>.Values is an IEnumerable.
            foreach (SpatialMeshObject meshObject in meshObjects.Values)
            {
                // Cleanup mesh object.
                // Destroy the game object, destroy the meshes.
                CleanupMeshObject(meshObject);
            }
            meshObjects.Clear();
        }

        /// <summary>
        /// Clean up the resources associated with the surface.
        /// </summary>
        /// <param name="meshObject">The <see cref="SpatialMeshObject"/> whose resources will be cleaned up.</param>
        /// <param name="destroyGameObject"></param>
        /// <param name="destroyMeshes"></param>
        protected void CleanupMeshObject(
            SpatialMeshObject meshObject,
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
    }
}