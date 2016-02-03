using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public class SpatialMappingSource : MonoBehaviour
    {
        /// <summary>
        /// Collection of surface objects that have been created for this spatial mapping source.
        /// </summary>
        protected List<GameObject> surfaceObjects = new List<GameObject>();

        /// <summary>
        /// Collection of mesh renderers that have been created for this spatial mapping source.
        /// </summary>
        protected List<MeshRenderer> surfaceObjectRenderers = new List<MeshRenderer>();

        /// <summary>
        /// Collection of mesh filters that have been created for this spatial mapping source.
        /// </summary>
        protected List<MeshFilter> surfaceObjectMeshFilters = new List<MeshFilter>();

        /// <summary>
        /// When a mesh is created we will need to create a game object with a minimum 
        /// set of components to contain the mesh.  These are the required component types.
        /// </summary>
        protected Type[] componentsRequiredForSurfaceMesh =
        {
            typeof(MeshFilter),
            typeof(MeshRenderer),
            typeof(MeshCollider)
        };

        /// <summary>
        /// Creates a new surface game object.
        /// </summary>
        /// <param name="mesh">The mesh to attach. Can be null.</param>
        /// <param name="objectName">What to name this object.</param>
        /// <param name="parentObject">What to parent this object to.</param>
        /// <param name="material">What material to use to draw this object.</param>
        /// <returns>The newly created game object.</returns>
        protected GameObject AddSurfaceObject(Mesh mesh, string objectName, Transform parentObject)
        {
            GameObject surface = new GameObject(objectName, componentsRequiredForSurfaceMesh);
            surface.transform.SetParent(parentObject);
            surfaceObjects.Add(surface);

            MeshFilter surfaceMeshFilter = surface.GetComponent<MeshFilter>();
            surfaceMeshFilter.sharedMesh = mesh;
            surfaceObjectMeshFilters.Add(surfaceMeshFilter);


            MeshRenderer surfaceMeshRenderer = surface.GetComponent<MeshRenderer>();
            surfaceMeshRenderer.sharedMaterial = SpatialMappingManager.Instance.SurfaceMaterial;
            surfaceObjectRenderers.Add(surfaceMeshRenderer);

            surface.layer = SpatialMappingManager.Instance.PhysicsLayer;
            return surface;
        }

        /// <summary>
        /// Cleans up references to objecs that we have created.
        /// </summary>
        protected void Cleanup()
        {
            // For renderers and filters, clearing the lists is sufficient, 
            // since renderers and filters are attached to the surface objects
            // that we will call destroy on.
            surfaceObjectRenderers.Clear();
            surfaceObjectMeshFilters.Clear();

            for (int index = 0; index < surfaceObjects.Count; index++)
            {
                Destroy(surfaceObjects[index]);
            }

            surfaceObjects.Clear();
        }

        /// <summary>
        /// Gets all mesh filters that have a valid mesh.
        /// </summary>
        /// <returns>A list of filters, each with a mesh containing at least one triangle.</returns>
        virtual public List<MeshFilter> GetMeshFilters()
        {
            List<MeshFilter> meshFilters = new List<MeshFilter>();

            foreach (MeshFilter filter in surfaceObjectMeshFilters)
            {
                if (filter != null && filter.sharedMesh != null && filter.sharedMesh.vertexCount > 2)
                {
                    meshFilters.Add(filter);
                }
            }

            return meshFilters;
        }

        /// <summary>
        /// Gets all mesh renderers that have been created.
        /// </summary>
        /// <returns></returns>
        virtual public List<MeshRenderer> GetMeshRenderers()
        {
            return surfaceObjectRenderers;
        }
    }
}
