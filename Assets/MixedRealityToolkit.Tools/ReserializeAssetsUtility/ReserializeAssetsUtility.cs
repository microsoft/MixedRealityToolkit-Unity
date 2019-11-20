// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Adds menu items to automate reserializing specific files in Unity.
    /// </summary>
    /// <remarks>
    /// Reserialization can be needed between Unity versions or when the
    /// underlying script or asset definitions are changed.
    /// </remarks>
    public class ReserializeUtility
    {
        [MenuItem("Mixed Reality Toolkit/Utilities/Reserialize/Prefabs, Scenes, and ScriptableObjects")]
        private static void ReserializePrefabsAndScenes()
        {
            var array = GetAssets("t:Prefab t:Scene t:ScriptableObject");
            AssetDatabase.ForceReserializeAssets(array);
        }

        [MenuItem("Mixed Reality Toolkit/Utilities/Reserialize/Materials and Textures")]
        private static void ReserializeMaterials()
        {
            var array = GetAssets("t:Material t:Texture");
            AssetDatabase.ForceReserializeAssets(array);
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
