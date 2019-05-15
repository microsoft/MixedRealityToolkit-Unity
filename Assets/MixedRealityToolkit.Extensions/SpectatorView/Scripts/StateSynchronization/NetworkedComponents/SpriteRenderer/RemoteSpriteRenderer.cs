// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RemoteSpriteRenderer : RemoteRenderer<SpriteRenderer, SpriteRendererService>
    {
        protected override void Read(SocketEndpoint sendingEndpoint, BinaryReader message, byte changeType)
        {
            base.Read(sendingEndpoint, message, changeType);

            if (SynchronizedSpriteRenderer.HasFlag(changeType, SynchronizedSpriteRenderer.SpriteRendererChangeType.Sprite))
            {
                Guid spriteGuid = message.ReadGuid();
                Renderer.sprite = ImageService.Instance.GetSprite(spriteGuid);
            }

            if (SynchronizedSpriteRenderer.HasFlag(changeType, SynchronizedSpriteRenderer.SpriteRendererChangeType.Properties))
            {
                Renderer.adaptiveModeThreshold = message.ReadSingle();
                Renderer.color = message.ReadColor();
                Renderer.drawMode = (SpriteDrawMode)message.ReadByte();
                Renderer.flipX = message.ReadBoolean();
                Renderer.flipY = message.ReadBoolean();
                Renderer.maskInteraction = (SpriteMaskInteraction)message.ReadByte();
                Renderer.size = message.ReadVector2();
                Renderer.tileMode = (SpriteTileMode)message.ReadByte();
            }
        }
    }
}
