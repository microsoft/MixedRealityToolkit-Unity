// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
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
        protected BaseSpatialObserver(string name, uint priority) : base(name, priority)
        {
            if (MixedRealityToolkit.SpatialAwarenessSystem != null)
            {
                SourceId = MixedRealityToolkit.SpatialAwarenessSystem.GenerateNewObserverId();
            }
        }

        #region IMixedRealityEventSource Implementation

        /// <inheritdoc />
        public string SourceName => Name;

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        #endregion IMixedRealityEventSource Implementation

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            if (MixedRealityToolkit.SpatialAwarenessSystem != null)
            {
                MixedRealityToolkit.SpatialAwarenessSystem.RaiseSpatialAwarenessObserverDetected(this);
            }
        }

        /// <inheritdoc />
        public override void Enable()
        {
            if (MixedRealityToolkit.SpatialAwarenessSystem != null &&
                MixedRealityToolkit.SpatialAwarenessSystem.UseMeshSystem &&
                MixedRealityToolkit.Instance.ActiveProfile.IsSpatialAwarenessSystemEnabled &&
                MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.StartupBehavior == AutoStartBehavior.AutoStart)
            {
                StartObserving();
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            StopObserving();

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

            // Clean up planar surface objects
            // todo
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            if (MixedRealityToolkit.SpatialAwarenessSystem != null)
            {
                MixedRealityToolkit.SpatialAwarenessSystem.RaiseSpatialAwarenessObserverLost(this);
            }
        }

        #endregion IMixedRealityService Implementation

        #region IMixedRealitySpatialAwarenessObserver Implementation

        /// <inheritdoc />
        public Vector3 ObservationExtents { get; protected set; }

        /// <inheritdoc />
        public bool IsStationaryObserver { get; protected set; }

        /// <inheritdoc />
        public Vector3 ObserverOrigin { get; protected set; }

        /// <inheritdoc />
        public Quaternion ObserverOrientation { get; protected set; }

        /// <inheritdoc />
        public float UpdateInterval { get; set; }

        /// <inheritdoc />
        public bool IsRunning { get; protected set; }

        private readonly Dictionary<int, SpatialMeshObject> spatialMeshObjects = new Dictionary<int, SpatialMeshObject>();

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

        /// <inheritdoc />
        public virtual void StartObserving() { }

        /// <inheritdoc />
        public virtual void StopObserving() { }

        #endregion IMixedRealitySpatialAwarenessObserver Implementation

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
        /// Clean up the resources associated with the surface.
        /// </summary>
        /// <param name="meshObject">The <see cref="SpatialMeshObject"/> whose resources will be cleaned up.</param>
        /// <param name="destroyGameObject"></param>
        /// <param name="destroyMeshes"></param>
        protected void DestroyMeshObject(SpatialMeshObject meshObject, bool destroyGameObject = true, bool destroyMeshes = true)
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

        #region IEquality Implementation

        /// <summary>
        /// Determines if the specified objects are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool Equals(IMixedRealitySpatialAwarenessObserver left, IMixedRealitySpatialAwarenessObserver right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealitySpatialAwarenessObserver)obj);
        }

        private bool Equals(IMixedRealitySpatialAwarenessObserver other)
        {
            return other != null && SourceId == other.SourceId && string.Equals(SourceName, other.SourceName);
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)SourceId;
                hashCode = (hashCode * 397) ^ (SourceName != null ? SourceName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation
    }
}