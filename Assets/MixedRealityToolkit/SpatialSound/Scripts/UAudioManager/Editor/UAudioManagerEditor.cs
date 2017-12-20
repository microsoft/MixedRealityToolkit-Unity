// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace MixedRealityToolkit.SpacialSound
{
    [CustomEditor(typeof(UAudioManager))]
    public class UAudioManagerEditor : UAudioManagerBaseEditor<AudioEvent, AudioEventBank>
    {
        SerializedProperty PropGlobalEventInstanceLimit;
        SerializedProperty PropGlobalInstanceBehaviour;

        private void OnEnable()
        {
            this.MyTarget = (UAudioManager)target;
            SetUpEditor();

            PropGlobalEventInstanceLimit = this.serializedObject.FindProperty("globalEventInstanceLimit");
            PropGlobalInstanceBehaviour = this.serializedObject.FindProperty("globalInstanceBehavior");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(PropGlobalEventInstanceLimit);
            EditorGUILayout.PropertyField(PropGlobalInstanceBehaviour);
            DrawExportToAsset();
            DrawBankList();
        }
    }
}