using System.Collections.Generic;
using UnityEngine;

namespace MixedRealityToolkit.UX.Lines
{
    [UnityEditor.CustomEditor(typeof(Spline))]
    public class SplineEditor : LineBaseEditor
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
}
