// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    /// <summary>
    /// Manages the scale of the marker to fit in different screen sizes
    /// </summary>
    public class Scale3DMarker : MonoBehaviour
    {
        /// <summary>
        /// Marker size in meters
        /// </summary>
        [Tooltip("Marker size in meters")]
        [SerializeField]
        private float markerSize;

        /// <summary>
        /// An orthoganal camera used for displaying the marker
        /// </summary>
        [Tooltip("An orthoganal camera used for displaying the marker")]
        [SerializeField]
        private Camera orthographicCamera;

        /// <summary>
        /// Marker size in meters
        /// </summary>
        public float MarkerSize
        {
            get { return markerSize; }
            set { markerSize = value; }
        }

        /// <summary>
        /// An orthoganal camera used for displaying the marker
        /// </summary>
        public Camera OrthographicCamera
        {
            get { return orthographicCamera; }
            set { orthographicCamera = value; }
        }

        private void Start()
        {
            if (!OrthographicCamera)
            {
                return;
            }

            float dpi = Screen.dpi;

            // Screen.dpi returns an incorrect value for the iPhoneX
            // Look for screens with its dimensions (in both orientations)
            // and manually set the screen dpi here.
            if ((Screen.width == 2436 && Screen.height == 1125) || (Screen.height == 2436 && Screen.width == 1125))
            {
                dpi = 458;
            }

            float screenSize = Screen.height;
            float screenWidthInMeters = (screenSize / dpi) * 0.0254f;

            float scale = (OrthographicCamera.orthographicSize * 2.0f) * MarkerSize / screenWidthInMeters;
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
