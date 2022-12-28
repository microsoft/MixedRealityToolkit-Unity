// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation.Editor
{
    /// <summary>
    /// A custom editor for ObjectManipulator used to separate
    /// ObjectManipulator options into distinct foldout panels.
    /// </summary>
    [CustomEditor(typeof(ObjectManipulator))]
    [CanEditMultipleObjects]
    public class ObjectManipulatorEditor : StatefulInteractableEditor
    {
        private ObjectManipulator instance;
        private SerializedProperty hostTransform;
        private SerializedProperty allowedManipulations;
        private SerializedProperty allowedInteractionTypes;
        private SerializedProperty applyTorque;
        private SerializedProperty springForceSoftness;
        private SerializedProperty springTorqueSoftness;
        private SerializedProperty springDamping;
        private SerializedProperty springForceLimit;

        private SerializedProperty rotationAnchorNear;
        private SerializedProperty rotationAnchorFar;

        private SerializedProperty manipulationLogicTypes;

        private SerializedProperty releaseBehavior;

        private SerializedProperty transformSmoothingLogicType;
        private SerializedProperty smoothingFar;
        private SerializedProperty smoothingNear;
        private SerializedProperty moveLerpTime;
        private SerializedProperty rotateLerpTime;
        private SerializedProperty scaleLerpTime;

        private SerializedProperty enableConstraints;
        private SerializedProperty constraintManager;

        private SerializedProperty selectMode;

        // These alias to the XRI First/Last Select Entered/Exited events
        private SerializedProperty manipulationStarted;
        private SerializedProperty manipulationEnded;

        static bool constraintsFoldout = false;
        static bool physicsFoldout = false;
        static bool smoothingFoldout = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            instance = target as ObjectManipulator;

            // General properties
            hostTransform = serializedObject.FindProperty("hostTransform");
            allowedManipulations = serializedObject.FindProperty("allowedManipulations");
            allowedInteractionTypes = serializedObject.FindProperty("allowedInteractionTypes");

            // Rotation anchor settings
            rotationAnchorNear = serializedObject.FindProperty("rotationAnchorNear");
            rotationAnchorFar = serializedObject.FindProperty("rotationAnchorFar");

            // Manipulation logic
            manipulationLogicTypes = serializedObject.FindProperty("manipulationLogicTypes");

            // Physics
            releaseBehavior = serializedObject.FindProperty("releaseBehavior");
            applyTorque = serializedObject.FindProperty("applyTorque");
            springForceSoftness = serializedObject.FindProperty("springForceSoftness");
            springTorqueSoftness = serializedObject.FindProperty("springTorqueSoftness");
            springDamping = serializedObject.FindProperty("springDamping");
            springForceLimit = serializedObject.FindProperty("springForceLimit");

            // Smoothing
            transformSmoothingLogicType = serializedObject.FindProperty("transformSmoothingLogicType");
            smoothingFar = serializedObject.FindProperty("smoothingFar");
            smoothingNear = serializedObject.FindProperty("smoothingNear");
            moveLerpTime = serializedObject.FindProperty("moveLerpTime");
            rotateLerpTime = serializedObject.FindProperty("rotateLerpTime");
            scaleLerpTime = serializedObject.FindProperty("scaleLerpTime");

            // Constraints
            enableConstraints = serializedObject.FindProperty("enableConstraints");
            constraintManager = serializedObject.FindProperty("constraintsManager");

            // Mirroring base XRI settings for easy access
            selectMode = serializedObject.FindProperty("m_SelectMode");
            manipulationStarted = serializedObject.FindProperty("m_FirstSelectEntered");
            manipulationEnded = serializedObject.FindProperty("m_LastSelectExited");
        }

        static bool baseInteractableFoldout = false;

        protected override void DrawProperties()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("ObjectManipulator Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(hostTransform);
            EditorGUILayout.PropertyField(allowedManipulations);
            EditorGUILayout.PropertyField(allowedInteractionTypes);

            // This is just the XRI SelectMode property, but renamed/aliased to avoid confusion.
            EditorGUILayout.PropertyField(selectMode, new GUIContent("Multiselect Mode", "Can the object can be grabbed by one interactor or multiple at a time?"));

            EditorGUILayout.PropertyField(rotationAnchorNear);
            EditorGUILayout.PropertyField(rotationAnchorFar);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(manipulationLogicTypes);

            ObjectManipulator objectManipulator = (ObjectManipulator)target;
            Rigidbody rb = objectManipulator.HostTransform.GetComponent<Rigidbody>();

            constraintsFoldout = ConstraintManagerEditor.DrawConstraintManagerFoldout(objectManipulator.gameObject,
                                                                                        enableConstraints,
                                                                                        constraintManager,
                                                                                        constraintsFoldout);

            physicsFoldout = EditorGUILayout.Foldout(physicsFoldout, "Physics", true);

            if (physicsFoldout)
            {
                if (rb != null && !rb.isKinematic)
                {
                    EditorGUILayout.PropertyField(releaseBehavior);
                    EditorGUILayout.PropertyField(applyTorque);
                    EditorGUILayout.PropertyField(springForceSoftness);
                    EditorGUILayout.PropertyField(springTorqueSoftness);
                    EditorGUILayout.PropertyField(springDamping);
                    EditorGUILayout.PropertyField(springForceLimit);
                }
                else
                {
                    EditorGUILayout.HelpBox("Physics options disabled. If you wish to enable physics options, add a Rigidbody component to this object.", MessageType.Info);
                }
            }

            smoothingFoldout = EditorGUILayout.Foldout(smoothingFoldout, "Smoothing", true);

            if (smoothingFoldout)
            {
                if (rb == null || rb.isKinematic)
                {
                    EditorGUILayout.PropertyField(moveLerpTime);
                    EditorGUILayout.PropertyField(rotateLerpTime);
                }
                else
                {
                    EditorGUILayout.HelpBox("Move&Rotation smoothing disabled for dynamic Rigidbody. Use Physics smoothing instead.", MessageType.Info);
                }

                EditorGUILayout.PropertyField(scaleLerpTime);
                EditorGUILayout.PropertyField(transformSmoothingLogicType);
                EditorGUILayout.PropertyField(smoothingFar);
                EditorGUILayout.PropertyField(smoothingNear);
            }

            EditorGUILayout.Space();

            // These events just alias to the existing XRI FirstSelectEntered/LastSelectExited events,
            // but are mirrored here for clarity + to be explicit that they can be used for manip start/end.
            EditorGUILayout.PropertyField(manipulationStarted, new GUIContent("Manipulation Started [FirstSelectEntered]", "Fired when manipulation starts."));
            EditorGUILayout.PropertyField(manipulationEnded, new GUIContent("Manipulation Ended [LastSelectExited]", "Fired when manipulation ends."));

            if (baseInteractableFoldout = EditorGUILayout.Foldout(baseInteractableFoldout, "Base Interactable Settings", true, EditorStyles.foldoutHeader))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    base.DrawProperties();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
