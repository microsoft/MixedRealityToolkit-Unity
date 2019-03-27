// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
{
    /// <summary>
    /// Base class that provides showing/hiding functionality for <see cref="IMobileOverlayVisualChild"/>
    /// </summary>
    public class MobileOverlayVisualChild : MonoBehaviour,
        IMobileOverlayVisualChild
    {
#pragma warning disable 67
        /// <inheritdoc />
        public event OverlayVisibilityRequest OverlayVisibilityRequest;
#pragma warning restore 67

        /// <inheritdoc />
        public void Show()
        {
            if (gameObject != null)
            {
                gameObject.SetActive(true);
            }
        }

        /// <inheritdoc />
        public void Hide()
        {
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
