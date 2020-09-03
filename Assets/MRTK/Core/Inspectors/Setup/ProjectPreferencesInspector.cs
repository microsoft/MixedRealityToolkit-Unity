// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [CustomEditor(typeof(ProjectPreferences))]
    internal class ProjectPreferencesInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Use Project Settings > MRTK to edit project preferences or interact in code via ProjectPreferences.cs", MessageType.Warning);

            DrawDefaultInspector();
        }
    }
}
