// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal abstract class MeshRendererBroadcaster<TComponentService> : RendererBroadcaster<MeshRenderer, TComponentService>
        where TComponentService : Singleton<TComponentService>, IComponentBroadcasterService
    {
        protected override byte InitialChangeType
        {
            get
            {
                return ChangeType.Enabled | ChangeType.Materials;
            }
        }
    }
}
