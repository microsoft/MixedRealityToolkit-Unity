// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(MixedRealityControllerVisualizer), true)]
    public class MixedRealityControllerVisualizerInspector : ControllerPoseSynchronizerInspector
    {
        private SerializedProperty rotationOffset;

        protected override void OnEnable()
        {
            rotationOffset = serializedObject.FindProperty("rotationOffset");
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(rotationOffset);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}