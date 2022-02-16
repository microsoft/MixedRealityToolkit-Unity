// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A custom inspector for ObjectManipulator used to separate
    /// ObjectManipulator options into distinct foldout panels.
    /// </summary>
    [CustomEditor(typeof(ObjectManipulator))]
    [CanEditMultipleObjects]
    public class ObjectManipulatorInspector : UnityEditor.Editor
    {
        private ObjectManipulator instance;
        private SerializedProperty hostTransform;
        private SerializedProperty manipulationType;
        private SerializedProperty allowFarManipulation;
        private SerializedProperty useForcesForNearManipulation;

        private SerializedProperty oneHandRotationModeNear;
        private SerializedProperty oneHandRotationModeFar;

        private SerializedProperty twoHandedManipulationType;

        private SerializedProperty releaseBehavior;

        private SerializedProperty transformSmoothingLogicType;
        private SerializedProperty smoothingFar;
        private SerializedProperty smoothingNear;
        private SerializedProperty moveLerpTime;
        private SerializedProperty rotateLerpTime;
        private SerializedProperty scaleLerpTime;

        private SerializedProperty onManipulationStarted;
        private SerializedProperty onManipulationEnded;
        private SerializedProperty onHoverEntered;
        private SerializedProperty onHoverExited;

        private SerializedProperty enableConstraints;
        private SerializedProperty constraintManager;

        private SerializedProperty elasticsManager;

        bool oneHandedFoldout = true;
        bool twoHandedFoldout = true;
        bool constraintsFoldout = true;
        bool nearInteractionFoldout = true;
        bool physicsFoldout = true;
        bool smoothingFoldout = true;
        bool eventsFoldout = true;

        public void OnEnable()
        {
            instance = target as ObjectManipulator;

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
            useForcesForNearManipulation = serializedObject.FindProperty("useForcesForNearManipulation");

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

            // Elastics
            elasticsManager = serializedObject.FindProperty("elasticsManager");

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

            // Near Interaction Support foldout
            nearInteractionFoldout = EditorGUILayout.Foldout(nearInteractionFoldout, "Near Interaction Support", true);

            if (nearInteractionFoldout)
            {
                if (instance.GetComponent<NearInteractionGrabbable>() == null)
                {
                    EditorGUILayout.HelpBox($"By default, {nameof(ObjectManipulator)} only responds to far interaction input.  Add a {nameof(NearInteractionGrabbable)} component to enable near interaction support.", MessageType.Warning);

                    if (GUILayout.Button("Add Near Interaction Grabbable"))
                    {
                        instance.gameObject.AddComponent<NearInteractionGrabbable>();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox($"A {nameof(NearInteractionGrabbable)} is attached to this object, near interaction support is enabled.", MessageType.Info);
                }
            }

            var handedness = (ManipulationHandFlags)manipulationType.intValue;

            EditorGUILayout.Space();
            GUIStyle style = EditorStyles.foldout;
            FontStyle previousStyle = style.fontStyle;
            style.fontStyle = FontStyle.Bold;
            oneHandedFoldout = EditorGUILayout.Foldout(oneHandedFoldout, "One Handed Manipulation", true);

            if (oneHandedFoldout)
            {
                if (handedness.IsMaskSet(ManipulationHandFlags.OneHanded))
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
                if (handedness.IsMaskSet(ManipulationHandFlags.TwoHanded))
                {
                    EditorGUILayout.PropertyField(twoHandedManipulationType);
                }
                else
                {
                    EditorGUILayout.HelpBox("Two handed manipulation disabled. If you wish to enable two handed manipulation select it as a Manipulation Type above.", MessageType.Info);
                }
            }

            var mh = (ObjectManipulator)target;
            var rb = mh.HostTransform.GetComponent<Rigidbody>();

            EditorGUILayout.Space();

            constraintsFoldout = ConstraintManagerInspector.DrawConstraintManagerFoldout(mh.gameObject,
                                                                                        enableConstraints,
                                                                                        constraintManager,
                                                                                        constraintsFoldout);

            EditorGUILayout.Space();
            physicsFoldout = EditorGUILayout.Foldout(physicsFoldout, "Physics", true);

            if (physicsFoldout)
            {
                if (rb != null)
                {
                    EditorGUILayout.PropertyField(releaseBehavior);
                    EditorGUILayout.PropertyField(useForcesForNearManipulation);
                }
                else
                {
                    EditorGUILayout.HelpBox("Physics options disabled. If you wish to enable physics options, add a Rigidbody component to this object.", MessageType.Info);
                }
            }

            EditorGUILayout.Space();
            smoothingFoldout = EditorGUILayout.Foldout(smoothingFoldout, "Smoothing", true);

            if (smoothingFoldout)
            {
                EditorGUILayout.PropertyField(transformSmoothingLogicType);
                EditorGUILayout.PropertyField(smoothingFar);
                EditorGUILayout.PropertyField(smoothingNear);
                EditorGUILayout.PropertyField(moveLerpTime);
                EditorGUILayout.PropertyField(rotateLerpTime);
                EditorGUILayout.PropertyField(scaleLerpTime);
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

            EditorGUILayout.Space();

            Microsoft.MixedReality.Toolkit.Experimental.Editor.ElasticsManagerInspector.DrawElasticsManagerLink(elasticsManager, mh.gameObject);

            EditorGUILayout.Space();

            // reset foldouts style
            style.fontStyle = previousStyle;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
