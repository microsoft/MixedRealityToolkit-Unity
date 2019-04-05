// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Creates a spline based on control points.
    /// </summary>
    public class SplineDataProvider : BaseMixedRealityLineDataProvider
    {
        [SerializeField]
        private MixedRealityPose[] controlPoints =
        {
            MixedRealityPose.ZeroIdentity,
            new MixedRealityPose(new Vector3(0.5f, 0.5f, 0f), Quaternion.identity),
            new MixedRealityPose(new Vector3(0.5f, -0.5f, 0f), Quaternion.identity),
            new MixedRealityPose(Vector3.right, Quaternion.identity),
        };

        public MixedRealityPose[] ControlPoints => controlPoints;

        [SerializeField]
        private bool alignAllControlPoints = true;

        public bool AlignAllControlPoints
        {
            get { return alignAllControlPoints; }
            set
            {
                if (alignAllControlPoints != value)
                {
                    alignAllControlPoints = value;
                    ForceUpdateAlignment();
                }
            }
        }

        /// <summary>
        /// Forces all the control points into alignment.
        /// </summary>
        public void ForceUpdateAlignment()
        {
            if (alignAllControlPoints)
            {
                for (int i = 0; i < PointCount; i++)
                {
                    UpdatePointAlignment(i);
                }
            }
        }

        private void UpdatePointAlignment(int pointIndex)
        {
            if (alignAllControlPoints)
            {
                int prevControlPoint;
                int changedControlPoint;
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

                    Vector3 midPoint = controlPoints[midPointIndex].Position;
                    Vector3 tangent = midPoint - controlPoints[prevControlPoint].Position;
                    tangent = tangent.normalized * Vector3.Distance(midPoint, controlPoints[changedControlPoint].Position);
                    controlPoints[changedControlPoint].Position = midPoint + tangent;
                }
                else if (changedControlPoint >= 0 && changedControlPoint < PointCount && prevControlPoint >= 0 && prevControlPoint < PointCount)
                {
                    Vector3 midPoint = controlPoints[midPointIndex].Position;
                    Vector3 tangent = midPoint - controlPoints[prevControlPoint].Position;
                    tangent = tangent.normalized * Vector3.Distance(midPoint, controlPoints[changedControlPoint].Position);
                    controlPoints[changedControlPoint].Position = midPoint + tangent;
                }
            }
        }

        #region BaseMixedRealityLineDataProvider Implementation

        /// <inheritdoc />
        public override int PointCount => controlPoints.Length;

        /// <inheritdoc />
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
                    return controlPoints[PointCount - 1].Position;
                }

                if (point1Index < 0)
                {
                    return controlPoints[0].Position;
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

            Vector3 point1 = controlPoints[point1Index].Position;
            Vector3 point2 = controlPoints[point2Index].Position;
            Vector3 point3 = controlPoints[point3Index].Position;
            Vector3 point4 = controlPoints[point4Index].Position;

            return LineUtility.InterpolateBezierPoints(point1, point2, point3, point4, subDistance);
        }

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(int pointIndex)
        {
            if (pointIndex < 0 || pointIndex >= controlPoints.Length)
            {
                Debug.LogError("Invalid point index!");
                return Vector3.zero;
            }

            if (Loops && pointIndex == PointCount - 1)
            {
                controlPoints[pointIndex] = controlPoints[0];
                pointIndex = 0;
            }

            return controlPoints[pointIndex].Position;
        }

        /// <inheritdoc />
        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            if (pointIndex < 0 || pointIndex >= controlPoints.Length)
            {
                Debug.LogError("Invalid point index!");
                return;
            }

            if (Loops && pointIndex == PointCount - 1)
            {
                controlPoints[pointIndex] = controlPoints[0];
                pointIndex = 0;
            }

            if (alignAllControlPoints)
            {
                if (pointIndex % 3 == 0)
                {
                    Vector3 delta = point - controlPoints[pointIndex].Position;
                    if (Loops)
                    {
                        if (pointIndex == 0)
                        {
                            controlPoints[1].Position += delta;
                            controlPoints[PointCount - 2].Position += delta;
                            controlPoints[PointCount - 1].Position = point;
                        }
                        else if (pointIndex == PointCount)
                        {
                            controlPoints[0].Position = point;
                            controlPoints[1].Position += delta;
                            controlPoints[pointIndex - 1].Position += delta;
                        }
                        else
                        {
                            controlPoints[pointIndex - 1].Position += delta;
                            controlPoints[pointIndex + 1].Position += delta;
                        }
                    }
                    else
                    {
                        if (pointIndex > 0)
                        {
                            controlPoints[pointIndex - 1].Position += delta;
                        }

                        if (pointIndex + 1 < controlPoints.Length)
                        {
                            controlPoints[pointIndex + 1].Position += delta;
                        }
                    }
                }
            }

            controlPoints[pointIndex].Position = point;
            UpdatePointAlignment(pointIndex);
        }

        /// <inheritdoc />
        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            float arrayValueLength = 1f / controlPoints.Length;
            int indexA = Mathf.FloorToInt(normalizedLength * controlPoints.Length);

            if (indexA >= controlPoints.Length)
            {
                indexA = 0;
            }

            int indexB = indexA + 1;

            if (indexB >= controlPoints.Length)
            {
                indexB = 0;
            }

            float blendAmount = (normalizedLength - (arrayValueLength * indexA)) / arrayValueLength;
            Quaternion rotation = Quaternion.Lerp(controlPoints[indexA].Rotation, controlPoints[indexB].Rotation, blendAmount);
            return rotation * transform.up;
        }

        /// <inheritdoc />
        protected override float GetUnClampedWorldLengthInternal()
        {
            float distance = 0f;
            Vector3 last = GetPoint(0f);

            for (int i = 1; i < BaseMixedRealityLineDataProvider.UnclampedWorldLengthSearchSteps; i++)
            {
                Vector3 current = GetPoint((float)i / BaseMixedRealityLineDataProvider.UnclampedWorldLengthSearchSteps);
                distance += Vector3.Distance(last, current);
            }

            return distance;
        }

        #endregion BaseMixedRealityLineDataProvider Implementation
    }
}