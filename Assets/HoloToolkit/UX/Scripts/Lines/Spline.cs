// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.UX
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

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(Spline))]
        public class CustomEditor : LineBaseEditor
        {
            private static bool editPositions = true;
            private static bool editRotations = false;
            private static List<SplinePoint> pointsList = new List<SplinePoint>();
            private static HashSet<int> overlappingPointIndexes = new HashSet<int>();

            private const float overlappingPointThreshold = 0.015f;

            // Convenience buttons for adding / removing points
            protected override void DrawCustomFooter()
            {
                base.DrawCustomFooter();

                Spline line = (Spline)target;

                overlappingPointIndexes.Clear();

                if (DrawSectionStart(line.name + " Points", "Point Editing"))
                {
                    if (GUILayout.Button(" + Add Points to Start"))
                    {
                        pointsList.Clear();
                        SplinePoint[] newPoints = new SplinePoint[3];
                        Vector3 direction = line.GetVelocity(0.01f);
                        float distance = Mathf.Max(line.UnclampedWorldLength * 0.05f, overlappingPointThreshold * 5);
                        newPoints[2].Point = line.FirstPoint - (direction * distance);
                        newPoints[1].Point = newPoints[2].Point - (direction * distance);
                        newPoints[0].Point = newPoints[1].Point - (direction * distance);
                        pointsList.AddRange(newPoints);
                        pointsList.AddRange(line.points);
                        line.points = pointsList.ToArray();
                    }
                    if (line.NumPoints > 4)
                    {
                        if (GUILayout.Button(" - Remove Points From Start"))
                        {
                            // Using lists for maximum clarity
                            pointsList.Clear();
                            pointsList.AddRange(line.points);
                            pointsList.RemoveAt(0);
                            pointsList.RemoveAt(0);
                            pointsList.RemoveAt(0);
                            line.points = pointsList.ToArray();
                        }
                    }

                    // Points list
                    UnityEditor.EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                    bool wideModeSetting = UnityEditor.EditorGUIUtility.wideMode;
                    UnityEditor.EditorGUIUtility.wideMode = false;
                    UnityEditor.EditorGUILayout.BeginHorizontal();

                    // Positions
                    UnityEditor.EditorGUILayout.BeginVertical();
                    GUI.color = (editPositions ? defaultColor : disabledColor);
                    editPositions = UnityEditor.EditorGUILayout.Toggle("Edit Positions", editPositions);
                    for (int i = 0; i < line.points.Length; i++)
                    {
                        GUI.color = (i % 3 == 0) ? handleColorCircle : handleColorSquare;
                        if (editPositions)
                        {
                            GUI.color = Color.Lerp(GUI.color, defaultColor, 0.75f);
                            // highlight points that are overlapping
                            for (int j = 0; j < line.points.Length; j++)
                            {
                                if (j == i)
                                {
                                    continue;
                                }

                                if (Vector3.Distance(line.points[j].Point, line.points[i].Point) < overlappingPointThreshold)
                                {
                                    overlappingPointIndexes.Add(i);
                                    overlappingPointIndexes.Add(j);
                                    GUI.color = errorColor;
                                    break;
                                }
                            }
                            line.points[i].Point = UnityEditor.EditorGUILayout.Vector3Field(string.Empty, line.points[i].Point);
                        }
                        else
                        {
                            GUI.color = Color.Lerp(GUI.color, disabledColor, 0.75f);
                            UnityEditor.EditorGUILayout.Vector3Field(string.Empty, line.points[i].Point);
                        }
                    }
                    UnityEditor.EditorGUILayout.EndVertical();

                    // Rotations
                    GUI.color = defaultColor;
                    UnityEditor.EditorGUILayout.BeginVertical();
                    editRotations = UnityEditor.EditorGUILayout.Toggle("Edit Rotations", editRotations);
                    GUI.color = (editRotations ? defaultColor : disabledColor);
                    for (int i = 0; i < line.points.Length; i++)
                    {
                        GUI.color = (i % 3 == 0) ? handleColorCircle : handleColorSquare;
                        if (editRotations)
                        {
                            GUI.color = Color.Lerp(GUI.color, defaultColor, 0.75f);
                            line.points[i].Rotation = Quaternion.Euler(UnityEditor.EditorGUILayout.Vector3Field(string.Empty, line.points[i].Rotation.eulerAngles));
                        }
                        else
                        {
                            GUI.color = Color.Lerp(GUI.color, disabledColor, 0.75f);
                            UnityEditor.EditorGUILayout.Vector3Field(string.Empty, line.points[i].Rotation.eulerAngles);
                        }
                    }
                    UnityEditor.EditorGUILayout.EndVertical();

                    UnityEditor.EditorGUILayout.EndHorizontal();
                    UnityEditor.EditorGUIUtility.wideMode = wideModeSetting;

                    GUI.color = defaultColor;
                    // If we found overlapping points, provide an option to auto-separate them
                    if (overlappingPointIndexes.Count > 0)
                    {
                        GUI.color = errorColor;
                        if (GUILayout.Button("Fix overlapping points"))
                        {
                            // Move them slightly out of the way
                            foreach (int overlappoingPointIndex in overlappingPointIndexes)
                            {
                                line.points[overlappoingPointIndex].Point += (UnityEngine.Random.onUnitSphere * overlappingPointThreshold * 2);
                            }
                        }
                    }

                    UnityEditor.EditorGUILayout.EndVertical();

                    GUI.color = defaultColor;
                    if (GUILayout.Button(" + Add Points To End"))
                    {
                        // Using lists for maximum clarity
                        pointsList.Clear();
                        SplinePoint[] newPoints = new SplinePoint[3];
                        Vector3 direction = line.GetVelocity(0.99f);
                        float distance = Mathf.Max(line.UnclampedWorldLength * 0.05f, overlappingPointThreshold * 5);
                        newPoints[0].Point = line.LastPoint + (direction * distance);
                        newPoints[1].Point = newPoints[0].Point + (direction * distance);
                        newPoints[2].Point = newPoints[1].Point + (direction * distance);
                        pointsList.AddRange(line.points);
                        pointsList.AddRange(newPoints);
                        line.points = pointsList.ToArray();
                    }
                    if (line.NumPoints > 4)
                    {
                        if (GUILayout.Button(" - Remove Points From End"))
                        {
                            // Using lists for maximum clarity
                            pointsList.Clear();
                            pointsList.AddRange(line.points);
                            pointsList.RemoveAt(pointsList.Count - 1);
                            pointsList.RemoveAt(pointsList.Count - 1);
                            pointsList.RemoveAt(pointsList.Count - 1);
                            line.points = pointsList.ToArray();
                        }
                    }
                }
                DrawSectionEnd();
            }

            protected override void DrawCustomSceneGUI()
            {
                Spline line = (Spline)target;

                base.DrawCustomSceneGUI();

                for (int i = 0; i < line.NumPoints; i++)
                {
                    if (editPositions)
                    {
                        if (i % 3 == 0)
                        {
                            if (i == 0)
                            {
                                line.SetPoint(i, SphereMoveHandle(line.GetPoint(i)));
                                UnityEditor.Handles.color = handleColorTangent;
                                UnityEditor.Handles.DrawLine(line.GetPoint(i), line.GetPoint(i + 1));
                            }
                            else if (i == line.NumPoints - 1)
                            {
                                line.SetPoint(i, SphereMoveHandle(line.GetPoint(i)));
                                UnityEditor.Handles.color = handleColorTangent;
                                UnityEditor.Handles.DrawLine(line.GetPoint(i), line.GetPoint(i - 1));
                            }
                            else
                            {
                                line.SetPoint(i, CircleMoveHandle(line.GetPoint(i)));
                                UnityEditor.Handles.color = handleColorTangent;
                                UnityEditor.Handles.DrawLine(line.GetPoint(i), line.GetPoint(i + 1));
                                UnityEditor.Handles.DrawLine(line.GetPoint(i), line.GetPoint(i - 1));
                            }

                        }
                        else
                        {
                            line.SetPoint(i, SquareMoveHandle(line.GetPoint(i)));
                        }
                    }

                    if (editRotations)
                    {
                        line.points[i].Rotation = RotationHandle(line.GetPoint(i), line.points[i].Rotation);
                    }
                }
            }
        }
#endif

    }
}