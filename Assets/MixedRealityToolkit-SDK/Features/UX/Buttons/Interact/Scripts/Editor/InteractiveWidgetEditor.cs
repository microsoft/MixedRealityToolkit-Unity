// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Interact.Widgets
{
    [CustomEditor(typeof(InteractiveThemeWidget))]
    public class InteractiveWidgetEditor : Editor
    {
        protected bool ShowBlendOptions = false;
        protected InteractiveThemeWidget inspector;
        protected SerializedObject inspectorRef;
        protected SerializedProperty blendData;

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

        protected virtual void OnEnable()
        {
            inspector = (InteractiveThemeWidget)target;

            SetInspectorRef();

            SetupProperties();
        }

        /// <summary>
        /// override this to setup localreferences to derived classes
        /// </summary>
        protected virtual void SetInspectorRef()
        {
            inspectorRef = new SerializedObject(inspector);
        }

        /// <summary>
        /// set up all the local properties
        /// </summary>
        protected virtual void SetupProperties()
        {
            interactive = inspectorRef.FindProperty("InteractiveHost");
            themeTag = inspectorRef.FindProperty("ThemeTag");

            blendData = inspectorRef.FindProperty("BlendSettings");
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
            
            inspectorRef.Update();

            DisplayThemeTag();

            if (ShowBlendOptions)
            {
                DisplayBlendOptions();
            }
            
            inspectorRef.ApplyModifiedProperties();
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
    }
}
