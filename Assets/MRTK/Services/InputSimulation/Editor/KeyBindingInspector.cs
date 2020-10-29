// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using System;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Inspector for KeyBindings.
    /// This shows a simple dropdown list for selecting a binding, as well as a button for binding keys by pressing them.
    /// </summary>
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

            // Show the traditional long dropdown list for selecting a key binding.
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

            // Show a popup for binding by pressing a key or mouse button.
            // Note that this method does not work for shift keys (Unity event limitation)
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

    /// <summary>
    /// Utility window that listens to input events to set a key binding.
    /// Pressing a key or mouse button will define the binding and then immediately close the popup.
    /// </summary>
    /// <remarks>
    /// The shift keys don't raise input events on their own, so this popup does not work for shift keys.
    /// These have to be bound by selecting from the traditional dropdown list.
    /// </remarks>
    public class KeyBindingPopupWindow : EditorWindow
    {
        private static KeyBindingPopupWindow window;

        private SerializedProperty keyBindingProp;
        private SerializedProperty bindingTypeProp;
        private SerializedProperty codeProp;

        /// <summary>
        /// Create a new popup.
        /// </summary>
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

        // Set the binding based on a keyboard key
        private void ApplyKeyCode(KeyCode keyCode)
        {
            bindingTypeProp.intValue = (int)KeyBinding.KeyType.Key;
            codeProp.intValue = (int)keyCode;
            keyBindingProp.serializedObject.ApplyModifiedProperties();

            Close();
        }

        // Set the binding based on a mouse button
        private void ApplyMouseButton(int button)
        {
            bindingTypeProp.intValue = (int)KeyBinding.KeyType.Mouse;
            codeProp.intValue = button;
            keyBindingProp.serializedObject.ApplyModifiedProperties();

            Close();
        }
    }
}
