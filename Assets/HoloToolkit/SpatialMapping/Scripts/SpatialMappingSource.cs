// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Rendering;

namespace HoloToolkit.Unity.SpatialMapping
{
    public class SpatialMappingSource : MonoBehaviour
    {
        /// <summary>
        /// Surface object
        /// </summary>
        public struct SurfaceObject
        {
            public int ID;
            public GameObject Object;
            public MeshRenderer Renderer;
            public MeshFilter Filter;
            public MeshCollider Collider;
        }

        public struct SurfaceUpdate
        {
            public SurfaceObject Old;
            public SurfaceObject New;
        }

        /// <summary>
        /// Collection of surface objects that have been created for this spatial mapping source.
        /// </summary>
        public ReadOnlyCollection<SurfaceObject> SurfaceObjects
        {
            get { return surfaceObjects; }
        }

        public event EventHandler<DataEventArgs<SurfaceObject>> SurfaceAdded;
        public event EventHandler<DataEventArgs<SurfaceUpdate>> SurfaceUpdated;
        public event EventHandler<DataEventArgs<SurfaceObject>> SurfaceRemoved;
        public event EventHandler<EventArgs> RemovingAllSurfaces;

        /// <summary>
        /// When a mesh is created we will need to create a game object with a minimum 
        /// set of components to contain the mesh.  These are the required component types.
        /// </summary>
        protected readonly Type[] componentsRequiredForSurfaceMesh =
        {
            typeof(MeshFilter),
            typeof(MeshRenderer),
            typeof(MeshCollider)
        };

        /// <summary>
        /// Material to use for rendering the mesh
        /// </summary>
        protected virtual Material RenderMaterial { get { return SpatialMappingManager.Instance.SurfaceMaterial; } }

        private readonly List<SurfaceObject> surfaceObjectsWriteable;
        private readonly ReadOnlyCollection<SurfaceObject> surfaceObjects;

        public SpatialMappingSource()
        {
            surfaceObjectsWriteable = new List<SurfaceObject>();
            surfaceObjects = new ReadOnlyCollection<SurfaceObject>(surfaceObjectsWriteable);
        }

        protected virtual void Awake()
        {
            // Nothing.
        }

        /// <summary>
        /// Create a new surface object.
        /// </summary>
        /// <param name="mesh">The mesh to attach. Can be null.</param>
        /// <param name="objectName">What to name this object.</param>
        /// <param name="parentObject">What to parent this object to.</param>
        /// <param name="meshID">Optional user specified ID for the mesh.</param>
        /// <param name="drawVisualMeshesOverride">If specified, overrides the default value for enabling/disabling the mesh renderer.</param>
        /// <param name="castShadowsOverride">If specified, overrides the default value for casting shadows.</param>
        /// <returns>The newly created surface object.</returns>
        protected SurfaceObject CreateSurfaceObject(
            Mesh mesh,
            string objectName,
            Transform parentObject,
            int meshID = 0,
            bool? drawVisualMeshesOverride = null,
            bool? castShadowsOverride = null
            )
        {
            SurfaceObject surfaceObject = new SurfaceObject();
            surfaceObject.ID = meshID;

            surfaceObject.Object = new GameObject(objectName, componentsRequiredForSurfaceMesh);
            surfaceObject.Object.transform.SetParent(parentObject);
            surfaceObject.Object.layer = SpatialMappingManager.Instance.PhysicsLayer;

            surfaceObject.Filter = surfaceObject.Object.GetComponent<MeshFilter>();
            surfaceObject.Filter.sharedMesh = mesh;

            surfaceObject.Renderer = surfaceObject.Object.GetComponent<MeshRenderer>();
            surfaceObject.Renderer.sharedMaterial = RenderMaterial;
            surfaceObject.Renderer.enabled = (drawVisualMeshesOverride ?? SpatialMappingManager.Instance.DrawVisualMeshes);
            surfaceObject.Renderer.shadowCastingMode = ((castShadowsOverride ?? SpatialMappingManager.Instance.CastShadows) ? ShadowCastingMode.On : ShadowCastingMode.Off);

            surfaceObject.Collider = surfaceObject.Object.GetComponent<MeshCollider>();

            // Reset the surface mesh collider to fit the updated mesh. 
            // Unity tribal knowledge indicates that to change the mesh assigned to a
            // mesh collider, the mesh must first be set to null.  Presumably there
            // is a side effect in the setter when setting the shared mesh to null.
            surfaceObject.Collider.sharedMesh = null;
            surfaceObject.Collider.sharedMesh = surfaceObject.Filter.sharedMesh;

            return surfaceObject;
        }

        /// <summary>
        /// Add the surface to <see cref="SurfaceObjects"/>.
        /// </summary>
        /// <param name="toAdd">The surface to add.</param>
        protected void AddSurfaceObject(SurfaceObject toAdd)
        {
            surfaceObjectsWriteable.Add(toAdd);

            var handlers = SurfaceAdded;
            if (handlers != null)
            {
                handlers(this, DataEventArgs.Create(toAdd));
            }
        }

        /// <summary>
        /// Update the first surface with a matching ID if one exists in <see cref="SurfaceObjects"/>, otherwise add the surface as new.
        /// </summary>
        /// <param name="toUpdateOrAdd">The surface to be updated or added.</param>
        /// <param name="destroyGameObjectIfReplaced">If a surface is updated, and a game object is being replaced, pass true to destroy the outgoing game object or false otherwise.</param>
        /// <param name="destroyMeshesIfReplaced">If a surface is updated, and new meshes are replacing old meshes, pass true to destroy the outgoing meshes or false otherwise.</param>
        /// <returns>The surface object that was updated or null if one was not found meaning a new surface was added.</returns>
        protected SurfaceObject? UpdateOrAddSurfaceObject(SurfaceObject toUpdateOrAdd, bool destroyGameObjectIfReplaced = true, bool destroyMeshesIfReplaced = true)
        {
            SurfaceObject? replaced = null;

            for (int iSurface = 0; iSurface < surfaceObjectsWriteable.Count; iSurface++)
            {
                SurfaceObject existing = surfaceObjectsWriteable[iSurface];

                if (existing.ID == toUpdateOrAdd.ID)
                {
                    surfaceObjectsWriteable[iSurface] = toUpdateOrAdd;

                    var handlers = SurfaceUpdated;
                    if (handlers != null)
                    {
                        handlers(this, DataEventArgs.Create(new SurfaceUpdate { Old = existing, New = toUpdateOrAdd }));
                    }

                    CleanUpSurface(
                        existing,
                        destroyGameObjectIfReplaced,
                        destroyMeshesIfReplaced,
                        objectToPreserve: toUpdateOrAdd.Object,
                        meshToPreserveA: toUpdateOrAdd.Filter.sharedMesh,
                        meshToPreserveB: toUpdateOrAdd.Collider.sharedMesh
                        );

                    replaced = existing;
                    break;
                }
            }

            if (replaced == null)
            {
                AddSurfaceObject(toUpdateOrAdd);
            }

            return replaced;
        }

        /// <summary>
        /// Remove the first surface with the specified ID if one exists in <see cref="SurfaceObjects"/>.
        /// </summary>
        /// <param name="surfaceID">The ID of the surface to remove.</param>
        /// <param name="destroyGameObject">True to destroy the <see cref="SurfaceObject.Object"/> associated with the surface, false otherwise.</param>
        /// <param name="destroyMeshes">True to destroy the meshes associated with the surface, false otherwise.</param>
        /// <returns>The surface object if one was found and removed or null if one was not found.</returns>
        protected SurfaceObject? RemoveSurfaceIfFound(int surfaceID, bool destroyGameObject = true, bool destroyMeshes = true)
        {
            SurfaceObject? removed = null;

            for (int iSurface = 0; iSurface < surfaceObjectsWriteable.Count; iSurface++)
            {
                SurfaceObject surface = surfaceObjectsWriteable[iSurface];

                if (surface.ID == surfaceID)
                {
                    surfaceObjectsWriteable.RemoveAt(iSurface);

                    var handlers = SurfaceRemoved;
                    if (handlers != null)
                    {
                        handlers(this, DataEventArgs.Create(surface));
                    }

                    CleanUpSurface(surface, destroyGameObject, destroyMeshes);

                    removed = surface;
                    break;
                }
            }

            return removed;
        }

        /// <summary>
        /// Clean up the resources associated with the surface.
        /// </summary>
        /// <param name="surface">The surface whose resources will be cleaned up.</param>
        /// <param name="destroyGameObject"></param>
        /// <param name="destroyMeshes"></param>
        /// <param name="objectToPreserve">If the surface's game object matches this parameter, it will not be destroyed.</param>
        /// <param name="meshToPreserveA">If either of the surface's meshes matches this parameter, it will not be destroyed.</param>
        /// <param name="meshToPreserveB">If either of the surface's meshes matches this parameter, it will not be destroyed.</param>
        protected void CleanUpSurface(
            SurfaceObject surface,
            bool destroyGameObject = true,
            bool destroyMeshes = true,
            GameObject objectToPreserve = null,
            Mesh meshToPreserveA = null,
            Mesh meshToPreserveB = null
            )
        {
            if (destroyGameObject
                && (surface.Object != null)
                && (surface.Object != objectToPreserve)
                )
            {
                Destroy(surface.Object);
                Debug.Assert(surface.GetType().IsValueType(), "If surface is no longer a value type, you should probably set surface.Object to null.");
            }

            Mesh filterMesh = surface.Filter.sharedMesh;
            Mesh colliderMesh = surface.Collider.sharedMesh;

            if (destroyMeshes
                && (filterMesh != null)
                && (filterMesh != meshToPreserveA)
                && (filterMesh != meshToPreserveB)
                )
            {
                Destroy(filterMesh);
                surface.Filter.sharedMesh = null;
            }

            if (destroyMeshes
                && (colliderMesh != null)
                && (colliderMesh != filterMesh)
                && (colliderMesh != meshToPreserveA)
                && (colliderMesh != meshToPreserveB)
                )
            {
                Destroy(colliderMesh);
                surface.Collider.sharedMesh = null;
            }
        }

        /// <summary>
        /// Cleans up references to objects that we have created.
        /// </summary>
        /// <param name="destroyGameObjects">True to destroy the game objects of each surface, false otherwise.</param>
        /// <param name="destroyMeshes">True to destroy the meshes of each surface, false otherwise.</param>
        protected void Cleanup(bool destroyGameObjects = true, bool destroyMeshes = true)
        {
            var handlers = RemovingAllSurfaces;
            if (handlers != null)
            {
                handlers(this, EventArgs.Empty);
            }

            for (int index = 0; index < surfaceObjectsWriteable.Count; index++)
            {
                CleanUpSurface(surfaceObjectsWriteable[index], destroyGameObjects, destroyMeshes);
            }
            surfaceObjectsWriteable.Clear();
        }

        /// <summary>
        /// Gets all mesh filters that have a valid mesh.
        /// </summary>
        /// <returns>A list of filters, each with a mesh containing at least one triangle.</returns>
        public virtual List<MeshFilter> GetMeshFilters()
        {
            List<MeshFilter> meshFilters = new List<MeshFilter>();

            for (int index = 0; index < surfaceObjectsWriteable.Count; index++)
            {
                if (surfaceObjectsWriteable[index].Filter != null &&
                    surfaceObjectsWriteable[index].Filter.sharedMesh != null &&
                    surfaceObjectsWriteable[index].Filter.sharedMesh.vertexCount > 2)
                {
                    meshFilters.Add(surfaceObjectsWriteable[index].Filter);
                }
            }

            return meshFilters;
        }

        /// <summary>
        /// Gets all mesh renderers that have been created.
        /// </summary>
        /// <returns></returns>
        public virtual List<MeshRenderer> GetMeshRenderers()
        {
            List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

            for (int index = 0; index < surfaceObjectsWriteable.Count; index++)
            {
                if (surfaceObjectsWriteable[index].Renderer != null)
                {
                    meshRenderers.Add(surfaceObjectsWriteable[index].Renderer);
                }
            }

            return meshRenderers;
        }
    }
}
