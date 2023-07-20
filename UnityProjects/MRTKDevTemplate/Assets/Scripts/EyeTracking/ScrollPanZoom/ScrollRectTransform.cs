// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// This script allows to zoom into and pan the texture of a GameObject. 
    /// It also allows for scrolling by restricting panning to one direction.  
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Examples/ScrollRectTransform")]
    public class ScrollRectTransform : PanZoomBaseRectTransform
    {
        // Scroll
        [Tooltip("RectTransform from, for example, your TextMeshPro game object.")]
        [SerializeField]
        private RectTransform rectTransformToNavigate = null;

        [Tooltip("Reference to the viewport restricting the view box. This is important for identifying the max constrains for panning.")]
        [SerializeField]
        private RectTransform referenceToViewPort = null;

        [Tooltip("Ability to scroll using your eye gaze without any additional input (e.g., air tap or button presses).")]
        [SerializeField]
        private bool autoGazeScrollIsActive = true;

        [Tooltip("Horizontal scroll speed. For example: 0.1f for slow panning. 0.6f for fast panning.")]
        [SerializeField]
        private float scrollSpeedHorizontal = 0.0f;

        [Tooltip("Vertical scroll speed. For example: 0.1f for slow panning. 0.6f for fast panning.")]
        [SerializeField]
        private float scrollSpeedVertical = 0.3f;

        [Tooltip("Minimal distance in x and y from center of the target's hit box (0, 0) to scroll. Thus, values must range between 0 (always scroll) and 0.5 (no scroll).")]
        [SerializeField]
        private Vector2 minDistFromCenterForAutoScroll = new Vector2(0.2f, 0.2f);

        [Tooltip("Set to true to prevent sudden scrolling when quickly looking around. This may make scrolling feel less responsive though.")]
        [SerializeField]
        private bool useSkimProofing = false;

        [Tooltip("The lower the value, the slower the scrolling will speed up after skimming. Recommended value: 5.")]
        [SerializeField]
        [Range(0, 10)]
        private float skimProofUpdateSpeed = 5f;
        
        // The base PanAndZoom class can also be used with UV textures for which the dimensions are different to a RectTransform.
        // To allow to keep the speed values that users can assign consistent, let's internally convert the values.
        private float convertSpeedToUVSpace = -200.0f;

        [Tooltip("Custom anchor start position.")]
        [SerializeField]
        private Vector2 customStartPosition;

        /// <summary>
        /// Custom anchor start position.
        /// </summary>
        public Vector2 CustomStartPosition => customStartPosition;

        /// <summary>
        /// A Unity event function that is called on the frame when a script is enabled just before any of the update methods are called the first time.
        /// </summary> 
        protected override void Start()
        {
            // Assigning values to base PanZoom class
            autoGazePanIsActive = autoGazeScrollIsActive;
            panSpeedUpDown = scrollSpeedVertical * convertSpeedToUVSpace;
            panSpeedLeftRight = scrollSpeedHorizontal * convertSpeedToUVSpace;
            minDistFromCenterForAutoPan = minDistFromCenterForAutoScroll;
            useSkimProof = useSkimProofing;

            navigationRectTransform = rectTransformToNavigate;
            viewportRectTransform = referenceToViewPort;

            navigationRectTransform.anchoredPosition = customStartPosition;
            isScrollText = true;

            base.Start();
        }

        private void UpdatePivot()
        {
            navigationRectTransform.pivot = new Vector2(0f, 1f);
            navigationRectTransform.anchorMin = new Vector2(0f, 1f);
            navigationRectTransform.anchorMax = new Vector2(0f, 1f);
        }

        /// <inheritdoc />
        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {   
            base.ProcessInteractable(updatePhase);

            // Dynamic is effectively just your normal Update().
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                UpdatePivot();

                UpdateValues(ref autoGazePanIsActive, autoGazeScrollIsActive);
                UpdateValues(ref panSpeedUpDown, scrollSpeedVertical * convertSpeedToUVSpace);
                UpdateValues(ref panSpeedLeftRight, scrollSpeedHorizontal * convertSpeedToUVSpace);
                UpdateValues(ref minDistFromCenterForAutoPan, minDistFromCenterForAutoScroll);
                UpdateValues(ref useSkimProof, useSkimProofing);

                if (UpdateValues(ref skimProofUpdateSpeedFromUser, skimProofUpdateSpeed))
                {
                    SetSkimProofUpdateSpeed(skimProofUpdateSpeedFromUser);
                }
            }
        }
    }
}
