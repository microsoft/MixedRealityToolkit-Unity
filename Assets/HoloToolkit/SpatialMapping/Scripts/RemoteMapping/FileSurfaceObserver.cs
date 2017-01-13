// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.SpatialMapping
{
    public class FileSurfaceObserver : SpatialMappingSource
    {
        [Tooltip("The file name to use when saving and loading meshes.")]
        public string MeshFileName = "roombackup";

        [Tooltip("Key to press in editor to load a spatial mapping mesh from a .room file.")]
        public KeyCode LoadFileKey = KeyCode.L;

        [Tooltip("Key to press in editor to save a spatial mapping mesh to file.")]
        public KeyCode SaveFileKey = KeyCode.S;

        /// <summary>
        /// Loads the SpatialMapping mesh from the specified file.
        /// </summary>
        /// <param name="fileName">The name, without path or extension, of the file to load.</param>
        public void Load(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
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
                    GameObject surface = AddSurfaceObject(mesh, "storedmesh-" + SurfaceObjects.Count, transform);
                    Renderer meshRenderer = surface.GetComponent<MeshRenderer>();

                    if (SpatialMappingManager.Instance.DrawVisualMeshes == false)
                    {
                        meshRenderer.enabled = false;
                    }

                    if (SpatialMappingManager.Instance.CastShadows == false)
                    {
                        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }

                    // Reset the surface mesh collider to fit the updated mesh. 
                    // Unity tribal knowledge indicates that to change the mesh assigned to a
                    // mesh collider, the mesh must first be set to null.  Presumably there
                    // is a side effect in the setter when setting the shared mesh to null.
                    MeshCollider meshCollider = surface.GetComponent<MeshCollider>();
                    meshCollider.sharedMesh = null;
                    meshCollider.sharedMesh = surface.GetComponent<MeshFilter>().mesh;
                }
            }
            catch
            {
                Debug.Log("Failed to load " + fileName);
            }
        }

        // Called every frame.
        private void Update()
        {
            // Keyboard commands for saving and loading a remotely generated mesh file.
#if UNITY_EDITOR || UNITY_STANDALONE
            // S - saves the active mesh
            if (Input.GetKeyUp(KeyCode.S))
            {
                MeshSaver.Save(MeshFileName, SpatialMappingManager.Instance.GetMeshes());
            }

            // L - loads the previously saved mesh into editor and sets it to be the spatial mapping source.
            if (Input.GetKeyUp(KeyCode.L))
            {
                SpatialMappingManager.Instance.SetSpatialMappingSource(this);
                Load(MeshFileName);
            }
#endif
        }
    }
}