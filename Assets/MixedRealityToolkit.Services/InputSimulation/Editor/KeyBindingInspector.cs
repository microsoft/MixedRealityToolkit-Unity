// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using System;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CustomPropertyDrawer(typeof(KeyBinding))]
    public class KeyBindingInspector : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty bindingType = property.FindPropertyRelative("bindingType");
            SerializedProperty code = property.FindPropertyRelative("code");

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            Rect autoBindPosition = new Rect(position.x + position.width - 20.0f, position.y, 20.0f, position.height);
            Rect codePosition = new Rect(position.x, position.y, position.width - 22.0f, position.height);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (KeyBinding.KeyBindingToEnumMap.TryGetValue(Tuple.Create((KeyBinding.KeyType)bindingType.intValue, code.intValue), out int index))
            {
                int newIndex = EditorGUI.Popup(codePosition, index, KeyBinding.AllCodeNames);

                if (newIndex != index)
                {
                    if (KeyBinding.EnumToKeyBindingMap.TryGetValue(newIndex, out var kb))
                    {
                        bindingType.intValue = (int)kb.Item1;
                        code.intValue = kb.Item2;
                    }
                }
            }

            if (GUI.Button(autoBindPosition, ""))
            {
                KeyBindingPopupWindow.Show(property);
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }
    }

    public class KeyBindingPopupWindow : EditorWindow
    {
        private static KeyBindingPopupWindow window;

        private SerializedProperty keyBindingProp;
        private SerializedProperty bindingTypeProp;
        private SerializedProperty codeProp;

        public static void Show(SerializedProperty keyBinding)
        {
            if (window != null)
            {
                window.Close();
            }

            window = null;

            window = CreateInstance<KeyBindingPopupWindow>();
            window.titleContent = new GUIContent($"Key Binding : {keyBinding.name}");
            window.keyBindingProp = keyBinding;
            window.bindingTypeProp = keyBinding.FindPropertyRelative("bindingType");
            window.codeProp = keyBinding.FindPropertyRelative("code");

            var windowSize = new Vector2(256f, 128f);
            window.maxSize = windowSize;
            window.minSize = windowSize;
            window.CenterOnMainWin();
            window.ShowUtility();
        }

        private void OnGUI()
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.KeyUp:
                    ApplyKeyCode(evt.keyCode);
                    break;

                case EventType.MouseUp:
                    ApplyMouseButton(evt.button);
                    break;
            }
        }

        private void ApplyKeyCode(KeyCode keyCode)
        {
            bindingTypeProp.intValue = (int)KeyBinding.KeyType.Key;
            codeProp.intValue = (int)keyCode;
            keyBindingProp.serializedObject.ApplyModifiedProperties();

            Close();
        }

        private void ApplyMouseButton(int button)
        {
            bindingTypeProp.intValue = (int)KeyBinding.KeyType.Mouse;
            codeProp.intValue = button;
            keyBindingProp.serializedObject.ApplyModifiedProperties();

            Close();
        }
    }
}
