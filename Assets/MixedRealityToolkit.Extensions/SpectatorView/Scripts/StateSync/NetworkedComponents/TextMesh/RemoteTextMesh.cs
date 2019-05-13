// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RemoteTextMesh : RemoteMeshRenderer<TextMeshService>
    {
        private TextMesh remoteTextMesh;

        protected override void EnsureRenderer(BinaryReader message, byte changeType)
        {
            if (remoteTextMesh == null)
            {
                remoteTextMesh = gameObject.AddComponent<TextMesh>();
            }
        }

        protected override void Read(SocketEndpoint sendingEndpoint, BinaryReader message, byte changeType)
        {
            if (SynchronizedTextMesh.HasFlag(changeType, SynchronizedTextMesh.TextMeshChangeType.Text))
            {
                remoteTextMesh.text = message.ReadString();
            }
            if (SynchronizedTextMesh.HasFlag(changeType, SynchronizedTextMesh.TextMeshChangeType.FontAndPlacement))
            {
                remoteTextMesh.alignment = (TextAlignment)message.ReadByte();
                remoteTextMesh.anchor = (TextAnchor)message.ReadByte();
                remoteTextMesh.characterSize = message.ReadSingle();
                remoteTextMesh.color = message.ReadColor();
                remoteTextMesh.fontSize = message.ReadInt32();
                remoteTextMesh.fontStyle = (FontStyle)message.ReadByte();
                remoteTextMesh.lineSpacing = message.ReadSingle();
                remoteTextMesh.offsetZ = message.ReadSingle();
                remoteTextMesh.richText = message.ReadBoolean();
                remoteTextMesh.tabSize = message.ReadSingle();
                remoteTextMesh.font = TextMeshService.Instance.GetFont(message.ReadGuid());

                if (remoteTextMesh.font != null)
                {
                    Renderer.material = remoteTextMesh.font.material;
                }
            }

            base.Read(sendingEndpoint, message, changeType);
        }
    }
}