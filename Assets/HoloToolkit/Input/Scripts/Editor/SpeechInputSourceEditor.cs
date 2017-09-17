// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    [CustomEditor(typeof(SpeechInputSource))]
    public class SpeechInputSourceEditor : Editor
    {
        private SerializedProperty persistentKeywordsProperty;
        private SerializedProperty recognizerStart;
        private SerializedProperty confidenceLevel;
        private SerializedProperty keywordsAndKeys;

        private void OnEnable()
        {
            persistentKeywordsProperty = serializedObject.FindProperty("PersistentKeywords");
            recognizerStart = serializedObject.FindProperty("RecognizerStart");
            confidenceLevel = serializedObject.FindProperty("recognitionConfidenceLevel");
            keywordsAndKeys = serializedObject.FindProperty("Keywords");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Recognizer options
            EditorGUILayout.PropertyField(persistentKeywordsProperty);
            EditorGUILayout.PropertyField(recognizerStart);
            EditorGUILayout.PropertyField(confidenceLevel);
            
            // the Keywords field
            ShowList(keywordsAndKeys);
            serializedObject.ApplyModifiedProperties();

            // error and warning messages
            if (keywordsAndKeys.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No keywords have been assigned!", MessageType.Warning);
            }
        }

        private static GUIContent removeButtonContent = new GUIContent("-", "Remove keyword");
        private static GUIContent addButtonContent = new GUIContent("+", "Add keyword");
        private static GUILayoutOption miniButtonWidth = GUILayout.Width(20.0f);

        private static void ShowList(SerializedProperty list)
        {
            // property name with expansion widget
            EditorGUILayout.PropertyField(list);

            if (list.isExpanded)
            {
                EditorGUI.indentLevel++;

                // header row
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Keyword");
                EditorGUILayout.LabelField("Key Shortcut");
                EditorGUILayout.EndHorizontal();

                // keyword rows
                for (int index = 0; index < list.arraySize; index++)
                {
                    EditorGUILayout.BeginHorizontal();
                    // the element
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(index));
                    // the remove element button
                    if (GUILayout.Button(removeButtonContent, EditorStyles.miniButton, miniButtonWidth))
                    {
                        list.DeleteArrayElementAtIndex(index);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                // add button row
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                // the add element button
                if (GUILayout.Button(addButtonContent, EditorStyles.miniButton, miniButtonWidth))
                {
                    list.InsertArrayElementAtIndex(list.arraySize);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }
        }
    }
}
