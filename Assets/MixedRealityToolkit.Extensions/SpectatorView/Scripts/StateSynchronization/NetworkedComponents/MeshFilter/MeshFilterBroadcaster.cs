// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class MeshFilterBroadcaster : MeshRendererBroadcaster<MeshFilterService>
    {
        public static class MeshFilterChangeType
        {
            public const byte Mesh = 0x8;
        }

        private MeshFilter meshFilter;
        private Guid assetId;

        protected override byte InitialChangeType
        {
            get { return (byte)(base.InitialChangeType | MeshFilterChangeType.Mesh); }
        }

        protected override void WriteRenderer(BinaryWriter message, byte changeType)
        {
            if (HasFlag(changeType, MeshFilterChangeType.Mesh))
            {
                message.Write(assetId);
            }

            base.WriteRenderer(message, changeType);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            meshFilter = GetComponent<MeshFilter>();
            assetId = AssetService.Instance.GetMeshId(meshFilter.sharedMesh);
            if (assetId == Guid.Empty)
            {
                Debug.LogError("Could not find the Mesh asset for GameObject " + this.gameObject.name + ". Check the NetworkAssetCache and ensure that you're not modifying the mesh by accessing the MeshFilter.mesh property");
            }
        }
    }
}