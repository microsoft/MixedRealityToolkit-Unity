// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.UI.Keyboard
{
    [CustomEditor(typeof(KeyboardInputField))]
    public class KeyboardInputFieldEditor : Editor
    {
        public Keyboard.LayoutType KeyboardLayout = Keyboard.LayoutType.Alpha;
        protected KeyboardInputField myField;

        private void Awake()
        {
            myField = (KeyboardInputField)target;
        }

        public override void OnInspectorGUI()
        {
            myField.m_KeyboardLayout = (Keyboard.LayoutType)EditorGUILayout.EnumPopup("Keyboard Type:", myField.m_KeyboardLayout);

            EditorGUILayout.Separator();
            base.OnInspectorGUI();
        }
    }
}
