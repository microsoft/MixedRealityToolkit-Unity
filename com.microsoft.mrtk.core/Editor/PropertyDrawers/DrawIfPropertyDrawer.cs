// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Conditionally draws a property based on the value associated
    /// with the <see cref="DrawIfAttribute"/> attribute.
    /// </summary>
    /// <remarks>
    /// Based on: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
    /// </remarks>
    [CustomPropertyDrawer(typeof(DrawIfAttribute))]
    public class DrawIfPropertyDrawer : PropertyDrawer
    {
        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!ShouldShow(property))
            {
                return 0f;
            }

            return base.GetPropertyHeight(property, label);
        }

        private bool ShouldShow(SerializedProperty property)
        {
            DrawIfAttribute drawIf = attribute as DrawIfAttribute;
            if (drawIf == null) { return true; }

            string path = drawIf.ComparedPropertyName;

            SerializedProperty propertyToCheck = property.serializedObject.FindProperty(path);
            if (propertyToCheck == null)
            {
                Debug.LogError($"DrawIfAttribute couldn't find the SerializedProperty to compare against! (property name: {drawIf.ComparedPropertyName})");
                return true;
            }

            switch (propertyToCheck.type.ToLower())
            {
                case "bool":
                    return drawIf.ComparisonMode != DrawIfAttribute.ComparisonType.Equal ^ propertyToCheck.boolValue.Equals(drawIf.CompareAgainst);
                case "enum":
                    return drawIf.ComparisonMode != DrawIfAttribute.ComparisonType.Equal ^ propertyToCheck.enumValueIndex.Equals((int)drawIf.CompareAgainst);
                default:
                    Debug.LogError($"DrawIfAttribute only supports bool and Enum types. Your property '{drawIf.ComparedPropertyName}' is a {propertyToCheck.type}");
                    return true;
            }
        }
    }
}
