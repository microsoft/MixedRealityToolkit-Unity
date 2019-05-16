// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class TextMeshProBroadcaster : TextMeshProBroadcasterBase<TextMeshProService>
    {
#if STATESYNC_TEXTMESHPRO
        protected override void Awake()
        {
            base.Awake();

            // Make sure there are no MeshFilterBroadcasters enabled on this component or its children.
            // The TextMeshPro manages these mesh filters and synchronizing the values will break
            // TextMeshPro on the other end.
            foreach (MeshFilterBroadcaster MeshFilterBroadcaster in GetComponentsInChildren<MeshFilterBroadcaster>(includeInactive: true))
            {
                MeshFilterBroadcaster.enabled = false;
            }
        }
#endif
    }
}