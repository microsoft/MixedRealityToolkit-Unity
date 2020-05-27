// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell.Editor
{
    [CustomEditor(typeof(DwellHandler), true)]
    public class DwellHandlerInspector : UnityEditor.Editor
    {
        private UnityEditor.Editor _editor;

        public override void OnInspectorGUI()
        {
            var dwellProfileAsset = this.serializedObject.FindProperty("dwellProfile");
            EditorGUILayout.PropertyField(dwellProfileAsset, true);

            EditorGUILayout.Foldout(true, "Dwell Profile Properties", true);
            EditorGUI.indentLevel++;
            if (dwellProfileAsset.objectReferenceValue != null)
            {
                CreateCachedEditor(dwellProfileAsset.objectReferenceValue, null, ref _editor);
                _editor.OnInspectorGUI();
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("DwellIntended"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("DwellStarted"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("DwellCompleted"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("DwellCanceled"), true);

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
