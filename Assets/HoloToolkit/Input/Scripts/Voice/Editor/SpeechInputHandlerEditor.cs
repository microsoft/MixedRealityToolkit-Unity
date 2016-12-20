using System;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    [CustomEditor(typeof(SpeechInputHandler))]
    public class SpeechInputHandlerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SerializedProperty keywordsAndResponses = serializedObject.FindProperty("KeywordsAndResponses");

            serializedObject.Update();
            EditorGUILayout.PropertyField(keywordsAndResponses, true);
            serializedObject.ApplyModifiedProperties();

            // error and warning messages
            if (keywordsAndResponses.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No keywords have been assigned!", MessageType.Warning);
            }
        }
    }
}
