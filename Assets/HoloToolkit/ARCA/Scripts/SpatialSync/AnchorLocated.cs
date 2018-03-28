// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;
using UnityEngine.XR.iOS;

namespace HoloToolkit.ARCapture
{
    public class AnchorLocated : MonoBehaviour
    {
        [Tooltip("The 3D marker generator")]
        public ARCAMarkerGenerator3D MarkerGenerator;
        public delegate void AnchorLocatedEvent();
        public AnchorLocatedEvent OnAnchorLocated;

        bool transitioned = false;

        void Start()
        {
            if(MarkerGenerator == null)
            {
                MarkerGenerator = FindObjectOfType<ARCAMarkerGenerator3D>();
            }
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += FrameUpdated;
        }

        void OnDestroy()
        {
            UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FrameUpdated;
        }

        void FrameUpdated(UnityARCamera camera)
        {
            if(camera.pointCloudData.Length > 4)
            {
                if (OnAnchorLocated != null)
                {
                    OnAnchorLocated();
                }
                if(!transitioned)
                {
                    MarkerGenerator.StartTransition();
                    transitioned = true;
                }
            }
        }
    }
}
