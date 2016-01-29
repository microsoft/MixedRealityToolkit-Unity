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
            DrawInspectorGUI(false);
        }
    }
}