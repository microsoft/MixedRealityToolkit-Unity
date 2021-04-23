// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Dwell.Editor
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

            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("dwellIntended"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("dwellStarted"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("dwellCompleted"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("dwellCanceled"), true);

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
