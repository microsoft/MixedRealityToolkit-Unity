// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.InputSources;
using MixedRealityToolkit.InputModule.Utilities.Interations;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.EditorScript
{
    [CustomEditor(typeof(SpeechInputHandler))]
    public class SpeechInputHandlerEditor : UnityEditor.Editor
    {
        private SerializedProperty keywordsProperty;
        private string[] registeredKeywords;
        private SerializedProperty isGlobalListenerProperty;
        private SerializedProperty persistentKeywordsProperty;

        private void OnEnable()
        {
            keywordsProperty = serializedObject.FindProperty("Keywords");            
            isGlobalListenerProperty = serializedObject.FindProperty("IsGlobalListener");
            persistentKeywordsProperty = serializedObject.FindProperty("PersistentKeywords");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(isGlobalListenerProperty);
            EditorGUILayout.PropertyField(persistentKeywordsProperty);

            registeredKeywords = RegisteredKeywords().Distinct().ToArray();
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
                    EditorGUILayout.HelpBox("Keyword '" + duplicateKeyword + "' is assigned more than once!", MessageType.Warning);
                }
            }
        }

        private static readonly GUIContent RemoveButtonContent = new GUIContent("-", "Remove keyword");
        private static readonly GUIContent AddButtonContent = new GUIContent("+", "Add keyword");
        private static readonly GUILayoutOption MiniButtonWidth = GUILayout.Width(20.0f);

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
                    SerializedProperty keywordProperty = elementProperty.FindPropertyRelative("Keyword");
                    string[] keywords = availableKeywords.Concat(new[] { keywordProperty.stringValue }).OrderBy(keyword => keyword).ToArray();
                    int previousSelection = ArrayUtility.IndexOf(keywords, keywordProperty.stringValue);
                    int currentSelection = EditorGUILayout.Popup("Keyword", previousSelection, keywords);

                    if (currentSelection != previousSelection)
                    {
                        keywordProperty.stringValue = keywords[currentSelection];
                    }

                    SerializedProperty responseProperty = elementProperty.FindPropertyRelative("Response");
                    EditorGUILayout.PropertyField(responseProperty, true);
                }
            }

            // add button row
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // the add element button
            if (GUILayout.Button(AddButtonContent, EditorStyles.miniButton, MiniButtonWidth))
            {
                list.InsertArrayElementAtIndex(list.arraySize);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }

        private static IEnumerable<string> RegisteredKeywords()
        {
            foreach (SpeechInputSource source in FindObjectsOfType<SpeechInputSource>())
            {
                for (var i = 0; i < source.Keywords.Length; i++)
                {
                    yield return source.Keywords[i].Keyword;
                }
            }
        }
    }
}
