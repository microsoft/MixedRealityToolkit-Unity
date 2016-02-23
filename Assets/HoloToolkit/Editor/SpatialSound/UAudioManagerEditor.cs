using UnityEditor;

namespace HoloToolkit.Unity
{
    [CustomEditor(typeof(UAudioManager))]
    public class UAudioManagerEditor : UAudioManagerBaseEditor<AudioEvent>
    {
        private void OnEnable()
        {
            this.myTarget = (UAudioManager)target;
            SetUpEditor();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("globalEventInstanceLimit"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("globalInstanceBehavior"));
            DrawInspectorGUI(false);
        }
    }
}