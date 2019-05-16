// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class LineRendererService : ComponentBroadcasterService<LineRendererService, LineRendererObserver>
    {
        public static readonly ShortID ID = new ShortID("LIN");

        public override ShortID GetID() { return ID; }

        private void Start()
        {
            StateSynchronizationSceneManager.Instance.RegisterService(this, new ComponentBroadcasterDefinition<LineRendererBroadcaster>(typeof(LineRenderer)));
        }

        public override void LerpRead(BinaryReader message, GameObject mirror, float lerpVal)
        {
            LineRendererObserver comp = mirror.GetComponent<LineRendererObserver>();
            if (comp)
                comp.LerpRead(message, lerpVal);
        }
    }
}
