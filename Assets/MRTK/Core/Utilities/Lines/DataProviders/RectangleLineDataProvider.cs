// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Defines a line in the shape of a rectangle.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Core/RectangleLineDataProvider")]
    public class RectangleLineDataProvider : BaseMixedRealityLineDataProvider
    {
        [SerializeField]
        private Vector3[] points = new Vector3[8];

        [SerializeField]
        private float width = 1f;

        public float Width
        {
            get { return width; }
            set
            {
                if (value < 0)
                {
                    value = width;
                }

                if (!width.Equals(value))
                {
                    width = value;
                    BuildPoints();
                }
            }
        }

        [SerializeField]
        private float height = 1f;

        public float Height
        {
            get { return height; }
            set
            {
                if (value < 0)
                {
                    value = height;
                }

                if (!height.Equals(value))
                {
                    height = value;
                    BuildPoints();
                }
            }
        }

        [SerializeField]
        private float zOffset = 0f;

        public float ZOffset
        {
            get
            {
                return zOffset;
            }
            set
            {
                if (!zOffset.Equals(value))
                {
                    zOffset = value;
                    BuildPoints();
                }
            }
        }

        #region MonoBehaviour Implementation

        private void OnValidate()
        {   // This is an appropriate use of OnValidate.
            BuildPoints();
        }

        #endregion MonoBehaviour Implementation

        #region BaseMixedRealityLineDataProvider Implementation

        public override int PointCount => 8;

        public override bool Loops
        {
            get
            {
                // Force to loop
                Loops = true;
                return true;
            }
        }

        /// <summary>
        /// When we get interpolated points we subdivide the square so our sampling has more to work with
        /// </summary>
        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            BuildPoints();
            return LineUtility.InterpolateVectorArray(points, normalizedDistance);
        }

        /// <inheritdoc />
        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            if (pointIndex < PointCount && pointIndex >= 0)
            {
                points[pointIndex] = point;
            }
        }

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(int pointIndex)
        {
            return pointIndex < PointCount && pointIndex >= 0 ? points[pointIndex] : Vector3.zero;
        }

        /// <inheritdoc />
        protected override float GetUnClampedWorldLengthInternal()
        {
            BuildPoints();

            float distance = 0f;
            Vector3 last = points[0];

            for (int i = 1; i < points.Length; i++)
            {
                distance += Vector3.Distance(last, points[i]);
                last = points[i];
            }

            return distance;
        }

        /// <inheritdoc />
        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            // Rectangle 'up' vector always points out from center
            return (GetPoint(normalizedLength) - transform.position).normalized;
        }

        protected override void DrawUnselectedGizmosPreview()
        {
            Vector3 firstPos = GetPoint(0);
            Vector3 lastPos = firstPos;
            Gizmos.color = Color.magenta;

            for (int i = 1; i < PointCount; i++)
            {
                Vector3 currentPos = GetPoint(i);
                Gizmos.DrawLine(lastPos, currentPos);
                lastPos = currentPos;
            }

            Gizmos.DrawLine(lastPos, firstPos);
        }

        #endregion BaseMixedRealityLineDataProvider Implementation

        private void BuildPoints()
        {
            Vector3 top = Vector3.up * Height * 0.5f;
            Vector3 bot = Vector3.down * Height * 0.5f;
            Vector3 left = Vector3.left * Width * 0.5f;
            Vector3 right = Vector3.right * Width * 0.5f;
            Vector3 offset = Vector3.forward * ZOffset;

            SetPointInternal(0, top + left + offset);
            SetPointInternal(1, top + offset);
            SetPointInternal(2, top + right + offset);
            SetPointInternal(3, right + offset);
            SetPointInternal(4, bot + right + offset);
            SetPointInternal(5, bot + offset);
            SetPointInternal(6, bot + left + offset);
            SetPointInternal(7, left + offset);
        }
    }
}