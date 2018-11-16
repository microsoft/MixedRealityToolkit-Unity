// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.PropertyDrawers
{
    /// <summary>
    /// Custom property drawer for <see cref="Vector3RangeAttribute"/> decorated <see cref="Vector3"/> values rendered in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(Vector3RangeAttribute))]
    public class Vector3RangePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.wideMode ? EditorGUIUtility.singleLineHeight : EditorGUIUtility.singleLineHeight * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var range = (Vector3RangeAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.Vector3)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(position, property);

                if (EditorGUI.EndChangeCheck())
                {
                    var vectorData = property.vector3Value;

                    vectorData.x = Mathf.Clamp(vectorData.x, range.Min, range.Max);
                    vectorData.y = Mathf.Clamp(vectorData.y, range.Min, range.Max);
                    vectorData.z = Mathf.Clamp(vectorData.z, range.Min, range.Max);

                    property.vector3Value = vectorData;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use Vector3Range with Vector3 fields only.");
            }
        }
    }
}