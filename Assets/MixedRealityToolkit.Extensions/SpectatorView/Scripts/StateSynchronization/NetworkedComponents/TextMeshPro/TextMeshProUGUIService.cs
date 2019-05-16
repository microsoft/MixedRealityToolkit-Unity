// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if STATESYNC_TEXTMESHPRO
using TMPro;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class TextMeshProUGUIService : ComponentBroadcasterService<TextMeshProUGUIService, TextMeshProUGUIObserver>
    {
        public static readonly ShortID ID = new ShortID("TMU");

        public override ShortID GetID() { return ID; }

#if STATESYNC_TEXTMESHPRO
        private void Start()
        {
            StateSynchronizationSceneManager.Instance.RegisterService(this, new ComponentBroadcasterDefinition<TextMeshProUGUIBroadcaster>(typeof(TextMeshProUGUI)));
        }
#endif
    }
}