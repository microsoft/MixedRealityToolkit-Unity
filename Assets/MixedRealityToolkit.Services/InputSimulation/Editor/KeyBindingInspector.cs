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
        private static KeyBinding.KeyType[] KeyTypeValues = (KeyBinding.KeyType[])Enum.GetValues(typeof(KeyBinding.KeyType));
        private static string[] KeyTypeNames = Enum.GetNames(typeof(KeyBinding.KeyType));

        private static KeyCode[] KeyCodeValues = (KeyCode[])Enum.GetValues(typeof(KeyCode));
        private static string[] KeyCodeNames = Enum.GetNames(typeof(KeyCode));

        private static KeyBinding.MouseButton[] MouseButtonValues = (KeyBinding.MouseButton[])Enum.GetValues(typeof(KeyBinding.MouseButton));
        private static string[] MouseButtonNames = Enum.GetNames(typeof(KeyBinding.MouseButton));

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty code = property.FindPropertyRelative("code");

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            Rect autoBindPosition = new Rect(position.x + position.width - 20.0f, position.y, 20.0f, position.height);
            float width = (position.width - 22.0f) / 2.0f;
            Rect keyTypePosition = new Rect(position.x, position.y, width, position.height);
            Rect keyCodePosition = new Rect(position.x + width + 2, position.y, width, position.height);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            KeyBinding.GetKeyTypeAndCode(code.intValue, out var keyType, out int keyCode);

            KeyBinding.KeyType newKeyType;
            {
                int keyTypeIndex = Array.FindIndex(KeyTypeValues, kt => kt == keyType);
                int newKeyTypeIndex = EditorGUI.Popup(keyTypePosition, keyTypeIndex, KeyTypeNames);
                newKeyType = KeyTypeValues[newKeyTypeIndex];
            }

            int newKeyCode;
            switch (keyType)
            {
                case KeyBinding.KeyType.Key:
                    int keyCodeIndex = Array.FindIndex(KeyCodeValues, kc => kc == (KeyCode)keyCode);
                    int newKeyCodeIndex = EditorGUI.Popup(keyCodePosition, keyCodeIndex, KeyCodeNames);
                    newKeyCode = (int)KeyCodeValues[newKeyCodeIndex];
                    break;
                case KeyBinding.KeyType.MouseButton:
                    int buttonIndex = Array.FindIndex(MouseButtonValues, kc => kc == (KeyBinding.MouseButton)keyCode);
                    int newButtonIndex = EditorGUI.Popup(keyCodePosition, buttonIndex, MouseButtonNames);
                    newKeyCode = (int)MouseButtonValues[newButtonIndex];
                    break;
                default:
                    newKeyCode = keyCode;
                    break;
            }

            if (newKeyType != keyType)
            {
                code.intValue = (int)newKeyType;
            }
            if (newKeyCode != keyCode)
            {
                code.intValue = (int)keyType + newKeyCode;
            }

            // if (GUI.Button(position, KeyBinding.CodeToString(code.intValue)))
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
        private SerializedProperty codeProp;

        private int newCode;

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
            window.codeProp = keyBinding.FindPropertyRelative("code");
            window.newCode = window.codeProp.intValue;

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
                    newCode = KeyBinding.FromKey(evt.keyCode).Code;
                    ApplyAndClose();
                    break;

                case EventType.MouseUp:
                    newCode = KeyBinding.FromMouseButton(evt.button).Code;
                    ApplyAndClose();
                    break;
            }
        }

        private void ApplyAndClose()
        {
            codeProp.intValue = newCode;
            keyBindingProp.serializedObject.ApplyModifiedProperties();

            Close();
        }
    }
}
