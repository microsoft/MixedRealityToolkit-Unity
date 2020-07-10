// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// This script allows to scroll a texture both horizontally and vertically.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/ScrollTexture")]
    public class ScrollTexture : PanZoomBaseTexture
    {
        // Pan
        [Tooltip("Renderer of the texture to be scrolled.")]
        [SerializeField]
        private Renderer textureRendererToBeScrolled = null;

        [Tooltip("Ability to scroll using your eye gaze without any additional input (e.g., air tap or button presses).")]
        [SerializeField]
        private bool autoGazeScrollIsActive = true;

        [Tooltip("Horizontal scroll speed. For example: 0.1f for slow panning. 0.6f for fast panning.")]
        [SerializeField]
        private float scrollSpeedHorizontal = 0.0f;

        [Tooltip("Vertical scroll speed. For example: 0.1f for slow panning. 0.6f for fast panning.")]
        [SerializeField]
        private float scrollSpeedVertical = 0.15f;

        [Tooltip("Minimal distance in x and y from center of the target's hit box (0, 0) to scroll. Thus, values must range between 0 (always scroll) and 0.5 (no scroll).")]
        [SerializeField]
        private Vector2 minDistFromCenterForAutoScroll = new Vector2(0.2f, 0.2f);

        [Tooltip("Size of the GameObject's collider when being looked at.")]
        [SerializeField]
        private Vector3 onLookAtColliderSize = new Vector3(1.2f, 1.2f, 1f);

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
            autoGazePanIsActive = autoGazeScrollIsActive;
            panSpeedUpDown = scrollSpeedVertical;
            panSpeedLeftRight = scrollSpeedHorizontal;
            minDistFromCenterForAutoPan = minDistFromCenterForAutoScroll;
            customColliderSizeOnLookAt = onLookAtColliderSize;
            textureRenderer = textureRendererToBeScrolled;
            useSkimProof = useSkimProofing;

            base.Start();
        }

        protected override void Update()
        {
            // Check if any values have been changed in the Unity Editor and update them in the base class
            UpdateValues(ref autoGazePanIsActive, autoGazeScrollIsActive);
            UpdateValues(ref panSpeedLeftRight, scrollSpeedHorizontal);
            UpdateValues(ref panSpeedUpDown, scrollSpeedVertical);
            UpdateValues(ref minDistFromCenterForAutoPan, minDistFromCenterForAutoScroll);
            UpdateValues(ref minDistFromCenterForAutoPan, minDistFromCenterForAutoScroll);
            UpdateValues(ref customColliderSizeOnLookAt, onLookAtColliderSize);
            UpdateValues(ref textureRenderer, textureRendererToBeScrolled);
            UpdateValues(ref useSkimProof, useSkimProofing);

            if (UpdateValues(ref skimproof_UpdateSpeedFromUser, skimProofUpdateSpeed))
            {
                SetSkimProofUpdateSpeed(skimproof_UpdateSpeedFromUser);
            }

            base.Update();
        }
    }
}