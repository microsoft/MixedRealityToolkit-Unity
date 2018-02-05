using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace HoloToolkit.UI.Keyboard
{
    public abstract class AtlasReferenceUpdater
    {
        protected abstract IList<string> PrefabPaths { get; }
        protected abstract string AtlasPath { get; }

        public void UpdateAtlasReferences()
        {
            var spriteAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(AtlasPath);
            var prefabs = LoadAssetsAtPaths<GameObject>(PrefabPaths);
            if (spriteAtlas == null)
            {
                Debug.LogWarning("Sprite atlas could not be loaded for reference updating from path: " + AtlasPath);
                return;
            }
            if (prefabs.Any(o => o == null))
            {
                Debug.LogWarning("One or more prefab references could not be loaded for reference updating from path: " + PrefabPaths.Aggregate((curr, next) => curr + "\n" + next));
                return;
            }


            var uniqueSprites = GetUniqueSpritesInAssets(prefabs).ToList();
            var serializedObject = new SerializedObject(spriteAtlas);
            var packables = serializedObject.FindProperty("m_EditorData.packables");

            InsertSprites(packables, uniqueSprites);
            serializedObject.ApplyModifiedProperties();
        }

        private static void InsertSprites(SerializedProperty packables, IList<Sprite> uniqueSprites)
        {
            packables.arraySize = uniqueSprites.Count;
            for (var i = 0; i < uniqueSprites.Count; i++)
            {
                packables.InsertArrayElementAtIndex(i);
                packables.GetArrayElementAtIndex(i).objectReferenceValue = uniqueSprites[i];
            }
        }

        private static IList<T> LoadAssetsAtPaths<T>(IList<string> paths) where T : Object
        {
            var assets = new List<T>();
            foreach (var path in paths)
            {
                assets.Add(AssetDatabase.LoadAssetAtPath<T>(path));
            }
            return assets;
        }

        private static HashSet<Sprite> GetUniqueSpritesInAssets(IList<GameObject> prefabs)
        {
            var imageSet = new HashSet<Sprite>();
            foreach (var prefab in prefabs)
            {
                foreach (var image in prefab.GetComponentsInChildren<Image>())
                {
                    if (!image.sprite) { continue; }
                    imageSet.Add(image.sprite);
                }
            }

            return imageSet;
        }
    }
}
