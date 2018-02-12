// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.UX.Keyboard;
using UnityEditor;

namespace MixedRealityToolkit.UX.EditorScript
{
    [CustomEditor(typeof(KeyboardInputField))]
    public class KeyboardInputFieldEditor : Editor
    {
        public KeyboardManager.LayoutType KeyboardLayout = KeyboardManager.LayoutType.Alpha;
        protected KeyboardInputField KeyboardField;

        protected virtual void Awake()
        {
            KeyboardField = (KeyboardInputField)target;
        }

        public override void OnInspectorGUI()
        {
            KeyboardField.KeyboardLayout = (KeyboardManager.LayoutType)EditorGUILayout.EnumPopup("Keyboard Type:", KeyboardField.KeyboardLayout);

            EditorGUILayout.Separator();
            base.OnInspectorGUI();
        }
    }
}
