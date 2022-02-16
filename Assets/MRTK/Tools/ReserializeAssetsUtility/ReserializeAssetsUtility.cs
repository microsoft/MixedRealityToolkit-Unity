// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Adds menu items to automate reserializing specific files in Unity.
    /// </summary>
    /// <remarks>
    /// <para>Reserialization can be needed between Unity versions or when the
    /// underlying script or asset definitions are changed.</para>
    /// </remarks>
    public class ReserializeUtility
    {
        [MenuItem("Mixed Reality/Toolkit/Utilities/Reserialize/Prefabs, Scenes, and ScriptableObjects")]
        private static void ReserializePrefabsAndScenes()
        {
            string[] array = GetAssets("t:Prefab t:Scene t:ScriptableObject");
            AssetDatabase.ForceReserializeAssets(array);
            Debug.Log($"Reserialized {array.Length} assets.");
        }

        [MenuItem("Mixed Reality/Toolkit/Utilities/Reserialize/Materials and Textures")]
        private static void ReserializeMaterials()
        {
            string[] array = GetAssets("t:Material t:Texture");
            AssetDatabase.ForceReserializeAssets(array);
            Debug.Log($"Reserialized {array.Length} assets.");
        }

        [MenuItem("Mixed Reality/Toolkit/Utilities/Reserialize/Reserialize Selection")]
        [MenuItem("Assets/Mixed Reality/Reserialize Selection")]
        public static void ReserializeSelection()
        {
            Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

            // Transform asset object to asset paths.
            List<string> assetsPath = new List<string>();
            foreach (Object asset in selectedAssets)
            {
                assetsPath.Add(AssetDatabase.GetAssetPath(asset));
            }

            string[] array = assetsPath.ToArray();
            AssetDatabase.ForceReserializeAssets(array);
            Debug.Log($"Reserialized {array.Length} assets.");
        }

        private static string[] GetAssets(string filter)
        {
            string[] allPrefabsGUID = AssetDatabase.FindAssets($"{filter}");

            List<string> allPrefabs = new List<string>();
            foreach (string guid in allPrefabsGUID)
            {
                allPrefabs.Add(AssetDatabase.GUIDToAssetPath(guid));
            }
            return allPrefabs.ToArray();
        }
    }
}
