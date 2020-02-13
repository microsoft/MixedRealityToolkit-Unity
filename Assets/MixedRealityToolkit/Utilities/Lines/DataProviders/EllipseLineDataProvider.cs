// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Creates an elliptical line shape.
    /// </summary>
    /// <remarks>This line loops.</remarks>
    [AddComponentMenu("Scripts/MRTK/Core/EllipseLineDataProvider")]
    public class EllipseLineDataProvider : BaseMixedRealityLineDataProvider
    {
        [SerializeField]
        [Tooltip("Resolution is the number of points used to define positions for points on the line. Equivalent to PointCount. Clamped at 2048 max")]
        [Range(0, MaxResolution)]
        private int resolution = 36;

        /// <summary>
        /// Resolution is the number of points used to define positions for points on the line. Equivalent to PointCount. Clamped at 2048 max
        /// </summary>
        public int Resolution
        {
            get => resolution;
            set => resolution = Mathf.Clamp(value, 0, MaxResolution);
        }

        [Tooltip("Radius of ellipsis defined by Vector2 where x is half-width and y is half-height")]
        [SerializeField]
        private Vector2 radius = Vector2.one;

        /// <summary>
        /// Radius of ellipsis defined by Vector2 where x is half-width and y is half-height
        /// </summary>
        public Vector2 Radius
        {
            get => radius;
            set
            {
                if (value.x < 0)
                {
                    value.x = 0;
                }

                if (value.y < 0)
                {
                    value.y = 0;
                }

                radius = value;
            }
        }

        private const int MaxResolution = 2048;


        #region BaseMixedRealityLineDataProvider Implementation

        /// <inheritdoc />
        public override int PointCount => Resolution;

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtility.GetEllipsePoint(Radius, normalizedDistance * 2f * Mathf.PI);
        }

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(int pointIndex)
        {
            float angle = ((float)pointIndex / Resolution) * 2f * Mathf.PI;
            return LineUtility.GetEllipsePoint(Radius, angle);
        }

        /// <inheritdoc />
        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            // Does nothing for an ellipse
        }

        /// <inheritdoc />
        protected override float GetUnClampedWorldLengthInternal()
        {
            float distance = 0f;
            Vector3 last = GetUnClampedPoint(0f);

            for (int i = 1; i < BaseMixedRealityLineDataProvider.UnclampedWorldLengthSearchSteps; i++)
            {
                Vector3 current = GetUnClampedPoint((float)i / BaseMixedRealityLineDataProvider.UnclampedWorldLengthSearchSteps);
                distance += Vector3.Distance(last, current);
            }

            return distance;
        }

        #endregion BaseMixedRealityLineDataProvider Implementation
    }
}