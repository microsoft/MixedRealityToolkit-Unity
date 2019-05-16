// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class ImageService : ComponentBroadcasterService<ImageService, ImageObserver>, IAssetCache
    {
        public static readonly ShortID ID = new ShortID("IMG");

        public override ShortID GetID() { return ID; }

        private SpriteAssetCache spriteAssets;

        protected override void Awake()
        {
            base.Awake();

            spriteAssets = SpriteAssetCache.LoadAssetCache<SpriteAssetCache>();
        }

        private void Start()
        {
            StateSynchronizationSceneManager.Instance.RegisterService(this, new ComponentBroadcasterDefinition<ImageBroadcaster>(typeof(Image)));
        }

        public override void LerpRead(BinaryReader message, GameObject mirror, float lerpVal)
        {
            ImageObserver comp = mirror.GetComponent<ImageObserver>();
            if (comp)
                comp.LerpRead(message, lerpVal);
        }


        public Guid GetSpriteId(Sprite sprite)
        {
            return spriteAssets?.GetAssetId(sprite) ?? Guid.Empty;
        }

        public Sprite GetSprite(Guid guid)
        {
            return spriteAssets?.GetAsset(guid);
        }

        public void UpdateAssetCache()
        {
            SpriteAssetCache.GetOrCreateAssetCache<SpriteAssetCache>().UpdateAssetCache();
        }

        public void ClearAssetCache()
        {
            SpriteAssetCache.GetOrCreateAssetCache<SpriteAssetCache>().ClearAssetCache();
        }
    }
}
