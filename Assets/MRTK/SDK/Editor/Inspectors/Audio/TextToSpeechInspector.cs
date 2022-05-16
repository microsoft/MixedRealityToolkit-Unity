// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Audio.Editor
{
    [CustomEditor(typeof(TextToSpeech))]
    public class TextToSpeechInspector : UnityEditor.Editor
    {
        private SerializedProperty voiceProperty;

        private void OnEnable()
        {
            voiceProperty = serializedObject.FindProperty("voice");
        }

        public override void OnInspectorGUI()
        {
            if (voiceProperty.intValue == (int)TextToSpeechVoice.Other)
            {
                DrawDefaultInspector();
                EditorGUILayout.HelpBox("Use the links below to find more available voices (for non en-US languages):", MessageType.Info);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Voices for HoloLens 2", EditorStyles.miniButton))
                    {
                        Application.OpenURL("https://docs.microsoft.com/hololens/hololens2-language-support");
                    }
                    if (GUILayout.Button("Voices for desktop Windows", EditorStyles.miniButton))
                    {
                        Application.OpenURL("https://support.microsoft.com/windows/appendix-a-supported-languages-and-voices-4486e345-7730-53da-fcfe-55cc64300f01#WindowsVersion=Windows_11");
                    }
                }
            }
            else
            {
                DrawPropertiesExcluding(serializedObject, "customVoice");
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
