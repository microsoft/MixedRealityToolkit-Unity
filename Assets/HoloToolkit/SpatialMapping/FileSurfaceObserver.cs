using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public class FileSurfaceObserver : SpatialMappingSource
    {
        [Tooltip("The file name to use when saving and loading meshes.")]
        public string MeshFileName = "roombackup";

        /// <summary>
        /// Loads the SpatialMapping mesh from the specified file.
        /// </summary>
        /// <param name="fileName">The name, without path or extension, of the file to load.</param>
        public void Load(string fileName)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                Debug.Log("No mesh file specified.");
                return;
            }

            Cleanup();

            List<Mesh> storedMeshes = new List<Mesh>();
            try
            {
                storedMeshes.AddRange(MeshSaver.Load(fileName));

                foreach (Mesh mesh in storedMeshes)
                {
                    GameObject surface = AddSurfaceObject(mesh, "storedmesh-" + surfaceObjects.Count, transform);
                    Renderer renderer = surface.GetComponent<MeshRenderer>();

                    if (SpatialMappingManager.Instance.DrawVisualMeshes == false)
                    {
                        renderer.enabled = false;
                    }

                    if(SpatialMappingManager.Instance.CastShadows == false)
                    {
                        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }

                    // Reset the surface mesh collider to fit the updated mesh. 
                    // Unity tribal knowledge indicates that to change the mesh assigned to a
                    // mesh collider, the mesh must first be set to null.  Presumably there
                    // is a side effect in the setter when setting the shared mesh to null.
                    MeshCollider collider = surface.GetComponent<MeshCollider>();
                    collider.sharedMesh = null;
                    collider.sharedMesh = surface.GetComponent<MeshFilter>().mesh;
                }
            }
            catch
            {
                Debug.Log("Failed to load " + fileName);
            }
        }
    }
}
