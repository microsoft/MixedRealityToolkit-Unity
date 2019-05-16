// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if STATESYNC_TEXTMESHPRO
using System;
using TMPro;
using UnityEngine;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class TextMeshProUGUIObserver : TextMeshProObserverBase
    {
        protected override void EnsureTextComponent()
        {
#if STATESYNC_TEXTMESHPRO
            if (TextMeshObserver == null)
            {
                RectTransformBroadcaster srt = new RectTransformBroadcaster();
                RectTransform rectTransform = GetComponent<RectTransform>();
                srt.Copy(rectTransform);
                TextMeshObserver = ComponentExtensions.EnsureComponent<TextMeshProUGUI>(gameObject);
                srt.Apply(rectTransform);
            }
#endif
        }

#if STATESYNC_TEXTMESHPRO
        public override Type ComponentType => typeof(TextMeshProUGUI);
#endif
    }
}