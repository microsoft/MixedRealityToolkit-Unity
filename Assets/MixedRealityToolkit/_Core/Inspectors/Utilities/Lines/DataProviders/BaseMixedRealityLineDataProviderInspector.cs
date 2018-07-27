// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Utilities.Lines.DataProviders
{
    [CustomEditor(typeof(BaseMixedRealityLineDataProvider))]
    public class BaseMixedRealityLineDataProviderInspector : Editor
    {
        private static readonly GUIContent BasicSettingsContent = new GUIContent("Basic Settings");
        private static readonly GUIContent RotationSettingsContent = new GUIContent("Rotation Settings");
        private static readonly GUIContent DistortionSettingsContent = new GUIContent("Distortion Settings");

        private static bool basicSettingsFoldout = true;
        private static bool rotationSettingsFoldout = true;
        private static bool distortionSettingsFoldout = true;

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

        protected virtual void OnEnable()
        {
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
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

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
                EditorGUILayout.PropertyField(manualUpVectorBlend);
                EditorGUILayout.PropertyField(manualUpVectors, true);
                EditorGUILayout.PropertyField(velocitySearchRange);

                EditorGUI.indentLevel--;
            }

            distortionSettingsFoldout = EditorGUILayout.Foldout(distortionSettingsFoldout, DistortionSettingsContent, true);

            if (distortionSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(distorters, true);

                if (distorters.arraySize > 0)
                {
                    EditorGUILayout.PropertyField(distortionType);
                    EditorGUILayout.PropertyField(distortionStrength);
                    EditorGUILayout.PropertyField(uniformDistortionStrength);
                }

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
