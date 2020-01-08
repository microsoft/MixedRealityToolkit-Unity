// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(SpeechInputHandler))]
    public class SpeechInputHandlerInspector : BaseInputHandlerInspector
    {
        private static readonly GUIContent RemoveButtonContent = new GUIContent("-", "Remove keyword");
        private static readonly GUIContent AddButtonContent = new GUIContent("+", "Add keyword");
        private static readonly GUILayoutOption MiniButtonWidth = GUILayout.Width(20.0f);

        private string[] distinctRegisteredKeywords;

        private SerializedProperty keywordsProperty;
        private SerializedProperty persistentKeywordsProperty;
        private SerializedProperty speechConfirmationTooltipPrefabProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            keywordsProperty = serializedObject.FindProperty("keywords");
            persistentKeywordsProperty = serializedObject.FindProperty("persistentKeywords");
            speechConfirmationTooltipPrefabProperty = serializedObject.FindProperty("speechConfirmationTooltipPrefab");

            if (MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                distinctRegisteredKeywords = GetDistinctRegisteredKeywords();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            bool enabled = CheckMixedRealityToolkit();
            if (enabled)
            {
                if (!MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled)
                {
                    EditorGUILayout.HelpBox("No input system is enabled, or you need to specify the type in the main configuration profile.", MessageType.Warning);
                }

                if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile == null)
                {
                    EditorGUILayout.HelpBox("No Input System Profile Found, be sure to specify a profile in the main configuration profile.", MessageType.Error);
                    enabled = false;
                }
                else if (MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile == null)
                {
                    EditorGUILayout.HelpBox("No Speech Commands profile Found, be sure to specify a profile in the Input System's configuration profile.", MessageType.Error);
                    enabled = false;
                }
            }

            bool validKeywords = distinctRegisteredKeywords != null && distinctRegisteredKeywords.Length != 0;

            // If we should be enabled but there are no valid keywords, alert developer
            if (enabled && !validKeywords)
            {
                distinctRegisteredKeywords = GetDistinctRegisteredKeywords();
                EditorGUILayout.HelpBox("No keywords registered. Some properties may not be editable.\n\nKeywords can be registered via Speech Commands Profile on the Mixed Reality Toolkit's Configuration Profile.", MessageType.Error);
            }
            enabled = enabled && validKeywords;

            serializedObject.Update();
            EditorGUILayout.PropertyField(persistentKeywordsProperty);
            EditorGUILayout.PropertyField(speechConfirmationTooltipPrefabProperty);

            bool wasGUIEnabled = GUI.enabled;
            GUI.enabled = enabled;

            ShowList(keywordsProperty);

            GUI.enabled = wasGUIEnabled;

            serializedObject.ApplyModifiedProperties();

            // error and warning messages
            if (keywordsProperty.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No keywords have been assigned!", MessageType.Warning);
            }
            else
            {
                var handler = (SpeechInputHandler)target;
                string duplicateKeyword = handler.Keywords
                    .GroupBy(keyword => keyword.Keyword.ToLower())
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key).FirstOrDefault();

                if (duplicateKeyword != null)
                {
                    EditorGUILayout.HelpBox($"Keyword \'{duplicateKeyword}\' is assigned more than once!", MessageType.Warning);
                }
            }
        }

        private void ShowList(SerializedProperty list)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                // remove the keywords already assigned from the registered list
                var handler = (SpeechInputHandler)target;
                var availableKeywords = System.Array.Empty<string>();

                if (handler.Keywords != null && distinctRegisteredKeywords != null)
                {
                    availableKeywords = distinctRegisteredKeywords.Except(handler.Keywords.Select(keywordAndResponse => keywordAndResponse.Keyword)).ToArray();
                }

                // keyword rows
                for (int index = 0; index < list.arraySize; index++)
                {
                    // the element
                    SerializedProperty speechCommandProperty = list.GetArrayElementAtIndex(index);
                    GUILayout.BeginHorizontal();
                    bool elementExpanded = EditorGUILayout.PropertyField(speechCommandProperty);
                    GUILayout.FlexibleSpace();
                    // the remove element button
                    bool elementRemoved = GUILayout.Button(RemoveButtonContent, EditorStyles.miniButton, MiniButtonWidth);

                    GUILayout.EndHorizontal();

                    if (elementRemoved)
                    {
                        list.DeleteArrayElementAtIndex(index);

                        if (index == list.arraySize)
                        {
                            EditorGUI.indentLevel--;
                            return;
                        }
                    }

                    SerializedProperty keywordProperty = speechCommandProperty.FindPropertyRelative("keyword");

                    bool invalidKeyword = true;
                    if (distinctRegisteredKeywords != null)
                    {
                        foreach (string keyword in distinctRegisteredKeywords)
                        {
                            if (keyword == keywordProperty.stringValue)
                            {
                                invalidKeyword = false;
                                break;
                            }
                        }
                    }

                    if (invalidKeyword)
                    {
                        EditorGUILayout.HelpBox("Registered keyword is not recognized in the speech command profile!", MessageType.Error);
                    }

                    if (!elementRemoved && elementExpanded)
                    {
                        string[] keywords = availableKeywords.Concat(new[] { keywordProperty.stringValue }).OrderBy(keyword => keyword).ToArray();
                        int previousSelection = ArrayUtility.IndexOf(keywords, keywordProperty.stringValue);
                        int currentSelection = EditorGUILayout.Popup("Keyword", previousSelection, keywords);

                        if (currentSelection != previousSelection)
                        {
                            keywordProperty.stringValue = keywords[currentSelection];
                        }

                        SerializedProperty responseProperty = speechCommandProperty.FindPropertyRelative("response");
                        EditorGUILayout.PropertyField(responseProperty, true);
                    }
                }

                // add button row
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    // the add element button
                    if (GUILayout.Button(AddButtonContent, EditorStyles.miniButton, MiniButtonWidth))
                    {
                        var index = list.arraySize;
                        list.InsertArrayElementAtIndex(index);
                        var elementProperty = list.GetArrayElementAtIndex(index);
                        SerializedProperty keywordProperty = elementProperty.FindPropertyRelative("keyword");
                        keywordProperty.stringValue = string.Empty;
                    }
                }
            }
        }

        private static string[] GetDistinctRegisteredKeywords()
        {
            if (!MixedRealityToolkit.IsInitialized ||
                !MixedRealityToolkit.Instance.HasActiveProfile ||
                !MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled ||
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile == null ||
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile.SpeechCommands.Length == 0)
            {
                return null;
            }

            List<string> keywords = new List<string>();
            var speechCommands = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.SpeechCommandsProfile.SpeechCommands;
            for (var i = 0; i < speechCommands.Length; i++)
            {
                keywords.Add(speechCommands[i].Keyword);
            }

            return keywords.Distinct().ToArray();
        }
    }
}