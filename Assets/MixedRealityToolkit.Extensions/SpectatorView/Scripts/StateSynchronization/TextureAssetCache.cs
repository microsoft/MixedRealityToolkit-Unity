// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    [Serializable]
    internal class TextureAsset : AssetCacheEntry<Texture> { }

    internal class TextureAssetCache : AssetCache<TextureAsset, Texture>, IAssetSerializer<Texture>
    {
        public static readonly ShortID ID = new ShortID("TAC");

        private static bool IsTextureAssetExtension(string fileExtension)
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

        public ShortID GetID()
        {
            return ID;
        }

        public bool CanSerialize(Texture asset)
        {
            return asset == null || GetAssetId(asset) != Guid.Empty;
        }

        public void Serialize(BinaryWriter writer, Texture asset)
        {
            writer.Write(GetAssetId(asset));
        }

        public Texture Deserialize(BinaryReader reader)
        {
            return GetAsset(reader.ReadGuid());
        }

        protected override IEnumerable<Texture> EnumerateAllAssets()
        {
            return EnumerateAllAssetsInAssetDatabase<Texture>(IsTextureAssetExtension);
        }
    }
}