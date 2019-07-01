// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Solvers
{
    [CustomEditor(typeof(Orbital2))]
    public class Orbital2Inspector : UnityEditor.Editor
    {
        private SerializedProperty updateLinkedTransform;
        private SerializedProperty moveLerpTime;
        private SerializedProperty rotateLerpTime;
        private SerializedProperty scaleLerpTime;
        private SerializedProperty maintainScale;
        private SerializedProperty smoothing;
        private SerializedProperty lifetime;
        private SerializedProperty referenceObjectType;
        private SerializedProperty trackedObjectToFace;
        private SerializedProperty faceTarget;
        private SerializedProperty pivotAxis;
        private SerializedProperty offsetSpace;
        private SerializedProperty offset;
        private SerializedProperty useAngleStepping;
        private SerializedProperty tetherAngleSteps;

        private Orbital2 orbital2;
        private void OnEnable()
        {
            orbital2 = target as Orbital2;

            updateLinkedTransform = serializedObject.FindProperty("updateLinkedTransform");
            moveLerpTime = serializedObject.FindProperty("moveLerpTime");
            rotateLerpTime = serializedObject.FindProperty("rotateLerpTime");
            scaleLerpTime = serializedObject.FindProperty("scaleLerpTime");
            maintainScale = serializedObject.FindProperty("maintainScale");
            smoothing = serializedObject.FindProperty("smoothing");
            lifetime = serializedObject.FindProperty("lifetime");
            referenceObjectType = serializedObject.FindProperty("referenceObjectType");
            trackedObjectToFace = serializedObject.FindProperty("trackedObjectToFace");
            faceTarget = serializedObject.FindProperty("_faceTarget");
            pivotAxis = serializedObject.FindProperty("pivotAxis");
            offsetSpace = serializedObject.FindProperty("offsetSpace");
            offset = serializedObject.FindProperty("offset");
            useAngleStepping = serializedObject.FindProperty("useAngleStepping");
            tetherAngleSteps = serializedObject.FindProperty("tetherAngleSteps");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(updateLinkedTransform);
            EditorGUILayout.PropertyField(smoothing);

            if (smoothing.boolValue)
            {
                EditorGUILayout.PropertyField(moveLerpTime);
                EditorGUILayout.PropertyField(rotateLerpTime);
                EditorGUILayout.PropertyField(scaleLerpTime);
            }

            EditorGUILayout.PropertyField(maintainScale);
            EditorGUILayout.PropertyField(lifetime);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(referenceObjectType);
            if (EditorGUI.EndChangeCheck())
            {
                orbital2.ReferenceObjectType = (ReferenceObjectType)referenceObjectType.enumValueIndex;
            }

            switch(referenceObjectType.enumValueIndex)
            {
                case 1:
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(trackedObjectToFace);
                    if (EditorGUI.EndChangeCheck())
                    {
                        orbital2.TrackedObjectToFace = (TrackedObjectType)trackedObjectToFace.enumValueIndex;
                    }
                    break;
                case 2:
                    EditorGUILayout.PropertyField(faceTarget);
                    if (faceTarget.objectReferenceValue != null)
                    {
                        EditorGUILayout.PropertyField(offsetSpace);
                        EditorGUILayout.PropertyField(offset);
                    }
                    break;
            }
            EditorGUILayout.PropertyField(pivotAxis);
            EditorGUILayout.PropertyField(useAngleStepping);
            if (useAngleStepping.boolValue)
            {
                EditorGUILayout.PropertyField(tetherAngleSteps);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}