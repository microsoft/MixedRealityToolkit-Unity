// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Utilities.Lines.DataProviders
{
    [CustomEditor(typeof(SplineDataProvider))]
    public class SplineDataProviderInspector : BaseMixedRealityLineDataProviderInspector
    {
        private const float OverlappingPointThreshold = 0.015f;
        private static HashSet<int> overlappingPointIndexes = new HashSet<int>();

        private static readonly GUIContent PositionContent = new GUIContent("Position");
        private static readonly GUIContent RotationContent = new GUIContent("Rotation");
        private static readonly GUIContent ControlPointHeaderContent = new GUIContent("Spline Control Points", "The current control points for the spline.");

        private static bool controlPointFoldout = true;

        private SerializedProperty alignControlPoints;
        private SerializedProperty controlPoints;

        private SplineDataProvider splineData;
        private ReorderableList controlPointList;

        protected override void OnEnable()
        {
            base.OnEnable();

            splineData = (SplineDataProvider)target;
            alignControlPoints = serializedObject.FindProperty("alignControlPoints");
            controlPoints = serializedObject.FindProperty("controlPoints");

            controlPointList = new ReorderableList(serializedObject, controlPoints, false, false, false, false)
            {
                elementHeight = EditorGUIUtility.singleLineHeight * 3
            };

            controlPointList.drawElementCallback += DrawControlPointElement;
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

            controlPoints.DeleteArrayElementAtIndex(controlPoints.arraySize - 1);
            controlPoints.DeleteArrayElementAtIndex(controlPoints.arraySize - 1);
            controlPoints.DeleteArrayElementAtIndex(controlPoints.arraySize - 1);
        }

        private static void DrawControlPointHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, ControlPointHeaderContent);
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

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.LabelField("Spline Settings");

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(alignControlPoints);
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
                if (overlappingPointIndexes.Count > 0)
                {
                    EditorGUILayout.HelpBox("We noticed some of your control points have the same position.", MessageType.Warning);

                    if (GUILayout.Button("Fix overlapping points"))
                    {
                        // Move them slightly out of the way
                        foreach (int pointIndex in overlappingPointIndexes)
                        {
                            var controlPointProperty = controlPoints.GetArrayElementAtIndex(pointIndex);
                            var position = controlPointProperty.FindPropertyRelative("position");
                            position.vector3Value += Random.onUnitSphere * OverlappingPointThreshold * 2;
                        }
                        overlappingPointIndexes.Clear();
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
                serializedObject.Update();
                var pointProperty = controlPoints.GetArrayElementAtIndex(i);
                var positionProperty = pointProperty.FindPropertyRelative("position");
                var rotationProperty = pointProperty.FindPropertyRelative("rotation");

                if (Tools.current == Tool.Move)
                {
                    EditorGUI.BeginChangeCheck();
                    Vector3 newTargetPosition = Handles.PositionHandle(LineData.transform.TransformPoint(positionProperty.vector3Value), rotationProperty.quaternionValue);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(LineData, "Change Spline Point Position");
                        positionProperty.vector3Value = LineData.transform.InverseTransformPoint(newTargetPosition);
                    }
                }
                else if (Tools.current == Tool.Rotate)
                {
                    EditorGUI.BeginChangeCheck();
                    Quaternion newTargetRotation = Handles.RotationHandle(rotationProperty.quaternionValue, LineData.transform.TransformPoint(positionProperty.vector3Value));

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(LineData, "Change Spline Point Rotation");
                        rotationProperty.quaternionValue = newTargetRotation;
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }

            // Check for overlapping points
            overlappingPointIndexes.Clear();
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
                            overlappingPointIndexes.Add(i);
                        }

                        if (j != 0)
                        {
                            overlappingPointIndexes.Add(j);
                        }

                        break;
                    }
                }
            }
        }
    }
}