// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class CanvasGroupService : ComponentBroadcasterService<CanvasGroupService, CanvasGroupObserver>
    {
        public static readonly ShortID ID = new ShortID("CVG");

        public override ShortID GetID() { return ID; }

        private void Start()
        {
            StateSynchronizationSceneManager.Instance.RegisterService(this, new ComponentBroadcasterDefinition<CanvasGroupBroadcaster>(typeof(CanvasGroup)));
        }
    }
}
