// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
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

        #region IMixedRealityManager implementation

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

        #endregion IMixedRealityManager implementation

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
        /// Creates a <see cref="SpatialMeshObject"/>.
        /// </summary>
        /// <returns>
        /// SpatialMeshObject containing the fields that describe the mesh.
        /// </returns>
        protected SpatialMeshObject CreateSpatialMeshObject(
            Mesh mesh,
            string name,
            int meshId = 0)
        {
            SpatialMeshObject newMesh = new SpatialMeshObject();

            newMesh.Id = meshId;
            newMesh.MeshObject = new GameObject(name, requiredMeshComponents);
            newMesh.MeshObject.layer = MixedRealityManager.SpatialAwarenessSystem.MeshPhysicsLayer;

            newMesh.Filter = newMesh.MeshObject.GetComponent<MeshFilter>();
            newMesh.Filter.sharedMesh = mesh;

            newMesh.Renderer = newMesh.MeshObject.GetComponent<MeshRenderer>();
            // todo - consider how to handle enabling, etc

            // Reset the surface mesh collider to fit the updated mesh. 
            // Unity tribal knowledge indicates that to change the mesh assigned to a
            // mesh collider, the mesh must first be set to null.  Presumably there
            // is a side effect in the setter when setting the shared mesh to null.
            newMesh.Collider.sharedMesh = null;
            newMesh.Collider.sharedMesh = newMesh.Filter.sharedMesh;

            return newMesh;
        }
    }
}