// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Rendering;

namespace HoloToolkit.Unity.SpatialMapping
{
    public class ObjectSurfaceObserver : SpatialMappingSource
    {
        [Tooltip("The room model to use when loading meshes in Unity.")]
        public GameObject RoomModel;

        // Use this for initialization.
        private void Start()
        {
#if UNITY_EDITOR
            // When in the Unity editor, try loading saved meshes from a model.
            Load(RoomModel);

            if (GetMeshFilters().Count > 0)
            {
                SpatialMappingManager.Instance.SetSpatialMappingSource(this);
            }
#endif
        }

        /// <summary>
        /// Loads the SpatialMapping mesh from the specified room object.
        /// </summary>
        /// <param name="roomModel">The room model to load meshes from.</param>
        public void Load(GameObject roomModel)
        {
            if (roomModel == null)
            {
                Debug.Log("No room model specified.");
                return;
            }

            GameObject roomObject = Instantiate(roomModel);
            Cleanup();

            try
            {
                MeshFilter[] roomFilters = roomObject.GetComponentsInChildren<MeshFilter>();

                foreach (MeshFilter filter in roomFilters)
                {
                    GameObject surface = AddSurfaceObject(filter.sharedMesh, "roomMesh-" + SurfaceObjects.Count, transform);
                    Renderer meshRenderer = surface.GetComponent<MeshRenderer>();

                    if (SpatialMappingManager.Instance.DrawVisualMeshes == false)
                    {
                        meshRenderer.enabled = false;
                    }

                    if (SpatialMappingManager.Instance.CastShadows == false)
                    {
                        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                    }

                    // Reset the surface mesh collider to fit the updated mesh. 
                    // Unity tribal knowledge indicates that to change the mesh assigned to a
                    // mesh collider, the mesh must first be set to null.  Presumably there
                    // is a side effect in the setter when setting the shared mesh to null.
                    MeshCollider meshCollider = surface.GetComponent<MeshCollider>();
                    meshCollider.sharedMesh = null;
                    meshCollider.sharedMesh = surface.GetComponent<MeshFilter>().sharedMesh;
                }
            }
            catch
            {
                Debug.Log("Failed to load object " + roomModel.name);
            }
            finally
            {
                if (roomObject != null)
                {
                    DestroyImmediate(roomObject);
                }
            }
        }
    }
}