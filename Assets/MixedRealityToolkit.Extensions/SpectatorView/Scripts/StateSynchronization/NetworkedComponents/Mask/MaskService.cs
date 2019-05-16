// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class MaskService : ComponentBroadcasterService<MaskService, MaskObserver>
    {
        public static readonly ShortID ID = new ShortID("MSK");

        public override ShortID GetID() { return ID; }

        private void Start()
        {
            StateSynchronizationSceneManager.Instance.RegisterService(this, new ComponentBroadcasterDefinition<MaskBroadcaster>(typeof(Mask)));
        }
    }
}
