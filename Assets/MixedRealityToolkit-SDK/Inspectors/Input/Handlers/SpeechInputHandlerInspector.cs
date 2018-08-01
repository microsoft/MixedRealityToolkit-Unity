// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Inspectors.Input.Handlers
{
    [CustomEditor(typeof(SpeechInputHandler))]
    public class SpeechInputHandlerInspector : BaseInputHandlerInspector
    {
        private static readonly GUIContent RemoveButtonContent = new GUIContent("-", "Remove keyword");
        private static readonly GUIContent AddButtonContent = new GUIContent("+", "Add keyword");
        private static readonly GUILayoutOption MiniButtonWidth = GUILayout.Width(20.0f);

        private string[] registeredKeywords;

        private SerializedProperty keywordsProperty;
        private SerializedProperty persistentKeywordsProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            keywordsProperty = serializedObject.FindProperty("keywords");
            persistentKeywordsProperty = serializedObject.FindProperty("persistentKeywords");
            registeredKeywords = RegisteredKeywords().Distinct().ToArray();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (registeredKeywords == null || registeredKeywords.Length == 0)
            {

                EditorGUILayout.HelpBox("No keywords registered.\n\nKeywords can be registered via Speech Commands Profile on the Mixed Reality Manager's Configuration Profile.", MessageType.Error);
                return;
            }

            EditorGUILayout.PropertyField(persistentKeywordsProperty);

            ShowList(keywordsProperty);
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
            EditorGUI.indentLevel++;

            // remove the keywords already assigned from the registered list
            var handler = (SpeechInputHandler)target;
            var availableKeywords = new string[0];

            if (handler.Keywords != null)
            {
                availableKeywords = registeredKeywords.Except(handler.Keywords.Select(keywordAndResponse => keywordAndResponse.Keyword)).ToArray();
            }

            // keyword rows
            for (int index = 0; index < list.arraySize; index++)
            {
                // the element
                SerializedProperty elementProperty = list.GetArrayElementAtIndex(index);
                EditorGUILayout.BeginHorizontal();
                bool elementExpanded = EditorGUILayout.PropertyField(elementProperty);
                GUILayout.FlexibleSpace();
                // the remove element button
                bool elementRemoved = GUILayout.Button(RemoveButtonContent, EditorStyles.miniButton, MiniButtonWidth);

                if (elementRemoved)
                {
                    list.DeleteArrayElementAtIndex(index);
                }

                EditorGUILayout.EndHorizontal();

                if (!elementRemoved && elementExpanded)
                {
                    SerializedProperty keywordProperty = elementProperty.FindPropertyRelative("keyword");
                    string[] keywords = availableKeywords.Concat(new[] { keywordProperty.stringValue }).OrderBy(keyword => keyword).ToArray();
                    int previousSelection = ArrayUtility.IndexOf(keywords, keywordProperty.stringValue);
                    int currentSelection = EditorGUILayout.Popup("keyword", previousSelection, keywords);

                    if (currentSelection != previousSelection)
                    {
                        keywordProperty.stringValue = keywords[currentSelection];
                    }

                    SerializedProperty responseProperty = elementProperty.FindPropertyRelative("response");
                    EditorGUILayout.PropertyField(responseProperty, true);
                }
            }

            // add button row
            EditorGUILayout.BeginHorizontal();
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

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        private static IEnumerable<string> RegisteredKeywords()
        {
            if (!MixedRealityManager.HasActiveProfile ||
                !MixedRealityManager.Instance.ActiveProfile.IsSpeechCommandsEnabled ||
                 MixedRealityManager.Instance.ActiveProfile.SpeechCommandsProfile.SpeechCommands.Length == 0)
            {
                yield break;
            }

            for (var i = 0; i < MixedRealityManager.Instance.ActiveProfile.SpeechCommandsProfile.SpeechCommands.Length; i++)
            {
                yield return MixedRealityManager.Instance.ActiveProfile.SpeechCommandsProfile.SpeechCommands[i].Keyword;
            }
        }
    }
}