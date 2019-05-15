// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    [Serializable]
    internal class TextMeshProFontAsset : AssetCacheEntry<ScriptableObject> { }

    internal class TextMeshProFontAssetCache : AssetCache<TextMeshProFontAsset, ScriptableObject>
    {
        private static bool IsAssetFileExtension(string fileExtension)
        {
            return fileExtension == ".asset";
        }

        protected override IEnumerable<ScriptableObject> EnumerateAllAssets()
        {
#if UNITY_EDITOR && STATESYNC_TEXTMESHPRO
            return EnumerateAllAssetsInAssetDatabase<TMPro.TMP_FontAsset>(IsAssetFileExtension);
#else
            yield break;
#endif
        }
    }
}