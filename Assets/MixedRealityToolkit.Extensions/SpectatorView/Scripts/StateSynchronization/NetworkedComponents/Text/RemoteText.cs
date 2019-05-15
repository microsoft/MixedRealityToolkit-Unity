// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RemoteText : RemoteComponent<Text>
    {
        public override void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            SynchronizedText.ChangeType changeType = (SynchronizedText.ChangeType)message.ReadByte();

            if (SynchronizedText.HasFlag(changeType, SynchronizedText.ChangeType.Text))
            {
                attachedComponent.text = message.ReadString();
            }
            if (SynchronizedText.HasFlag(changeType, SynchronizedText.ChangeType.FontAndPlacement))
            {
                attachedComponent.alignment = (TextAnchor)message.ReadByte();
                attachedComponent.color = message.ReadColor();
                attachedComponent.fontSize = message.ReadInt32();
                attachedComponent.fontStyle = (FontStyle)message.ReadByte();
                attachedComponent.lineSpacing = message.ReadSingle();
                attachedComponent.horizontalOverflow = (HorizontalWrapMode)message.ReadByte();
                attachedComponent.verticalOverflow = (VerticalWrapMode)message.ReadByte();
                attachedComponent.font = TextService.Instance.GetFont(message.ReadGuid());
            }
            if (SynchronizedText.HasFlag(changeType, SynchronizedText.ChangeType.Materials))
            {
                attachedComponent.material = MaterialPropertyAsset.ReadMaterials(message, null)?[0];
            }
            if (SynchronizedText.HasFlag(changeType, SynchronizedText.ChangeType.MaterialProperty))
            {
                int materialIndex = message.ReadInt32();
                MaterialPropertyAsset.Read(message, new Material[] { attachedComponent.material }, materialIndex);
            }
        }
    }
}