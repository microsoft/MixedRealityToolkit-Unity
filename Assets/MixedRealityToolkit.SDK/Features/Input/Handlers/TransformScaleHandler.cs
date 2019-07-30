// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Component for setting the min/max scale values for ManipulationHandler
    /// or BoundingBox
    /// </summary>
    public class TransformScaleHandler : MonoBehaviour
    {
        #region Properties

        [SerializeField]
        [Tooltip("Transform being scaled. Defaults to the object of the component.")]
        private Transform targetTransform = null;

        public Transform TargetTransform
        {
            get => targetTransform;
            set => targetTransform = value;
        }

        private Vector3 initialScale;

        [SerializeField]
        [Tooltip("Minimum scaling allowed")]
        private  float scaleMinimum = 0.2f;

        private Vector3 minimumScale;
        
        public float ScaleMinimum
        {
            get => minimumScale.x;
            set
            {
                scaleMinimum = value;
                SetScaleLimits();
            }
        }

        [SerializeField]
        [Tooltip("Maximum scaling allowed")]
        private float scaleMaximum = 2f;

        private Vector3 maximumScale;
        
        public float ScaleMaximum
        {
            get => maximumScale.x;
            set
            {
                scaleMaximum = value;
                SetScaleLimits();
            }
        }

        [SerializeField]
        [Tooltip("Min/max scaling relative to initial scale if true")]
        private bool relativeToInitialState = true;
        
        public bool RelativeToInitialState
        {
            get => relativeToInitialState;
            set
            {
                relativeToInitialState = value;
                SetScaleLimits();
            }
        }

        #endregion Properties

        #region MonoBehaviour Methods

        public void Start()
        {
            if (targetTransform == null)
            {
                targetTransform = transform;
            }
            initialScale = targetTransform.localScale;

            SetScaleLimits();
        }

        #endregion MonoBehaviour Methods

        #region Public Methods

        /// <summary>
        /// Clamps the given scale to the scale limits set by <see cref="SetScaleLimits"/> such that:
        /// - No one component of the returned vector will be greater than the max scale.
        /// - No one component of the returned vector will be less than the min scale.
        /// - The returned vector's direction will be the same as the given vector
        /// </summary>
        /// <param name="scale">Scale value to clamp</param>
        /// <returns>The clamped scale vector</returns>
        public Vector3 ClampScale(Vector3 scale)
        {
            if (Vector3.Min(maximumScale, scale) != scale)
            {
                float maxRatio = 0.0f;
                int maxIdx = -1;

                // Find out the component with the maximum ratio to its maximum allowed value
                for (int i = 0; i < 3; ++i)
                {
                    if (maximumScale[i] > 0)
                    {
                        float ratio = scale[i] / maximumScale[i];
                        if (ratio > maxRatio)
                        {
                            maxRatio = ratio;
                            maxIdx = i;
                        }
                    }
                }

                if (maxIdx != -1)
                {
                    scale /= maxRatio;
                }
            }

            if (Vector3.Max(minimumScale, scale) != scale)
            {
                float minRatio = 1.0f;
                int minIdx = -1;

                // Find out the component with the minimum ratio to its minimum allowed value
                for (int i = 0; i < 3; ++i)
                {
                    if (minimumScale[i] > 0)
                    {
                        float ratio = scale[i] / minimumScale[i];
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            minIdx = i;
                        }
                    }
                }

                if (minIdx != -1)
                {
                    scale /= minRatio;
                }
            }

            return scale;
        }

        #endregion Public Methods

        #region Private Methods
        
        private void SetScaleLimits()
        {
            if (relativeToInitialState)
            {
                minimumScale = initialScale * scaleMinimum;
                maximumScale = initialScale * scaleMaximum;
            }
            else
            {
                minimumScale = new Vector3(scaleMinimum, scaleMinimum, scaleMinimum);
                maximumScale = new Vector3(scaleMaximum, scaleMaximum, scaleMaximum);
            }
        }

        #endregion Private Methods
    }
}