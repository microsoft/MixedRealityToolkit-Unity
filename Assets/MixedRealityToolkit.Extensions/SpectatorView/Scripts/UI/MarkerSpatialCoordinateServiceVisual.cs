// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

using Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.SpatialCoordinates;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.UI
{
    public class MarkerSpatialCoordinateServiceVisual : MonoBehaviour,
        IMarkerSpatialCoordinateServiceVisual
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

        public void UpdateVisualState(MarkerSpatialCoordinateServiceVisualState state)
        {
            if (_text == null)
            {
                Debug.LogError("Error: Text not defined for MarkerSpatialCoordinateServiceVisual");
            }

            string text = "";
            switch (state)
            {
                case MarkerSpatialCoordinateServiceVisualState.waitingForUser:
                    text = waitingForUserText;
                    break;
                case MarkerSpatialCoordinateServiceVisualState.locatingLocalOrigin:
                    text = locatingLocalOriginText;
                    break;
                case MarkerSpatialCoordinateServiceVisualState.locatingMarker:
                    text = locatingMarkerText;
                    break;
                case MarkerSpatialCoordinateServiceVisualState.none:
                default:
                    text = "";
                    break;
            }

            Debug.Log("Updating marker spatial coordinate service visual content: " + text);
            _text.text = text;
        }
    }
}
