// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness.Utilities
{
    public static class SpatialMeshExporter
    {
        public static async Task Save(string folderPath)
        {
            ThrowIfDoesNotExist(folderPath);

            var meshObservers = (CoreServices.SpatialAwarenessSystem as IMixedRealityDataProviderAccess).GetDataProviders<IMixedRealitySpatialAwarenessMeshObserver>();

            foreach (var observer in meshObservers)
            {
                await Save(observer, folderPath);
            }
        }

        public static async Task Save(IMixedRealitySpatialAwarenessMeshObserver meshObserver, string folderPath)
        {
            ThrowIfDoesNotExist(folderPath);

            await SaveInternal(meshObserver, folderPath);
            /*
            foreach (SpatialAwarenessMeshObject meshObject in meshObserver.Meshes.Values)
            {
                var target = meshObject.GameObject.transform.parent;

                string filePath = folderPath + "/" + target.name + ".obj";
                UnityEngine.Debug.Log(filePath);
                await target.gameObject.ExportOBJAsync(filePath, true);
                break;
            }*/
        }

        private static IEnumerator SaveInternal(IMixedRealitySpatialAwarenessMeshObserver meshObserver, string folderPath)
        {
            foreach (SpatialAwarenessMeshObject meshObject in meshObserver.Meshes.Values)
            {
                var target = meshObject.GameObject.transform.parent;

                string filePath = folderPath + "/" + target.name + ".obj";
                UnityEngine.Debug.Log(filePath);
                yield return target.gameObject.ExportOBJAsync(filePath, true);
                break;
            }
        }

        private static void ThrowIfDoesNotExist(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new Exception($"Directory does not exist at: {folderPath}");
            }
        }
    }
}
