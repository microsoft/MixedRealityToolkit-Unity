// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class SynchronizedSkinnedMeshRenderer : SynchronizedRenderer<SkinnedMeshRenderer, SkinnedMeshRendererService>
    {
        public static class SkinnedMeshRendererChangeType
        {
            public const byte Bones = 0x8;
            public const byte Mesh = 0x10;
        }

        public bool BonesReady { get; private set;}

        public Guid NetworkAssetId
        {
            get { return AssetService.Instance.GetMeshId(Renderer.sharedMesh); }
        }

        protected override bool IsRendererEnabled
        {
            get { return base.IsRendererEnabled && this.BonesReady; }
        }

        protected override byte InitialChangeType
        {
            get
            {
                return ChangeType.Materials | ChangeType.Enabled | SkinnedMeshRendererChangeType.Mesh;
            }
        }

        protected override void SendCompleteChanges(IEnumerable<SocketEndpoint> endpoints)
        {
            base.SendCompleteChanges(endpoints);

            TrySendBones(endpoints);
        }

        protected override void SendDeltaChanges(IEnumerable<SocketEndpoint> endpoints, byte changeFlags)
        {
            base.SendDeltaChanges(endpoints, changeFlags);

            if (!BonesReady)
            {
                TrySendBones(endpoints);
            }
        }

        private bool TrySendBones(IEnumerable<SocketEndpoint> endpoints)
        {
            BonesReady = false;
            Transform[] bones = Renderer.bones;

            //Make sure we have transforms ready for all our bones
            foreach(var b in bones)
            {
                if (b.GetComponent<SynchronizedTransform>() == null)
                    return false;

            }
            BonesReady = true;
            SendDeltaChanges(endpoints, SkinnedMeshRendererChangeType.Bones);
            return true;
        }

        protected override void WriteRenderer(BinaryWriter message, byte changeType)
        {
            if (HasFlag(changeType, SkinnedMeshRendererChangeType.Mesh))
            {
                message.Write(NetworkAssetId);
            }

            base.WriteRenderer(message, changeType);

            if (HasFlag(changeType, SkinnedMeshRendererChangeType.Bones))
            {
                Transform[] bones = Renderer.bones;
                int numBones = bones.Length;
                message.Write((UInt16)numBones);
                for (int i = 0; i < numBones; i++)
                {
                    SynchronizedTransform rootBoneTransform = bones[i].GetComponent<SynchronizedTransform>();
                    message.Write(rootBoneTransform.Id);
                }
            }
        }
    }
}
