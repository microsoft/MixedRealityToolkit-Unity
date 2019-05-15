// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Editor
{
    public static class StateSynchronizationMenuItems
    {
        [MenuItem("Spectator View/Update All Asset Caches", priority = 100)]
        public static void UpdateAllAssetCaches()
        {
            var sceneManager = SynchronizedSceneManager.Instance;
            if (sceneManager == null)
            {
                Debug.LogWarning("SynchronizedSceneManager was not found in scene. Is the SynchronizedSceneManager active in the current scene?");
                return;
            }

            sceneManager.UpdateAllAssetCaches();
        }

        [MenuItem("Spectator View/Clear All Asset Caches", priority = 101)]
        public static void ClearAllAssetCaches()
        {
            var sceneManager = SynchronizedSceneManager.Instance;
            if (sceneManager == null)
            {
                Debug.LogWarning("SynchronizedSceneManager was not found in scene. Is the SynchronizedSceneManager active in the current scene?");
                return;
            }

            sceneManager.UpdateAllAssetCaches();
        }

        [MenuItem("Spectator View/Edit Global Performance Parameters", priority = 200)]
        private static void EditGlobalPerformanceParameters()
        {
            GameObject prefab = Resources.Load<GameObject>(SynchronizedSceneManager.DefaultSynchronizationPerformanceParametersPrefabName);
            if (prefab == null)
            {
                GameObject hierarchyPrefab = new GameObject(SynchronizedSceneManager.DefaultSynchronizationPerformanceParametersPrefabName);
                hierarchyPrefab.AddComponent<DefaultSynchronizationPerformanceParameters>();

                AssetCache.EnsureAssetDirectoryExists();
#if UNITY_2018_3_OR_NEWER
                prefab = PrefabUtility.SaveAsPrefabAsset(hierarchyPrefab, AssetCache.GetAssetPath(SynchronizedSceneManager.DefaultSynchronizationPerformanceParametersPrefabName, ".prefab"));
#else
                prefab = PrefabUtility.CreatePrefab(AssetCache.GetAssetPath(SynchronizedSceneManager.DefaultSynchronizationPerformanceParametersPrefabName, ".prefab"), hierarchyPrefab);
#endif
                Object.DestroyImmediate(hierarchyPrefab);
            }

            Selection.activeObject = prefab;
        }

        [MenuItem("Spectator View/Edit Custom Network Services", priority = 201)]
        private static void EditCustomShaderProperties()
        {
            GameObject prefab = Resources.Load<GameObject>(SynchronizedSceneManager.CustomNetworkServicesPrefabName);
            if (prefab == null)
            {
                GameObject hierarchyPrefab = new GameObject(SynchronizedSceneManager.CustomNetworkServicesPrefabName);

                AssetCache.EnsureAssetDirectoryExists();
#if UNITY_2018_3_OR_NEWER
                prefab = PrefabUtility.SaveAsPrefabAsset(hierarchyPrefab, AssetCache.GetAssetPath(SynchronizedSceneManager.CustomNetworkServicesPrefabName, ".prefab"));
#else
                prefab = PrefabUtility.CreatePrefab(AssetCache.GetAssetPath(SynchronizedSceneManager.CustomNetworkServicesPrefabName, ".prefab"), hierarchyPrefab);
#endif
                Object.DestroyImmediate(hierarchyPrefab);
            }

            Selection.activeObject = prefab;
        }

        [MenuItem("Spectator View/Edit Settings", priority = 202)]
        private static void EditCustomSettingsProperties()
        {
            GameObject prefab = Resources.Load<GameObject>(SynchronizedSceneManager.SettingsPrefabName);
            if (prefab == null)
            {
                GameObject hierarchyPrefab = new GameObject(SynchronizedSceneManager.SettingsPrefabName);
                hierarchyPrefab.AddComponent<BroadcasterSettings>();

                AssetCache.EnsureAssetDirectoryExists();
#if UNITY_2018_3_OR_NEWER
                prefab = PrefabUtility.SaveAsPrefabAsset(hierarchyPrefab, AssetCache.GetAssetPath(SynchronizedSceneManager.SettingsPrefabName, ".prefab"));
#else
                prefab = PrefabUtility.CreatePrefab(AssetCache.GetAssetPath(SynchronizedSceneManager.SettingsPrefabName, ".prefab"), hierarchyPrefab);
#endif
                Object.DestroyImmediate(hierarchyPrefab);
            }

            Selection.activeObject = prefab;
        }

        private static string ConvertToResourcePath(string assetPath)
        {
            string resourcesFolderName = "MixedRealityToolkit.Extensions/SpectatorView/Resources/";
            string lowerAssetPath = assetPath.ToLowerInvariant();
            int resIdx = lowerAssetPath.LastIndexOf(resourcesFolderName);
            if (resIdx >= 0)
            {
                assetPath = assetPath.Substring(resIdx + resourcesFolderName.Length);
                int dotIdx = assetPath.LastIndexOf('.');
                if (dotIdx >= 0)
                {
                    assetPath = assetPath.Substring(0, dotIdx);
                }
            }
            return assetPath;
        }
    }
}