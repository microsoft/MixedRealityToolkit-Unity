// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

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
            public int UpdateID;
            public GameObject Object;
            public MeshRenderer Renderer;
            public MeshFilter Filter;
        }

        /// <summary>
        /// Collection of surface objects that have been created for this spatial mapping source.
        /// </summary>
        public List<SurfaceObject> SurfaceObjects { get; private set; }

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

        protected virtual void Awake()
        {
            SurfaceObjects = new List<SurfaceObject>();
        }

        /// <summary>
        /// Creates a new surface game object.
        /// </summary>
        /// <param name="mesh">The mesh to attach. Can be null.</param>
        /// <param name="objectName">What to name this object.</param>
        /// <param name="parentObject">What to parent this object to.</param>
        /// <param name="meshID">Optional user specified ID for the mesh.</param>
        /// <returns>The newly created game object.</returns>
        protected GameObject AddSurfaceObject(Mesh mesh, string objectName, Transform parentObject, int meshID = 0)
        {
            SurfaceObject surfaceObject = new SurfaceObject();
            surfaceObject.ID = meshID;
            surfaceObject.UpdateID = 0;

            surfaceObject.Object = new GameObject(objectName, componentsRequiredForSurfaceMesh);
            surfaceObject.Object.transform.SetParent(parentObject);
            surfaceObject.Object.layer = SpatialMappingManager.Instance.PhysicsLayer;

            surfaceObject.Filter = surfaceObject.Object.GetComponent<MeshFilter>();
            surfaceObject.Filter.sharedMesh = mesh;

            surfaceObject.Renderer = surfaceObject.Object.GetComponent<MeshRenderer>();
            surfaceObject.Renderer.sharedMaterial = RenderMaterial;

            SurfaceObjects.Add(surfaceObject);

            return surfaceObject.Object;
        }

        /// <summary>
        /// Updates an existing surface object.
        /// </summary>
        /// <param name="surfaceGameObject">Game object reference to the surfaceObject.</param>
        /// <param name="meshID">User specified ID for the mesh.</param>
        /// <returns>True if successful</returns>
        protected void UpdateSurfaceObject(GameObject surfaceGameObject, int meshID)
        {
            // If it's in the list, update it
            for (int i = 0; i < SurfaceObjects.Count; ++i)
            {
                if (SurfaceObjects[i].Object == surfaceGameObject)
                {
                    SurfaceObject thisSurfaceObject = SurfaceObjects[i];
                    thisSurfaceObject.ID = meshID;
                    thisSurfaceObject.UpdateID++;
                    SurfaceObjects[i] = thisSurfaceObject;
                    return;
                }
            }

            // Not in the list, add it
            SurfaceObject surfaceObject = new SurfaceObject();
            surfaceObject.ID = meshID;
            surfaceObject.UpdateID = 0;

            surfaceObject.Object = surfaceGameObject;
            surfaceObject.Filter = surfaceObject.Object.GetComponent<MeshFilter>();
            surfaceObject.Renderer = surfaceObject.Object.GetComponent<MeshRenderer>();

            SurfaceObjects.Add(surfaceObject);
        }

        /// <summary>
        /// Removes and optionally destroys the specified surfaceObject that we've created
        /// </summary>
        /// <param name="surface">The surface game object</param>
        /// <param name="removeAndDestroy">If true, the surface will be removed and destroyed. If false, it will only be removed.</param>
        protected void RemoveSurfaceObject(GameObject surface, bool removeAndDestroy = true)
        {
            // Remove it from our list
            for (int index = 0; index < SurfaceObjects.Count; index++)
            {
                if (SurfaceObjects[index].Object == surface)
                {
                    if (removeAndDestroy)
                    {
                        Destroy(SurfaceObjects[index].Object);
                    }
                    SurfaceObjects.RemoveAt(index);
                    break;
                }
            }
        }

        /// <summary>
        /// Cleans up references to objects that we have created.
        /// </summary>
        protected void Cleanup()
        {
            for (int index = 0; index < SurfaceObjects.Count; index++)
            {
                Destroy(SurfaceObjects[index].Object);
            }
            SurfaceObjects.Clear();
        }

        /// <summary>
        /// Gets all mesh filters that have a valid mesh.
        /// </summary>
        /// <returns>A list of filters, each with a mesh containing at least one triangle.</returns>
        public virtual List<MeshFilter> GetMeshFilters()
        {
            List<MeshFilter> meshFilters = new List<MeshFilter>();

            for (int index = 0; index < SurfaceObjects.Count; index++)
            {
                if (SurfaceObjects[index].Filter != null &&
                    SurfaceObjects[index].Filter.sharedMesh != null &&
                    SurfaceObjects[index].Filter.sharedMesh.vertexCount > 2)
                {
                    meshFilters.Add(SurfaceObjects[index].Filter);
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

            for (int index = 0; index < SurfaceObjects.Count; index++)
            {
                if (SurfaceObjects[index].Renderer != null)
                {
                    meshRenderers.Add(SurfaceObjects[index].Renderer);
                }
            }

            return meshRenderers;
        }
    }
}
