// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if STATESYNC_TEXTMESHPRO
using System;
using TMPro;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class TextMeshProService : ComponentBroadcasterService<TextMeshProService, TextMeshProObserver>, IAssetCache
    {
        public static readonly ShortID ID = new ShortID("TMP");

        public override ShortID GetID() { return ID; }

#if STATESYNC_TEXTMESHPRO
        private TextMeshProFontAssetCache fontAssets;

        protected override void Awake()
        {
            base.Awake();

            fontAssets = TextMeshProFontAssetCache.LoadAssetCache<TextMeshProFontAssetCache>();
        }

        private void Start()
        {
            StateSynchronizationSceneManager.Instance.RegisterService(this, new ComponentBroadcasterDefinition<TextMeshProBroadcaster>(typeof(TextMeshPro)));
        }

        public Guid GetFontId(TMP_FontAsset font)
        {
            return fontAssets?.GetAssetId(font) ?? Guid.Empty;
        }

        public TMP_FontAsset GetFont(Guid guid)
        {
            return (TMP_FontAsset)fontAssets?.GetAsset(guid);
        }
#endif

        public void UpdateAssetCache()
        {
            TextMeshProFontAssetCache.GetOrCreateAssetCache<TextMeshProFontAssetCache>().UpdateAssetCache();
        }

        public void ClearAssetCache()
        {
            TextMeshProFontAssetCache.GetOrCreateAssetCache<TextMeshProFontAssetCache>().ClearAssetCache();
        }
    }
}
