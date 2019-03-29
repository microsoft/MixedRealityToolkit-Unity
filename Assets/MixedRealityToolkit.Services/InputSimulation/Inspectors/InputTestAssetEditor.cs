// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Input.Utilities
{
    [CustomEditor(typeof(InputTestAsset))]
    public class InputTestAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            bool isGUIEnabled = GUI.enabled;
            var inputTest = target as InputTestAsset;
            bool hasKeyframes = inputTest.InputAnimation.keyframeCount > 0;

            // show "Clear Input Animation"
            {
                GUI.enabled = isGUIEnabled && hasKeyframes;

                if (GUILayout.Button("Clear Input Animation"))
                {
                    Undo.RecordObject(inputTest, "Cleared input test keyframes");
                    inputTest.InputAnimation.ClearKeyframes();
                    EditorUtility.SetDirty(inputTest);
                }

                GUI.enabled = isGUIEnabled;
            }
            // show "Record New Input Animation"
            {
                if (GUILayout.Button("Record New Input Animation"))
                {
                    Undo.RecordObject(inputTest, "Recording input test keyframes");
                    inputTest.InputAnimation.ClearKeyframes();
                    EditorUtility.SetDirty(inputTest);

                    inputTest.EnableInputRecording();
                    EditorApplication.isPlaying = true;
                }
            }

            GUI.enabled = isGUIEnabled;
        }
    }
}