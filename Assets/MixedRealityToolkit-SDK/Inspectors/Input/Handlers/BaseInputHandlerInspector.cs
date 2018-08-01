// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SDK.Inspectors.Input.Handlers
{
    public class BaseInputHandlerInspector : Editor
    {
        private SerializedProperty isFocusRequiredProperty;

        protected virtual void OnEnable()
        {
            isFocusRequiredProperty = serializedObject.FindProperty("isFocusRequired");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(isFocusRequiredProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}