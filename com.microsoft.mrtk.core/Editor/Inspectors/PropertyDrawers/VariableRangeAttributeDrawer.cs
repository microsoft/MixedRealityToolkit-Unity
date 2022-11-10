// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomPropertyDrawer(typeof(VariableRangeAttribute))]
    public class VariableRangePropertyDrawerer : PropertyDrawer
    {
        private GUIContent dropLabel;
        private GUIStyle labelStyle;

        public VariableRangePropertyDrawerer()
        {
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            VariableRangeAttribute range = attribute as VariableRangeAttribute;
            if (range == null)
            {
                Debug.LogError($"Property was did not have a VariableRangeAttribute");
                return;
            }

            string minVariablePath = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, range.MinVariableName) : range.MinVariableName;
            SerializedProperty minVariableProperty = property.serializedObject.FindProperty(minVariablePath);
            if (minVariableProperty == null || !(minVariableProperty.propertyType == SerializedPropertyType.Float || minVariableProperty.propertyType == SerializedPropertyType.Integer))
            {
                Debug.LogError($"VariableRangeAttribute couldn't find the variable used to define the minRange (property name: {range.MinVariableName})");
                return;
            }

            string maxVariablePath = property.propertyPath.Contains(".") ? System.IO.Path.ChangeExtension(property.propertyPath, range.MaxVariableName) : range.MaxVariableName;
            SerializedProperty maxVariableProperty = property.serializedObject.FindProperty(maxVariablePath);
            if (maxVariableProperty == null || !(maxVariableProperty.propertyType == SerializedPropertyType.Float || maxVariableProperty.propertyType == SerializedPropertyType.Integer))
            {
                Debug.LogError($"VariableRangeAttribute couldn't find a valid variable used to define the maxRange (property name: {range.MaxVariableName})");
                return;
            }

            float minValue = minVariableProperty.floatValue;
            float maxValue = maxVariableProperty.floatValue;

            labelStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).label;
            float labelWidth = labelStyle.CalcSize(label).x + EditorGUIUtility.singleLineHeight;
            Rect labelRect = new Rect(position.x, position.y, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.PrefixLabel(labelRect, label);

            Rect sliderRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, EditorGUIUtility.singleLineHeight);
            property.floatValue = EditorGUI.Slider(sliderRect, property.floatValue, minValue, maxValue);

            EditorGUI.BeginChangeCheck();
            property.floatValue = EditorGUI.Slider(sliderRect, property.floatValue, minValue, maxValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
            }

            EditorGUI.EndProperty();
        }
    }
}
