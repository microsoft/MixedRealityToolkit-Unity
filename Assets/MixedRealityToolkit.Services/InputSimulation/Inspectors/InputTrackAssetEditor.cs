// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Input.Utilities
{
    [CustomEditor(typeof(InputTrackAsset))]
    public class InputTrackAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            bool isGUIEnabled = GUI.enabled;
            var asset = target as InputTrackAsset;

            GUI.enabled = isGUIEnabled && Application.isPlaying;

            bool wasRecording = asset.IsRecording;
            bool record;
            record = GUILayout.Toggle(wasRecording, wasRecording ? "Stop Recording" : "Start Recording", "Button");

            if (record != wasRecording)
            {
                if (record)
                {
                    asset.StartRecording();
                }
                else
                {
                    asset.StopRecording();
                    EditorUtility.SetDirty(asset);
                }
            }

            GUI.enabled = isGUIEnabled;
        }
    }
}