using System;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    [CustomEditor(typeof(SpeechInputSource))]
    public class SpeechInputSourceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SerializedProperty recognizerStart = serializedObject.FindProperty("RecognizerStart");
            SerializedProperty keywordsAndKeys = serializedObject.FindProperty("Keywords");

            serializedObject.Update();
            EditorGUILayout.PropertyField(recognizerStart);
            ShowList(keywordsAndKeys);
            serializedObject.ApplyModifiedProperties();

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
            // property name 
            EditorGUILayout.PropertyField(list);

            EditorGUI.indentLevel++;
            if (list.isExpanded)
            {
                // header row
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Keyword");
                EditorGUILayout.LabelField("Key Shortcut");
                EditorGUILayout.EndHorizontal();

                // 
                for (int index = 0; index < list.arraySize; index++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(index));
                    if (GUILayout.Button(removeButtonContent, EditorStyles.miniButton, miniButtonWidth))
                    {
                        list.DeleteArrayElementAtIndex(index);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                // row for add button
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(addButtonContent, EditorStyles.miniButton, miniButtonWidth))
                {
                    list.InsertArrayElementAtIndex(list.arraySize);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }
    }
}
