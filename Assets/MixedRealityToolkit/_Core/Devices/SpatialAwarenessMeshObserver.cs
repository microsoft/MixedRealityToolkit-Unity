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

        IReadOnlyDictionary<int, SpatialAwarenessMeshObject> IMixedRealitySpatialAwarenessMeshObserver.Meshes => throw new System.NotImplementedException();



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
                SpatialAwarenessMeshObject.CleanUpMeshObject(meshObject);
            }
            meshObjects.Clear();
        }



        //Old implmementation
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
