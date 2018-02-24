// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.SpatialMapping.RemoteMapping
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

            try
            {
                IList<Mesh> storedMeshes = MeshSaver.Load(fileName);

                for(int iMesh = 0; iMesh < storedMeshes.Count; iMesh++)
                {
                    AddSurfaceObject(CreateSurfaceObject(
                        mesh: storedMeshes[iMesh],
                        objectName: "storedmesh-" + iMesh,
                        parentObject: transform,
                        meshID: iMesh
                        ));
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
            if (Input.GetKeyUp(SaveFileKey))
            {
                MeshSaver.Save(MeshFileName, SpatialMappingManager.Instance.GetMeshes());
            }

            // L - loads the previously saved mesh into editor and sets it to be the spatial mapping source.
            if (Input.GetKeyUp(LoadFileKey))
            {
                SpatialMappingManager.Instance.SetSpatialMappingSource(this);
                Load(MeshFileName);
            }
#endif
        }
    }
}