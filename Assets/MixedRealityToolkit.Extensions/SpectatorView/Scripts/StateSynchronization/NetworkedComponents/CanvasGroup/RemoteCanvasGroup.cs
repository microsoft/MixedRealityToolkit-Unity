// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RemoteCanvasGroup : RemoteComponent<CanvasGroup>
    {
        public override void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            SynchronizedCanvasGroup.ChangeType changeType = (SynchronizedCanvasGroup.ChangeType)message.ReadByte();

            if (SynchronizedCanvasGroup.HasFlag(changeType, SynchronizedCanvasGroup.ChangeType.Properties))
            {
                attachedComponent.alpha = message.ReadSingle();
                attachedComponent.ignoreParentGroups = message.ReadBoolean();
            }
        }
    }
}
