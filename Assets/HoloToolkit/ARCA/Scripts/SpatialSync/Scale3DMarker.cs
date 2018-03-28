// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;

namespace HoloToolkit.ARCapture
{
    public class Scale3DMarker : MonoBehaviour
    {
        [Tooltip("Marker size in meters")]
        public float MarkerSize;
        [Tooltip("An orthoganal camera used for displaying the marker")]
        public Camera OrthographicCamera;

        void Start ()
        {
            if(!OrthographicCamera)
            {
                return;
            }

            float dpi = Screen.dpi;

            // Detect iPhoneX and manually set the screen dpi
            if((Screen.width == 2436 && Screen.height == 1125) || (Screen.height == 2436 && Screen.width == 1125))
            {
                dpi = 458;
            }

            float screenSize = Screen.height;
            float screenWidthInMeters = (screenSize / dpi) * 0.0254f;

            float scale = (OrthographicCamera.orthographicSize * 2.0f) *  MarkerSize / screenWidthInMeters;
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
