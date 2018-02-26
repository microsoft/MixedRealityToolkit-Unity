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
        private static readonly List<AtlasPrefabReference> References = new List<AtlasPrefabReference>();

        static AtlasReferenceUpdater()
        {
            References.Clear();
            var references = Resources.FindObjectsOfTypeAll<AtlasPrefabReference>();
            foreach (var atlasPrefabReference in references)
            {
                if (atlasPrefabReference.Atlas == null)
                {
                    Debug.LogWarning("No sprite atlas referenced.");
                    continue;
                }
                if (atlasPrefabReference.Prefabs.Any(o => o == null))
                {
                    Debug.LogWarning("One or more prefab references are null");
                    continue;
                }
                References.Add(atlasPrefabReference);
            }

            PrefabUtility.prefabInstanceUpdated += PrefabInstanceUpdated;
        }

        private static void PrefabInstanceUpdated(GameObject instance)
        {
            var prefabForInstance = PrefabUtility.GetPrefabParent(instance);
            foreach (var atlasPrefabReference in References)
            {
                foreach (var gameObject in atlasPrefabReference.Prefabs)
                {
                    if (gameObject == prefabForInstance)
                    {
                        UpdateAtlasReferences(atlasPrefabReference);
                    }
                }
            }
        }

        public static void UpdateAtlasReferences(AtlasPrefabReference reference)
        {
            UpdateAtlasReferences(reference, Enumerable.Empty<Sprite>());
        }

        public static void UpdateAtlasReferences(AtlasPrefabReference reference, IEnumerable<Sprite> ignoreSprites)
        {
            var uniqueSprites = GetDistinctSprites(reference.Prefabs).ToList();
            foreach (var ignoreSprite in ignoreSprites)
            {
                uniqueSprites.Remove(ignoreSprite);
            }
            reference.Atlas.SetSprites(uniqueSprites);
        }

        private static IEnumerable<Sprite> GetDistinctSprites(IEnumerable<GameObject> prefabs)
        {
            return prefabs.SelectMany(p => p.GetComponentsInChildren<Image>())
                .Select(i => i.sprite).Where(i => i != null).Distinct();
        }

        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            var deletedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            if (deletedSprite == null) { return AssetDeleteResult.DidNotDelete; }
            foreach (var atlasPrefabReference in References)
            {
                if (!atlasPrefabReference.Atlas.ContainsSprite(deletedSprite)) { continue; }
                UpdateAtlasReferences(atlasPrefabReference, new List<Sprite> { deletedSprite });
                break;
            }
            return AssetDeleteResult.DidNotDelete;
        }
    }
}
