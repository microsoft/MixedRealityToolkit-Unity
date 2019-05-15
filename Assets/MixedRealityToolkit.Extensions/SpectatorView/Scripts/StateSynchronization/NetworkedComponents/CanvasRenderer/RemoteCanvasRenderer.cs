// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RemoteCanvasRenderer : RemoteComponent<CanvasRenderer>
    {
        public override void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            SynchronizedCanvasRenderer.ChangeType changeType = (SynchronizedCanvasRenderer.ChangeType)message.ReadByte();

            if (SynchronizedCanvasRenderer.HasFlag(changeType, SynchronizedCanvasRenderer.ChangeType.Properties))
            {
                attachedComponent.SetAlpha(message.ReadSingle());
                attachedComponent.SetColor(message.ReadColor());
            }
        }
    }
}
