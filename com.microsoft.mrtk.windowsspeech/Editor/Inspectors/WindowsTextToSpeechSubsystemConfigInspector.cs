// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Speech.Windows
{
    /// <summary>
    /// A custom editor for the <see cref="WindowsTextToSpeechSubsystemConfig"/> class.
    /// </summary>
    [CustomEditor(typeof(WindowsTextToSpeechSubsystemConfig))]
    public class WindowsTextToSpeechSubsystemConfigInspector : UnityEditor.Editor
    {
        private SerializedProperty voiceProperty;

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary>
        private void OnEnable()
        {
            voiceProperty = serializedObject.FindProperty("voice");
        }

        /// <summary>
        /// Called by the Unity editor to render custom inspector UI for this component.
        /// </summary>
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
