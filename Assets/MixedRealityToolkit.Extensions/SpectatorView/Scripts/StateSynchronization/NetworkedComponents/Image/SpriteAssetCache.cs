// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    [Serializable]
    internal class SpriteAsset : AssetCacheEntry<Sprite> { }

    internal class SpriteAssetCache : AssetCache<SpriteAsset, Sprite>
    {
        protected override IEnumerable<Sprite> EnumerateAllAssets()
        {
            foreach (Sprite sprite in EnumerateAllAssetsInAssetDatabase<Sprite>(IsImageFileExtension))
            {
                yield return sprite;
            }

            foreach (Image image in EnumerateAllComponentsInScenesAndPrefabs<Image>())
            {
                if (image.sprite != null)
                {
                    yield return image.sprite;
                }
                if (image.overrideSprite != null)
                {
                    yield return image.overrideSprite;
                }
            }

            foreach (Selectable selectable in EnumerateAllComponentsInScenesAndPrefabs<Selectable>())
            {
                if (selectable.spriteState.disabledSprite != null)
                {
                    yield return selectable.spriteState.disabledSprite;
                }
                if (selectable.spriteState.highlightedSprite != null)
                {
                    yield return selectable.spriteState.highlightedSprite;
                }
                if (selectable.spriteState.pressedSprite != null)
                {
                    yield return selectable.spriteState.pressedSprite;
                }
            }
        }

        private static bool IsImageFileExtension(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".psd":
                case ".tga":
                case ".gif":
                case ".bmp":
                case ".pict":
                case ".iff":
                    return true;
                default:
                    return false;
            }
        }
    }
}