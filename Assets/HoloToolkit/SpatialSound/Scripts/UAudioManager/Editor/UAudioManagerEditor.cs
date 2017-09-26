// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace HoloToolkit.Unity
{
    [CustomEditor(typeof(UAudioManager))]
    public class UAudioManagerEditor : UAudioManagerBaseEditor<AudioEvent>
    {
        private void OnEnable()
        {
            this.MyTarget = (UAudioManager)target;
            SetUpEditor();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("globalEventInstanceLimit"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("globalInstanceBehavior"));
            DrawInspectorGUI(false);
        }
    }
}