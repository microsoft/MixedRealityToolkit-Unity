// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// This script allows to zoom into and pan the texture of a GameObject. 
    /// It also allows for scrolling by restricting panning to one direction.  
    /// </summary>    
    public class Scroll_RectTransf : PanZoomBase_RectTransf
    {
        // Scroll
        [Tooltip("RectTransform from, for example, your TextMeshPro game object.")]
        [SerializeField]
        private RectTransform RectTransfToNavigate = null;

        [Tooltip("Reference to the viewport restricting the viewbox. This is important for identifying the max constrains for panning.")]
        [SerializeField]
        private RectTransform RefToViewPort = null;

        [Tooltip("Ability to scroll using your eye gaze without any additional input (e.g., air tap or button presses).")]
        [SerializeField]
        private bool AutoGazeScrollIsActive = true;

        [Tooltip("Horizontal scroll speed. For example: 0.1f for slow panning. 0.6f for fast panning.")]
        [SerializeField]
        private float ScrollSpeed_x = 0.0f;

        [Tooltip("Vertical scroll speed. For example: 0.1f for slow panning. 0.6f for fast panning.")]
        [SerializeField]
        private float ScrollSpeed_y = 0.3f;

        [Tooltip("Minimal distance in x and y from center of the target's hit box (0, 0) to scroll. Thus, values must range between 0 (always scroll) and 0.5 (no scroll).")]
        [SerializeField]
        private Vector2 MinDistFromCenterForAutoScroll = new Vector2(0.2f, 0.2f);

        [Tooltip("Set to true to prevent sudden scrolling when quickly looking around. This may make scrolling feel less responsive though.")]
        [SerializeField]
        private bool UseSkimProofing = false;

        [Tooltip("The lower the value, the slower the scrolling will speed up after skimming. Recommended value: 5.")]
        [SerializeField]
        [Range(0, 10)]
        private float SkimProofUpdateSpeed = 5f;

        // The base PanAndZoom class can also be used with UV textures for which the dimensions are different to a RectTransform.
        // To allow to keep the speed values that users can assign consistent, let's internally convert the values.
        private float convertSpeedToUVSpace = -200.0f;

        [Tooltip("Custom anchor start position.")]
        [SerializeField]
        public Vector2 CustomStartPos;

        protected override void Start()
        {
            // Assigning values to base PanZoom class
            AutoGazePanIsActive = AutoGazeScrollIsActive;
            PanSpeedUpDown = ScrollSpeed_y * convertSpeedToUVSpace;
            PanSpeedLeftRight = ScrollSpeed_x * convertSpeedToUVSpace;
            MinDistFromCenterForAutoPan = MinDistFromCenterForAutoScroll;
            useSkimProof = UseSkimProofing;

            navRectTransf = RectTransfToNavigate;
            viewportRectTransf = RefToViewPort;

            navRectTransf.anchoredPosition = CustomStartPos;
            isScrollText = true;

            base.Start();
        }

        private void UpdatePivot()
        {
            navRectTransf.pivot = new Vector2(0, 1);
            navRectTransf.anchorMin = new Vector2(0, 1);
            navRectTransf.anchorMax = new Vector2(0, 1);
        }

        protected override void Update()
        {
            UpdatePivot();

            UpdateValues(ref AutoGazePanIsActive, AutoGazeScrollIsActive);
            UpdateValues(ref PanSpeedUpDown, ScrollSpeed_y * convertSpeedToUVSpace);
            UpdateValues(ref PanSpeedLeftRight, ScrollSpeed_x * convertSpeedToUVSpace);
            UpdateValues(ref MinDistFromCenterForAutoPan, MinDistFromCenterForAutoScroll);
            UpdateValues(ref useSkimProof, UseSkimProofing);

            if (UpdateValues(ref skimproof_UpdateSpeedFromUser, SkimProofUpdateSpeed))
            {
                SetSkimProofUpdateSpeed(skimproof_UpdateSpeedFromUser);
            }

            base.Update();
        }
    }
}