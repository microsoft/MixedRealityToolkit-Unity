// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
{
    public class MobileOverlayVisualChild : MonoBehaviour,
        IMobileOverlayVisualChild
    {
        public event OverlayVisibilityRequest OverlayVisibilityRequest;

        public void Show()
        {
            if (gameObject != null)
            {
                gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
