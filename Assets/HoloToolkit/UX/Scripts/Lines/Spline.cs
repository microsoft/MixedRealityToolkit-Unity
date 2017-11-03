using System;
using UnityEngine;

namespace MRTK.UX
{
    public class Spline : LineBase
    {
        [Header("Spline Settings")]
        public SplinePoint[] Points = new SplinePoint[4];

        [SerializeField]
        private bool alignControlPoints = true;

        public bool AlignControlPoints {
            get {
                return alignControlPoints;
            }
            set {
                if (alignControlPoints != value) {
                    alignControlPoints = value;
                    ForceUpdateAlignment();
                }
            }
        }

        public override int NumPoints
        {
            get
            {
                return Points.Length;
            }
        }

        public void ForceUpdateAlignment() {
            if (AlignControlPoints) {
                for (int i = 0; i < NumPoints; i++) {
                    ForceUpdateAlignment(i);
                }
            }
        }

        private void ForceUpdateAlignment(int pointIndex) {
            if (AlignControlPoints) {
                Vector3 point = Points[pointIndex].Point;

                int prevControlPoint = 0;
                int changedControlPoint = 0;
                int midPointIndex = ((pointIndex + 1) / 3) * 3;

                if (pointIndex <= midPointIndex) {
                    prevControlPoint = midPointIndex - 1;
                    changedControlPoint = midPointIndex + 1;
                } else {
                    prevControlPoint = midPointIndex + 1;
                    changedControlPoint = midPointIndex - 1;
                }

                if (loops) {
                    if (changedControlPoint < 0) {
                        changedControlPoint = (NumPoints - 1) + changedControlPoint;
                    } else if (changedControlPoint >= NumPoints) {
                        changedControlPoint = changedControlPoint % (NumPoints - 1);
                    }

                    if (prevControlPoint < 0) {
                        prevControlPoint = (NumPoints - 1) + prevControlPoint;
                    } else if (prevControlPoint >= NumPoints) {
                        prevControlPoint = prevControlPoint % (NumPoints - 1);
                    }

                    Vector3 midPoint = Points[midPointIndex].Point;
                    Vector3 tangent = midPoint - Points[prevControlPoint].Point;
                    tangent = tangent.normalized * Vector3.Distance(midPoint, Points[changedControlPoint].Point);
                    Points[changedControlPoint].Point = midPoint + tangent;

                } else if (changedControlPoint >= 0 && changedControlPoint < NumPoints && prevControlPoint >= 0 && prevControlPoint < NumPoints) {
                    Vector3 midPoint = Points[midPointIndex].Point;
                    Vector3 tangent = midPoint - Points[prevControlPoint].Point;
                    tangent = tangent.normalized * Vector3.Distance(midPoint, Points[changedControlPoint].Point);
                    Points[changedControlPoint].Point = midPoint + tangent;
                }
            }
        }

        public override void AppendPoint(Vector3 point)
        {
            int pointIndex = Points.Length;
            Array.Resize<SplinePoint>(ref Points, Points.Length + 1);
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

            if (!loops) {
                if (point1Index + 3 >= NumPoints) {
                    return Points[NumPoints - 1].Point;
                }
                if (point1Index < 0)
                    return Points[0].Point;

                point2Index = point1Index + 1;
                point3Index = point1Index + 2;
                point4Index = point1Index + 3;

            } else {
                point2Index = (point1Index + 1) % (NumPoints - 1);
                point3Index = (point1Index + 2) % (NumPoints - 1);
                point4Index = (point1Index + 3) % (NumPoints - 1);
            }

            Vector3 point1 = Points[point1Index].Point;
            Vector3 point2 = Points[point2Index].Point;
            Vector3 point3 = Points[point3Index].Point;
            Vector3 point4 = Points[point4Index].Point;

            return LineUtils.InterpolateBezeirPoints(point1, point2, point3, point4, subDistance);
        }

        protected override Vector3 GetPointInternal(int pointIndex)
        {
            if (pointIndex < 0 || pointIndex >= Points.Length)
                throw new IndexOutOfRangeException();

            if (loops && pointIndex == NumPoints - 1) {
                Points[pointIndex] = Points[0];
                pointIndex = 0;
            }

            return Points[pointIndex].Point;
        }
       
        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            if (pointIndex < 0 || pointIndex >= Points.Length)
                throw new IndexOutOfRangeException();

            if (loops && pointIndex == NumPoints - 1) {
                Points[pointIndex] = Points[0];
                pointIndex = 0;
            }

            if (AlignControlPoints) {
                if (pointIndex % 3 == 0) {
                    Vector3 delta = point - Points[pointIndex].Point;
                    if (loops) {
                        if (pointIndex == 0) {
                            Points[1].Point += delta;
                            Points[NumPoints - 2].Point += delta;
                            Points[NumPoints - 1].Point = point;
                        } else if (pointIndex == NumPoints) {
                            Points[0].Point = point;
                            Points[1].Point += delta;
                            Points[pointIndex - 1].Point += delta;
                        } else {
                            Points[pointIndex - 1].Point += delta;
                            Points[pointIndex + 1].Point += delta;
                        }
                    } else {
                        if (pointIndex > 0) {
                            Points[pointIndex - 1].Point += delta;
                        }
                        if (pointIndex + 1 < Points.Length) {
                            Points[pointIndex + 1].Point += delta;
                        }
                    }
                }
            }

            Points[pointIndex].Point = point;

            ForceUpdateAlignment(pointIndex);
        }

        protected override Vector3 GetUpVectorInternal(float normalizedLength) {

            float arrayValueLength = 1f / Points.Length;
            int indexA = Mathf.FloorToInt(normalizedLength * Points.Length);
            if (indexA >= Points.Length)
                indexA = 0;

            int indexB = indexA + 1;
            if (indexB >= Points.Length)
                indexB = 0;

            float blendAmount = (normalizedLength - (arrayValueLength * indexA)) / arrayValueLength;
            Quaternion rotation = Quaternion.Lerp(Points[indexA].Rotation, Points[indexB].Rotation, blendAmount);
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

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(Spline))]
        public class CustomEditor : LineBaseEditor
        {
            protected override void DrawCustomSceneGUI()
            {
                base.DrawCustomSceneGUI();

                Spline line = (Spline)target;
                
                for (int i = 0; i < line.NumPoints; i++)
                {
                    // Draw squares at start / end and circles for mid-points
                    if (i == 0 || i == line.NumPoints - 1)
                    {
                        line.SetPoint(i, SquareMoveHandle(line.GetPoint(i)));
                    }
                    else
                    {
                        line.SetPoint(i, CircleMoveHandle(line.GetPoint(i)));
                    }
                }
            }
        }
#endif

    }
}