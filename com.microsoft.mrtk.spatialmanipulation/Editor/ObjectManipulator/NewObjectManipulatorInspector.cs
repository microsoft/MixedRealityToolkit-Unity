// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation.Editor
{
    /// <summary>
    /// A custom inspector for ObjectManipulator used to separate
    /// ObjectManipulator options into distinct foldout panels.
    /// </summary>
    [CustomEditor(typeof(NewObjectManipulator))]
    [CanEditMultipleObjects]
    public class NewObjectManipulatorInspector : StatefulInteractableInspector
    {
        private NewObjectManipulator instance;
        private SerializedProperty allowedManipulations;
        private SerializedProperty rotationAnchorNear;
        private SerializedProperty rotationAnchorFar;
        private SerializedProperty manipulationLogicTypes;
        private SerializedProperty selectMode;

        private SerializedProperty releaseBehavior;

        private SerializedProperty smoothingFar;
        private SerializedProperty smoothingNear;

        // These alias to the XRI First/Last Select Entered/Exited events
        private SerializedProperty manipulationStarted;
        private SerializedProperty manipulationEnded;

        protected override void OnEnable()
        {
            base.OnEnable();
            instance = target as NewObjectManipulator;
            allowedManipulations = SetUpProperty(nameof(allowedManipulations));

            // Rotation anchor settings
            rotationAnchorNear = SetUpProperty(nameof(rotationAnchorNear));
            rotationAnchorFar = SetUpProperty(nameof(rotationAnchorFar));

            // Manipulation logic
            manipulationLogicTypes = SetUpProperty(nameof(manipulationLogicTypes));

            // Physics
            releaseBehavior = SetUpProperty(nameof(releaseBehavior));

            //Smoothing
            smoothingFar = SetUpProperty(nameof(smoothingFar));
            smoothingNear = SetUpProperty(nameof(smoothingNear));

            // Mirroring base XRI settings for easy access
            selectMode = serializedObject.FindProperty("m_SelectMode");
            manipulationStarted = serializedObject.FindProperty("m_FirstSelectEntered");
            manipulationEnded = serializedObject.FindProperty("m_LastSelectExited");
        }

        static bool baseInteractableFoldout = false;
        static bool advancedSettingFoldout = false;
        static bool physicsFoldout = false;
        static bool smoothingFoldout = false;
        protected override void DrawProperties()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Object Manipulator Settings", EditorStyles.boldLabel);
            using (new EditorGUI.IndentLevelScope())
            {
                EditorGUILayout.PropertyField(allowedManipulations);

                // This is just the XRI SelectMode property, but renamed/aliased to avoid confusion.
                EditorGUILayout.PropertyField(selectMode, new GUIContent("Multiselect Mode", "Can the object can be grabbed by one interactor or multiple at a time?"));

                if (advancedSettingFoldout = EditorGUILayout.Foldout(advancedSettingFoldout, "Advanced Object Manipulator Settings", true, EditorStyles.foldoutHeader))
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.PropertyField(rotationAnchorNear);
                        EditorGUILayout.PropertyField(rotationAnchorFar);
                        EditorGUILayout.PropertyField(manipulationLogicTypes);

                        Rigidbody rb = instance.GetComponent<Rigidbody>();
                        if (physicsFoldout = EditorGUILayout.Foldout(physicsFoldout, "Physics", true))
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                if (rb != null && !rb.isKinematic)
                                {
                                    EditorGUILayout.PropertyField(releaseBehavior);
                                }
                                else
                                {
                                    EditorGUILayout.HelpBox("Physics options disabled. If you wish to enable physics options, add a Rigidbody component to this object.", MessageType.Info);
                                }
                            }
                        }

                        smoothingFoldout = EditorGUILayout.Foldout(smoothingFoldout, "Smoothing", true);
                        if (smoothingFoldout)
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUILayout.PropertyField(smoothingFar);
                                EditorGUILayout.PropertyField(smoothingNear);
                            }
                        }
                    }
                }
            }

            if (baseInteractableFoldout = EditorGUILayout.Foldout(baseInteractableFoldout, "Stateful Interactable Settings", true, EditorStyles.foldoutHeader))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    base.DrawProperties();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        static bool manipulationEventsFoldout = false;
        protected override void DrawInteractableEvents()
        {
            if (manipulationEventsFoldout = EditorGUILayout.Foldout(manipulationEventsFoldout, "Manipulation Events", true))
            {
                // These events just alias to the existing XRI FirstSelectEntered/LastSelectExited events,
                // but are mirrored here for clarity + to be explicit that they can be used for manip start/end.
                EditorGUILayout.PropertyField(manipulationStarted, new GUIContent("Manipulation Started [FirstSelectEntered]", "Fired when manipulation starts."));
                EditorGUILayout.PropertyField(manipulationEnded, new GUIContent("Manipulation Ended [LastSelectExited]", "Fired when manipulation ends."));
            }

            base.DrawInteractableEvents();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
