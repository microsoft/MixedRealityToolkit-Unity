// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class CameraService : ComponentBroadcasterService<CameraService, CameraObserver>
    {
        public static readonly ShortID ID = new ShortID("CAM");

        public override ShortID GetID() { return ID; }

        private void Start()
        {
            StateSynchronizationSceneManager.Instance.RegisterService(this, new ComponentBroadcasterDefinition<CameraBroadcaster>(typeof(Camera)));
        }
    }
}
