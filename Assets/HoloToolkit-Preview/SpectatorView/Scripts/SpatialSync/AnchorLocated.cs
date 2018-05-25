// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.XR.iOS;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    /// <summary>
    /// Detects when an anchor has been located
    /// </summary>
    public class AnchorLocated : MonoBehaviour
    {
        /// <summary>
        /// Delegate for when an achor is located
        /// </summary>
        public delegate void AnchorLocatedEvent();

        /// <summary>
        /// The 3D marker generator
        /// </summary>
        [SerializeField]
        [Tooltip("The 3D marker generator")]
        private SpectatorViewMarkerGenerator3D markerGenerator;

        /// <summary>
        /// Callback when an anchor is located by the HoloLens
        /// </summary>
        public AnchorLocatedEvent OnAnchorLocated;

        /// <summary>
        /// Flag to indicated whether the 3D marker has been displayed
        /// </summary>
        private bool transitioned;

        /// <summary>
        /// The 3D marker generator
        /// </summary>
        public SpectatorViewMarkerGenerator3D MarkerGenerator
        {
            get { return markerGenerator; }
            set { markerGenerator = value; }
        }

        private void Start()
        {
#if UNITY_IOS || UNITY_EDITOR
            if (MarkerGenerator == null)
            {
                MarkerGenerator = FindObjectOfType<SpectatorViewMarkerGenerator3D>();
            }
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += FrameUpdated;
#endif
        }

        private void OnDestroy()
        {
#if UNITY_IOS || UNITY_EDITOR
            UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FrameUpdated;
#endif
        }

#if UNITY_IOS || UNITY_EDITOR
        /// <summary>
        /// Called by the API. It checks whether an anchor has been located and signals
        /// the marker generator so that it can create and show an AR marker
        /// </summary>
        /// <param name="arCamera"></param>
        private void FrameUpdated( UnityARCamera arCamera )
        {
            if (arCamera.pointCloudData.Length > 4)
            {
                if (OnAnchorLocated != null) OnAnchorLocated();
                if (!transitioned)
                {
                    MarkerGenerator.StartTransition();
                    transitioned = true;
                }
            }
        }
#endif 
    }
}
