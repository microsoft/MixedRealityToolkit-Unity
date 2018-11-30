// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers;
using Microsoft.MixedReality.Toolkit.Core.Services;
using System.Collections;
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
        protected BaseSpatialObserver(string name, uint priority) : base(name, priority)
        {
            SourceId = MixedRealityToolkit.SpatialAwarenessSystem.GenerateNewObserverId();
        }

        #region IMixedRealityService Implementation

        /// <inheritdoc />
        public override void Initialize()
        {
            StartupBehavior = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.StartupBehavior;
            ObservationExtents = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.ObservationExtents;
            IsStationaryObserver = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.IsStationaryObserver;
            UpdateInterval = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile.UpdateInterval;
        }

        #endregion IMixedRealityService Implementation

        #region IMixedRealityEventSource Implementation

        /// <inheritdoc />
        public string SourceName => Name;

        /// <inheritdoc />
        public uint SourceId { get; }

        #region IEquality Implementation

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <paramref name="left" /> and <paramref name="right" /> are of different types and neither one can handle comparisons with the other.</exception>
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
            return other != null && SourceId == other.SourceId && string.Equals(Name, other.SourceName);
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
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation

        #endregion IMixedRealityEventSource Implementation

        #region IMixedRealitySpatialAwarenessObserver Implementation

        /// <inheritdoc />
        public AutoStartBehavior StartupBehavior { get; set; } = AutoStartBehavior.AutoStart;

        /// <inheritdoc />
        public Vector3 ObservationExtents { get; set; } = Vector3.one * 3;

        /// <inheritdoc />
        public bool IsStationaryObserver { get; set; } = false;

        /// <inheritdoc />
        public Vector3 ObserverOrigin { get; set; } = Vector3.zero;

        /// <inheritdoc />
        public Quaternion ObserverOrientation { get; set; }

        /// <inheritdoc />
        public float UpdateInterval { get; set; } = 3.5f;

        /// <inheritdoc />
        public bool IsRunning { get; protected set; }

        /// <inheritdoc />
        public IReadOnlyDictionary<int, SpatialMeshObject> SpatialMeshObjects => spatialMeshObjects;

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
        protected readonly Dictionary<int, SpatialMeshObject> spatialMeshObjects = new Dictionary<int, SpatialMeshObject>();

        /// <summary>
        /// Creates a <see cref="SpatialMeshObject"/>.
        /// </summary>
        /// <param name="mesh">The <see cref="Mesh"/> data to use when constructing the spatial mesh object.</param>
        /// <param name="name">The name of the spatial mesh <see cref="GameObject"/> reference.</param>
        /// <param name="meshId">the id of the spatial mesh object.</param>
        /// <returns>
        /// <see cref="SpatialMeshObject"/> containing the fields that describe the spatial mesh object.
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
            var spatialMeshObject = new SpatialMeshObject(meshId, gameObject, meshRenderer, meshFilter, meshCollider);
            spatialMeshObjects.Add(meshId, spatialMeshObject);
            return spatialMeshObject;
        }

        /// <summary>
        /// Cleans up mesh objects managed by the observer.
        /// </summary>
        protected void CleanupMeshes()
        {
            // Clean up mesh objects.
            // NOTE: We use foreach here since Dictionary<key, value>.Values is an IEnumerable.
            foreach (SpatialMeshObject meshObject in spatialMeshObjects.Values)
            {
                // Cleanup mesh object.
                // Destroy the game object, destroy the meshes.
                CleanupMeshObject(meshObject);
            }

            spatialMeshObjects.Clear();
        }

        /// <summary>
        /// Clean up the resources associated with the surface.
        /// </summary>
        /// <param name="meshObject">The <see cref="SpatialMeshObject"/> whose resources will be cleaned up.</param>
        /// <param name="destroyGameObject"></param>
        /// <param name="destroyMeshes"></param>
        protected void CleanupMeshObject(SpatialMeshObject meshObject, bool destroyGameObject = true, bool destroyMeshes = true)
        {
            // Only clean up meshes if we're not going to destroy the GameObject.
            // If the GameObject is to be destroyed we can skip this step and have Unity clean up for us.
            if (!destroyGameObject && destroyMeshes)
            {
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

            if (destroyGameObject)
            {
                Debug.Assert(meshObject.GameObject != null);
                Object.Destroy(meshObject.GameObject);
            }
        }

        #endregion IMixedRealitySpatialAwarenessObserver Implementation
    }
}