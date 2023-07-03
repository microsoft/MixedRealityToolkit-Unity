// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Represents a set parametric curve points.
    /// </summary>
    [AddComponentMenu("MRTK/Input/Bezier Data Provider")]
    internal class BezierDataProvider : BaseMixedRealityLineDataProvider
    {
        /// <summary>
        /// A structure that holds four 3D points along a parametric curve.
        /// </summary>
        [Serializable]
        private struct BezierPointSet
        {
            public Vector3 Point1;
            public Vector3 Point2;
            public Vector3 Point3;
            public Vector3 Point4;
        }

        /// <inheritdoc />
        public override int PointCount { get { return 4; } }

        [Header("Bezier Settings")]
        [SerializeField]
        private BezierPointSet controlPoints = new BezierPointSet();

        [Tooltip("If true, control points 2 and 3 will be transformed relative to points 1 and 4 respectively")]
        [SerializeField]
        private bool useLocalTangentPoints = false;

        private Vector3 localOffset;

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(int pointIndex)
        {
            switch (pointIndex)
            {
                case 0:
                    return controlPoints.Point1;

                case 1:
                    return controlPoints.Point2;

                case 2:
                    return controlPoints.Point3;

                case 3:
                    return controlPoints.Point4;

                default:
                    return Vector3.zero;
            }
        }

        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            switch (pointIndex)
            {
                case 0:
                    localOffset = Vector3.zero;
                    // If we're using local tangent points, apply this change to control point 2
                    if (useLocalTangentPoints)
                    {
                        localOffset = point - controlPoints.Point1;
                    }

                    controlPoints.Point1 = point;
                    controlPoints.Point2 += localOffset;
                    break;

                case 1:
                    controlPoints.Point2 = point;
                    break;

                case 2:
                    controlPoints.Point3 = point;
                    break;

                case 3:
                    localOffset = Vector3.zero;
                    if (useLocalTangentPoints)
                    {
                        localOffset = point - controlPoints.Point4;
                    }

                    controlPoints.Point4 = point;
                    controlPoints.Point3 += localOffset;
                    break;

                default:
                    break;
            }
        }

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtility.InterpolateBezierPoints(controlPoints.Point1, controlPoints.Point2, controlPoints.Point3, controlPoints.Point4, normalizedDistance);
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

        /// <inheritdoc />
        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            // Bezier up vectors just use transform up
            return transform.up;
        }
    }
}
