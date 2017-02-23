// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    [CustomEditor(typeof(DictationInputManager))]
    public class DictationInputManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DictationInputManager.InitialSilenceTimeout = EditorGUILayout.Slider(new GUIContent("Initial Silence Timeout", "The default timeout with initial silence is 5 seconds."), DictationInputManager.InitialSilenceTimeout, 0.1f, 5f);
            DictationInputManager.AutoSilenceTimeout = EditorGUILayout.Slider(new GUIContent("Auto Silence Timeout", "The default timeout after a recognition is 20 seconds."), DictationInputManager.AutoSilenceTimeout, 5f, 60f);
            DictationInputManager.RecordingTime = EditorGUILayout.IntSlider(new GUIContent("Recording Time", "The default recording time is 10 seconds."), DictationInputManager.RecordingTime, 1, 60);
        }
    }
}
