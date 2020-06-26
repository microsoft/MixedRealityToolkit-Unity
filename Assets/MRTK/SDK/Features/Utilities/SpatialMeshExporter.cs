// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness.Utilities
{
    /// <summary>
    /// Utility to export current Spatial Mesh Data to OBJ file
    /// </summary>
    public static class SpatialMeshExporter
    {
        /// <summary>
        /// Save spatial mesh data for all observers under the current Spatial Awareness system 
        /// </summary>
        /// <param name="folderPath">Absolute folder path to place OBJ files</param>
        /// <param name="consolidate">If true, attempts to consolidate all meshes per Observer into one OBJ file. If false, creates an OBJ file per mesh object on each observer</param>
        /// <remarks>
        /// Accessing GameObject/Mesh data will occur as Coroutine on Unity Main thread. May impact performance.
        /// If folder path does not exist, throws exception
        /// </remarks>
        public static async Task Save(string folderPath, bool consolidate = true)
        {
            CreateFoldersIfDoesNotExist(folderPath);

            var meshObservers = (CoreServices.SpatialAwarenessSystem as IMixedRealityDataProviderAccess).GetDataProviders<IMixedRealitySpatialAwarenessMeshObserver>();

            foreach (var observer in meshObservers)
            {
                await Save(observer, folderPath, consolidate);
            }
        }

        /// <summary>
        /// Save spatial mesh data for given observer to folder path provided
        /// </summary>
        /// <param name="meshObserver">Observer to target for requests of spatial mesh data</param>
        /// <param name="folderPath">Folder path to pull all OBJ files</param>
        /// <param name="consolidate">if true, merge all mesh data from observer into one OBJ file. If false, create OBJ file per mesh object</param>
        /// <remarks>
        /// Accessing GameObject/Mesh data will occur as Coroutine on Unity Main thread. May impact performance.
        /// If folder path does not exist, throws exception
        /// </remarks>
        public static async Task Save(IMixedRealitySpatialAwarenessMeshObserver meshObserver, string folderPath, bool consolidate = true)
        {
            CreateFoldersIfDoesNotExist(folderPath);

            await SaveInternal(meshObserver, folderPath, consolidate);
        }

        private static IEnumerator SaveInternal(IMixedRealitySpatialAwarenessMeshObserver meshObserver, string folderPath, bool consolidate = true)
        {
            var targets = new HashSet<Transform>();

            // Build up unique set of GameObjects to target to pull Mesh data
            foreach (SpatialAwarenessMeshObject meshObject in meshObserver.Meshes.Values)
            {
                targets.Add(consolidate ? meshObject.GameObject.transform.parent : meshObject.GameObject.transform);
            }

            foreach (var target in targets)
            {
                string filePath = Path.Combine(folderPath, target.name + ".obj");
                yield return target.gameObject.ExportOBJAsync(filePath, true);
            }
        }

        private static void CreateFoldersIfDoesNotExist(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
    }
}
