//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace MRTK.UX
{
    public class Rectangle : LineBase
    {
        public override int NumPoints
        {
            get
            {
                return 8;
            }
        }

        public override bool Loops
        {
            get
            {
                // Force to loop
                loops = true;
                return loops;
            }
        }

        [Header("Rectangle Settings")]
        public Vector2 Dimensions = Vector2.one;
        public float ZOffset;

        /// <summary>
        /// When we get interpolated points we subdivide the square so our sampling has more to work with
        /// </summary>
        /// <param name="normalizedDistance"></param>
        /// <returns></returns>
        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            if (points == null || points.Length != 8)
                points = new Vector3[8];

            BuildPoints();

            return LineUtils.InterpolateVectorArray(points, normalizedDistance);
            //return InterpolateCatmullRomPoints(TopLeft, TopRight, BotLeft, BotRight, normalizedDistance);
        }

        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            if (points == null || points.Length != 8)
                points = new Vector3[8];

            if (pointIndex <= 7 && pointIndex >= 0)
                points[pointIndex] = point;
        }

        protected override Vector3 GetPointInternal(int pointIndex)
        {
            if (points == null || points.Length != 8)
                points = new Vector3[8];

            if (pointIndex <= 7 && pointIndex >= 0)
                    return points[pointIndex];

            return Vector3.zero;
        }

        protected override float GetUnclampedWorldLengthInternal()
        {
            BuildPoints();

            Vector3 last = points[0];
            float distance = 0f;
            for (int i = 1; i < points.Length; i++)
            {
                distance += Vector3.Distance(last, points[i]);
                last = points[i];
            }
            return distance;
        }

        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            // Rectangle 'up' vector always points out from center
            return (GetPoint(normalizedLength) - transform.position).normalized;
        }

        private void BuildPoints()
        {
            Vector3 offset = Vector3.forward * ZOffset;
            Vector3 top = (Vector3.up * Dimensions.y * 0.5f);
            Vector3 bot = (Vector3.down * Dimensions.y * 0.5f);
            Vector3 left = (Vector3.left * Dimensions.x * 0.5f);
            Vector3 right = (Vector3.right * Dimensions.x * 0.5f);

            SetPointInternal(0, top + left + offset);
            SetPointInternal(1, top + offset);
            SetPointInternal(2, top + right + offset);
            SetPointInternal(3, right + offset);
            SetPointInternal(4, bot + right + offset);
            SetPointInternal(5, bot + offset);
            SetPointInternal(6, bot + left + offset);
            SetPointInternal(7, left + offset);
        }

        [SerializeField]
        private Vector3[] points;
        
    }
}