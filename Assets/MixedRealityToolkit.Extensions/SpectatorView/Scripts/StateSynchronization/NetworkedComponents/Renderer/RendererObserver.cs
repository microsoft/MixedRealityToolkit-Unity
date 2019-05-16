// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal abstract class RendererObserver<TRenderer, TComponentService> : ComponentObserver<TRenderer>
        where TRenderer : Renderer
        where TComponentService : Singleton<TComponentService>, IComponentBroadcasterService
    {
        public TRenderer Renderer
        {
            get { return attachedComponent; }
        }

        protected virtual void EnsureRenderer(BinaryReader message, byte changeType)
        {
        }

        public override void Read(SocketEndpoint sendingEndpoint, BinaryReader message)
        {
            byte changeType = message.ReadByte();

            EnsureRenderer(message, changeType);

            Read(sendingEndpoint, message, changeType);
        }

        protected virtual void Read(SocketEndpoint sendingEndpoint, BinaryReader message, byte changeType)
        {
            if (RendererBroadcaster<TRenderer, TComponentService>.HasFlag(changeType, RendererBroadcaster<TRenderer, TComponentService>.ChangeType.Enabled) && Renderer)
            {
                Renderer.enabled = message.ReadBoolean();
            }
            if (RendererBroadcaster<TRenderer, TComponentService>.HasFlag(changeType, RendererBroadcaster<TRenderer, TComponentService>.ChangeType.Materials) && Renderer)
            {
                Renderer.materials = MaterialPropertyAsset.ReadMaterials(message, Renderer.materials);
            }
            if (RendererBroadcaster<TRenderer, TComponentService>.HasFlag(changeType, RendererBroadcaster<TRenderer, TComponentService>.ChangeType.MaterialProperty) && Renderer)
            {
                int materialIndex = message.ReadInt32();
                MaterialPropertyAsset.Read(message, Renderer.materials, materialIndex);
            }
        }
    }
}