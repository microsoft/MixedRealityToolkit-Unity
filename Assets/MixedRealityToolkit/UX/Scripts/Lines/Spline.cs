// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace MixedRealityToolkit.UX.Lines
{
    public class Spline : LineBase
    {
        [Header("Spline Settings")]
        [SerializeField]
        private SplinePoint[] points = new SplinePoint[4];

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

        public override int NumPoints
        {
            get
            {
                return points.Length;
            }
        }

        public SplinePoint[] Points
        {
            get
            {
                return points;
            }
            set
            {
                points = value;
            }
        }

        public void ForceUpdateAlignment()
        {
            if (AlignControlPoints)
            {
                for (int i = 0; i < NumPoints; i++)
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

                if (loops)
                {
                    if (changedControlPoint < 0)
                    {
                        changedControlPoint = (NumPoints - 1) + changedControlPoint;
                    }
                    else if (changedControlPoint >= NumPoints)
                    {
                        changedControlPoint = changedControlPoint % (NumPoints - 1);
                    }

                    if (prevControlPoint < 0)
                    {
                        prevControlPoint = (NumPoints - 1) + prevControlPoint;
                    }
                    else if (prevControlPoint >= NumPoints)
                    {
                        prevControlPoint = prevControlPoint % (NumPoints - 1);
                    }

                    Vector3 midPoint = points[midPointIndex].Point;
                    Vector3 tangent = midPoint - points[prevControlPoint].Point;
                    tangent = tangent.normalized * Vector3.Distance(midPoint, points[changedControlPoint].Point);
                    points[changedControlPoint].Point = midPoint + tangent;

                }
                else if (changedControlPoint >= 0 && changedControlPoint < NumPoints && prevControlPoint >= 0 && prevControlPoint < NumPoints)
                {
                    Vector3 midPoint = points[midPointIndex].Point;
                    Vector3 tangent = midPoint - points[prevControlPoint].Point;
                    tangent = tangent.normalized * Vector3.Distance(midPoint, points[changedControlPoint].Point);
                    points[changedControlPoint].Point = midPoint + tangent;
                }
            }
        }

        public override void AppendPoint(Vector3 point)
        {
            int pointIndex = points.Length;
            Array.Resize<SplinePoint>(ref points, points.Length + 1);
            SetPoint(pointIndex, point);
        }

        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            float totalDistance = normalizedDistance * (NumPoints - 1);

            int point1Index = Mathf.FloorToInt(totalDistance);
            point1Index -= (point1Index % 3);
            float subDistance = (totalDistance - point1Index) / 3;

            int point2Index = 0;
            int point3Index = 0;
            int point4Index = 0;

            if (!loops)
            {
                if (point1Index + 3 >= NumPoints)
                {
                    return points[NumPoints - 1].Point;
                }
                if (point1Index < 0)
                {
                    return points[0].Point;
                }

                point2Index = point1Index + 1;
                point3Index = point1Index + 2;
                point4Index = point1Index + 3;

            }
            else
            {
                point2Index = (point1Index + 1) % (NumPoints - 1);
                point3Index = (point1Index + 2) % (NumPoints - 1);
                point4Index = (point1Index + 3) % (NumPoints - 1);
            }

            Vector3 point1 = points[point1Index].Point;
            Vector3 point2 = points[point2Index].Point;
            Vector3 point3 = points[point3Index].Point;
            Vector3 point4 = points[point4Index].Point;

            return LineUtils.InterpolateBezeirPoints(point1, point2, point3, point4, subDistance);
        }

        protected override Vector3 GetPointInternal(int pointIndex)
        {
            if (pointIndex < 0 || pointIndex >= points.Length)
            {
                throw new IndexOutOfRangeException();
            }

            if (loops && pointIndex == NumPoints - 1)
            {
                points[pointIndex] = points[0];
                pointIndex = 0;
            }

            return points[pointIndex].Point;
        }

        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            if (pointIndex < 0 || pointIndex >= points.Length)
            {
                throw new IndexOutOfRangeException();
            }

            if (loops && pointIndex == NumPoints - 1)
            {
                points[pointIndex] = points[0];
                pointIndex = 0;
            }

            if (AlignControlPoints)
            {
                if (pointIndex % 3 == 0)
                {
                    Vector3 delta = point - points[pointIndex].Point;
                    if (loops)
                    {
                        if (pointIndex == 0)
                        {
                            points[1].Point += delta;
                            points[NumPoints - 2].Point += delta;
                            points[NumPoints - 1].Point = point;
                        }
                        else if (pointIndex == NumPoints)
                        {
                            points[0].Point = point;
                            points[1].Point += delta;
                            points[pointIndex - 1].Point += delta;
                        }
                        else
                        {
                            points[pointIndex - 1].Point += delta;
                            points[pointIndex + 1].Point += delta;
                        }
                    }
                    else
                    {
                        if (pointIndex > 0)
                        {
                            points[pointIndex - 1].Point += delta;
                        }
                        if (pointIndex + 1 < points.Length)
                        {
                            points[pointIndex + 1].Point += delta;
                        }
                    }
                }
            }

            points[pointIndex].Point = point;

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

        protected override float GetUnclampedWorldLengthInternal()
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