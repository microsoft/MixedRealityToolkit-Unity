// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using System.IO;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.MarkerDetection
{
    public class QRCodeMarkerVisual : MonoBehaviour,
        IMarkerVisual
    {
        /// <summary>
        /// RawImage used for displaying the ArUco marker
        /// </summary>
        [Tooltip("RawImage used for displaying the ArUco marker")]
        [SerializeField]
        protected RawImage _rawImage;

        /// <summary>
        /// Physical size to display the marker (in meters)
        /// </summary>
        [Tooltip("Physical size to display the marker (in meters)")]
        [SerializeField]
        protected float _markerSize = 0.03f; // meters

        /// <summary>
        /// Any additional scale factor to account for when displaying the marker
        /// </summary>
        [Tooltip("Any additional scale factor to account for when displaying the marker")]
        [SerializeField]
        public float _additionalScaleFactor = 1.0f;

        private float _paddingScaling =  200.0f / (200.0f - (2.0f * 24.0f)); // sv*.png images have 24 pixels of padding
        private const string _textureFileName = "sv*";

        /// <inheritdoc />
        public void SetMarkerSize(float size)
        {
            _markerSize = size;

            if (_rawImage != null)
            {
                var sizeInPixels = GetMarkerSizeInPixels();
                _rawImage.rectTransform.sizeDelta = new Vector2(sizeInPixels, sizeInPixels);
            }
        }

        /// <inheritdoc />
        public void ShowMarker(int id)
        {
            if (_rawImage == null)
            {
                Debug.LogError("Rawimage was not set for QRCodeMarkerVisual. Unable to show marker.");
                return;
            }

            gameObject.SetActive(true);
            var textureFileName = _textureFileName.Replace("*", id.ToString());

            Texture2D texture;
            if (_rawImage != null &&
                TryLoadTexture(textureFileName, out texture))
            {
                _rawImage.texture = texture;
                var size = GetMarkerSizeInPixels();
                _rawImage.rectTransform.sizeDelta = new Vector2(size, size);
            }
            else
            {
                Debug.LogError("Failed to load texture: " + textureFileName);
            }
        }

        /// <inheritdoc />
        public void HideMarker()
        {
            if (_rawImage == null)
            {
                Debug.LogError("RawImage was not set for QRCodeMarkerVisual. Unable to hide marker.");
                return;
            }

            gameObject.SetActive(false);
        }

        private float GetMarkerSizeInPixels()
        {
            float dpi = Screen.dpi;

#if UNITY_IOS
            // Screen.dpi returns an incorrect value for the iPhoneX
            // Look for screens with its dimensions (in both orientations)
            // and manually set the screen dpi here.
            if ((Screen.width == 2436 && Screen.height == 1125) || (Screen.height == 2436 && Screen.width == 1125))
            {
                dpi = 458;
            }
#endif
            float screenWidth = Screen.width;
            float screenWidthInMeters = (screenWidth / dpi) * 0.0254f;
            float correctedMarkerSize = _markerSize * _paddingScaling;
            float markerWidthPercentageOfScreen = correctedMarkerSize / screenWidthInMeters;
            float markerWidthInPixels = (markerWidthPercentageOfScreen * Screen.width);
            float markerWidthInPixelsScaled = markerWidthInPixels * _additionalScaleFactor;

            Debug.Log("Calculating QR Code Marker Size, Screen Dimensions: " + Screen.width + "x" + Screen.height +
                ", dpi: " + Screen.dpi + ", Screen width in meters: " + screenWidthInMeters +
                ", Marker width in pixels: " + markerWidthInPixels + ", Final marker width in pixels: " + markerWidthInPixelsScaled);

            return markerWidthInPixelsScaled;
        }

        private bool TryLoadTexture(string fileName, out Texture2D texture)
        {
            texture = Resources.Load<Texture2D>(fileName);
            return (texture != null);
        }
    }
}
