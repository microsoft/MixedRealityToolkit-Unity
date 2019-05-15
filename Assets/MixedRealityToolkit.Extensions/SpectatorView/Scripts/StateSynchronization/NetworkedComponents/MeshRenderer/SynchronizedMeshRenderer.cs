// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal abstract class SynchronizedMeshRenderer<TComponentService> : SynchronizedRenderer<MeshRenderer, TComponentService>
        where TComponentService : Singleton<TComponentService>, ISynchronizedComponentService
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
