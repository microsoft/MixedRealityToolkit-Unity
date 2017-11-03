using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MRTK.UX
{
    [CanEditMultipleObjects]
    //[CustomEditor(typeof(Spline))]
    public class SplineInspector : LineBaseEditor
    {
        static bool adjustRotations = false;
        static bool showAlignment = false;

        /*public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Spline s = (Spline)target;

            HUXEditorUtils.BeginSectionBox("Control point alignment");
            s.AlignControlPoints = EditorGUILayout.Toggle("Align Control Points", s.AlignControlPoints);
            if (s.AlignControlPoints) {
                if (GUILayout.Button("Enforce alignment")) {
                    s.ForceUpdateAlignment();
                }
            }
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.BeginSectionBox("Control point rotation");
            GUI.color = adjustRotations ? HUXEditorUtils.WarningColor : HUXEditorUtils.DefaultColor;
            if (GUILayout.Button (adjustRotations ? "Editing rotation in scene (click to stop)" : "Edit in scene")) {
                adjustRotations = !adjustRotations;
            }
            for (int i = 0; i < s.Points.Length; i++) {
                s.Points[i].Rotation = Quaternion.Euler(EditorGUILayout.Vector3Field(i.ToString ("000"), s.Points[i].Rotation.eulerAngles));
            }
            showAlignment = EditorGUILayout.Toggle("Show aligment", showAlignment);
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.BeginSectionBox ("Points");
            if (GUILayout.Button(" + Add Points to Start"))
            {
                Undo.RegisterCompleteObjectUndo(target, "Add Points to Start");
                List<SplinePoint> points = new List<SplinePoint>();
                SplinePoint[] newPoints = new SplinePoint[3];
                Vector3 direction = (s.GetPoint(0) - s.GetPoint(1)).normalized;
                newPoints[0].Point = s.GetPoint(0) + direction * 0.1f;
                newPoints[1].Point = newPoints[1].Point + direction * 0.1f;
                newPoints[2].Point = newPoints[2].Point + direction * 0.1f;
                points.AddRange(newPoints);
                points.AddRange(s.Points);
                s.Points = points.ToArray();
            }

            if (GUILayout.Button(" + Add Points To End"))
            {
                Undo.RegisterCompleteObjectUndo(target, "Add Points to End");
                List<SplinePoint> points = new List<SplinePoint>();
                SplinePoint[] newPoints = new SplinePoint[3];
                Vector3 direction = (s.GetPoint(s.NumPoints - 1) - s.GetPoint(s.NumPoints - 2)).normalized;
                newPoints[0].Point = s.GetPoint(0) + direction * 0.1f;
                newPoints[1].Point = newPoints[1].Point + direction * 0.1f;
                newPoints[2].Point = newPoints[2].Point + direction * 0.1f;
                points.AddRange(newPoints);
                s.Points = points.ToArray();
            }

            GUI.color = (s.Points.Length > 3) ? Color.white : Color.gray;
            if (GUILayout.Button(" - Remove Points from Start")) {
                if (s.Points.Length <= 3)
                    return;

                Undo.RegisterCompleteObjectUndo(target, "Remove Points from Start");
                int numToRemove = s.Points.Length % 3;
                if (numToRemove == 0) {
                    numToRemove = 3;
                }
                List<SplinePoint> points = new List<SplinePoint>(s.Points);
                for (int i = 0; i < numToRemove; i++) {
                    points.RemoveAt(0);
                }
                s.Points = points.ToArray();
            }

            GUI.color = (s.Points.Length > 3) ? Color.white : Color.gray;
            if (GUILayout.Button(" - Remove Points from End")) {
                if (s.Points.Length <= 3)
                    return;

                Undo.RegisterCompleteObjectUndo(target, "Remove Points from End");
                int numToRemove = s.Points.Length % 3;
                if (numToRemove == 0) {
                    numToRemove = 3;
                }
                List<SplinePoint> points = new List<SplinePoint>(s.Points);
                for (int i = 0; i < numToRemove; i++) {
                    points.RemoveAt(points.Count - 1);
                }
                s.Points = points.ToArray();
            }
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.SaveChanges(target, serializedObject);
        }

        public override void OnSceneGUI()
        {
            base.OnSceneGUI();

            Spline s = (Spline)target;

            Handles.color = Color.cyan;
            Vector3 lastPoint = s.GetPoint(0);
            for (int i = 0; i < s.NumPoints; i++)
            {
                Vector3 currentPoint = s.GetPoint(i);
                bool drawSphere = true;
                
                if (i == 0)
                {
                    Handles.Label(currentPoint + Vector3.down * 0.05f, "Start");
                }
                else if (i == s.NumPoints - 1)
                {
                    if (!s.Loops) {
                        Handles.Label(currentPoint + Vector3.down * 0.05f, "End");
                    }
                }
                else
                {
                    if (i % 3 == 0) {
                        Handles.Label(currentPoint + Vector3.down * 0.05f, (i + 1).ToString("000"));
                    }
                    drawSphere = false;
                }

                if (adjustRotations) {
                    // Show handles for rotation
                    Handles.color = Color.cyan;
                    Handles.DrawLine(lastPoint, currentPoint);
                    Handles.color = Color.Lerp(Color.white, Color.magenta, 0.9f);
                    lastPoint = currentPoint;

                    Quaternion rotation = Handles.RotationHandle(s.Points[i].Rotation, currentPoint);
                    if (rotation != s.Points[i].Rotation) {
                        if (!recordingUndo) {
                            recordingUndo = true;
                            Undo.RegisterCompleteObjectUndo(target, "Edit Spline");
                        }
                        s.Points[i].Rotation = rotation;
                    }

                } else {
                    // Show handles for transform
                    Handles.color = Color.cyan;
                    Handles.DrawLine(lastPoint, currentPoint);
                    Handles.color = Color.Lerp(Color.white, Color.magenta, 0.9f);
                    lastPoint = currentPoint;
                    Vector3 newPoint = currentPoint;

                    if (drawSphere) {
                        newPoint = Handles.FreeMoveHandle(currentPoint, Quaternion.identity, HandleUtility.GetHandleSize(newPoint) * 0.3f, Vector3.zero, Handles.SphereHandleCap);
                    } else {
                        if (i % 3 == 0) {
                            newPoint = Handles.FreeMoveHandle(currentPoint, Quaternion.identity, HandleUtility.GetHandleSize(newPoint) * 0.1f, Vector3.zero, Handles.CircleHandleCap);
                        } else {
                            Handles.color = Color.yellow;
                            newPoint = Handles.FreeMoveHandle(currentPoint, Quaternion.identity, HandleUtility.GetHandleSize(newPoint) * 0.075f, Vector3.zero, Handles.RectangleHandleCap);
                        }
                    }
                    if (Event.current.alt) {
                        newPoint.x = currentPoint.x;
                        newPoint.z = currentPoint.z;
                        Handles.color = Handles.yAxisColor;
                        Handles.DrawArrow(0, newPoint, Quaternion.LookRotation(Vector3.up, Vector3.up), HandleUtility.GetHandleSize(newPoint) * 0.25f);
                        Handles.DrawArrow(0, newPoint, Quaternion.LookRotation(Vector3.down, Vector3.up), HandleUtility.GetHandleSize(newPoint) * 0.25f);
                    }
                    if (Event.current.shift) {
                        newPoint.y = currentPoint.y;
                        newPoint.z = currentPoint.z;
                        Handles.color = Handles.xAxisColor;
                        Handles.DrawArrow(0, newPoint, Quaternion.LookRotation(Vector3.left, Vector3.up), HandleUtility.GetHandleSize(newPoint) * 0.25f);
                        Handles.DrawArrow(0, newPoint, Quaternion.LookRotation(Vector3.right, Vector3.up), HandleUtility.GetHandleSize(newPoint) * 0.25f);

                    }
                    if (Event.current.control) {
                        newPoint.x = currentPoint.x;
                        newPoint.y = currentPoint.y;
                        Handles.color = Handles.zAxisColor;
                        Handles.DrawArrow(0, newPoint, Quaternion.LookRotation(Vector3.forward, Vector3.up), HandleUtility.GetHandleSize(newPoint) * 0.25f);
                        Handles.DrawArrow(0, newPoint, Quaternion.LookRotation(Vector3.back, Vector3.up), HandleUtility.GetHandleSize(newPoint) * 0.25f);
                    }
                    if (currentPoint != newPoint) {
                        if (!recordingUndo) {
                            recordingUndo = true;
                            Undo.RegisterCompleteObjectUndo(target, "Edit Spline");
                        }
                        s.SetPoint(i, newPoint);
                    }
                }
            }

            if (s.Loops)
            {
                Handles.color = Color.cyan;
                Handles.DrawLine(lastPoint, s.GetPoint(0));
            }

            Vector3 lastPos = s.GetPoint(0f);
            Vector3 currentPos = Vector3.zero;

            for (int i = 1; i < s.NumPoints * 4; i++) {
                float normalizedDistance = (float)i / (s.NumPoints * 4);
                currentPos = s.GetPoint(normalizedDistance);
                Handles.color = Color.white;
                Handles.DrawDottedLine(lastPos, currentPos, 5f);
                if (showAlignment) {
                    Handles.color = Color.Lerp(Handles.zAxisColor, Color.clear, 0.5f);
                    Quaternion rotation = s.GetRotation(normalizedDistance);
                    Handles.DrawArrow(-1, currentPos, rotation, HandleUtility.GetHandleSize(currentPos) * 2.5f);
                }
                lastPos = currentPos;
            }
        }

        private bool recordingUndo = false;
        private bool mouseDown = false;*/
    }
}