// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// This script allows to scroll a texture both horizontally and vertically.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/PanZoomRectTransf")]
    public class PanZoomRectTransf : PanZoomBaseRectTransf
    {
        [Tooltip("RectTransform from, for example, your TextMeshPro game object.")]
        [SerializeField]
        private RectTransform rectTransfToNavigate = null;

        [Tooltip("Reference to the viewport restricting the viewbox. This is important for identifying the max constrains for panning.")]
        [SerializeField]
        private RectTransform refToViewPort = null;

        // Zoom
        [Tooltip("Zoom acceleration defining the steepness of logistic speed function mapping.")]
        [SerializeField]
        private float zoomAcceleration = 10f;

        [Tooltip("Maximum zoom speed.")]
        [SerializeField]
        private float zoomSpeedMax = 0.02f;

        [Tooltip("Minimum scale of the texture for zoom in - e.g., 0.5f (half the original size)")]
        [SerializeField]
        private float zoomMinScale = 0.1f;

        [Tooltip("Maximum scale of the texture for zoom out - e.g., 1f (the original size) or 2.0f (double the original size).")]
        [SerializeField]
        private float zoomMaxScale = 1.0f;

        [Tooltip("Timed zoom: Once triggered, a zoom in/out will be performed for the given amount of time in seconds.")]
        [SerializeField]
        private float zoomTimeInSecToZoom = 0.5f;

        [Tooltip("Enable or disable hand gestures for zooming on startup.")]
        [SerializeField]
        private bool zoomGestureEnabledOnStartup = false;

        // Pan
        [Tooltip("Ability to scroll using your eye gaze without any additional input (e.g., air tap or button presses).")]
        [SerializeField]
        private bool panAutoScrollIsActive = true;

        [Tooltip("Horizontal scroll speed. For example: 0.1f for slow panning. 0.6f for fast panning.")]
        [SerializeField]
        private float panSpeedHorizontal = 0.3f;

        [Tooltip("Vertical scroll speed. For example: 0.1f for slow panning. 0.6f for fast panning.")]
        [SerializeField]
        private float panSpeedVertical = 0.3f;

        [Tooltip("Minimal distance in x and y from center of the target's hit box (0, 0) to scroll. Thus, values must range between 0 (always scroll) and 0.5 (no scroll).")]
        [SerializeField]
        private Vector2 panMinDistFromCenter = new Vector2(0.2f, 0.2f);

        [Tooltip("Set to true to prevent sudden scrolling when quickly looking around. This may make scrolling feel less responsive though.")]
        [SerializeField]
        private bool useSkimProofing = false;

        [Tooltip("The lower the value, the slower the scrolling will speed up after skimming. Recommended value: 5.")]
        [SerializeField]
        [Range(0, 10)]
        private float skimProofUpdateSpeed = 5f;

        protected override void Start()
        {
            // Assigning values to base PanZoom class
            // Zoom
            ZoomAcceleration = zoomAcceleration;
            ZoomSpeedMax = zoomSpeedMax;
            ZoomMinScale = zoomMinScale;
            ZoomMaxScale = zoomMaxScale;
            ZoomGestureEnabledOnStartup = zoomGestureEnabledOnStartup;
            timeInSecondsToZoom = zoomTimeInSecToZoom;

            // Pan
            autoGazePanIsActive = panAutoScrollIsActive;
            panSpeedLeftRight = panSpeedHorizontal;
            panSpeedUpDown = panSpeedVertical;
            minDistFromCenterForAutoPan = panMinDistFromCenter;
            useSkimProof = useSkimProofing;

            // Set up rect transform
            viewportRectTransf = refToViewPort;
            navRectTransf = rectTransfToNavigate;
            navRectTransf.anchorMin = new Vector2(0.5f, 0.5f);
            navRectTransf.anchorMax = new Vector2(0.5f, 0.5f);
            navRectTransf.pivot = new Vector2(0.5f, 0.5f);

            base.Start();
        }

        protected override void Update()
        {
            UpdateValues(ref navRectTransf, rectTransfToNavigate);
            UpdateValues(ref ZoomAcceleration, zoomAcceleration);
            UpdateValues(ref ZoomSpeedMax, zoomSpeedMax);
            UpdateValues(ref ZoomMinScale, zoomMinScale);
            UpdateValues(ref ZoomMaxScale, zoomMaxScale);
            UpdateValues(ref timeInSecondsToZoom, zoomTimeInSecToZoom);

            UpdateValues(ref autoGazePanIsActive, panAutoScrollIsActive);
            UpdateValues(ref panSpeedLeftRight, panSpeedHorizontal);
            UpdateValues(ref panSpeedUpDown, panSpeedVertical);
            UpdateValues(ref minDistFromCenterForAutoPan, panMinDistFromCenter);
            UpdateValues(ref useSkimProof, useSkimProofing);

            if (UpdateValues(ref skimproof_UpdateSpeedFromUser, skimProofUpdateSpeed))
            {
                SetSkimProofUpdateSpeed(skimproof_UpdateSpeedFromUser);
            }

            base.Update();
        }
    }
}
