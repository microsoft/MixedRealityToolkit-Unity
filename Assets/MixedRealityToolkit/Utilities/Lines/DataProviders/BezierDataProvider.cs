// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Lines.DataProviders
{
    public class BezierDataProvider : BaseMixedRealityLineDataProvider
    {
        [Serializable]
        private struct BezierPointSet
        {
            public BezierPointSet(float spread)
            {
                Point1 = Vector3.right * spread * 0.5f;
                Point2 = Vector3.right * spread * 0.25f;
                Point3 = Vector3.left * spread * 0.25f;
                Point4 = Vector3.left * spread * 0.5f;
            }

            public Vector3 Point1;
            public Vector3 Point2;
            public Vector3 Point3;
            public Vector3 Point4;
        }

        public override int PointCount { get { return 4; } }

        [Header("Bezier Settings")]
        [SerializeField]
        private BezierPointSet controlPoints = new BezierPointSet(0.5f);

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
                    controlPoints.Point1 = point;
                    break;

                case 1:
                    controlPoints.Point2 = point;
                    break;

                case 2:
                    controlPoints.Point3 = point;
                    break;

                case 3:
                    controlPoints.Point4 = point;
                    break;

                default:
                    break;
            }
        }

        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtility.InterpolateBezierPoints(controlPoints.Point1, controlPoints.Point2, controlPoints.Point3, controlPoints.Point4, normalizedDistance);
        }

        protected override float GetUnClampedWorldLengthInternal()
        {
            // Crude approximation
            // TODO optimize
            float distance = 0f;
            Vector3 last = GetUnClampedPoint(0f);
            for (int i = 1; i < 10; i++)
            {
                Vector3 current = GetUnClampedPoint((float)i / 10);
                distance += Vector3.Distance(last, current);
            }
            return distance;
        }

        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            // Bezeir up vectors just use transform up
            return transform.up;
        }
    }
}