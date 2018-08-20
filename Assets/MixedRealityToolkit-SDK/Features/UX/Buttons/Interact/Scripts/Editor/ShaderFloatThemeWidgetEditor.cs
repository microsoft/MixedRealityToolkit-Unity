// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Interact.Widgets
{
    [CustomEditor(typeof(ShaderFloatThemeWidget))]
    public class ShaderFloatThemeWidgetEditor : ShaderPropertyEditor
    {
        protected ShaderFloatThemeWidget widget;
        private SerializedObject widgetRef;
        protected SerializedProperty blendData;

        private ShaderFloatThemeWidget Inspector;

        // Widget variables
        protected SerializedProperty interactive;
        protected SerializedProperty themeTag;

        // Blend variables
        protected SerializedProperty useBlend;
        protected SerializedProperty blendProperties;
        protected SerializedProperty inited;
        protected SerializedProperty time;
        protected SerializedProperty ease;
        protected SerializedProperty loop;

        protected void OnEnable()
        {
            Inspector = (ShaderFloatThemeWidget)target;
            widget = (ShaderFloatThemeWidget)target;
            widgetRef = new SerializedObject(widget);
            interactive = widgetRef.FindProperty("InteractiveHost");
            themeTag = widgetRef.FindProperty("ThemeTag");

            blendData = widgetRef.FindProperty("BlendSettings");
            useBlend = blendData.FindPropertyRelative("UseBlend");
            blendProperties = blendData.FindPropertyRelative("BlendProperties");
            inited = blendData.FindPropertyRelative("Inited");
            time = blendProperties.FindPropertyRelative("LerpTime");
            ease = blendProperties.FindPropertyRelative("EaseCurve");
            time = blendProperties.FindPropertyRelative("LerpTime");
            ease = blendProperties.FindPropertyRelative("EaseCurve");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            widgetRef.Update();

            DisplayThemeTag();

            DisplayDropDown();

            DisplayBlendOptions();
            widgetRef.ApplyModifiedProperties();

            EditorGUILayout.Space();
        }

        protected virtual void DisplayThemeTag()
        {
            EditorGUILayout.Space();
            //interactive.objectReferenceValue = EditorGUILayout.ObjectField("Interactive Host", interactive.objectReferenceValue, typeof(Interactive), true);
            themeTag.stringValue = EditorGUILayout.TextField("Theme Tag", themeTag.stringValue);
        }

        protected virtual void DisplayBlendOptions()
        {
            EditorGUILayout.Space();
            useBlend.boolValue = EditorGUILayout.Toggle("Use Blend", useBlend.boolValue);

            if (useBlend.boolValue)
            {
                EditorGUI.indentLevel += 1;
                // Blend values

                if (!inited.boolValue)
                {
                    inited.boolValue = true;
                    ease.animationCurveValue = AbstractBlend.GetEaseCurve(BasicEaseCurves.Linear);
                    time.floatValue = 1;
                }

                time.floatValue = EditorGUILayout.FloatField("Lerp Time", time.floatValue);

                EditorGUILayout.PropertyField(ease, new GUIContent("Ease Curve"));

                /* we are not using loop settings for widgets, at least for now.
                
                //SerializedProperty loop = blend.FindPropertyRelative("LoopType");
                
                LoopTypes loopType = (LoopTypes)EditorGUILayout.EnumPopup("Loop Type", (LoopTypes)loop.enumValueIndex);

                if ((int)loopType != loop.enumValueIndex)
                {
                    loop.enumValueIndex = (int)loopType;
                }
                */

                EditorGUI.indentLevel -= 1;
            }
        }

        protected override int GetCurrentIndex()
        {
            if (shaderOptions != null && shaderOptions.Length > 0)
            {
                return ReverseLookup(Inspector.ShaderPropery);
            }
            else
            {
                Debug.Log("No Shader found on GameObject " + Inspector.gameObject.name);
                return 0;
            }
        }

        protected override ShaderPropertyType[] GetFilters()
        {
            return new ShaderPropertyType[] { ShaderPropertyType.Float, ShaderPropertyType.Range };
        }

        protected override GameObject GetGameObject()
        {
            return Inspector.gameObject;
        }

        protected override void SelectProperty()
        {
            if (shaderPropertyList.Length > selectedIndex)
            {
                Inspector.ShaderPropery = shaderPropertyList[selectedIndex].Name;
            }
            else
            {
                Inspector.ShaderPropery = "No Shader";
            }
        }
    }
}
