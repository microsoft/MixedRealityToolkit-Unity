// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class LineRendererService : SynchronizedComponentService<LineRendererService, RemoteLineRenderer>
    {
        public static readonly ShortID ID = new ShortID("LIN");

        public override ShortID GetID() { return ID; }

        private void Start()
        {
            SynchronizedSceneManager.Instance.RegisterService(this, new SynchronizedComponentDefinition<SynchronizedLineRenderer>(typeof(LineRenderer)));
        }

        public override void LerpRead(BinaryReader message, GameObject mirror, float lerpVal)
        {
            RemoteLineRenderer comp = mirror.GetComponent<RemoteLineRenderer>();
            if (comp)
                comp.LerpRead(message, lerpVal);
        }
    }
}
