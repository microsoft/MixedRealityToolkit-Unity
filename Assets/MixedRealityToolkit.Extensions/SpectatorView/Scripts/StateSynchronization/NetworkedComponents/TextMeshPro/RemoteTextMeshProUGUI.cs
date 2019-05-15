// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if STATESYNC_TEXTMESHPRO
using System;
using TMPro;
using UnityEngine;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RemoteTextMeshProUGUI : RemoteTextMeshProBase
    {
        protected override void EnsureTextComponent()
        {
#if STATESYNC_TEXTMESHPRO
            if (RemoteTextMesh == null)
            {
                SynchronizedRectTransform srt = new SynchronizedRectTransform();
                RectTransform rectTransform = GetComponent<RectTransform>();
                srt.Copy(rectTransform);
                RemoteTextMesh = ComponentExtensions.EnsureComponent<TextMeshProUGUI>(gameObject);
                srt.Apply(rectTransform);
            }
#endif
        }

#if STATESYNC_TEXTMESHPRO
        public override Type ComponentType => typeof(TextMeshProUGUI);
#endif
    }
}