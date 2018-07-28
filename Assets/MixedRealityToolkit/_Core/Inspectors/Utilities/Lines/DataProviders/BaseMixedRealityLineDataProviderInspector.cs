// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.Renderers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities.Physics.Distorters;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Utilities.Lines.DataProviders
{
    [CustomEditor(typeof(BaseMixedRealityLineDataProvider))]
    public class BaseMixedRealityLineDataProviderInspector : Editor
    {
        private static readonly GUIContent BasicSettingsContent = new GUIContent("Basic Settings");
        private static readonly GUIContent EditorSettingsContent = new GUIContent("Editor Settings");
        private static readonly GUIContent RotationSettingsContent = new GUIContent("Rotation Settings");
        private static readonly GUIContent DistortionSettingsContent = new GUIContent("Distortion Settings");
        private static readonly GUIContent ManualUpVectorContent = new GUIContent("Manual Up Vectors");

        private static bool basicSettingsFoldout = true;
        private static bool editorSettingsFoldout = false;
        private static bool rotationSettingsFoldout = true;
        private static bool distortionSettingsFoldout = true;

        protected static int LinePreviewResolution = 16;
        protected static bool DrawLinePoints = false;
        protected static bool DrawDottedLine = true;
        protected static bool DrawLineRotations = false;
        protected static bool DrawLineManualUpVectors = false;
        protected static float LineRotationLength = 0.5f;
        protected static float LineManualUpVectorLength = 1f;

        private SerializedProperty customLineTransform;
        private SerializedProperty lineStartClamp;
        private SerializedProperty lineEndClamp;
        private SerializedProperty loops;
        private SerializedProperty rotationType;
        private SerializedProperty flipUpVector;
        private SerializedProperty originOffset;
        private SerializedProperty manualUpVectorBlend;
        private SerializedProperty manualUpVectors;
        private SerializedProperty velocitySearchRange;
        private SerializedProperty distorters;
        private SerializedProperty distortionType;
        private SerializedProperty distortionStrength;
        private SerializedProperty uniformDistortionStrength;

        private ReorderableList manualUpVectorList;

        protected BaseMixedRealityLineDataProvider LineData;
        protected bool RenderLinePreview = true;

        protected virtual void OnEnable()
        {
            LineData = (BaseMixedRealityLineDataProvider)target;
            customLineTransform = serializedObject.FindProperty("customLineTransform");
            lineStartClamp = serializedObject.FindProperty("lineStartClamp");
            lineEndClamp = serializedObject.FindProperty("lineEndClamp");
            loops = serializedObject.FindProperty("loops");
            rotationType = serializedObject.FindProperty("rotationType");
            flipUpVector = serializedObject.FindProperty("flipUpVector");
            originOffset = serializedObject.FindProperty("originOffset");
            manualUpVectorBlend = serializedObject.FindProperty("manualUpVectorBlend");
            manualUpVectors = serializedObject.FindProperty("manualUpVectors");
            velocitySearchRange = serializedObject.FindProperty("velocitySearchRange");
            distorters = serializedObject.FindProperty("distorters");
            distortionType = serializedObject.FindProperty("distortionType");
            distortionStrength = serializedObject.FindProperty("distortionStrength");
            uniformDistortionStrength = serializedObject.FindProperty("uniformDistortionStrength");

            manualUpVectorList = new ReorderableList(serializedObject, manualUpVectors, true, true, true, true);
            manualUpVectorList.drawElementCallback += DrawManualUpVectorListElement;
            manualUpVectorList.drawHeaderCallback += DrawManualUpVectorHeader;

            RenderLinePreview = LineData.gameObject.GetComponent<BaseMixedRealityLineRenderer>() == null;

            var newDistorters = LineData.gameObject.GetComponents<Distorter>();
            distorters.arraySize = newDistorters.Length;

            for (int i = 0; i < newDistorters.Length; i++)
            {
                var distorterProperty = distorters.GetArrayElementAtIndex(i);
                distorterProperty.objectReferenceValue = newDistorters[i];
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            editorSettingsFoldout = EditorGUILayout.Foldout(editorSettingsFoldout, EditorSettingsContent, true);

            if (editorSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                LinePreviewResolution = EditorGUILayout.IntSlider("Preview Resolution", LinePreviewResolution, 2, 128);

                if (EditorGUI.EndChangeCheck())
                {
                    SceneView.RepaintAll();
                }

                DrawDottedLine = EditorGUILayout.Toggle("Draw Dotted Line", DrawDottedLine);
                DrawLinePoints = EditorGUILayout.Toggle("Draw Line Points", DrawLinePoints);
                DrawLineRotations = EditorGUILayout.Toggle("Draw Line Rotations", DrawLineRotations);

                if (DrawLineRotations)
                {
                    LineRotationLength = EditorGUILayout.Slider("Rotation Arrow Length", LineRotationLength, 0.01f, 5f);
                }

                DrawLineManualUpVectors = EditorGUILayout.Toggle("Draw Manual Up Vectors", DrawLineManualUpVectors);

                if (DrawLineManualUpVectors)
                {
                    LineManualUpVectorLength = EditorGUILayout.Slider("Manual Up Vector Length", LineManualUpVectorLength, 1f, 10f);
                }

                EditorGUI.indentLevel--;
            }

            basicSettingsFoldout = EditorGUILayout.Foldout(basicSettingsFoldout, BasicSettingsContent, true);

            if (basicSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(customLineTransform);
                EditorGUILayout.PropertyField(lineStartClamp);
                EditorGUILayout.PropertyField(lineEndClamp);
                EditorGUILayout.PropertyField(loops);

                EditorGUI.indentLevel--;
            }

            rotationSettingsFoldout = EditorGUILayout.Foldout(rotationSettingsFoldout, RotationSettingsContent, true);

            if (rotationSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(rotationType);
                EditorGUILayout.PropertyField(flipUpVector);
                EditorGUILayout.PropertyField(originOffset);
                EditorGUILayout.PropertyField(velocitySearchRange);

                if (DrawLineManualUpVectors)
                {
                    manualUpVectorList.DoLayoutList();

                    if (GUILayout.Button("Normalize Up Vectors"))
                    {

                        for (int i = 0; i < manualUpVectors.arraySize; i++)
                        {
                            var manualUpVectorProperty = manualUpVectors.GetArrayElementAtIndex(i);

                            Vector3 upVector = manualUpVectorProperty.vector3Value;

                            if (upVector == Vector3.zero)
                            {
                                upVector = Vector3.up;
                            }

                            manualUpVectorProperty.vector3Value = upVector.normalized;
                        }
                    }

                    EditorGUILayout.PropertyField(manualUpVectorBlend);
                }

                EditorGUI.indentLevel--;
            }

            distortionSettingsFoldout = EditorGUILayout.Foldout(distortionSettingsFoldout, DistortionSettingsContent, true);

            if (distortionSettingsFoldout)
            {
                if (distorters.arraySize > 0)
                {
                    EditorGUI.indentLevel++;

                    if (distorters.arraySize > 0)
                    {
                        EditorGUILayout.PropertyField(distortionType);
                        EditorGUILayout.PropertyField(distortionStrength);
                        EditorGUILayout.PropertyField(uniformDistortionStrength);
                    }

                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUILayout.HelpBox("No distorters attached to this line.\nTry adding a distortion component.", MessageType.Info);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void OnSceneGUI()
        {
            if (Application.isPlaying || !RenderLinePreview)
            {
                return;
            }

            Vector3 firstPos = LineData.GetPoint(0f);
            Vector3 lastPos = firstPos;
            Handles.color = Color.magenta;

            for (int i = 1; i < LinePreviewResolution; i++)
            {
                float normalizedLength = (1f / (LinePreviewResolution - 1)) * i;
                Vector3 currentPos = LineData.GetPoint(normalizedLength);
                Handles.DrawLine(lastPos, currentPos);
                lastPos = currentPos;
            }

            if (LineData.Loops)
            {
                Handles.DrawLine(lastPos, firstPos);
            }
        }

        private static void DrawManualUpVectorHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, ManualUpVectorContent);
        }

        private void DrawManualUpVectorListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.BeginChangeCheck();

            var property = manualUpVectors.GetArrayElementAtIndex(index);
            property.vector3Value = EditorGUI.Vector3Field(rect, GUIContent.none, property.vector3Value);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
