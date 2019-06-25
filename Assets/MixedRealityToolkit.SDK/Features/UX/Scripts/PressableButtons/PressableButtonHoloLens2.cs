// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    ///<summary>
    /// HoloLens 2 shell's style button specific elements
    ///</summary>
    public class PressableButtonHoloLens2 : PressableButton
    {
        [SerializeField]
        [Tooltip("The icon and text content moving inside the button.")]
        private GameObject movingButtonIconText = null;

        [SerializeField]
        [Tooltip("The plate which represents the press-able surface of the button that highlights when focused.")]
        private Renderer highlightPlate = null;

        [SerializeField]
        [Tooltip("The duration of time it takes to animate in/out the highlight plate.")]
        private float highlightPlateAnimationTime = 0.25f;

        #region Private Members

        private int fluentLightIntensityID = 0;
        private float targetFluentLightIntensity = 1.0f;
        private MaterialPropertyBlock properties = null;
        private Coroutine highlightPlateAnimationRoutine = null;

        #endregion

        protected override void Start()
        {
            base.Start();

            if (highlightPlate != null)
            {
                // Cache the initial highlight plate state.
                fluentLightIntensityID = Shader.PropertyToID("_FluentLightIntensity");
                properties = new MaterialPropertyBlock();
                targetFluentLightIntensity = highlightPlate.sharedMaterial.GetFloat(fluentLightIntensityID);

                // Hide the highlight plate initially.
                UpdateHightlightPlateVisuals(0.0f);
                highlightPlate.enabled = false;
            }
        }

        /// <summary>
        /// Public property to set the moving content part(icon and text) of the button. 
        /// This content part moves 1/2 distance of the front cage 
        /// </summary>
        public GameObject MovingButtonIconText
        {
            get
            {
                return movingButtonIconText;
            }
            set
            {
                if (movingButtonIconText != value)
                {
                    movingButtonIconText = value;
                }
            }
        }

        protected override void UpdateMovingVisualsPosition()
        {
            base.UpdateMovingVisualsPosition();

            if (movingButtonIconText != null)
            {
                // Always move relative to startPushDistance
                movingButtonIconText.transform.position = GetWorldPositionAlongPushDirection((currentPushDistance - startPushDistance) / 2);
            }
        }

        /// <summary>
        /// Animates in the highlight plate.
        /// </summary>
        public void AnimateInHighlightPlate()
        {
            if (highlightPlate != null)
            {
                if (highlightPlateAnimationRoutine != null)
                {
                    StopCoroutine(highlightPlateAnimationRoutine);
                }

                highlightPlateAnimationRoutine = StartCoroutine(AnimateHighlightPlate(true, highlightPlateAnimationTime));
            }
        }

        /// <summary>
        /// Animates out the highlight plate and disables it when animated out.
        /// </summary>
        public void AnimateOutHighlightPlate()
        {
            if (highlightPlate != null)
            {
                if (highlightPlateAnimationRoutine != null)
                {
                    StopCoroutine(highlightPlateAnimationRoutine);
                }

                highlightPlateAnimationRoutine = StartCoroutine(AnimateHighlightPlate(false, highlightPlateAnimationTime));
            }
        }

        private IEnumerator AnimateHighlightPlate(bool fadeIn, float time)
        {
            highlightPlate.enabled = true;

            // Calculate how much time is left in the blend based on current intensity.
            var normalizedIntensity = (targetFluentLightIntensity != 0.0f) ? properties.GetFloat(fluentLightIntensityID) / targetFluentLightIntensity : 1.0f;
            var blendTime = fadeIn ? (1.0f - normalizedIntensity) * time : normalizedIntensity * time;

            while (blendTime > 0.0f)
            {
                float t =  1.0f - (blendTime / time);
                UpdateHightlightPlateVisuals(fadeIn ? t : 1.0f - t);
                blendTime -= Time.deltaTime;

                yield return null;
            }

            UpdateHightlightPlateVisuals(fadeIn ? targetFluentLightIntensity : 0.0f);

            // When completely faded out, hide the highlight plate.
            if (!fadeIn)
            {
                highlightPlate.enabled = false;
            }
        }

        private void UpdateHightlightPlateVisuals(float lightIntensity)
        {
            highlightPlate.GetPropertyBlock(properties);
            properties.SetFloat(fluentLightIntensityID, lightIntensity);
            highlightPlate.SetPropertyBlock(properties);
        }
    }
}
