// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
