// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Sharing;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
{
    /// <summary>
    /// Helper class for UI responsible for resetting the <see cref="MarkerSpatialCoordinateService"/>.
    /// </summary>
    public class MarkerSpatialCoordinateServiceResetVisual : MonoBehaviour,
        IMarkerSpatialCoordinateServiceResetVisual,
        IMobileOverlayVisualChild
    {
        /// <summary>
        /// Button pressed to reset the MarkerSpatialCoordinateService
        /// </summary>
        [Tooltip("Button pressed to reset the MarkerSpatialCoordinateService")]
        [SerializeField]
        Button _resetButton;

        /// <inheritdoc />
        public event ResetSpatialCoordinatesHandler ResetSpatialCoordinates;

        /// <inheritdoc />
        public event OverlayVisibilityRequest OverlayVisibilityRequest;

        protected void Awake()
        {
            if (_resetButton != null)
                _resetButton.onClick.AddListener(OnResetClick);
        }

        /// <inheritdoc />
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <inheritdoc />
        public void Show()
        {
            gameObject.SetActive(true);
        }

        private void OnResetClick()
        {
            ResetSpatialCoordinates?.Invoke();
        }
    }
}
