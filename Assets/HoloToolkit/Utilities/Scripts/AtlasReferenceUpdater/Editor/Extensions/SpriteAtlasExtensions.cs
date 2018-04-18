// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace HoloToolkit.Unity
{
    public static class SpriteAtlasExtensions
    {
#if UNITY_2017_1_OR_NEWER
        public const string SpritePackables = "m_EditorData.packables";

        public static void SetSprites(this SpriteAtlas spriteAtlas, IList<Sprite> sprites)
        {
            var serializedObject = new SerializedObject(spriteAtlas);
            var packables = serializedObject.FindProperty(SpritePackables);
            packables.SetObjects(sprites);
            serializedObject.ApplyModifiedProperties();
        }

        public static bool ContainsSprite(this SpriteAtlas spriteAtlas, Sprite sprite)
        {
            var serializedObject = new SerializedObject(spriteAtlas);
            var packables = serializedObject.FindProperty(SpritePackables);
            for (var i = 0; i < packables.arraySize; i++)
            {
                var containedSprite = packables.GetArrayElementAtIndex(i).objectReferenceValue as Sprite;
                if (sprite != containedSprite) { continue; }
                return true;
            }
            return false;
        }
#endif // UNITY_2017_1_OR_NEWER
    }
}
