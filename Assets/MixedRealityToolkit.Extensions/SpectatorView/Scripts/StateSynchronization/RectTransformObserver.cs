// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RectTransformObserver : MonoBehaviour
    {
        private RectTransformBroadcaster RectTransformBroadcaster;
        private RectTransform rectTransform;

        public void CaptureRectTransform()
        {
            if (RectTransformBroadcaster == null)
            {
                RectTransformBroadcaster = new RectTransformBroadcaster();
                rectTransform = GetComponent<RectTransform>();
            }
            RectTransformBroadcaster.Copy(rectTransform);
        }

        public void Update()
        {
            if (RectTransformBroadcaster != null && rectTransform != null)
            {
                RectTransformBroadcaster.Apply(rectTransform);
            }
        }
    }
}
