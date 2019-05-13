// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class TextMeshService : SynchronizedComponentService<TextMeshService, RemoteTextMesh>, IAssetCache
    {
        public static readonly ShortID ID = new ShortID("TXT");

        public override ShortID GetID() { return ID; }

        private FontAssetCache fontAssets;

        protected override void Awake()
        {
            base.Awake();

            fontAssets = FontAssetCache.LoadAssetCache<FontAssetCache>();
        }

        private void Start()
        {
            SynchronizedSceneManager.Instance.RegisterService(this, new SynchronizedComponentDefinition<SynchronizedTextMesh>(typeof(TextMesh), typeof(MeshRenderer)));
        }

        public Guid GetFontId(Font font)
        {
            return fontAssets?.GetAssetId(font) ?? Guid.Empty;
        }

        public Font GetFont(Guid guid)
        {
            return fontAssets?.GetAsset(guid);
        }

        public void UpdateAssetCache()
        {
            FontAssetCache.GetOrCreateAssetCache<FontAssetCache>().UpdateAssetCache();
        }

        public void ClearAssetCache()
        {
            FontAssetCache.GetOrCreateAssetCache<FontAssetCache>().ClearAssetCache();
        }
    }
}
