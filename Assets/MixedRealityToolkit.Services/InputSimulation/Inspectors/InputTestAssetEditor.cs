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
            bool hasExpectedValues = inputTest.ExpectedValues.Count > 0;

            // show "Clear Input Animation"
            {
                GUI.enabled = isGUIEnabled && hasKeyframes;

                if (GUILayout.Button("Clear Input Animation"))
                {
                    Undo.RecordObject(inputTest, "Cleared input test keyframes");
                    inputTest.InputAnimation.ClearKeyframes();
                    inputTest.ExpectedValues.Clear();
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
                    inputTest.ExpectedValues.Clear();
                    EditorUtility.SetDirty(inputTest);

                    inputTest.EnableInputRecording();
                    EditorApplication.isPlaying = true;
                }
            }

            if (hasKeyframes)
            {
                // show "Clear Expected Values"
                {
                    GUI.enabled = isGUIEnabled && hasExpectedValues;

                    if (GUILayout.Button("Clear Expected Values"))
                    {
                        Undo.RecordObject(inputTest, "Cleared input test expected values");
                        inputTest.ExpectedValues.Clear();
                        EditorUtility.SetDirty(inputTest);
                    }

                    GUI.enabled = isGUIEnabled;
                }

                // show "Record New Expected Values"
                {
                    if (GUILayout.Button("Record New Expected Values"))
                    {
                        Undo.RecordObject(inputTest, "Recording input test expected values");
                        inputTest.ExpectedValues.Clear();
                        EditorUtility.SetDirty(inputTest);

                        inputTest.EnableTestRecording();
                        EditorApplication.isPlaying = true;
                    }
                }
            }

            GUI.enabled = isGUIEnabled;
        }
    }
}