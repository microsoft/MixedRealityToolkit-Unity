// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    /// <summary>
    /// Helper class containing move/rotate/scale related utility functions.
    /// <summary>
    public class TransformHelper : MonoBehaviour
    {
        #region Properties

        private Transform targetTransform;
        
        private Vector3 initialScale;
        private Vector3 minimumScale;
        private Vector3 maximumScale;

        public float ScaleMinimum => minimumScale.x;
        public float ScaleMaximum => maximumScale.x;

        #endregion Properties

        public void Initialize(Transform transform)
        {
            if (targetTransform == null)
            {
                targetTransform = transform;
                initialScale = transform.localScale;
            }
        }

        #region Scale Utilities

        /// <summary>
        /// Sets the minimum/maximum scale for the transform helper.
        /// </summary>
        /// <param name="min">Minimum scale</param>
        /// <param name="max">Maximum scale</param>
        /// <param name="relativeToInitialState">If true the values will be multiplied by scale of target at startup. If false they will be in absolute local scale.</param>
        public void SetScaleLimits(float min, float max, bool relativeToInitialState = true)
        {
            if (relativeToInitialState)
            {
                maximumScale = initialScale * max;
                minimumScale = initialScale * min;
            }
            else
            {
                maximumScale = new Vector3(max, max, max);
                minimumScale = new Vector3(min, min, min);
            }
        }

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

        #endregion Scale Utilities
    }
}