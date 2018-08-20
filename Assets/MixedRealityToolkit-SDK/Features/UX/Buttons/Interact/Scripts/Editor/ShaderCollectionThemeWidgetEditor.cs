// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Interact.Widgets
{
    [CustomEditor(typeof(ShaderCollectionThemeWidget))]
    public class ShaderCollectionThemeWidgetEditor : Editor
    {
        private ShaderCollectionThemeWidget inspector;
        private SerializedObject inspectorRef;
        private SerializedProperty shaderDataList;
        private ShaderProperties[] shaderPropertyList;
        private string[] shaderOptions;
        private Material material;

        private string noMaterial = "No Material";
        private ShaderPropertyType[] filters = new ShaderPropertyType[] { ShaderPropertyType.Color, ShaderPropertyType.Float, ShaderPropertyType.Range };

        private void OnEnable()
        {
            inspector = (ShaderCollectionThemeWidget)target;
            inspectorRef = new SerializedObject(inspector);
            shaderDataList = inspectorRef.FindProperty("BlendData");
        }

        public override void OnInspectorGUI()
        {
            UpdateShaderList();
            inspectorRef.Update();

            EditorGUILayout.Space();
            GUILayout.Label("Shader Widget");

            // custom list button
            if (GUILayout.Button("Add Widget (" + shaderDataList.arraySize + ")"))
            {
                BlendShaderWidgetData data = GetGenericShaderData();

                inspector.BlendData.Add(data);
            }
            
            // create custom list
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            for (int i = 0; i < shaderDataList.arraySize; i++)
            {
                
                BlendShaderWidgetData shaderData = inspector.BlendData[i];
                SerializedProperty item = shaderDataList.GetArrayElementAtIndex(i);

                SerializedProperty useBlend = item.FindPropertyRelative("Blend");
                SerializedProperty tag = item.FindPropertyRelative("Tag");
                //SerializedProperty floatTheme = item.FindPropertyRelative("FloatTheme");
                //SerializedProperty colorTheme = item.FindPropertyRelative("ColorTheme");

                SerializedProperty itemData = item.FindPropertyRelative("BlendData");

                // set up the dropdown list for the shader property.
                SerializedProperty shaderProperty = itemData.FindPropertyRelative("ShaderProperty");
                SerializedProperty name = shaderProperty.FindPropertyRelative("Name");
                SerializedProperty type = shaderProperty.FindPropertyRelative("Type");
                SerializedProperty range = shaderProperty.FindPropertyRelative("Range");

                //SerializedProperty targetColor = itemData.FindPropertyRelative("ColorValues.TargetValue");
                //SerializedProperty targetFloat = itemData.FindPropertyRelative("FloatValues.TargetValue");
                SerializedProperty blend = itemData.FindPropertyRelative("InstanceProperties");

                int index = EditorGUILayout.Popup("Shader Property", ReverseLookup(name.stringValue), shaderOptions);

                if (shaderOptions[index] != name.stringValue)
                {
                    ShaderProperties props = GetShaderProperties(shaderOptions[index]);
                    name.stringValue = props.Name;
                    type.enumValueIndex = (int)props.Type;
                    range.vector2Value = props.Range;
                }

                EditorGUI.indentLevel += 1;

                if (type.enumValueIndex == (int)ShaderPropertyType.Color)
                {
                    // show color value
                    //targetColor.colorValue = EditorGUILayout.ColorField("Color", targetColor.colorValue);
                    tag.stringValue = EditorGUILayout.TextField("Color Theme Tag", tag.stringValue);
                }
                else
                {
                    // show float value
                    //targetFloat.floatValue = EditorGUILayout.FloatField("Float", targetFloat.floatValue);
                    tag.stringValue = EditorGUILayout.TextField("Float Theme Tag", tag.stringValue);

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

                    /*
                    LoopTypes loopType = (LoopTypes)EditorGUILayout.EnumPopup("Loop Type", (LoopTypes)loop.enumValueIndex);

                    if ((int)loopType != loop.enumValueIndex)
                    {
                        loop.enumValueIndex = (int)loopType;
                    }
                    */

                    EditorGUI.indentLevel -= 1;
                }

                // delete button
                float indent = 20;
                GUIContent labelName = new GUIContent("Widget " + i.ToString());
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
                    shaderDataList.DeleteArrayElementAtIndex(i);
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

        private BlendShaderWidgetData GetGenericShaderData()
        {
            BlendShaderWidgetData data = new BlendShaderWidgetData();
            data.BlendData = new BlendShaderData();

            BlendShaderData shaderData = new BlendShaderData();
            shaderData.ColorValues = new BlendColors();
            shaderData.ColorValues.TargetValue = Color.white;
            shaderData.FloatValues = new BlendFloats();
            shaderData.ShaderProperty = GetShaderProperties(shaderOptions[0]);
            shaderData.InstanceProperties.EaseCurve = AbstractBlend.GetEaseCurve(BasicEaseCurves.Linear);
            shaderData.InstanceProperties.LerpTime = 1;

            data.BlendData = shaderData;

            return data;
        }

        private GameObject GetGameObject()
        {
            return inspector.gameObject;
        }

        private void UpdateShaderList()
        {
            Material checkMaterial = ColorAbstraction.GetValidMaterial(GetGameObject().GetComponent<Renderer>());
            if (checkMaterial != material || shaderPropertyList == null)
            {
                if (checkMaterial != null)
                {
                    GetShaderProperties(GetGameObject());
                    material = checkMaterial;
                }
            }
        }

        private void GetShaderProperties(GameObject gameObject)
        {
            shaderPropertyList = BlendShaderUtils.GetShaderProperties(gameObject.GetComponent<Renderer>(), filters);

            if (shaderPropertyList.Length < 1)
            {
                shaderPropertyList = new ShaderProperties[] { NullShaderProperties() };
            }

            if (shaderPropertyList != null)
            {
                List<string> options = new List<string>();

                for (int i = 0; i < shaderPropertyList.Length; i++)
                {
                    if (shaderPropertyList[i].Name == noMaterial || !OptionUsed(shaderPropertyList[i].Name))
                    {
                        options.Add(shaderPropertyList[i].Name);
                    }
                }

                shaderOptions = options.ToArray();
            }
        }

        private ShaderProperties NullShaderProperties()
        {
            return new ShaderProperties() { Name = noMaterial, Range = new Vector2(), Type = ShaderPropertyType.None };
        }

        private bool OptionUsed(string property)
        {
            // TODO: look in the serialized list to see if any of the values are already used,
            // we do not want two properties animating competing

            return false;
        }

        private int ReverseLookup(string name)
        {
            for (int i = 0; i < shaderOptions.Length; i++)
            {
                if (shaderOptions[i] == name)
                {
                    return i;
                }
            }

            return 0;
        }

        private ShaderProperties GetShaderProperties(string name)
        {
            for (int i = 0; i < shaderPropertyList.Length; i++)
            {
                if (shaderPropertyList[i].Name == name)
                {
                    return shaderPropertyList[i];
                }
            }

            return NullShaderProperties();
        }
    }
}
