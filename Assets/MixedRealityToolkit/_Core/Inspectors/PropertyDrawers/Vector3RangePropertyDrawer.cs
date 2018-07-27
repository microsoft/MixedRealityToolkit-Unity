// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Attributes;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Inspectors.PropertyDrawers
{
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

                    if (vectorData.x < range.Min)
                    {
                        vectorData.x = range.Min;
                    }
                    else if (vectorData.x > range.Max)
                    {
                        vectorData.x = range.Max;
                    }

                    if (vectorData.y < range.Min)
                    {
                        vectorData.y = range.Min;
                    }
                    else if (vectorData.y > range.Max)
                    {
                        vectorData.y = range.Max;
                    }

                    if (vectorData.z < range.Min)
                    {
                        vectorData.z = range.Min;
                    }
                    else if (vectorData.z > range.Max)
                    {
                        vectorData.z = range.Max;
                    }

                    property.vector3Value = vectorData;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use Vector3Range with Vector3 only.");
            }
        }
    }
}