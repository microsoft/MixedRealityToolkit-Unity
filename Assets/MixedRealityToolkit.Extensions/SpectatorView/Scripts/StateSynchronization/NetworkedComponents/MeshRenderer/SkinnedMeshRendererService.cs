// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class SkinnedMeshRendererService : ComponentBroadcasterService<SkinnedMeshRendererService, SkinnedMeshRendererObserver>
    {
        public static readonly ShortID ID = new ShortID("SKN");

        public override ShortID GetID() { return ID; }

        private void Start()
        {
            StateSynchronizationSceneManager.Instance.RegisterService(this, new ComponentBroadcasterDefinition<SkinnedMeshRendererBroadcaster>(typeof(SkinnedMeshRenderer)));
        }
    }
}
