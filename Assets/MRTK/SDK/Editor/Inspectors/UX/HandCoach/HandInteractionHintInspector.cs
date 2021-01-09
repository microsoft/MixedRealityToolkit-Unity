// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using Microsoft.MixedReality.Toolkit.UI.HandCoach;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A custom inspector for ElasticsManager used to separate
    /// Elastics configurations into distinct foldout panels.
    /// </summary>
    [CustomEditor(typeof(HandInteractionHint))]
    [CanEditMultipleObjects]
    public class HandInteractionHintInspector : UnityEditor.Editor
    {
        private SerializedProperty hintDisplayDelay;
        private SerializedProperty hideIfHandTracked;
        private SerializedProperty trackedHandHintDisplayDelay;
        private SerializedProperty repeats;
        private SerializedProperty autoActivate;
        private SerializedProperty animationState;
        private SerializedProperty repeatDelay;

        public void OnEnable()
        {
            hintDisplayDelay = serializedObject.FindProperty("hintDisplayDelay");
            hideIfHandTracked = serializedObject.FindProperty("hideIfHandTracked");
            trackedHandHintDisplayDelay = serializedObject.FindProperty("trackedHandHintDisplayDelay");
            repeats = serializedObject.FindProperty("repeats");
            autoActivate = serializedObject.FindProperty("autoActivate");
            animationState = serializedObject.FindProperty("animationState");
            repeatDelay = serializedObject.FindProperty("repeatDelay");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(hintDisplayDelay);
            EditorGUILayout.PropertyField(hideIfHandTracked);

            if (!hideIfHandTracked.boolValue)
            {
                EditorGUILayout.PropertyField(trackedHandHintDisplayDelay);
            }

            EditorGUILayout.PropertyField(repeats);
            EditorGUILayout.PropertyField(autoActivate);
            EditorGUILayout.PropertyField(animationState);
            EditorGUILayout.PropertyField(repeatDelay);


            serializedObject.ApplyModifiedProperties();
        }
    }
}
