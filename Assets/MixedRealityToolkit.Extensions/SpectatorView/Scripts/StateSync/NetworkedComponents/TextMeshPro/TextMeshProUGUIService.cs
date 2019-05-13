// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if STATESYNC_TEXTMESHPRO
using TMPro;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class TextMeshProUGUIService : SynchronizedComponentService<TextMeshProUGUIService, RemoteTextMeshProUGUI>
    {
        public static readonly ShortID ID = new ShortID("TMU");

        public override ShortID GetID() { return ID; }

#if STATESYNC_TEXTMESHPRO
        private void Start()
        {
            SynchronizedSceneManager.Instance.RegisterService(this, new SynchronizedComponentDefinition<SynchronizedTextMeshProUGUI>(typeof(TextMeshProUGUI)));
        }
#endif
    }
}