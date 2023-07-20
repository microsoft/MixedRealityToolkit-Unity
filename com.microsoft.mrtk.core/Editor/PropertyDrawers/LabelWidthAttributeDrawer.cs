// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A custom property drawer for fields decorated with the <see cref="LabelWidthAttribute"/> attribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(LabelWidthAttribute))]
    internal class LabelWidthAttributeDrawer : PropertyDrawer
    {
        /// <inheritdoc />
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
