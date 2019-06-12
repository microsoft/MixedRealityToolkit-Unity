// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit
{
    [CustomEditor(typeof(MixedRealityPlayspace))]
    public class MixedRealityPlayspaceInspector : UnityEditor.Editor
    {
        private static string description =
          "This is where the Toolkit instantiates objects like cursors, pointers and boundary visualizations." +
          "\n- Your main camera should be kept here (unless you are using a custom CameraSystem implementation.)" +
          "\n- This transform can be moved, but it should not be parented under another object." +
          "\n- If multiple MixedRealityPlayspace transforms are loaded in multiple scenes, all but the first loaded will be disabled and ignored.";

        public override void OnInspectorGUI()
        {
            MixedRealityPlayspace playspace = (MixedRealityPlayspace)target;

            if (!MixedRealityPlayspace.IsActivePlayspace(playspace))
            {
                EditorGUILayout.HelpBox("This playspace is inactive. There can only be one active playspace loaded at any time.", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select Active Playspace"))
                {
                    Selection.activeGameObject = MixedRealityPlayspace.Transform.gameObject;
                }
                if (GUILayout.Button("Make this the Active Playspace"))
                {
                    MixedRealityPlayspace.SetActivePlayspace(playspace);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox(description, MessageType.Info);
            }
        }
    }
}
