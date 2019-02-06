// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SpeechCommands))]
    public class SpeechCommandPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent content)
        {
            EditorGUI.BeginProperty(rect, content, property);

            // calculate field rectangle with half of total drawer length for each
            var fieldWidth = rect.width * 0.5f;
            var keywordRect = new Rect(rect.x, rect.y, fieldWidth, rect.height);
            var keyCodeRect = new Rect(rect.x + fieldWidth, rect.y, fieldWidth, rect.height);

            // the Keyword field without label
            EditorGUI.PropertyField(keywordRect, property.FindPropertyRelative("keyword"), GUIContent.none);
            // the KeyCode field without label
            EditorGUI.PropertyField(keyCodeRect, property.FindPropertyRelative("keyCode"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}