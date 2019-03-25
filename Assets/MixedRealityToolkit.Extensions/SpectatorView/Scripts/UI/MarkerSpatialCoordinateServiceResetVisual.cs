// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Sharing;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
{
    public class MarkerSpatialCoordinateServiceResetVisual : MonoBehaviour,
        IMarkerSpatialCoordinateServiceResetVisual,
        IMobileOverlayVisualChild
    {
        [SerializeField] Button _resetButton;

        public event ResetSpatialCoordinatesHandler ResetSpatialCoordinates;
        public event OverlayVisibilityRequest OverlayVisibilityRequest;

        void Awake()
        {
            if (_resetButton != null)
                _resetButton.onClick.AddListener(OnResetClick);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

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
