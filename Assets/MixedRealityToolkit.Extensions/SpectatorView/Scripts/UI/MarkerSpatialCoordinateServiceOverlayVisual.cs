// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Sharing;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
{
    /// <summary>
    /// Helper class for displaying state changes related to the <see cref="Sharing.MarkerSpatialCoordinateService"/>.
    /// </summary>
    public class MarkerSpatialCoordinateServiceOverlayVisual : MonoBehaviour,
        IMarkerSpatialCoordinateServiceOverlayVisual
    {
        /// <summary>
        /// Text UI element for displaying text related to MarkerSpatialCoordinateService state changes.
        /// </summary>
        [Tooltip("Text UI element for displaying text related to MarkerSpatialCoordinateService state changes.")]
        [SerializeField]
        protected Text _text;

        protected string waitingForUserText = "Waiting for user...";
        protected string locatingLocalOriginText = "Locating local origin...";
        protected string locatingMarkerText = "Locating marker...";

        /// <inheritdoc />
        public void ShowVisual()
        {
            gameObject.SetActive(true);
        }

        /// <inheritdoc />
        public void HideVisual()
        {
            gameObject.SetActive(false);
        }

        /// <inheritdoc />
        public void UpdateVisualState(MarkerSpatialCoordinateServiceOverlayState state)
        {
            if (_text == null)
            {
                Debug.LogError("Error: Text not defined for MarkerSpatialCoordinateServiceVisual");
            }

            string text = "";
            switch (state)
            {
                case MarkerSpatialCoordinateServiceOverlayState.waitingForUser:
                    text = waitingForUserText;
                    break;
                case MarkerSpatialCoordinateServiceOverlayState.locatingLocalOrigin:
                    text = locatingLocalOriginText;
                    break;
                case MarkerSpatialCoordinateServiceOverlayState.locatingMarker:
                    text = locatingMarkerText;
                    break;
                case MarkerSpatialCoordinateServiceOverlayState.none:
                default:
                    text = "";
                    break;
            }

            Debug.Log("Updating marker spatial coordinate service visual content: " + text);
            _text.text = text;
        }
    }
}
