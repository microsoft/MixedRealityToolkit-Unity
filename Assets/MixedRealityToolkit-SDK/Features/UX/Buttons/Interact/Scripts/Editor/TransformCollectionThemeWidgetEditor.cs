// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Interact.Widgets
{
    [CustomEditor(typeof(TransformCollectionThemeWidget))]
    public class TransformsCollectionThemeWidgetEditor : Editor
    {
        private TransformCollectionThemeWidget inspector;
        private SerializedObject inspectorRef;
        private SerializedProperty transformDataList;

        private void OnEnable()
        {
            inspector = (TransformCollectionThemeWidget)target;
            inspectorRef = new SerializedObject(inspector);
            transformDataList = inspectorRef.FindProperty("BlendData");
        }

        public override void OnInspectorGUI()
        {
            inspectorRef.Update();

            EditorGUILayout.Space();
            GUILayout.Label("Transform Blends");

            // custom list button
            if (GUILayout.Button("Add Blend (" + transformDataList.arraySize + ")"))
            {
                BlendTransformWidgetData data = GetGenericTransformData();

                inspector.BlendData.Add(data);
            }

            // create custom list
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            for (int i = 0; i < transformDataList.arraySize; i++)
            {

                /*
                    public string Tag;
                    public Vector3InteractiveTheme VectorTheme;
                    public FloatInteractiveTheme FloatTheme;
                    public QuaternionInteractiveTheme QuaternionTheme;
                    public bool Blend;
                    public BlendTransformData BlendData;
                */

                BlendTransformWidgetData transformData = inspector.BlendData[i];
                SerializedProperty item = transformDataList.GetArrayElementAtIndex(i);

                SerializedProperty useBlend = item.FindPropertyRelative("Blend");
                SerializedProperty tag = item.FindPropertyRelative("Tag");

                SerializedProperty itemData = item.FindPropertyRelative("BlendData");

                // set up the dropdown list for the shader property.
                SerializedProperty transformProperties = itemData.FindPropertyRelative("TransformProperties");
                SerializedProperty local = transformProperties.FindPropertyRelative("ToLocalTransform");
                SerializedProperty type = transformProperties.FindPropertyRelative("Type");

                //SerializedProperty targetVector = transformProperties.FindPropertyRelative("VectorValues.TargetValue");
                //SerializedProperty targetQuaternion = transformProperties.FindPropertyRelative("QuaternionValues.TargetValue");
                SerializedProperty blend = itemData.FindPropertyRelative("InstanceProperties");

                TransformTypes transform = (TransformTypes)EditorGUILayout.EnumPopup("Transform Type", (TransformTypes)type.enumValueIndex);

                if ((int)transform != type.enumValueIndex)
                {
                    type.enumValueIndex = (int)transform;
                }

                EditorGUILayout.Space();

                EditorGUI.indentLevel += 1;
                bool showLocal = true;

                if (transform == TransformTypes.Quaternion)
                {
                    tag.stringValue = EditorGUILayout.TextField("Quaternion Theme Tag", tag.stringValue);
                }
                else
                {
                    tag.stringValue = EditorGUILayout.TextField("Vector3 Theme Tag", tag.stringValue);

                    if (transform == TransformTypes.Scale)
                    {
                        showLocal = false;
                    }
                }

                if (showLocal)
                {
                    local.boolValue = EditorGUILayout.Toggle("To Local Transform", local.boolValue);
                }

                EditorGUILayout.Space();
                useBlend.boolValue = EditorGUILayout.Toggle("Use Blend", useBlend.boolValue);

                if (useBlend.boolValue)
                {
                    EditorGUI.indentLevel += 1;
                    // Blend values
                    SerializedProperty time = blend.FindPropertyRelative("LerpTime");
                    SerializedProperty ease = blend.FindPropertyRelative("EaseCurve");
                    //SerializedProperty loop = blend.FindPropertyRelative("LoopType");

                    time.floatValue = EditorGUILayout.FloatField("Lerp Time", time.floatValue);

                    EditorGUILayout.PropertyField(ease, new GUIContent("Ease Curve"));

                    /* let's make sure we do not need this
                    LoopTypes loopType = (LoopTypes)EditorGUILayout.EnumPopup("Loop Type", (LoopTypes)loop.enumValueIndex);

                    if ((int)loopType != loop.enumValueIndex)
                    {
                        loop.enumValueIndex = (int)loopType;
                    }
                    */

                    EditorGUI.indentLevel -= 1;
                }

                float indent = 20;
                GUIContent labelName = new GUIContent("Blend " + i.ToString());
                float labelWidth = GUI.skin.label.CalcSize(labelName).x + indent * EditorGUI.indentLevel;
                GUILayoutOption LabelStyle = GUILayout.Width(labelWidth);
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                RectOffset margin = buttonStyle.margin;

                GUIContent buttonLabel = new GUIContent("Delete");
                float buttonWidth = GUI.skin.button.CalcSize(buttonLabel).x;
                margin.right = 0;
                margin.left = 0;
                buttonStyle.margin = margin;

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(labelName, LabelStyle);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(buttonLabel, buttonStyle, GUILayout.Width(buttonWidth)))
                {
                    transformDataList.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel -= 1;

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            }

            //Apply the changes to our list
            inspectorRef.ApplyModifiedProperties();

            // end custom list

            DrawDefaultInspector();
        }

        private BlendTransformWidgetData GetGenericTransformData()
        {
            BlendTransformWidgetData data = new BlendTransformWidgetData();
            data.BlendData.FloatValues = new BlendFloats();
            data.BlendData.QuaternionValues = new BlendQuaternions();
            data.BlendData.VectorValues = new BlendVectors();
            data.BlendData.InstanceProperties.EaseCurve = AbstractBlend.GetEaseCurve(BasicEaseCurves.Linear);
            data.BlendData.InstanceProperties.LerpTime = 1;

            return data;
        }

        private GameObject GetGameObject()
        {
            return inspector.gameObject;
        }
    }
}
