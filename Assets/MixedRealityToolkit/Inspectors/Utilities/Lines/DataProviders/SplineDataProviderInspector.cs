// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [CustomEditor(typeof(SplineDataProvider))]
    public class SplineDataProviderInspector : BaseLineDataProviderInspector
    {
        private const float OverlappingPointThreshold = 0.015f;
        private const float HandleSizeModifier = 0.04f;
        private const float PickSizeModifier = 0.06f;

        private static readonly HashSet<int> OverlappingPointIndexes = new HashSet<int>();

        private static readonly Vector2 ControlPointButtonSize = new Vector2(16, 16);
        private static readonly Vector2 LeftControlPointPositionOffset = Vector2.left * 12;
        private static readonly Vector2 RightControlPointPositionOffset = Vector2.right * 24;

        private static readonly Vector2 ControlPointButtonHandleOffset = Vector3.up * 24;

        private static readonly GUIContent PositionContent = new GUIContent("Position");
        private static readonly GUIContent RotationContent = new GUIContent("Rotation");
        private static readonly GUIContent AddControlPointContent = new GUIContent("+", "Add a control point");
        private static readonly GUIContent RemoveControlPointContent = new GUIContent("-", "Remove a control point");
        private static readonly GUIContent ControlPointHeaderContent = new GUIContent("Spline Control Points", "The current control points for the spline.");

        private static bool controlPointFoldout = true;

        private SerializedProperty controlPoints;
        private SerializedProperty alignAllControlPoints;

        private SplineDataProvider splineData;
        private ReorderableList controlPointList;

        private int selectedHandleIndex = -1;

        protected override void OnEnable()
        {
            base.OnEnable();

            splineData = (SplineDataProvider)target;
            controlPoints = serializedObject.FindProperty("controlPoints");
            alignAllControlPoints = serializedObject.FindProperty("alignAllControlPoints");

            controlPointList = new ReorderableList(serializedObject, controlPoints, false, false, false, false)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 3
            };

            controlPointList.drawElementCallback += DrawControlPointElement;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            // We always draw line points for splines.
            DrawLinePoints = true;

            EditorGUILayout.LabelField("Spline Settings");

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(alignAllControlPoints);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add New Point", GUILayout.Width(48f), GUILayout.ExpandWidth(true)))
            {
                AddControlPoint();
            }

            GUI.enabled = controlPoints.arraySize > 4;

            if (GUILayout.Button("Remove Last Point", GUILayout.Width(48f), GUILayout.ExpandWidth(true)))
            {
                RemoveControlPoint();
            }

            GUI.enabled = true;
            GUILayout.EndHorizontal();

            controlPointFoldout = EditorGUILayout.Foldout(controlPointFoldout, ControlPointHeaderContent, true);

            if (controlPointFoldout)
            {
                // If we found overlapping points, provide an option to auto-separate them
                if (OverlappingPointIndexes.Count > 0)
                {
                    EditorGUILayout.HelpBox("We noticed some of your control points have the same position.", MessageType.Warning);

                    if (GUILayout.Button("Fix overlapping points"))
                    {
                        // Move them slightly out of the way
                        foreach (int pointIndex in OverlappingPointIndexes)
                        {
                            var controlPointProperty = controlPoints.GetArrayElementAtIndex(pointIndex);
                            var position = controlPointProperty.FindPropertyRelative("position");
                            position.vector3Value += Random.onUnitSphere * OverlappingPointThreshold * 2;
                        }
                        OverlappingPointIndexes.Clear();
                    }
                }

                controlPointList.DoLayoutList();
            }

            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();

            // We skip the first point as it should always remain at the GameObject's local origin (Pose.ZeroIdentity)
            for (int i = 1; i < controlPoints?.arraySize; i++)
            {
                bool isTangentHandle = i % 3 != 0;

                serializedObject.Update();

                bool isLastPoint = i == controlPoints.arraySize - 1;

                var controlPointPosition = LineData.GetPoint(i);
                var controlPointProperty = controlPoints.GetArrayElementAtIndex(i);
                var controlPointRotation = controlPointProperty.FindPropertyRelative("rotation");

                // Draw our tangent lines
                Handles.color = Color.gray;
                if (i == 1)
                {
                    Handles.DrawLine(LineData.GetPoint(0), LineData.GetPoint(1));
                }
                else if (!isTangentHandle)
                {
                    Handles.DrawLine(LineData.GetPoint(i), LineData.GetPoint(i - 1));

                    if (!isLastPoint)
                    {
                        Handles.DrawLine(LineData.GetPoint(i), LineData.GetPoint(i + 1));
                    }
                }

                Handles.color = isTangentHandle ? Color.white : Color.green;
                float handleSize = HandleUtility.GetHandleSize(controlPointPosition);

                if (Handles.Button(controlPointPosition, controlPointRotation.quaternionValue, handleSize * HandleSizeModifier, handleSize * PickSizeModifier, Handles.DotHandleCap))
                {
                    selectedHandleIndex = i;
                }

                // Draw our handles
                if (Tools.current == Tool.Move && selectedHandleIndex == i)
                {
                    EditorGUI.BeginChangeCheck();

                    var newTargetPosition = Handles.PositionHandle(controlPointPosition, controlPointRotation.quaternionValue);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(LineData, "Change Spline Point Position");
                        LineData.SetPoint(i, newTargetPosition);
                    }

                    if (isLastPoint)
                    {
                        DrawSceneControlOptionButtons(controlPointPosition);
                    }
                }
                else if (Tools.current == Tool.Rotate && selectedHandleIndex == i)
                {
                    EditorGUI.BeginChangeCheck();
                    Quaternion newTargetRotation = Handles.RotationHandle(controlPointRotation.quaternionValue, controlPointPosition);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(LineData, "Change Spline Point Rotation");
                        controlPointRotation.quaternionValue = newTargetRotation;
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }

            // Check for overlapping points
            OverlappingPointIndexes.Clear();

            for (int i = 0; i < splineData.ControlPoints.Length; i++)
            {
                for (int j = 0; j < splineData.ControlPoints.Length; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    if (Vector3.Distance(splineData.ControlPoints[i].Position, splineData.ControlPoints[j].Position) < OverlappingPointThreshold)
                    {
                        if (i != 0)
                        {
                            OverlappingPointIndexes.Add(i);
                        }

                        if (j != 0)
                        {
                            OverlappingPointIndexes.Add(j);
                        }

                        break;
                    }
                }
            }
        }

        private void AddControlPoint()
        {
            serializedObject.Update();

            Undo.RecordObject(LineData, "Add Spline Control Point");

            var newControlPoints = new MixedRealityPose[3];
            Vector3 direction = LineData.GetVelocity(0.99f);
            float distance = Mathf.Max(LineData.UnClampedWorldLength * 0.05f, OverlappingPointThreshold * 5);
            newControlPoints[0].Position = LineData.LastPoint + (direction * distance);
            newControlPoints[1].Position = newControlPoints[0].Position + (direction * distance);
            newControlPoints[2].Position = newControlPoints[1].Position + (direction * distance);

            for (int i = 0; i < 3; i++)
            {
                controlPoints.arraySize = controlPoints.arraySize + 1;
                var newControlPointProperty = controlPoints.GetArrayElementAtIndex(controlPoints.arraySize - 1);
                newControlPointProperty.FindPropertyRelative("position").vector3Value = newControlPoints[i].Position;
                newControlPointProperty.FindPropertyRelative("rotation").quaternionValue = Quaternion.identity;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveControlPoint()
        {
            if (controlPoints.arraySize <= 4) { return; }

            serializedObject.Update();
            Undo.RecordObject(LineData, "Remove Spline Control Point");
            controlPoints.DeleteArrayElementAtIndex(controlPoints.arraySize - 1);
            controlPoints.DeleteArrayElementAtIndex(controlPoints.arraySize - 1);
            controlPoints.DeleteArrayElementAtIndex(controlPoints.arraySize - 1);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSceneControlOptionButtons(Vector3 position)
        {
            Handles.BeginGUI();

            var buttonPosition = HandleUtility.WorldToGUIPoint(position);
            var buttonRect = new Rect(buttonPosition + ControlPointButtonHandleOffset, ControlPointButtonSize);

            // Move the button slightly to the left
            buttonRect.position += LeftControlPointPositionOffset;

            if (GUI.Button(buttonRect, AddControlPointContent))
            {
                AddControlPoint();
            }

            if (controlPoints.arraySize > 4)
            {
                // Move the button slightly to the right
                buttonRect.position += RightControlPointPositionOffset;

                if (GUI.Button(buttonRect, RemoveControlPointContent))
                {
                    RemoveControlPoint();
                }
            }

            Handles.EndGUI();
        }

        private void DrawControlPointElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            bool lastMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;

            var lastLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 88f;

            var property = controlPoints.GetArrayElementAtIndex(index);
            var fieldHeight = EditorGUIUtility.singleLineHeight * 0.5f;
            var labelRect = new Rect(rect.x - 8f, rect.y + fieldHeight * 2, rect.width, EditorGUIUtility.singleLineHeight);
            var positionRect = new Rect(rect.x, rect.y + fieldHeight, rect.width, EditorGUIUtility.singleLineHeight);
            var rotationRect = new Rect(rect.x, rect.y + fieldHeight * 3, rect.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(labelRect, $"{index + 1}");

            EditorGUI.indentLevel++;

            GUI.enabled = index != 0;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(positionRect, property.FindPropertyRelative("position"), PositionContent);
            bool hasPositionChanged = EditorGUI.EndChangeCheck();

            var rotationProperty = property.FindPropertyRelative("rotation");

            EditorGUI.BeginChangeCheck();
            var newEulerRotation = EditorGUI.Vector3Field(rotationRect, RotationContent, rotationProperty.quaternionValue.eulerAngles);
            bool hasRotationChanged = EditorGUI.EndChangeCheck();

            if (hasRotationChanged)
            {
                rotationProperty.quaternionValue = Quaternion.Euler(newEulerRotation);
            }

            if (hasPositionChanged || hasRotationChanged)
            {
                EditorUtility.SetDirty(target);
            }

            GUI.enabled = true;
            EditorGUI.indentLevel--;
            EditorGUIUtility.wideMode = lastMode;
            EditorGUIUtility.labelWidth = lastLabelWidth;
        }
    }
}