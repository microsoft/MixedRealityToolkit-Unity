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
            if (profile == null)
            {
                Debug.LogError($"Missing profile for {name}");
                return;
            }

            MeshLevelOfDetail = profile.MeshLevelOfDetail;
            MeshTrianglesPerCubicMeter = profile.MeshTrianglesPerCubicMeter;
            MeshRecalculateNormals = profile.MeshRecalculateNormals;
            MeshDisplayOption = profile.MeshDisplayOption;
            MeshVisibleMaterial = profile.MeshVisibleMaterial;
            MeshOcclusionMaterial = profile.MeshOcclusionMaterial;
            ObservationExtents = profile.ObservationExtents;
            IsStationaryObserver = profile.IsStationaryObserver;
        }

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

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            // Cleanup the mesh objects that are being managed by this observer.
            // Clean up mesh objects.
            // NOTE: We use foreach here since Dictionary<key, value>.Values is an IEnumerable.
            foreach (SpatialMeshObject meshObject in spatialMeshObjects.Values)
            {
                // Cleanup mesh object.
                // Destroy the game object, destroy the meshes.
                DestroyMeshObject(meshObject);
            }

            spatialMeshObjects.Clear();
        }

        #endregion IMixedRealityService Implementation

        #region IMixedRealitySpatialMeshObserver Implementation

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

        /// <inheritdoc />
        public virtual void RaiseMeshAdded(SpatialMeshObject spatialMeshObject)
        {
            spatialMeshObjects.Add(spatialMeshObject.Id, spatialMeshObject);
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
            MixedRealityToolkit.SpatialAwarenessSystem.RaiseMeshRemoved(this, spatialMeshObject);
        }

        /// <summary>
        /// Creates a <see cref="SpatialMeshObject"/>.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="name"></param>
        /// <param name="meshId"></param>
        /// <returns>
        /// <see cref="SpatialMeshObject"/> containing the fields that describe the mesh.
        /// </returns>
        protected SpatialMeshObject CreateSpatialMeshObject(Mesh mesh, string name, int meshId)
        {
            var gameObject = new GameObject(name, requiredMeshComponents)
            {
                layer = PhysicsLayer
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
        /// Clean up the resources associated with the surface.
        /// </summary>
        /// <param name="meshObject">The <see cref="SpatialMeshObject"/> whose resources will be cleaned up.</param>
        protected void DestroyMeshObject(SpatialMeshObject meshObject)
        {
            if (meshObject.GameObject != null)
            {
                Object.Destroy(meshObject.GameObject);
            }
        }

        /// <summary>
        /// Removes the <see cref="SpatialMeshObject"/> associated with the specified id.
        /// </summary>
        /// <param name="meshId">The id of the mesh to be removed.</param>
        protected void RemoveMeshObject(int meshId)
        {
            SpatialMeshObject mesh;

            if (spatialMeshObjects.TryGetValue(meshId, out mesh))
            {
                // Remove the mesh object from the collection.
                spatialMeshObjects.Remove(meshId);

                // Sent the mesh removed event.
                RaiseMeshRemoved(mesh);

                // Cleanup the mesh object.
                // Destroy the game object, destroy the meshes.
                DestroyMeshObject(mesh);
            }
        }

        #endregion IMixedRealitySpatialMeshObserver Implementation
    }
}