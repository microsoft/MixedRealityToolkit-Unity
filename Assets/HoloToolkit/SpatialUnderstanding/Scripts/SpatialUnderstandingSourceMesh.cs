// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA;
using HoloToolkit.Unity.SpatialMapping;

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
                mappingObserver.DataReady += MappingObserver_DataReady;
                mappingObserver.SurfaceChanged += MappingObserver_SurfaceChanged;
            }
        }

        /// <summary>
        /// Called when a surface is going to be added, removed, or updated. 
        /// We only care about removal so we can remove our internal copy of the surface mesh.
        /// </summary>
        /// <param name="surfaceId">The surface ID that is being added/removed/updated</param>
        /// <param name="changeType">Added | Removed | Updated</param>
        /// <param name="bounds">The world volume the mesh is in.</param>
        /// <param name="updateTime">When the mesh was updated.</param>
        private void MappingObserver_SurfaceChanged(SurfaceId surfaceId, SurfaceChange changeType, Bounds bounds, DateTime updateTime)
        {
            // We only need to worry about removing meshes from our list.  Adding and updating is 
            // done when the mesh data is actually ready.
            if (changeType == SurfaceChange.Removed)
            {
                int meshIndex = FindMeshIndexInInputMeshList(surfaceId.handle);
                if(meshIndex >= 0)
                {
                    inputMeshList.RemoveAt(meshIndex);
                }
            }
        }

        /// <summary>
        /// Called by the surface observer when a mesh has had its data changed.
        /// </summary>
        /// <param name="bakedData">The data describing the surface.</param>
        /// <param name="outputWritten">If the data was successfully updated.</param>
        /// <param name="elapsedBakeTimeSeconds">How long it took to update.</param>
        private void MappingObserver_DataReady(SurfaceData bakedData, bool outputWritten, float elapsedBakeTimeSeconds)
        {
            if (!outputWritten)
                return;

            AddOrUpdateMeshInList(bakedData);
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

        /// <summary>
        /// Updates an element of the behavior's mesh list from an element of the the spatial mapping's surface object list.
        /// Element will be either added or updated to match up to the surfaceObject list.
        /// </summary>
        /// <param name="surfaceId">The unique ID for the mesh (matches the id provided by spatial mapping)</param>
        /// <param name="surfaceObjectIndex">Index in the surfaceObjects list</param>
        /// <param name="surfaceObjects">The list of surfaceObjects</param>
        /// <param name="meshDataIndex">Index into the locally stored mesh data list</param>
        private void AddOrUpdateMeshInList(
            SurfaceData bakedData)
        {
            SurfaceId surfaceId = bakedData.id;
            MeshFilter meshFilter = bakedData.outputMesh;
            int meshDataIndex = FindMeshIndexInInputMeshList(surfaceId.handle);
            SpatialUnderstandingDll.MeshData meshData = new SpatialUnderstandingDll.MeshData();
            int meshUpdateID = (meshDataIndex >= 0) ? (inputMeshList[meshDataIndex].LastUpdateID + 1) : 1;
            
            if ((meshFilter != null) &&
                (meshFilter.mesh != null) &&
                (meshFilter.mesh.triangles.Length > 0))
            {
                // Fix surface mesh normals so we can get correct plane orientation.
                meshFilter.mesh.RecalculateNormals();

                // Convert
                meshData.CopyFrom(meshFilter, surfaceId.handle, meshUpdateID);
            }
            else
            {
                // No filter yet, add as an empty mesh (will be updated later in the update loop)
                meshData.CopyFrom(null, surfaceId.handle, meshUpdateID);
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

        /// <summary>
        /// Update the internal mesh list and provides an array pointer in
        /// the form the dll will accept.
        /// </summary>
        /// <param name="meshCount">Number of meshes contains in the return mesh list</param>
        /// <param name="meshList">Marshalled mesh list pointer. Valid only with the caller's function context</param>
        /// <returns></returns>
        public bool GetInputMeshList(out int meshCount, out IntPtr meshList)
        {
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