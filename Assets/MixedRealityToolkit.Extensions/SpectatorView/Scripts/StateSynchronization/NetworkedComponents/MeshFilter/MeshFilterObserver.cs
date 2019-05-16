// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class MeshFilterObserver : MeshRendererObserver<MeshFilterService>
    {
        protected override void EnsureRenderer(BinaryReader message, byte changeType)
        {
            if (MeshFilterBroadcaster.HasFlag(changeType, MeshFilterBroadcaster.MeshFilterChangeType.Mesh))
            {
                Guid assetId = message.ReadGuid();
                AssetService.Instance.AttachMeshFilter(this.gameObject, assetId);
            }

            base.EnsureRenderer(message, changeType);
        }
    }
}