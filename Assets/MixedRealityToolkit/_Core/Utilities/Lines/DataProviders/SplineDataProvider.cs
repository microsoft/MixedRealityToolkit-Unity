// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders
{
    public class SplineDataProvider : BaseMixedRealityLineDataProvider
    {
        [SerializeField]
        [Header("Spline Settings")]
        private MixedRealityPose[] points = new MixedRealityPose[4];

        public MixedRealityPose[] Points
        {
            get { return points; }
            set { points = value; }
        }

        [SerializeField]
        private bool alignControlPoints = true;

        public bool AlignControlPoints
        {
            get { return alignControlPoints; }
            set
            {
                if (alignControlPoints != value)
                {
                    alignControlPoints = value;
                    ForceUpdateAlignment();
                }
            }
        }

        public override int PointCount => points.Length;

        public void ForceUpdateAlignment()
        {
            if (AlignControlPoints)
            {
                for (int i = 0; i < PointCount; i++)
                {
                    ForceUpdateAlignment(i);
                }
            }
        }

        private void ForceUpdateAlignment(int pointIndex)
        {
            if (AlignControlPoints)
            {
                int prevControlPoint = 0;
                int changedControlPoint = 0;
                int midPointIndex = ((pointIndex + 1) / 3) * 3;

                if (pointIndex <= midPointIndex)
                {
                    prevControlPoint = midPointIndex - 1;
                    changedControlPoint = midPointIndex + 1;
                }
                else
                {
                    prevControlPoint = midPointIndex + 1;
                    changedControlPoint = midPointIndex - 1;
                }

                if (Loops)
                {
                    if (changedControlPoint < 0)
                    {
                        changedControlPoint = (PointCount - 1) + changedControlPoint;
                    }
                    else if (changedControlPoint >= PointCount)
                    {
                        changedControlPoint = changedControlPoint % (PointCount - 1);
                    }

                    if (prevControlPoint < 0)
                    {
                        prevControlPoint = (PointCount - 1) + prevControlPoint;
                    }
                    else if (prevControlPoint >= PointCount)
                    {
                        prevControlPoint = prevControlPoint % (PointCount - 1);
                    }

                    Vector3 midPoint = points[midPointIndex].Position;
                    Vector3 tangent = midPoint - points[prevControlPoint].Position;
                    tangent = tangent.normalized * Vector3.Distance(midPoint, points[changedControlPoint].Position);
                    points[changedControlPoint].Position = midPoint + tangent;

                }
                else if (changedControlPoint >= 0 && changedControlPoint < PointCount && prevControlPoint >= 0 && prevControlPoint < PointCount)
                {
                    Vector3 midPoint = points[midPointIndex].Position;
                    Vector3 tangent = midPoint - points[prevControlPoint].Position;
                    tangent = tangent.normalized * Vector3.Distance(midPoint, points[changedControlPoint].Position);
                    points[changedControlPoint].Position = midPoint + tangent;
                }
            }
        }

        public override void AppendPoint(Vector3 point)
        {
            int pointIndex = points.Length;
            Array.Resize(ref points, points.Length + 1);
            SetPoint(pointIndex, point);
        }

        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            var totalDistance = normalizedDistance * (PointCount - 1);
            int point1Index = Mathf.FloorToInt(totalDistance);
            point1Index -= point1Index % 3;
            float subDistance = (totalDistance - point1Index) / 3;

            int point2Index;
            int point3Index;
            int point4Index;

            if (!Loops)
            {
                if (point1Index + 3 >= PointCount)
                {
                    return points[PointCount - 1].Position;
                }
                if (point1Index < 0)
                {
                    return points[0].Position;
                }

                point2Index = point1Index + 1;
                point3Index = point1Index + 2;
                point4Index = point1Index + 3;

            }
            else
            {
                point2Index = (point1Index + 1) % (PointCount - 1);
                point3Index = (point1Index + 2) % (PointCount - 1);
                point4Index = (point1Index + 3) % (PointCount - 1);
            }

            Vector3 point1 = points[point1Index].Position;
            Vector3 point2 = points[point2Index].Position;
            Vector3 point3 = points[point3Index].Position;
            Vector3 point4 = points[point4Index].Position;

            return LineUtility.InterpolateBezeirPoints(point1, point2, point3, point4, subDistance);
        }

        protected override Vector3 GetPointInternal(int pointIndex)
        {
            if (pointIndex < 0 || pointIndex >= points.Length)
            {
                Debug.LogError("Invalid point index!");
                return Vector3.zero;
            }

            if (Loops && pointIndex == PointCount - 1)
            {
                points[pointIndex] = points[0];
                pointIndex = 0;
            }

            return points[pointIndex].Position;
        }

        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            if (pointIndex < 0 || pointIndex >= points.Length)
            {
                Debug.LogError("Invalid point index!");
                return;
            }

            if (Loops && pointIndex == PointCount - 1)
            {
                points[pointIndex] = points[0];
                pointIndex = 0;
            }

            if (AlignControlPoints)
            {
                if (pointIndex % 3 == 0)
                {
                    Vector3 delta = point - points[pointIndex].Position;
                    if (Loops)
                    {
                        if (pointIndex == 0)
                        {
                            points[1].Position += delta;
                            points[PointCount - 2].Position += delta;
                            points[PointCount - 1].Position = point;
                        }
                        else if (pointIndex == PointCount)
                        {
                            points[0].Position = point;
                            points[1].Position += delta;
                            points[pointIndex - 1].Position += delta;
                        }
                        else
                        {
                            points[pointIndex - 1].Position += delta;
                            points[pointIndex + 1].Position += delta;
                        }
                    }
                    else
                    {
                        if (pointIndex > 0)
                        {
                            points[pointIndex - 1].Position += delta;
                        }
                        if (pointIndex + 1 < points.Length)
                        {
                            points[pointIndex + 1].Position += delta;
                        }
                    }
                }
            }

            points[pointIndex].Position = point;

            ForceUpdateAlignment(pointIndex);
        }

        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {

            float arrayValueLength = 1f / points.Length;
            int indexA = Mathf.FloorToInt(normalizedLength * points.Length);
            if (indexA >= points.Length)
            {
                indexA = 0;
            }

            int indexB = indexA + 1;
            if (indexB >= points.Length)
            {
                indexB = 0;
            }

            float blendAmount = (normalizedLength - (arrayValueLength * indexA)) / arrayValueLength;
            Quaternion rotation = Quaternion.Lerp(points[indexA].Rotation, points[indexB].Rotation, blendAmount);
            return rotation * transform.up;
        }

        protected override float GetUnClampedWorldLengthInternal()
        {
            // Crude approximation
            // TODO optimize
            float distance = 0f;
            Vector3 last = GetPoint(0f);

            for (int i = 1; i < 10; i++)
            {
                Vector3 current = GetPoint((float)i / 10);
                distance += Vector3.Distance(last, current);
            }

            return distance;
        }
    }
}