using UnityEditor;

namespace HoloToolkit.Unity.InputModule
{
    [CustomEditor(typeof(SpeechInputSource)), CanEditMultipleObjects]
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

        private static void ShowList(SerializedProperty list)
        {
            EditorGUILayout.PropertyField(list);
            EditorGUI.indentLevel += 1;
            if (list.isExpanded)
            {
                EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
                if (list.arraySize > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Keyword");
                    EditorGUILayout.LabelField("Key Shortcut");
                    EditorGUILayout.EndHorizontal();
                    for (int i = 0; i < list.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                    }
                }
            }
            EditorGUI.indentLevel -= 1;
        }
    }
}
