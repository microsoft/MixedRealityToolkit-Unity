// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
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
        private SerializedProperty useLocalSpaceForConstraint;
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
            useLocalSpaceForConstraint = serializedObject.FindProperty("useLocalSpaceForConstraint");
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
                EditorGUILayout.PropertyField(useLocalSpaceForConstraint);
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

            // Draws warning message for deprecated object with button for migration option
            DrawDeprecated();
        }

        private void DrawDeprecated()
        {
            List<Type> requiringTypes;

            if ((target as ManipulationHandler).gameObject.IsComponentRequired<ManipulationHandler>(out requiringTypes))
            {
                string requiringComponentNames = null;

                for (int i = 0; i < requiringTypes.Count; i++)
                {
                    requiringComponentNames += "- " + requiringTypes[i].FullName;
                    if (i < requiringTypes.Count - 1)
                    {
                        requiringComponentNames += '\n';
                    }
                }

                EditorGUILayout.HelpBox($"This component is deprecated. Please migrate object to up to date version. Remove the RequiredComponentAttribute from:\n{requiringComponentNames}", MessageType.Error);
                return;
            }

            EditorGUILayout.HelpBox("This component is deprecated. Please migrate object to up to date version", MessageType.Warning);
            if (GUILayout.Button("Migrate Object"))
            {
                MigrationTool migrationTool = new MigrationTool();

                var component = (ManipulationHandler)target;

                migrationTool.TryAddObjectForMigration(typeof(ObjectManipulatorMigrationHandler), (GameObject)component.gameObject);
                migrationTool.MigrateSelection(typeof(ObjectManipulatorMigrationHandler), true);
            }
        }
    }
}