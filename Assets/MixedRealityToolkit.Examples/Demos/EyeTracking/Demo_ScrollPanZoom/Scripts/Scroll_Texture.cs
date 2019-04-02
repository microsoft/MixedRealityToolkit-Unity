// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// This script allows to scroll a texture both horizontally and vertically.
    /// </summary>
    [RequireComponent(typeof(EyeTrackingTarget))]
    public class Scroll_Texture : PanZoomBase_Texture
    {
        // Pan
        [Tooltip("Renderer of the texture to be scrolled.")]
        [SerializeField]
        private Renderer TextureRendererToBeScrolled = null;

        [Tooltip("Ability to scroll using your eye gaze without any additional input (e.g., air tap or button presses).")]
        [SerializeField]
        private bool AutoGazeScrollIsActive = true;

        [Tooltip("Horizontal scroll speed. For example: 0.1f for slow panning. 0.6f for fast panning.")]
        [SerializeField]
        private float ScrollSpeed_x = 0.0f;

        [Tooltip("Vertical scroll speed. For example: 0.1f for slow panning. 0.6f for fast panning.")]
        [SerializeField]
        private float ScrollSpeed_y = 0.15f;

        [Tooltip("Minimal distance in x and y from center of the target's hit box (0, 0) to scroll. Thus, values must range between 0 (always scroll) and 0.5 (no scroll).")]
        [SerializeField]
        private Vector2 MinDistFromCenterForAutoScroll = new Vector2(0.2f, 0.2f);

        [Tooltip("Size of the GameObject's collider when being looked at.")]
        [SerializeField]
        private Vector3 OnLookAt_ColliderSize = new Vector3(1.2f, 1.2f, 1f);

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
            AutoGazePanIsActive = AutoGazeScrollIsActive;
            PanSpeedUpDown = ScrollSpeed_y;
            PanSpeedLeftRight = ScrollSpeed_x;
            MinDistFromCenterForAutoPan = MinDistFromCenterForAutoScroll;
            customColliderSizeOnLookAt = OnLookAt_ColliderSize;
            textureRenderer = TextureRendererToBeScrolled;
            useSkimProof = UseSkimProofing;

            base.Start();
        }

        protected override void Update()
        {
            // Check if any values have been changed in the Unity Editor and update them in the base class
            UpdateValues(ref AutoGazePanIsActive, AutoGazeScrollIsActive);
            UpdateValues(ref PanSpeedLeftRight, ScrollSpeed_x);
            UpdateValues(ref PanSpeedUpDown, ScrollSpeed_y);
            UpdateValues(ref MinDistFromCenterForAutoPan, MinDistFromCenterForAutoScroll);
            UpdateValues(ref MinDistFromCenterForAutoPan, MinDistFromCenterForAutoScroll);
            UpdateValues(ref customColliderSizeOnLookAt, OnLookAt_ColliderSize);
            UpdateValues(ref textureRenderer, TextureRendererToBeScrolled);
            UpdateValues(ref useSkimProof, UseSkimProofing);

            if (UpdateValues(ref skimproof_UpdateSpeedFromUser, SkimProofUpdateSpeed))
            {
                SetSkimProofUpdateSpeed(skimproof_UpdateSpeedFromUser);
            }

            base.Update();
        }
    }
}