// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation.Editor
{
    /// <summary>
    /// A custom editor for the <see cref="InBetween"/> class.
    /// </summary>
    [CustomEditor(typeof(InBetween))]
    [CanEditMultipleObjects]
    public class InBetweenEditor : SolverEditor
    {
        private SerializedProperty secondTrackedTargetTypeProperty;
        private SerializedProperty secondTrackedHandednessProperty;
        private SerializedProperty secondTrackedHandJointProperty;
        private SerializedProperty secondTransformOverrideProperty;
        private SerializedProperty partwayOffsetProperty;

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            secondTrackedTargetTypeProperty = serializedObject.FindProperty("secondTrackedObjectType");
            secondTrackedHandednessProperty = serializedObject.FindProperty("secondTrackedHandedness");
            secondTrackedHandJointProperty = serializedObject.FindProperty("secondTrackedHandJoint");
            secondTransformOverrideProperty = serializedObject.FindProperty("secondTransformOverride");
            partwayOffsetProperty = serializedObject.FindProperty("partwayOffset");
        }

        /// <summary>
        /// Called by the Unity editor to render custom inspector UI for this component.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(secondTrackedTargetTypeProperty);

            if (secondTrackedTargetTypeProperty.intValue == (int)TrackedObjectType.ControllerRay)
            {
                EditorGUILayout.PropertyField(secondTrackedHandednessProperty);
            }
            else if (secondTrackedTargetTypeProperty.intValue == (int)TrackedObjectType.HandJoint)
            {
                EditorGUILayout.PropertyField(secondTrackedHandednessProperty);
                EditorGUILayout.PropertyField(secondTrackedHandJointProperty);
            }
            else if (secondTrackedTargetTypeProperty.intValue == (int)TrackedObjectType.CustomOverride)
            {
                EditorGUILayout.PropertyField(secondTransformOverrideProperty);
            }

            EditorGUILayout.PropertyField(partwayOffsetProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
