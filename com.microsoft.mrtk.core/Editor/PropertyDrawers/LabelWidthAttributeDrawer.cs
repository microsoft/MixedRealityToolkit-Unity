// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomPropertyDrawer(typeof(LabelWidthAttribute))]
    internal class LabelWidthAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LabelWidthAttribute labelWidthAttribute = attribute as LabelWidthAttribute;

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidthAttribute.Width;
            EditorGUI.PropertyField(position, property, label);
            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
    }
}
