// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    internal class RemoteRectTransform : MonoBehaviour
    {
        private SynchronizedRectTransform synchronizedRectTransform;
        private RectTransform rectTransform;

        public void CaptureRectTransform()
        {
            if (synchronizedRectTransform == null)
            {
                synchronizedRectTransform = new SynchronizedRectTransform();
                rectTransform = GetComponent<RectTransform>();
            }
            synchronizedRectTransform.Copy(rectTransform);
        }

        public void Update()
        {
            if (synchronizedRectTransform != null && rectTransform != null)
            {
                synchronizedRectTransform.Apply(rectTransform);
            }
        }
    }
}
