using UnityEngine;
using UnityEditor;

namespace HoloToolkit.Unity.InputModule
{
    [CustomPropertyDrawer(typeof(SpeechInputSource.KeywordAndKeyCode))]
    public class KeywordAndKeyCodeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent content)
        {
            EditorGUI.BeginProperty(rect, content, property);

            // calculate field reactangle with half of total drawer length for each
            float fieldWidth = rect.width * 0.5f;
            Rect keywordRect = new Rect(rect.x, rect.y, fieldWidth, rect.height);
            Rect keyCodeRect = new Rect(rect.x + fieldWidth, rect.y, fieldWidth, rect.height);

            // the Keyword field without label
            EditorGUI.PropertyField(keywordRect, property.FindPropertyRelative("Keyword"), GUIContent.none);
            // the KeyCode field without label
            EditorGUI.PropertyField(keyCodeRect, property.FindPropertyRelative("KeyCode"), GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}
