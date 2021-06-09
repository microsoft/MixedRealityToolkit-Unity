// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    ///<summary>
    /// HoloLens 2 shell's style button specific elements
    ///</summary>
    [AddComponentMenu("Scripts/MRTK/SDK/PressableButtonHoloLens2")]
    public class PressableButtonHoloLens2 : PressableButton
    {
        [SerializeField]
        [Tooltip("The icon and text content moving inside the button.")]
        private GameObject movingButtonIconText = null;

        [SerializeField]
        [Tooltip("The visuals which become compressed (scaled) along the z-axis when pressed.")]
        private GameObject compressableButtonVisuals = null;

        /// <summary>
        /// The visuals which become compressed (scaled) along the z-axis when pressed.
        /// </summary>
        public GameObject CompressableButtonVisuals
        {
            get => compressableButtonVisuals;
            set
            {
                compressableButtonVisuals = value;

                if (compressableButtonVisuals != null)
                {
                    initialCompressableButtonVisualsLocalScale = compressableButtonVisuals.transform.localScale;
                }

            }
        }

        [SerializeField]
        [Range(0.0f, 1.0f)]
        [Tooltip("The minimum percentage of the original scale the compressableButtonVisuals can be compressed to.")]
        private float minCompressPercentage = 0.25f;

        /// <summary>
        /// The minimum percentage of the original scale the compressableButtonVisuals can be compressed to.
        /// </summary>
        public float MinCompressPercentage { get => minCompressPercentage; set => minCompressPercentage = value; }

        [SerializeField]
        [Tooltip("The plate which represents the press-able surface of the button that highlights when focused.")]
        private Renderer highlightPlate = null;

        [SerializeField]
        [Tooltip("The duration of time it takes to animate in/out the highlight plate.")]
        private float highlightPlateAnimationTime = 0.25f;

        #region Private Members

        Vector3 initialCompressableButtonVisualsLocalScale = Vector3.one;
        private int fluentLightIntensityID = 0;
        private float targetFluentLightIntensity = 1.0f;
        private MaterialPropertyBlock properties = null;
        private Coroutine highlightPlateAnimationRoutine = null;

        #endregion

        protected override void Start()
        {
            base.Start();

            if (compressableButtonVisuals != null)
            {
                initialCompressableButtonVisualsLocalScale = compressableButtonVisuals.transform.localScale;
            }

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

        /// <inheritdoc />
        protected override void UpdateMovingVisualsPosition()
        {
            base.UpdateMovingVisualsPosition();

            if (compressableButtonVisuals != null)
            {
                // Compress the button visuals by the push amount.
                Vector3 scale = compressableButtonVisuals.transform.localScale;
                float pressPercentage;

                // Prevent divide by zero when calculating pressPercentage.
                if (MaxPushDistance <= float.Epsilon)
                {
                    pressPercentage = 0.0f;
                }
                else
                {
                    pressPercentage = Mathf.Max(minCompressPercentage, (1.0f - (CurrentPushDistance - startPushDistance) / MaxPushDistance));
                }

                scale.z = initialCompressableButtonVisualsLocalScale.z * pressPercentage;
                compressableButtonVisuals.transform.localScale = scale;
            }

            if (movingButtonIconText != null)
            {
                // Always move relative to startPushDistance
                movingButtonIconText.transform.localPosition = GetLocalPositionAlongPushDirection((CurrentPushDistance - startPushDistance) / 2.0f);
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
                float t = 1.0f - (blendTime / time);
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
