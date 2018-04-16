// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity
{
    [InitializeOnLoad]
    public class AtlasReferenceUpdater : UnityEditor.AssetModificationProcessor
    {
#if UNITY_2017_1_OR_NEWER
        private static readonly List<AtlasPrefabReference> References = new List<AtlasPrefabReference>();

        static AtlasReferenceUpdater()
        {
            FindAtlasPrefabReferences();
            UpdateAllReferences();
            PrefabUtility.prefabInstanceUpdated += PrefabInstanceUpdated;
        }

        private static void FindAtlasPrefabReferences()
        {
            References.Clear();
            foreach (var reference in FindAllAssets<AtlasPrefabReference>())
            {
                if (reference.Atlas == null)
                {
                    Debug.LogWarning("No sprite atlas referenced: " + reference.name);
                    continue;
                }
                if (reference.Prefabs.Any(o => o == null))
                {
                    Debug.LogWarning("One or more prefab references are null: " + reference.name);
                    continue;
                }
                References.Add(reference);
            }
        }

        private static void UpdateAllReferences()
        {
            foreach (var atlasPrefabReference in References)
            {
                UpdateAtlasReferences(atlasPrefabReference);
            }
        }

        private static void PrefabInstanceUpdated(GameObject instance)
        {
            var prefab = PrefabUtility.GetPrefabParent(instance) as GameObject;
            foreach (var atlasPrefabReference in References)
            {
                if (atlasPrefabReference.Prefabs.Contains(prefab))
                {
                    UpdateAtlasReferences(atlasPrefabReference);
                }
            }
        }
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            var atlasReference = AssetDatabase.LoadAssetAtPath<AtlasPrefabReference>(assetPath);
            if (atlasReference != null)
            {
                EditorApplication.delayCall += FindAtlasPrefabReferences;
                return AssetDeleteResult.DidNotDelete;
            }

            var deletedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (deletedSprite != null)
            {
                foreach (var reference in References.Where(_ref => _ref.Atlas.ContainsSprite(deletedSprite)))
                {
                    var localRef = reference;
                    EditorApplication.delayCall += () => UpdateAtlasReferences(localRef);
                }
                return AssetDeleteResult.DidNotDelete;
            }

            return AssetDeleteResult.DidNotDelete;
        }

        private static string[] OnWillSaveAssets(string[] paths)
        {
            if (paths.Select(AssetDatabase.LoadAssetAtPath<AtlasPrefabReference>).Any(_ref => _ref != null))
            {
                EditorApplication.delayCall += FindAtlasPrefabReferences;
                EditorApplication.delayCall += UpdateAllReferences;
            }
            return paths;
        }

        private static IEnumerable<T> FindAllAssets<T>() where T : UnityEngine.Object
        {
            var type = typeof(T).FullName;
            foreach (var assetGuid in AssetDatabase.FindAssets("t:" + type))
            {
                var obj = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(assetGuid));
                if (obj != null)
                {
                    yield return obj;
                }
            }
        }

        public static void UpdateAtlasReferences(AtlasPrefabReference reference)
        {
            var uniqueSprites = GetDistinctSprites(reference.Prefabs).ToList();
            reference.Atlas.SetSprites(uniqueSprites);
        }

        private static IEnumerable<Sprite> GetDistinctSprites(IEnumerable<GameObject> prefabs)
        {
            return prefabs.SelectMany(p => p.GetComponentsInChildren<Image>())
                .Select(img => img.sprite).Where(img => img != null).Distinct();
        }
#endif // UNITY_2017_1_OR_NEWER
    }
}
