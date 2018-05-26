// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Managers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityManager))]
    public class MixedRealityManagerInspector : Editor
    {
        private SerializedProperty activeProfile;
        private SerializedProperty resetOnProfileChange;

        private void OnEnable()
        {
            activeProfile = serializedObject.FindProperty("activeProfile");
            resetOnProfileChange = serializedObject.FindProperty("resetOnProfileChange");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(activeProfile);

            if (EditorGUI.EndChangeCheck() && resetOnProfileChange.boolValue)
            {
                serializedObject.ApplyModifiedProperties();
                MixedRealityManager.Instance.ResetConfiguration();
            }

            EditorGUILayout.PropertyField(resetOnProfileChange);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
