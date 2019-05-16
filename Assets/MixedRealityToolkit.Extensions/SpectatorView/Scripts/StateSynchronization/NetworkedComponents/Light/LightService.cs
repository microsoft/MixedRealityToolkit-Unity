// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class LightService : ComponentBroadcasterService<LightService, LightObserver>
    {
        public static readonly ShortID ID = new ShortID("LIT");

        public override ShortID GetID() { return ID; }

        private void Start()
        {
            StateSynchronizationSceneManager.Instance.RegisterService(this, new ComponentBroadcasterDefinition<LightBroadcaster>(typeof(Light)));
        }
    }
}
