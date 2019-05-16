// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class TextMeshObserver : MeshRendererObserver<TextMeshService>
    {
        private TextMesh textMesh;

        protected override void EnsureRenderer(BinaryReader message, byte changeType)
        {
            if (textMesh == null)
            {
                textMesh = gameObject.AddComponent<TextMesh>();
            }
        }

        protected override void Read(SocketEndpoint sendingEndpoint, BinaryReader message, byte changeType)
        {
            if (TextMeshBroadcaster.HasFlag(changeType, TextMeshBroadcaster.TextMeshChangeType.Text))
            {
                textMesh.text = message.ReadString();
            }
            if (TextMeshBroadcaster.HasFlag(changeType, TextMeshBroadcaster.TextMeshChangeType.FontAndPlacement))
            {
                textMesh.alignment = (TextAlignment)message.ReadByte();
                textMesh.anchor = (TextAnchor)message.ReadByte();
                textMesh.characterSize = message.ReadSingle();
                textMesh.color = message.ReadColor();
                textMesh.fontSize = message.ReadInt32();
                textMesh.fontStyle = (FontStyle)message.ReadByte();
                textMesh.lineSpacing = message.ReadSingle();
                textMesh.offsetZ = message.ReadSingle();
                textMesh.richText = message.ReadBoolean();
                textMesh.tabSize = message.ReadSingle();
                textMesh.font = TextMeshService.Instance.GetFont(message.ReadGuid());

                if (textMesh.font != null)
                {
                    Renderer.material = textMesh.font.material;
                }
            }

            base.Read(sendingEndpoint, message, changeType);
        }
    }
}