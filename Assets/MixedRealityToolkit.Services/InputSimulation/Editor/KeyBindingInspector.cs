// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CustomPropertyDrawer(typeof(KeyBinding))]
    public class KeyBindingInspector : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty code = property.FindPropertyRelative("code");

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // var x = target as KeyBinding;
            if (GUI.Button(position, KeyBinding.CodeToString(code.intValue)))
            {
                KeyBindingPopupWindow.Show(property);
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }

    public class KeyBindingPopupWindow : EditorWindow
    {
        private static KeyBindingPopupWindow window;

        private SerializedProperty keyBinding;
        private SerializedProperty code;

        public static void Show(SerializedProperty keyBinding)
        {
            if (window != null)
            {
                window.Close();
            }

            window = null;

            window = CreateInstance<KeyBindingPopupWindow>();
            window.titleContent = new GUIContent($"Key Binding : {keyBinding.name}");
            window.keyBinding = keyBinding;
            window.code = keyBinding.FindPropertyRelative("code");

            var windowSize = new Vector2(256f, 128f);
            window.maxSize = windowSize;
            window.minSize = windowSize;
            window.CenterOnMainWin();
            window.ShowUtility();
        }

        private void OnGUI()
        {
            Event evt = Event.current;
            KeyBinding kb;
            switch (evt.type)
            {
                case EventType.KeyDown:
                    kb = KeyBinding.FromKey(evt.keyCode);
                    code.intValue = kb.Code;
                    keyBinding.serializedObject.ApplyModifiedProperties();

                    Close();
                    break;

                case EventType.MouseDown:
                    kb = KeyBinding.FromMouseButton(evt.button);
                    code.intValue = kb.Code;
                    keyBinding.serializedObject.ApplyModifiedProperties();

                    Close();
                    break;
            }
        }
    }
}
