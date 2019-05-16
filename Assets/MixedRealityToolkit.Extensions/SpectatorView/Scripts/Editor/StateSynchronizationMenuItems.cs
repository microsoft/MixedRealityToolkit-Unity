// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Editor
{
    public static class StateSynchronizationMenuItems
    {
        [MenuItem("Spectator View/Update All Asset Caches", priority = 100)]
        public static void UpdateAllAssetCaches()
        {
            bool assetCacheFound = false;
            var scene = SceneManager.GetActiveScene();
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (IAssetCache assetCache in root.GetComponentsInChildren<IAssetCache>(includeInactive: true))
                {
                    assetCacheFound = true;
                    assetCache.UpdateAssetCache();
                }
            }

            if (!assetCacheFound)
            {
                Debug.LogWarning("No asset caches were found in the scene. Unable to update asset caches.");
                return;
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Asset caches updated.");
        }

        [MenuItem("Spectator View/Clear All Asset Caches", priority = 101)]
        public static void ClearAllAssetCaches()
        {
            bool assetCacheFound = false;
            var scene = SceneManager.GetActiveScene();
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (IAssetCache assetCache in root.GetComponentsInChildren<IAssetCache>(includeInactive: true))
                {
                    assetCacheFound = true;
                    assetCache.ClearAssetCache();
                }
            }

            if (!assetCacheFound)
            {
                Debug.LogWarning("No asset caches were found in the scene. Unable to clear asset caches.");
                return;
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Asset caches cleared.");
        }

        [MenuItem("Spectator View/Edit Global Performance Parameters", priority = 200)]
        private static void EditGlobalPerformanceParameters()
        {
            GameObject prefab = Resources.Load<GameObject>(StateSynchronizationSceneManager.DefaultStateSynchronizationPerformanceParametersPrefabName);
            if (prefab == null)
            {
                GameObject hierarchyPrefab = new GameObject(StateSynchronizationSceneManager.DefaultStateSynchronizationPerformanceParametersPrefabName);
                hierarchyPrefab.AddComponent<DefaultStateSynchronizationPerformanceParameters>();

                AssetCache.EnsureAssetDirectoryExists();
#if UNITY_2018_3_OR_NEWER
                prefab = PrefabUtility.SaveAsPrefabAsset(hierarchyPrefab, AssetCache.GetAssetPath(StateSynchronizationSceneManager.DefaultStateSynchronizationPerformanceParametersPrefabName, ".prefab"));
#else
                prefab = PrefabUtility.CreatePrefab(AssetCache.GetAssetPath(StateSynchronizationSceneManager.DefaultSynchronizationPerformanceParametersPrefabName, ".prefab"), hierarchyPrefab);
#endif
                Object.DestroyImmediate(hierarchyPrefab);
            }

            Selection.activeObject = prefab;
        }

        [MenuItem("Spectator View/Edit Custom Network Services", priority = 201)]
        private static void EditCustomShaderProperties()
        {
            GameObject prefab = Resources.Load<GameObject>(StateSynchronizationSceneManager.CustomBroadcasterServicesPrefabName);
            if (prefab == null)
            {
                GameObject hierarchyPrefab = new GameObject(StateSynchronizationSceneManager.CustomBroadcasterServicesPrefabName);

                AssetCache.EnsureAssetDirectoryExists();
#if UNITY_2018_3_OR_NEWER
                prefab = PrefabUtility.SaveAsPrefabAsset(hierarchyPrefab, AssetCache.GetAssetPath(StateSynchronizationSceneManager.CustomBroadcasterServicesPrefabName, ".prefab"));
#else
                prefab = PrefabUtility.CreatePrefab(AssetCache.GetAssetPath(StateSynchronizationSceneManager.CustomNetworkServicesPrefabName, ".prefab"), hierarchyPrefab);
#endif
                Object.DestroyImmediate(hierarchyPrefab);
            }

            Selection.activeObject = prefab;
        }

        [MenuItem("Spectator View/Edit Settings", priority = 202)]
        private static void EditCustomSettingsProperties()
        {
            GameObject prefab = Resources.Load<GameObject>(StateSynchronizationSceneManager.SettingsPrefabName);
            if (prefab == null)
            {
                GameObject hierarchyPrefab = new GameObject(StateSynchronizationSceneManager.SettingsPrefabName);
                hierarchyPrefab.AddComponent<BroadcasterSettings>();

                AssetCache.EnsureAssetDirectoryExists();
#if UNITY_2018_3_OR_NEWER
                prefab = PrefabUtility.SaveAsPrefabAsset(hierarchyPrefab, AssetCache.GetAssetPath(StateSynchronizationSceneManager.SettingsPrefabName, ".prefab"));
#else
                prefab = PrefabUtility.CreatePrefab(AssetCache.GetAssetPath(StateSynchronizationSceneManager.SettingsPrefabName, ".prefab"), hierarchyPrefab);
#endif
                Object.DestroyImmediate(hierarchyPrefab);
            }

            Selection.activeObject = prefab;
        }
    }
}