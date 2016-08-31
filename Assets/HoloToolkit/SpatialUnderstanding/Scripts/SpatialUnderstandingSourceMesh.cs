// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine.VR.WSA;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Provides the input meshes to the spatial understanding dll.
    /// The component relies on the spatial mapping module. It maintains
    /// a mesh list in the required dll format which is updated from 
    /// the spatial mapping's SurfaceObject list.
    /// </summary>
    public class SpatialUnderstandingSourceMesh : MonoBehaviour
    {
        // Privates
        /// <summary>
        /// Internal list of meshes that is passed to the dll. This is
        /// kept up to date with spatial mapping's SurfaceObject list.
        /// </summary>
        private List<SpatialUnderstandingDll.MeshData> inputMeshList = new List<SpatialUnderstandingDll.MeshData>();

        // Functions
        private void Start()
        {
            // Register for change events on the mapping manager
            SpatialMappingObserver mappingObserver = SpatialMappingManager.Instance.Source as SpatialMappingObserver;
            if (mappingObserver != null)
            {
                mappingObserver.SurfaceChanged += OnSurfaceChanged;
            }
        }

        private int FindMeshIndexInInputMeshList(int meshID)
        {
            for (int i = 0; i < inputMeshList.Count; ++i)
            {
                if (inputMeshList[i].MeshID == meshID)
                {
                    return i;
                }
            }
            return -1;
        }

        private int FindSurfaceIndexInList(int surfaceObjectID, List<SpatialMappingSource.SurfaceObject> surfaceObjects)
        {
            for (int i = 0; i < surfaceObjects.Count; ++i)
            {
                if (surfaceObjects[i].ID == surfaceObjectID)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Updates an element of the behavior's mesh list from an element of the the spatial mapping's surface object list.
        /// Element will be either added or updated to match up to the surfaceObject list.
        /// </summary>
        /// <param name="surfaceId">The unique ID for the mesh (matches the id provided by spatial mapping)</param>
        /// <param name="surfaceObjectIndex">Index in the surfaceObjects list</param>
        /// <param name="surfaceObjects">The list of surfaceObjects</param>
        /// <param name="meshDataIndex">Index into the locally stored mesh data list</param>
        private void AddOrUpdateMeshInList(
            int surfaceId, 
            int surfaceObjectIndex, 
            List<SpatialMappingSource.SurfaceObject> surfaceObjects,
            int meshDataIndex = -1)
        {
            SpatialUnderstandingDll.MeshData meshData = new SpatialUnderstandingDll.MeshData();
            int meshUpdateID = (meshDataIndex > 0) ? (inputMeshList[meshDataIndex].LastUpdateID + 1) : 0;

            // Checks.
            MeshFilter meshFilter = surfaceObjects[surfaceObjectIndex].Filter;
            if ((meshFilter != null) &&
                (meshFilter.mesh != null) &&
                (meshFilter.mesh.triangles.Length > 0))
            {
                // Fix surface mesh normals so we can get correct plane orientation.
                meshFilter.mesh.RecalculateNormals();

                // Convert
                meshData.CopyFrom(meshFilter, surfaceId, meshUpdateID);
            }
            else
            {
                // No filter yet, add as an empty mesh (will be updated later in the update loop)
                meshData.CopyFrom(null, surfaceId, meshUpdateID);
            }

            // And add it (unless an index of an update item is specified)
            if (meshDataIndex < 0)
            {
                inputMeshList.Add(meshData);
            }
            else
            {
                inputMeshList[meshDataIndex] = meshData;
            }
        }

        private void OnSurfaceChanged(SurfaceId surfaceId, SurfaceChange changeType, Bounds bounds, DateTime updateTime)
        {
            // Find the surface
            List<SpatialMappingSource.SurfaceObject> surfaceObjects = SpatialMappingManager.Instance.GetSurfaceObjects();

            // Find it (in both lists)
            int surfaceObjectIndex = FindSurfaceIndexInList(surfaceId.handle, surfaceObjects);
            int meshDataIndex = FindMeshIndexInInputMeshList(surfaceId.handle);

            // Deal with the change
            switch (changeType)
            {
                case SurfaceChange.Added:
                    {
                        if (surfaceObjectIndex >= 0 && surfaceObjectIndex < surfaceObjects.Count)
                        {
                            AddOrUpdateMeshInList(surfaceId.handle, surfaceObjectIndex, surfaceObjects);
                        }
                    }
                    break;
                case SurfaceChange.Removed:
                    {
                        if (meshDataIndex >= 0 && meshDataIndex < inputMeshList.Count)
                        {
                            inputMeshList.RemoveAt(meshDataIndex);
                        }
                    }
                    break;
                case SurfaceChange.Updated:
                    {
                        if ((surfaceObjectIndex >= 0 && surfaceObjectIndex < surfaceObjects.Count) &&
                            (meshDataIndex >= 0 && meshDataIndex < inputMeshList.Count))
                        {
                            AddOrUpdateMeshInList(surfaceId.handle, surfaceObjectIndex, surfaceObjects, meshDataIndex);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Update the internal mesh list from spatial mapping's surface object list.
        /// </summary>
        private void UpdateInputMeshList()
        {
            List<SpatialMappingSource.SurfaceObject> surfaceObjects = SpatialMappingManager.Instance.GetSurfaceObjects();

            // If we have any meshes with zero indices, but with filters
            // that indicates that the filters are now ready to be processed
            for (int i = 0; i < inputMeshList.Count; ++i)
            {
                if ((inputMeshList[i].Indices == null) ||
                    (inputMeshList[i].Indices.Length == 0))
                {
                    int surfaceObjectIndex = FindSurfaceIndexInList(inputMeshList[i].MeshID, surfaceObjects);
                    if ((surfaceObjectIndex > 0) &&
                        (surfaceObjects[surfaceObjectIndex].UpdateID != inputMeshList[i].LastUpdateID))
                    {
                        AddOrUpdateMeshInList(inputMeshList[i].MeshID, surfaceObjectIndex, surfaceObjects, i);
                    }
                }
            }
        }

        /// <summary>
        /// Update the internal mesh list and provides an array pointer in
        /// the form the dll will accept.
        /// </summary>
        /// <param name="meshCount">Number of meshes contains in the return mesh list</param>
        /// <param name="meshList">Marshalled mesh list pointer. Valid only with the caller's function context</param>
        /// <returns></returns>
        public bool GetInputMeshList(out int meshCount, out IntPtr meshList)
        {
            // First, update our mesh data
            UpdateInputMeshList();
            if (inputMeshList.Count == 0)
            {
                meshCount = 0;
                meshList = IntPtr.Zero;
                return false;
            }

            // Convert to IntPtr
            SpatialUnderstandingDll dll = SpatialUnderstanding.Instance.UnderstandingDLL;
            meshCount = inputMeshList.Count;
            meshList = dll.PinMeshDataForMarshalling(inputMeshList);

            return true;
        }
    }
}