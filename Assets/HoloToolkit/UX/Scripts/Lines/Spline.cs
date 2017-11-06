// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
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
            private static bool editPositions = true;
            private static bool editRotations = false;

            private const float overlappingPointThreshold = 0.015f;

            // Convenience buttons for adding / removing points
            protected override void DrawCustomFooter()
            {
                base.DrawCustomFooter();

                Spline line = (Spline)target;

                HashSet<int> overlappingPointIndexes = new HashSet<int>();

                if (DrawSectionStart(line.name + " Points", "Point Editing"))
                {
                    if (GUILayout.Button(" + Add Points to Start"))
                    {
                        List<SplinePoint> points = new List<SplinePoint>();
                        SplinePoint[] newPoints = new SplinePoint[3];
                        Vector3 direction = line.GetVelocity(0.01f);
                        float distance = Mathf.Max(line.UnclampedWorldLength * 0.05f, overlappingPointThreshold * 5);
                        newPoints[2].Point = line.FirstPoint - (direction * distance);
                        newPoints[1].Point = newPoints[2].Point - (direction * distance);
                        newPoints[0].Point = newPoints[1].Point - (direction * distance);
                        points.AddRange(newPoints);
                        points.AddRange(line.Points);
                        line.Points = points.ToArray();
                    }
                    if (line.NumPoints > 4)
                    {
                        if (GUILayout.Button(" - Remove Points From Start"))
                        {
                            // Using lists for maximum clarity
                            List<SplinePoint> points = new List<SplinePoint>(line.Points);
                            points.RemoveAt(0);
                            points.RemoveAt(0);
                            points.RemoveAt(0);
                            line.Points = points.ToArray();
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
                    for (int i = 0; i < line.Points.Length; i++)
                    {
                        GUI.color = (i % 3 == 0) ? handleColorCircle : handleColorSquare;
                        if (editPositions)
                        {
                            GUI.color = Color.Lerp(GUI.color, defaultColor, 0.75f);
                            // highlight points that are overlapping
                            for (int j = 0; j < line.Points.Length; j++)
                            {
                                if (j == i)
                                    continue;

                                if (Vector3.Distance(line.Points[j].Point, line.Points[i].Point) < overlappingPointThreshold)
                                {
                                    overlappingPointIndexes.Add(i);
                                    overlappingPointIndexes.Add(j);
                                    GUI.color = errorColor;
                                    break;
                                }
                            }
                            line.Points[i].Point = UnityEditor.EditorGUILayout.Vector3Field(string.Empty, line.Points[i].Point);
                        }
                        else
                        {
                            GUI.color = Color.Lerp(GUI.color, disabledColor, 0.75f);
                            UnityEditor.EditorGUILayout.Vector3Field(string.Empty, line.Points[i].Point);
                        }
                    }
                    UnityEditor.EditorGUILayout.EndVertical();

                    // Rotations
                    GUI.color = defaultColor;
                    UnityEditor.EditorGUILayout.BeginVertical();
                    editRotations = UnityEditor.EditorGUILayout.Toggle("Edit Rotations", editRotations);
                    GUI.color = (editRotations ? defaultColor : disabledColor);
                    for (int i = 0; i < line.Points.Length; i++)
                    {
                        GUI.color = (i % 3 == 0) ? handleColorCircle : handleColorSquare;
                        if (editRotations)
                        {
                            GUI.color = Color.Lerp(GUI.color, defaultColor, 0.75f);
                            line.Points[i].Rotation = Quaternion.Euler(UnityEditor.EditorGUILayout.Vector3Field(string.Empty, line.Points[i].Rotation.eulerAngles));
                        }
                        else
                        {
                            GUI.color = Color.Lerp(GUI.color, disabledColor, 0.75f);
                            UnityEditor.EditorGUILayout.Vector3Field(string.Empty, line.Points[i].Rotation.eulerAngles);
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
                        if (GUILayout.Button("Fix overlappoing points"))
                        {
                            // Move them slightly out of the way
                            foreach (int overlappoingPointIndex in overlappingPointIndexes)
                            {
                                line.Points[overlappoingPointIndex].Point += (UnityEngine.Random.onUnitSphere * overlappingPointThreshold * 2);
                            }
                        }
                    }

                    UnityEditor.EditorGUILayout.EndVertical();

                    GUI.color = defaultColor;
                    if (GUILayout.Button(" + Add Points To End"))
                    {
                        // Using lists for maximum clarity
                        List<SplinePoint> points = new List<SplinePoint>();
                        SplinePoint[] newPoints = new SplinePoint[3];
                        Vector3 direction = line.GetVelocity(0.99f);
                        float distance = Mathf.Max(line.UnclampedWorldLength * 0.05f, overlappingPointThreshold * 5);
                        newPoints[0].Point = line.LastPoint + (direction * distance);
                        newPoints[1].Point = newPoints[0].Point + (direction * distance);
                        newPoints[2].Point = newPoints[1].Point + (direction * distance);
                        points.AddRange(line.Points);
                        points.AddRange(newPoints);
                        line.Points = points.ToArray();
                    }
                    if (line.NumPoints > 4)
                    {
                        if (GUILayout.Button(" - Remove Points From End"))
                        {
                            // Using lists for maximum clarity
                            List<SplinePoint> points = new List<SplinePoint>(line.Points);
                            points.RemoveAt(points.Count - 1);
                            points.RemoveAt(points.Count - 1);
                            points.RemoveAt(points.Count - 1);
                            line.Points = points.ToArray();
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
                        line.Points[i].Rotation = RotationHandle(line.GetPoint(i), line.Points[i].Rotation);
                    }
                }
            }
        }
#endif

    }
}