// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.Sharing;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
{
    public class MarkerSpatialCoordinateServiceOverlayVisual : MonoBehaviour,
        IMarkerSpatialCoordinateServiceOverlayVisual
    {
        [SerializeField] Text _text;

        const string waitingForUserText = "Waiting for user...";
        const string locatingLocalOriginText = "Locating local origin...";
        const string locatingMarkerText = "Locating marker...";

        public void ShowVisual()
        {
            gameObject.SetActive(true);
        }

        public void HideVisual()
        {
            gameObject.SetActive(false);
        }

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
