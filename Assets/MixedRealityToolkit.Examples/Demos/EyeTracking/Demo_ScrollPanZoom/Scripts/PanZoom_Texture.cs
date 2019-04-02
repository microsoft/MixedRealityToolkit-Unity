// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// This script allows to scroll a texture both horizontally and vertically.
    /// </summary>
    [RequireComponent(typeof(EyeTrackingTarget))]
    public class PanZoom_Texture : PanZoomBase_Texture
    {
        [Tooltip("Referenced renderer of the texture to be navigated.")]
        [SerializeField]
        private Renderer RendererOfTextureToBeNavigated = null;

        // Zoom
        [Tooltip("Zoom acceleration defining the steepness of logistic speed function mapping.")]
        [SerializeField]
        private float Zoom_Acceleration = 10f;

        [Tooltip("Maximum zoom speed.")]
        [SerializeField]
        private float Zoom_SpeedMax = 0.02f;

        [Tooltip("Minimum scale of the texture for zoom in - e.g., 0.5f (half the original size)")]
        [SerializeField]
        private float Zoom_MinScale = 0.1f;

        [Tooltip("Maximum scale of the texture for zoom out - e.g., 1f (the original size) or 2.0f (double the original size).")]
        [SerializeField]
        private float Zoom_MaxScale = 1.0f;

        [Tooltip("Timed zoom: Once triggered, a zoom in/out will be performed for the given amount of time in seconds.")]
        [SerializeField]
        private float Zoom_TimeInSecToZoom = 0.5f;

        [Tooltip("Type of hand gesture to use to zoom in/out.")]
        [SerializeField]
        private MixedRealityInputAction Zoom_Gesture = MixedRealityInputAction.None;

        // Pan
        [Tooltip("Ability to scroll using your eye gaze without any additional input (e.g., air tap or button presses).")]
        [SerializeField]
        private bool Pan_AutoScrollIsActive = true;

        [Tooltip("Horizontal scroll speed. For example: 0.1f for slow panning. 0.6f for fast panning.")]
        [SerializeField]
        private float Pan_Speed_x = 0.3f;

        [Tooltip("Vertical scroll speed. For example: 0.1f for slow panning. 0.6f for fast panning.")]
        [SerializeField]
        private float Pan_Speed_y = 0.3f;

        [Tooltip("Minimal distance in x and y from center of the target's hit box (0, 0) to scroll. Thus, values must range between 0 (always scroll) and 0.5 (no scroll).")]
        [SerializeField]
        private Vector2 Pan_MinDistFromCenter = new Vector2(0.2f, 0.2f);

        [Tooltip("Set to true to prevent sudden scrolling when quickly looking around. This may make scrolling feel less responsive though.")]
        [SerializeField]
        private bool UseSkimProofing = false;

        [Tooltip("The lower the value, the slower the scrolling will speed up after skimming. Recommended value: 5.")]
        [SerializeField]
        [Range(0, 10)]
        private float SkimProofUpdateSpeed = 5f;

        protected override void Start()
        {
            // Assigning values to base PanZoom class
            textureRederer = RendererOfTextureToBeNavigated;

            ZoomAcceleration = Zoom_Acceleration;
            ZoomSpeedMax = Zoom_SpeedMax;
            ZoomMinScale = Zoom_MinScale;
            ZoomMaxScale = Zoom_MaxScale;
            ZoomGesture = Zoom_Gesture;
            timeInSecondsToZoom = Zoom_TimeInSecToZoom;

            AutoGazePanIsActive = Pan_AutoScrollIsActive;
            PanSpeedLeftRight = Pan_Speed_x;
            PanSpeedUpDown = Pan_Speed_y;
            MinDistFromCenterForAutoPan = Pan_MinDistFromCenter;
            useSkimProof = UseSkimProofing;

            base.Start();
        }

        protected override void Update()
        {
            UpdateValues(ref textureRederer, RendererOfTextureToBeNavigated);

            UpdateValues(ref ZoomAcceleration, Zoom_Acceleration);
            UpdateValues(ref ZoomSpeedMax, Zoom_SpeedMax);
            UpdateValues(ref ZoomMinScale, Zoom_MinScale);
            UpdateValues(ref ZoomMaxScale, Zoom_MaxScale);
            UpdateValues(ref ZoomGesture, Zoom_Gesture);
            UpdateValues(ref timeInSecondsToZoom, Zoom_TimeInSecToZoom);

            UpdateValues(ref AutoGazePanIsActive, Pan_AutoScrollIsActive);
            UpdateValues(ref PanSpeedLeftRight, Pan_Speed_x);
            UpdateValues(ref PanSpeedUpDown, Pan_Speed_y);
            UpdateValues(ref MinDistFromCenterForAutoPan, Pan_MinDistFromCenter);
            UpdateValues(ref useSkimProof, UseSkimProofing);

            if (UpdateValues(ref skimproof_UpdateSpeedFromUser, SkimProofUpdateSpeed))
            {
                SetSkimProofUpdateSpeed(skimproof_UpdateSpeedFromUser);
            }

            base.Update();
        }
    }
}