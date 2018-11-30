// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.SpatialObservers
{
    /// <summary>
    /// Base class for spatial awareness observers.
    /// </summary>
    public abstract class BaseSpatialObserver : BaseDataProvider, IMixedRealitySpatialAwarenessObserver
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        protected BaseSpatialObserver(string name, uint priority) : base(name, priority) { }

        /// <inheritdoc />
        public bool IsRunning { get; protected set; }

        /// <inheritdoc />
        /// <remarks>
        /// This method returns a copy of the collection maintained by the observer so that application
        /// code can iterate through the collection without concern for changes to the backing data.
        /// </remarks>
        public virtual IReadOnlyDictionary<int, SpatialMeshObject> Meshes => new Dictionary<int, SpatialMeshObject>(SpatialMeshObjects);

        /// <inheritdoc />
        public virtual void StartObserving() { }

        /// <inheritdoc />
        public virtual void StopObserving() { }

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

        /// <summary>
        /// The collection of meshes being managed by the observer.
        /// </summary>
        protected readonly Dictionary<int, SpatialMeshObject> SpatialMeshObjects = new Dictionary<int, SpatialMeshObject>();

        /// <summary>
        /// Creates a <see cref="SpatialMeshObject"/>.
        /// </summary>
        /// <param name="mesh"></param> todo: add comments
        /// <param name="name"></param>
        /// <param name="meshId"></param>
        /// <returns>
        /// <see cref="SpatialMeshObject"/> containing the fields that describe the mesh.
        /// </returns>
        protected SpatialMeshObject CreateSpatialMeshObject(Mesh mesh, string name, int meshId)
        {
            var gameObject = new GameObject(name, requiredMeshComponents)
            {
                layer = MixedRealityToolkit.SpatialAwarenessSystem.MeshPhysicsLayer
            };

            var meshFilter = gameObject.GetComponent<MeshFilter>();
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            var meshCollider = gameObject.GetComponent<MeshCollider>();

            // Reset the surface mesh collider to fit the updated mesh. 
            // Unity tribal knowledge indicates that to change the mesh assigned to a
            // mesh collider and mesh filter, the mesh must first be set to null.  Presumably there
            // is a side effect in the setter when setting the shared mesh to null.
            meshFilter.sharedMesh = null;
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = meshFilter.sharedMesh;

            return new SpatialMeshObject(meshId, gameObject, meshRenderer, meshFilter, meshCollider);
        }

        /// <summary>
        /// Cleans up mesh objects managed by the observer.
        /// </summary>
        protected void CleanupMeshes()
        {
            // Clean up mesh objects.
            // NOTE: We use foreach here since Dictionary<key, value>.Values is an IEnumerable.
            foreach (SpatialMeshObject meshObject in SpatialMeshObjects.Values)
            {
                // Cleanup mesh object.
                // Destroy the game object, destroy the meshes.
                CleanupMeshObject(meshObject);
            }

            SpatialMeshObjects.Clear();
        }

        /// <summary>
        /// Clean up the resources associated with the surface.
        /// </summary>
        /// <param name="meshObject">The <see cref="SpatialMeshObject"/> whose resources will be cleaned up.</param>
        /// <param name="destroyGameObject"></param>
        /// <param name="destroyMeshes"></param>
        protected void CleanupMeshObject(SpatialMeshObject meshObject, bool destroyGameObject = true, bool destroyMeshes = true)
        {
            if (destroyGameObject && (meshObject.GameObject != null))
            {
                Object.Destroy(meshObject.GameObject);
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