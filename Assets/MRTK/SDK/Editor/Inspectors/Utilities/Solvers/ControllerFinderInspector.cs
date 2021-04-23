// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Solvers
{
    [CustomEditor(typeof(ControllerFinder))]
    public abstract class ControllerFinderInspector : UnityEditor.Editor
    {
        private SerializedProperty handednessProperty;

        protected virtual void OnEnable()
        {
            handednessProperty = serializedObject.FindProperty("handedness");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Controller Options", new GUIStyle("Label") { fontStyle = FontStyle.Bold });
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(handednessProperty);

            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
