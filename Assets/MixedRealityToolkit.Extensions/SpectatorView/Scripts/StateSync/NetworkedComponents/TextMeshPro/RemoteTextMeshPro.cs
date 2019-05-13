// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if STATESYNC_TEXTMESHPRO
using System;
using TMPro;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RemoteTextMeshPro : RemoteTextMeshProBase
    {
        protected override void EnsureTextComponent()
        {
#if STATESYNC_TEXTMESHPRO
            if (RemoteTextMesh == null)
            {
                RemoteTextMesh = ComponentExtensions.EnsureComponent<TextMeshPro>(gameObject);
            }
#endif
        }

#if STATESYNC_TEXTMESHPRO
        public override Type ComponentType => typeof(TextMeshPro);
#endif
    }
}