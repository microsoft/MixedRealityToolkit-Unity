// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace HoloToolkit.UI.Keyboard
{
    [CustomEditor(typeof(KeyboardInputField))]
    public class KeyboardInputFieldEditor : Editor
    {
        public Keyboard.LayoutType KeyboardLayout = Keyboard.LayoutType.Alpha;
        protected KeyboardInputField KeyboardField;

        protected virtual void Awake()
        {
            KeyboardField = (KeyboardInputField)target;
        }

        public override void OnInspectorGUI()
        {
            KeyboardField.KeyboardLayout = (Keyboard.LayoutType)EditorGUILayout.EnumPopup("Keyboard Type:", KeyboardField.KeyboardLayout);

            EditorGUILayout.Separator();
            base.OnInspectorGUI();
        }
    }
}
