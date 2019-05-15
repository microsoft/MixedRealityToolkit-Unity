// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class CanvasService : ComponentBroadcasterService<CanvasService, CanvasObserver>
    {
        public static readonly ShortID ID = new ShortID("CAN");

        public override ShortID GetID() { return ID; }

        private void Start()
        {
            StateSynchronizationSceneManager.Instance.RegisterService(this, new ComponentBroadcasterDefinition<CanvasBroadcaster>(typeof(Canvas)));
        }
    }
}
