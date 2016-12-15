using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    [CustomEditor(typeof(SpeechInputSource)), CanEditMultipleObjects]
    public class SpeechInputSourceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SerializedProperty recognizerStart = serializedObject.FindProperty("RecognizerStart");
            SerializedProperty keywordsAndKeys = serializedObject.FindProperty("KeywordsAndKeys");

            serializedObject.Update();
            EditorGUILayout.PropertyField(recognizerStart);
            EditorGUILayout.PropertyField(keywordsAndKeys, true);
            serializedObject.ApplyModifiedProperties();

            if (keywordsAndKeys.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No keywords have been assigned!", MessageType.Warning);
            }
        }
    }
}
