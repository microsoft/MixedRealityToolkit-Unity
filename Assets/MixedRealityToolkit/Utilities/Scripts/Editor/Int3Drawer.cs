// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using UnityEditor;
using UnityEngine;

namespace MixedRealityToolkit.Utilities.EditorScript
{
    /// <summary>
    /// Property drawer for in-editor display/editing of Int3 data
    /// </summary>
    [CustomPropertyDrawer(typeof(Int3))]
    public class Int3Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            var contentPosition = EditorGUI.PrefixLabel(position, label);

            var subLabels = new GUIContent[3];
            subLabels[0] = new GUIContent("x");
            subLabels[1] = new GUIContent("y");
            subLabels[2] = new GUIContent("z");

            EditorGUI.MultiPropertyField(contentPosition, subLabels, property.FindPropertyRelative("x"));

            EditorGUI.EndProperty();
        }
    }
}