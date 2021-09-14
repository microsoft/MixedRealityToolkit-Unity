// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

        private IEnumerable<string> distinctRegisteredKeywords;

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
                distinctRegisteredKeywords = SpeechKeywordUtility.GetDistinctRegisteredKeywords();
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

            bool validKeywords = distinctRegisteredKeywords != null && distinctRegisteredKeywords.Count() != 0;

            // If we should be enabled but there are no valid keywords, alert developer
            if (enabled && !validKeywords)
            {
                distinctRegisteredKeywords = SpeechKeywordUtility.GetDistinctRegisteredKeywords();
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
                // keyword rows
                for (int index = 0; index < list.arraySize; index++)
                {
                    // the element
                    SerializedProperty speechCommandProperty = list.GetArrayElementAtIndex(index);

                    bool elementExpanded = false;
                    bool elementRemoved = false;
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        elementExpanded = EditorGUILayout.PropertyField(speechCommandProperty, false);
                        GUILayout.FlexibleSpace();
                        // the remove element button
                        elementRemoved = GUILayout.Button(RemoveButtonContent, EditorStyles.miniButton, MiniButtonWidth);
                    }

                    if (elementRemoved)
                    {
                        list.DeleteArrayElementAtIndex(index);

                        if (index == list.arraySize)
                        {
                            return;
                        }
                    }

                    SerializedProperty keywordProperty = speechCommandProperty.FindPropertyRelative("keyword");

                    if (!distinctRegisteredKeywords?.Contains(keywordProperty.stringValue) ?? true)
                    {
                        EditorGUILayout.HelpBox("Registered keyword is not recognized in the speech command profile!", MessageType.Error);
                    }

                    if (!elementRemoved && elementExpanded)
                    {
                        // remove the keywords already assigned from the registered list
                        SpeechInputHandler handler = (SpeechInputHandler)target;
                        SpeechKeywordUtility.RenderKeywordsExcept(handler.Keywords?.Select(keywordAndResponse => keywordAndResponse.Keyword)?.ToArray(), keywordProperty);

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
    }
}