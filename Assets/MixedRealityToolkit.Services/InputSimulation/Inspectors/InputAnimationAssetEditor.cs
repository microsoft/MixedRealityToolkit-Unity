// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Input.Utilities
{
    [CustomEditor(typeof(InputAnimationAsset))]
    public class InputAnimationAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            bool isGUIEnabled = GUI.enabled;
            var asset = target as InputAnimationAsset;
            bool hasKeyframes = asset.InputAnimation.keyframeCount > 0;

            // show "Clear Input Animation"
            {
                GUI.enabled = isGUIEnabled && hasKeyframes;

                if (GUILayout.Button("Clear Input Animation"))
                {
                    Undo.RecordObject(asset, "Cleared input test keyframes");
                    asset.InputAnimation.ClearKeyframes();
                    EditorUtility.SetDirty(asset);
                }

                GUI.enabled = isGUIEnabled;
            }
            // show "Record New Input Animation"
            {
                if (GUILayout.Button("Record New Input Animation"))
                {
                    Undo.RecordObject(asset, "Recording input test keyframes");
                    asset.InputAnimation.ClearKeyframes();
                    EditorUtility.SetDirty(asset);

                    asset.EnableInputRecording();
                    EditorApplication.isPlaying = true;
                }
            }

            GUI.enabled = isGUIEnabled;
        }
    }
}