using Microsoft.MixedReality.Toolkit.SDK.UX.Pointers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SDK.Inspectors.UX.Pointers
{
    [CustomEditor(typeof(ParabolicTeleportPointer))]
    public class ParabolicTeleportPointerInspector : TeleportPointerInspector
    {
        private SerializedProperty minParabolaVelocity;
        private SerializedProperty maxParabolaVelocity;
        private SerializedProperty minDistanceModifier;
        private SerializedProperty maxDistanceModifier;

        private bool parabolicTeleportFoldout = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            minParabolaVelocity = serializedObject.FindProperty("minParabolaVelocity");
            maxParabolaVelocity = serializedObject.FindProperty("maxParabolaVelocity");
            minDistanceModifier = serializedObject.FindProperty("minDistanceModifier");
            maxDistanceModifier = serializedObject.FindProperty("maxDistanceModifier");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            parabolicTeleportFoldout = EditorGUILayout.Foldout(parabolicTeleportFoldout, "Parabolic Pointer Options", true);

            if (parabolicTeleportFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(minParabolaVelocity);
                EditorGUILayout.PropertyField(maxParabolaVelocity);
                EditorGUILayout.PropertyField(minDistanceModifier);
                EditorGUILayout.PropertyField(maxDistanceModifier);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}