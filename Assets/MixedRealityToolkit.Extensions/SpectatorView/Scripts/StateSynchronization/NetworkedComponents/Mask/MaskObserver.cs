// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class MaskObserver : ComponentObserver<Mask>
    {
        public override void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            MaskBroadcaster.ChangeType changeType = (MaskBroadcaster.ChangeType)message.ReadByte();

            if (MaskBroadcaster.HasFlag(changeType, MaskBroadcaster.ChangeType.Properties))
            {
                attachedComponent.enabled = message.ReadBoolean();
            }
        }
    }
}
