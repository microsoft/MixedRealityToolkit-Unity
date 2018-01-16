// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Inspectors.EditorScript;
using MixedRealityToolkit.UX.Buttons.Utilities;

namespace MixedRealityToolkit.UX.EditorScript
{
    [UnityEditor.CustomEditor(typeof(CompoundButtonSpeech))]
    public class CompoundButtonSpeechEditor : MRTKEditor
    {
        protected override void DrawCustomFooter()
        {
            CompoundButtonSpeech speechButton = (CompoundButtonSpeech)target;

            bool microphoneEnabled = UnityEditor.PlayerSettings.WSA.GetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone);
            if (!microphoneEnabled)
            {
                DrawWarning("Microphone capability not present. Speech recognition will be disabled.");
                return;
            }

            UnityEditor.EditorGUILayout.LabelField("Keyword source", UnityEditor.EditorStyles.miniBoldLabel);
            speechButton.KeywordSource = (CompoundButtonSpeech.KeywordSourceEnum)UnityEditor.EditorGUILayout.EnumPopup(speechButton.KeywordSource);
            CompoundButtonText text = speechButton.GetComponent<CompoundButtonText>();
            switch (speechButton.KeywordSource)
            {
                case CompoundButtonSpeech.KeywordSourceEnum.ButtonText:
                default:
                    if (text == null)
                    {
                        DrawError("No CompoundButtonText component found.");
                    }
                    else if (string.IsNullOrEmpty(text.Text))
                    {
                        DrawWarning("No keyword found in button text.");
                    }
                    else
                    {
                        UnityEditor.EditorGUILayout.LabelField("Keyword: " + text.Text);
                    }
                    break;

                case CompoundButtonSpeech.KeywordSourceEnum.LocalOverride:
                    speechButton.Keyword = UnityEditor.EditorGUILayout.TextField(speechButton.Keyword);
                    break;

                case CompoundButtonSpeech.KeywordSourceEnum.None:
                    UnityEditor.EditorGUILayout.LabelField("(Speech control disabled)", UnityEditor.EditorStyles.miniBoldLabel);
                    break;
            }
        }

        private void EnableMicrophone()
        {
            UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.Microphone, true);
        }

        private void AddText()
        {
            CompoundButtonSpeech speechButton = (CompoundButtonSpeech)target;
            speechButton.gameObject.AddComponent<CompoundButtonText>();
        }
    }
}