// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines
{
    public class Rectangle : LineBase
    {
        public override int PointCount => 8;

        public override bool Loops
        {
            get
            {
                // Force to loop
                Loops = true;
                return base.Loops;
            }
        }

        [Header("Rectangle Settings")]

        [SerializeField]
        private Vector3[] points;

        [SerializeField]
        private float xSize = 1f;

        public float XSize
        {
            get { return xSize; }
            set
            {
                if (!xSize.Equals(value))
                {
                    xSize = value;
                    BuildPoints();
                }
            }
        }

        [SerializeField]
        private float ySize = 1f;

        public float YSize
        {
            get { return ySize; }
            set
            {
                if (!ySize.Equals(value))
                {
                    ySize = value;
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

        private void OnValidate()
        {
            if (points == null || points.Length != 8)
            {
                points = new Vector3[PointCount];
            }
        }

        /// <summary>
        /// When we get interpolated points we subdivide the square so our sampling has more to work with
        /// </summary>
        /// <param name="normalizedDistance"></param>
        /// <returns></returns>
        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            BuildPoints();
            return LineUtility.InterpolateVectorArray(points, normalizedDistance);
        }

        /// <inheritdoc />
        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            if (pointIndex <= 7 && pointIndex >= 0)
            {
                points[pointIndex] = point;
            }
        }

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(int pointIndex)
        {
            return pointIndex <= 7 && pointIndex >= 0 ? points[pointIndex] : Vector3.zero;
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

        private void BuildPoints()
        {
            Vector3 top = Vector3.up * YSize * 0.5f;
            Vector3 bot = Vector3.down * YSize * 0.5f;
            Vector3 left = Vector3.left * XSize * 0.5f;
            Vector3 right = Vector3.right * XSize * 0.5f;
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

#if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            // Show gizmos if this object is not selected
            // (SceneGUI will display it otherwise)

            if (Application.isPlaying || UnityEditor.Selection.activeGameObject == gameObject)
            {
                return;
            }

            // Only draw a gizmo if we don't have a line renderer
            var baseLineRenderer = gameObject.GetComponent<LineRendererBase>();

            if (baseLineRenderer != null)
            {
                return;
            }

            Vector3 firstPos = GetPoint(0);
            Vector3 lastPos = firstPos;
            Gizmos.color = Color.Lerp(Color.white, Color.clear, 0.25f);

            for (int i = 1; i < PointCount; i++)
            {
                Vector3 currentPos = GetPoint(i);
                Gizmos.DrawLine(lastPos, currentPos);
                lastPos = currentPos;
            }

            Gizmos.DrawLine(lastPos, firstPos);
        }
#endif
    }
}