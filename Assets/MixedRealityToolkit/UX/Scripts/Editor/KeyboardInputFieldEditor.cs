// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.UX.VirtualKeyboard;
using UnityEditor;

namespace MixedRealityToolkit.UX.EditorScript
{
    [CustomEditor(typeof(KeyboardInputField))]
    public class KeyboardInputFieldEditor : Editor
    {
        protected KeyboardInputField KeyboardField;

        protected virtual void Awake()
        {
            KeyboardField = (KeyboardInputField)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Separator();
            KeyboardField.KeyboardLayout = (VirtualKeyboardManager.LayoutType)EditorGUILayout.EnumPopup("Keyboard Type:", KeyboardField.KeyboardLayout);
        }
    }
}
