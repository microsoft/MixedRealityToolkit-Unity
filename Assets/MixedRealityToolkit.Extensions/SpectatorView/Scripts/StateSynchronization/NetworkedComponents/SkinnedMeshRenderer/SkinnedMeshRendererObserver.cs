// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class SkinnedMeshRendererObserver : RendererObserver<SkinnedMeshRenderer, SkinnedMeshRendererService>
    {
        protected override void EnsureRenderer(BinaryReader message, byte changeType)
        {
            // SkinnedMeshRenderer seems to compute the updateWhenOffscreen state based on the wrong camera.
            // Always update the skinned mesh so that it updates correctly in the compositor's camera.
            Renderer.updateWhenOffscreen = true;

            if (SkinnedMeshRendererBroadcaster.HasFlag(changeType, SkinnedMeshRendererBroadcaster.SkinnedMeshRendererChangeType.Mesh))
            {
                Guid networkAssetId = message.ReadGuid();
                if (!AssetService.Instance.AttachSkinnedMeshRenderer(this.gameObject, networkAssetId))
                {
                    Debug.Log("Missing mesh for:" + gameObject.name);
                }
            }
        }

        protected override void Read(SocketEndpoint sendingEndpoint, BinaryReader message, byte changeType)
        {
            base.Read(sendingEndpoint, message, changeType);

            if (SkinnedMeshRendererBroadcaster.HasFlag(changeType, SkinnedMeshRendererBroadcaster.SkinnedMeshRendererChangeType.Bones) && Renderer != null)
            {
                Transform[] boneTransforms = new Transform[message.ReadUInt16()];
                for (int i = 0; i < boneTransforms.Length; i++)
                {
                    GameObject rootBoneObj = StateSynchronizationSceneManager.Instance.FindGameObjectWithId(message.ReadInt16());
                    boneTransforms[i] = rootBoneObj ? rootBoneObj.transform : null;
                }
                Renderer.bones = boneTransforms;
            }
        }
    }
}
