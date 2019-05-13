// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RemoteCanvas : RemoteComponent<Canvas>
    {
        public override void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            SynchronizedCanvas.ChangeType changeType = (SynchronizedCanvas.ChangeType)message.ReadByte();

            if (SynchronizedCanvas.HasFlag(changeType, SynchronizedCanvas.ChangeType.Enabled))
            {
                attachedComponent.enabled = message.ReadBoolean();
            }

            if (SynchronizedCanvas.HasFlag(changeType, SynchronizedCanvas.ChangeType.Properties))
            {
                attachedComponent.renderMode = (RenderMode)message.ReadByte();
                attachedComponent.sortingLayerID = message.ReadInt32();
                attachedComponent.sortingOrder = message.ReadInt32();
                attachedComponent.overrideSorting = message.ReadBoolean();
            }
        }
    }
}
