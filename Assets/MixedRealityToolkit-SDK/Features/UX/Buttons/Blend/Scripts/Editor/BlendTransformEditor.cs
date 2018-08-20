// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Blend
{
    [CustomEditor(typeof(BlendTransform))]
    public class BlendTransformEditor : Editor
    {
        private BlendTransform inspector;
        private SerializedObject inspectorRef;
        private SerializedProperty transformDataList;
        
        private void OnEnable()
        {
            inspector = (BlendTransform)target;
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
                BlendTransformData data = GetGenericTransformData();

                inspector.BlendData.Add(data);
            }

            // create custom list
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            for (int i = 0; i < transformDataList.arraySize; i++)
            {
                
                BlendTransformData shaderData = inspector.BlendData[i];
                SerializedProperty item = transformDataList.GetArrayElementAtIndex(i);
                
                // set up the dropdown list for the shader property.
                SerializedProperty transformProperties = item.FindPropertyRelative("TransformProperties");
                SerializedProperty local = transformProperties.FindPropertyRelative("ToLocalTransform");
                SerializedProperty type = transformProperties.FindPropertyRelative("Type");

                SerializedProperty targetVector = item.FindPropertyRelative("VectorValues.TargetValue");
                SerializedProperty targetQuaternion = item.FindPropertyRelative("QuaternionValues.TargetValue");
                SerializedProperty blend = item.FindPropertyRelative("InstanceProperties");
                
                TransformTypes transform = (TransformTypes)EditorGUILayout.EnumPopup("Transform Type", (TransformTypes)type.enumValueIndex);

                if ((int)transform != type.enumValueIndex)
                {
                    type.enumValueIndex = (int)transform;
                }

                EditorGUILayout.Space();

                EditorGUI.indentLevel += 1;
                bool showLocal = true;

                switch (transform)
                {
                    case TransformTypes.Position:
                        targetVector.vector3Value = EditorGUILayout.Vector3Field("Position", targetVector.vector3Value);
                        break;
                    case TransformTypes.Scale:
                        targetVector.vector3Value = EditorGUILayout.Vector3Field("Scale", targetVector.vector3Value);
                        showLocal = false;
                        break;
                    case TransformTypes.Rotation:
                        targetVector.vector3Value = EditorGUILayout.Vector3Field("Rotation", targetVector.vector3Value);
                        break;
                    case TransformTypes.Quaternion:
                        Vector4 currentValue = BlendTransformCollection.QuaternionToVector4(targetQuaternion.quaternionValue);
                        Vector4 newValue = EditorGUILayout.Vector4Field("Rotation", currentValue);
                        targetQuaternion.quaternionValue = BlendTransformCollection.Vector4ToQuaternion(newValue);
                        break;
                    default:
                        break;
                }

                if (showLocal)
                {
                    local.boolValue = EditorGUILayout.Toggle("To Local Transform", local.boolValue);
                }

                EditorGUILayout.Space();
                
                // Blend values
                SerializedProperty time = blend.FindPropertyRelative("LerpTime");
                SerializedProperty ease = blend.FindPropertyRelative("EaseCurve");
                SerializedProperty loop = blend.FindPropertyRelative("LoopType");
                
                time.floatValue = EditorGUILayout.FloatField("Lerp Time", time.floatValue);
                
                EditorGUILayout.PropertyField(ease, new GUIContent("Ease Curve"));
               
                LoopTypes loopType = (LoopTypes)EditorGUILayout.EnumPopup("Loop Type", (LoopTypes)loop.enumValueIndex);

                if ((int)loopType != loop.enumValueIndex)
                {
                    loop.enumValueIndex = (int)loopType;
                    shaderData.InstanceProperties.LoopType = loopType;
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

        private BlendTransformData GetGenericTransformData()
        {
            BlendTransformData data = new BlendTransformData();
            
            data.InstanceProperties.EaseCurve = AbstractBlend.GetEaseCurve(BasicEaseCurves.Linear);
            data.InstanceProperties.LerpTime = 1;

            return data;
        }

        private GameObject GetGameObject()
        {
            return inspector.gameObject;
        }
    }
}
