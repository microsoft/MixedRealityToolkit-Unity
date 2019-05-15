// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal abstract class RemoteRenderer<TRenderer, TComponentService> : RemoteComponent<TRenderer>
        where TRenderer : Renderer
        where TComponentService : Singleton<TComponentService>, ISynchronizedComponentService
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
            if (SynchronizedRenderer<TRenderer, TComponentService>.HasFlag(changeType, SynchronizedRenderer<TRenderer, TComponentService>.ChangeType.Enabled) && Renderer)
            {
                Renderer.enabled = message.ReadBoolean();
            }
            if (SynchronizedRenderer<TRenderer, TComponentService>.HasFlag(changeType, SynchronizedRenderer<TRenderer, TComponentService>.ChangeType.Materials) && Renderer)
            {
                Renderer.materials = MaterialPropertyAsset.ReadMaterials(message, Renderer.materials);
            }
            if (SynchronizedRenderer<TRenderer, TComponentService>.HasFlag(changeType, SynchronizedRenderer<TRenderer, TComponentService>.ChangeType.MaterialProperty) && Renderer)
            {
                int materialIndex = message.ReadInt32();
                MaterialPropertyAsset.Read(message, Renderer.materials, materialIndex);
            }
        }
    }
}