// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Observers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Devices
{
    public class SpatialAwarenessMeshObserver : BaseSpatialObserver, IMixedRealitySpatialAwarenessMeshObserver
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public SpatialAwarenessMeshObserver(string name, uint priority) : base(name, priority)
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
        public bool RecalculateNormals { get; set; }

        /// <inheritdoc />
        public SpatialObjectDisplayOptions DisplayOption { get; set; } = SpatialObjectDisplayOptions.None;

        /// <inheritdoc />
        public Material VisibleMaterial { get; set; } = null;

        /// <inheritdoc />
        public Material OcclusionMaterial { get; set; } = null;

        IReadOnlyDictionary<int, SpatialAwarenessMeshObject> IMixedRealitySpatialAwarenessMeshObserver.Meshes => new Dictionary<int, SpatialAwarenessMeshObject>();

        private GameObject meshParent = null;

        /// <inheritdoc />
        public GameObject MeshParent => meshParent != null ? meshParent : (meshParent = CreateMeshParent);

        /// <summary>
        /// Creates the parent for this observer's mesh objects so that the scene hierarchy does not get overly cluttered.
        /// </summary>
        /// <returns>
        /// The <see cref="GameObject"/> to which mesh objects created by this observer will be parented.
        /// </returns>
        private GameObject CreateMeshParent => new GameObject("MeshObserver");

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
        protected System.Type[] requiredMeshComponents =
        {
            typeof(MeshFilter),
            typeof(MeshRenderer),
            typeof(MeshCollider)
        };

        protected Dictionary<int, SpatialAwarenessMeshObject> meshObjects = new Dictionary<int, SpatialAwarenessMeshObject>();

        private MixedRealitySpatialAwarenessEventData meshEventData = null;

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

        #region mesh events

        /// <inheritdoc />
        public void RaiseMeshAdded(IMixedRealitySpatialAwarenessMeshObserver meshObserver, int meshId, GameObject mesh)
        {
            // Parent the mesh object
            mesh.transform.parent = MeshParent.transform;

            meshEventData.Initialize(meshObserver, meshId, mesh);
            HandleEvent(meshEventData, OnMeshAdded);
        }

        ///<inheritdoc />
        public void RaiseMeshUpdated(IMixedRealitySpatialAwarenessMeshObserver meshObserver, int meshId, GameObject mesh)
        {
            // Parent the mesh object
            mesh.transform.parent = MeshParent.transform;

            meshEventData.Initialize(meshObserver, meshId, mesh);
            HandleEvent(meshEventData, OnMeshUpdated);
        }

        ///<inheritdoc />
        public void RaiseMeshRemoved(IMixedRealitySpatialAwarenessMeshObserver meshObserver, int meshId)
        {
            meshEventData.Initialize(meshObserver, meshId, null);
            HandleEvent(meshEventData, OnMeshRemoved);
        }

        #endregion mesh events

        #endregion IMixedRealitySpatialAwarenessObserver implementation
    }
}
