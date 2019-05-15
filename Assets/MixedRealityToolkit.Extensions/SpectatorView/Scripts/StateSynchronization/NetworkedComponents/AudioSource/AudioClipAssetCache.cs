// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    [Serializable]
    internal class AudioClipAsset : AssetCacheEntry<AudioClip> { }

    [Serializable]
    internal class AudioClipAssetCache : AssetCache<AudioClipAsset, AudioClip>
    {
        protected override IEnumerable<AudioClip> EnumerateAllAssets()
        {
            return EnumerateAllAssetsInAssetDatabase<AudioClip>(IsAudioClipFileExtension);
        }

        private static bool IsAudioClipFileExtension(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".mp3":
                case ".ogg":
                case ".wav":
                case ".aiff":
                case ".aif":
                case ".mod":
                case ".it":
                case ".s3m":
                case ".xm":
                    return true;
                default:
                    return false;
            }
        }
    }
}