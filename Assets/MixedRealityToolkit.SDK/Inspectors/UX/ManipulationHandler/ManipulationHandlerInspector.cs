//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(ManipulationHandler))]
    [CanEditMultipleObjects]
    public class ManipulationHandlerInspector : UnityEditor.Editor
    {
        private SerializedProperty hostTransform;
        private SerializedProperty manipulationType;
        private SerializedProperty allowFarManipulation;
        private SerializedProperty oneHandRotationModeNear;
        private SerializedProperty oneHandRotationModeFar;
        private SerializedProperty twoHandedManipulationType;
        private SerializedProperty releaseBehavior;
        private SerializedProperty constraintOnRotation;
        private SerializedProperty constraintOnMovement;
        private SerializedProperty smoothingActive;
        private SerializedProperty smoothingAmountOneHandManip;
        private SerializedProperty onManipulationStarted;
        private SerializedProperty onManipulationEnded;
        private SerializedProperty onHoverEntered;
        private SerializedProperty onHoverExited;

        bool oneHandedFoldout = true;
        bool twoHandedFoldout = true;
        bool physicsFoldout = true;
        bool constraintsFoldout = true;
        bool smoothingFoldout = true;
        bool eventsFoldout = true;

        public void OnEnable()
        {
            // General properties
            hostTransform = serializedObject.FindProperty("hostTransform");
            manipulationType = serializedObject.FindProperty("manipulationType");
            allowFarManipulation = serializedObject.FindProperty("allowFarManipulation");

            // One handed
            oneHandRotationModeNear = serializedObject.FindProperty("oneHandRotationModeNear");
            oneHandRotationModeFar = serializedObject.FindProperty("oneHandRotationModeFar");

            // Two handed
            twoHandedManipulationType = serializedObject.FindProperty("twoHandedManipulationType");

            // Physics
            releaseBehavior = serializedObject.FindProperty("releaseBehavior");

            // Constraints
            constraintOnRotation = serializedObject.FindProperty("constraintOnRotation");
            constraintOnMovement = serializedObject.FindProperty("constraintOnMovement");

            // Smoothing
            smoothingActive = serializedObject.FindProperty("smoothingActive");
            smoothingAmountOneHandManip = serializedObject.FindProperty("smoothingAmountOneHandManip");

            // Manipulation Events
            onManipulationStarted = serializedObject.FindProperty("onManipulationStarted");
            onManipulationEnded = serializedObject.FindProperty("onManipulationEnded");
            onHoverEntered = serializedObject.FindProperty("onHoverEntered");
            onHoverExited = serializedObject.FindProperty("onHoverExited");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Manipulation Handler will soon be removed, please upgrade to Object Manipulator", MessageType.Warning);
            if (GUILayout.Button("Upgrade to Object Manipulator"))
            {
                Migrate();
                return;
            }

            EditorGUILayout.PropertyField(hostTransform);
            EditorGUILayout.PropertyField(manipulationType);
            EditorGUILayout.PropertyField(allowFarManipulation);

            var handedness = (ManipulationHandler.HandMovementType)manipulationType.intValue;

            EditorGUILayout.Space();
            GUIStyle style = EditorStyles.foldout;
            FontStyle previousStyle = style.fontStyle;
            style.fontStyle = FontStyle.Bold;
            oneHandedFoldout = EditorGUILayout.Foldout(oneHandedFoldout, "One Handed Manipulation", true);

            if (oneHandedFoldout)
            {
                if (handedness == ManipulationHandler.HandMovementType.OneHandedOnly ||
                    handedness == ManipulationHandler.HandMovementType.OneAndTwoHanded)
                {
                    EditorGUILayout.PropertyField(oneHandRotationModeNear);
                    EditorGUILayout.PropertyField(oneHandRotationModeFar);
                }
                else
                {
                    EditorGUILayout.HelpBox("One handed manipulation disabled. If you wish to enable one handed manipulation select it as a Manipulation Type above.", MessageType.Info);
                }
            }

            EditorGUILayout.Space();
            twoHandedFoldout = EditorGUILayout.Foldout(twoHandedFoldout, "Two Handed Manipulation", true);

            if (twoHandedFoldout)
            {
                if (handedness == ManipulationHandler.HandMovementType.TwoHandedOnly ||
                    handedness == ManipulationHandler.HandMovementType.OneAndTwoHanded)
                {
                    EditorGUILayout.PropertyField(twoHandedManipulationType);
                }
                else
                {
                    EditorGUILayout.HelpBox("Two handed manipulation disabled. If you wish to enable two handed manipulation select it as a Manipulation Type above.", MessageType.Info);
                }
            }

            var mh = (ManipulationHandler)target;
            var rb = mh.GetComponent<Rigidbody>();

            EditorGUILayout.Space();
            physicsFoldout = EditorGUILayout.Foldout(physicsFoldout, "Physics", true);

            if (physicsFoldout)
            {
                if (rb != null)
                {
                    EditorGUILayout.PropertyField(releaseBehavior);
                }
                else
                {
                    EditorGUILayout.HelpBox("Physics options disabled. If you wish to enable physics options, add a Rigidbody component to this object.", MessageType.Info);
                }
            }

            EditorGUILayout.Space();
            constraintsFoldout = EditorGUILayout.Foldout(constraintsFoldout, "Constraints", true);

            if (constraintsFoldout)
            {
                EditorGUILayout.PropertyField(constraintOnRotation);
                EditorGUILayout.PropertyField(constraintOnMovement);
            }

            EditorGUILayout.Space();
            smoothingFoldout = EditorGUILayout.Foldout(smoothingFoldout, "Smoothing", true);

            if (smoothingFoldout)
            {
                EditorGUILayout.PropertyField(smoothingActive);
                EditorGUILayout.PropertyField(smoothingAmountOneHandManip);
            }

            EditorGUILayout.Space();
            eventsFoldout = EditorGUILayout.Foldout(eventsFoldout, "Manipulation Events", true);

            if (eventsFoldout)
            {
                EditorGUILayout.PropertyField(onManipulationStarted);
                EditorGUILayout.PropertyField(onManipulationEnded);
                EditorGUILayout.PropertyField(onHoverEntered);
                EditorGUILayout.PropertyField(onHoverExited);
            }

            // reset foldouts style
            style.fontStyle = previousStyle;

            serializedObject.ApplyModifiedProperties();
        }

        private void Migrate()
        {
            var mh1 = target as ManipulationHandler;
            var mh2 = mh1.gameObject.AddComponent<ObjectManipulator>();

            mh2.HostTransform = mh1.HostTransform;

            switch (mh1.ManipulationType)
            {
                case ManipulationHandler.HandMovementType.OneHandedOnly:
                    mh2.ManipulationType = ObjectManipulator.HandMovementType.OneHanded;
                    break;
                case ManipulationHandler.HandMovementType.TwoHandedOnly:
                    mh2.ManipulationType = ObjectManipulator.HandMovementType.TwoHanded;
                    break;
                case ManipulationHandler.HandMovementType.OneAndTwoHanded:
                    mh2.ManipulationType = ObjectManipulator.HandMovementType.OneHanded | 
                        ObjectManipulator.HandMovementType.TwoHanded;
                    break;
            }

            mh2.AllowFarManipulation = mh1.AllowFarManipulation;
            mh2.OneHandRotationModeNear = (ObjectManipulator.RotateInOneHandType)mh1.OneHandRotationModeNear;
            mh2.OneHandRotationModeFar = (ObjectManipulator.RotateInOneHandType)mh1.OneHandRotationModeFar;

            switch (mh1.TwoHandedManipulationType)
            {
                case ManipulationHandler.TwoHandedManipulation.Scale:
                    mh2.TwoHandedManipulationType = ObjectManipulator.TwoHandedManipulation.Scale;
                    break;
                case ManipulationHandler.TwoHandedManipulation.Rotate:
                    mh2.TwoHandedManipulationType = ObjectManipulator.TwoHandedManipulation.Rotate;
                    break;
                case ManipulationHandler.TwoHandedManipulation.MoveScale:
                    mh2.TwoHandedManipulationType = ObjectManipulator.TwoHandedManipulation.Move |
                        ObjectManipulator.TwoHandedManipulation.Scale;
                    break;
                case ManipulationHandler.TwoHandedManipulation.MoveRotate:
                    mh2.TwoHandedManipulationType = ObjectManipulator.TwoHandedManipulation.Move |
                        ObjectManipulator.TwoHandedManipulation.Rotate;
                    break;
                case ManipulationHandler.TwoHandedManipulation.RotateScale:
                    mh2.TwoHandedManipulationType = ObjectManipulator.TwoHandedManipulation.Rotate |
                        ObjectManipulator.TwoHandedManipulation.Scale;
                    break;
                case ManipulationHandler.TwoHandedManipulation.MoveRotateScale:
                    mh2.TwoHandedManipulationType = ObjectManipulator.TwoHandedManipulation.Move |
                        ObjectManipulator.TwoHandedManipulation.Rotate |
                        ObjectManipulator.TwoHandedManipulation.Scale;
                    break;
            }

            mh2.ReleaseBehavior = (ObjectManipulator.ReleaseBehaviorType)mh1.ReleaseBehavior;
            mh2.ConstraintOnRotation = mh1.ConstraintOnRotation;
            mh2.ConstraintOnMovement = mh1.ConstraintOnMovement;
            mh2.SmoothingActive = mh1.SmoothingActive;
            mh2.MoveLerpTime = mh1.SmoothingAmoutOneHandManip;
            mh2.RotateLerpTime = mh1.SmoothingAmoutOneHandManip;
            mh2.ScaleLerpTime = mh1.SmoothingAmoutOneHandManip;
            mh2.OnManipulationStarted = mh1.OnManipulationStarted;
            mh2.OnManipulationEnded = mh1.OnManipulationEnded;
            mh2.OnHoverEntered = mh1.OnHoverEntered;
            mh2.OnHoverExited = mh1.OnHoverExited;

            DestroyImmediate(mh1);
        }
    }
}
