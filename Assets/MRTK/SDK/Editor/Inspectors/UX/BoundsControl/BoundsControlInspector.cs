//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
//

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Experimental.Editor;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(BoundsControl), true)]
    [CanEditMultipleObjects]
    public class BoundsControlInspector : UnityEditor.Editor
    {
        private SerializedProperty targetObject;
        private SerializedProperty boundsOverride;
        private SerializedProperty boundsCalculationMethod;
        private SerializedProperty activationType;
        private SerializedProperty controlPadding;
        private SerializedProperty flattenAxis;

        private SerializedProperty smoothingActive;
        private SerializedProperty rotateLerpTime;
        private SerializedProperty scaleLerpTime;

        // Configs
        private SerializedProperty boxDisplayConfiguration;
        private SerializedProperty linksConfiguration;
        private SerializedProperty scaleHandlesConfiguration;
        private SerializedProperty rotationHandlesConfiguration;
        private SerializedProperty translationHandlesConfiguration;
        private SerializedProperty proximityEffectConfiguration;

        // Debug
        private SerializedProperty hideElementsInHierarchyEditor;

        // Events
        private SerializedProperty rotateStartedEvent;
        private SerializedProperty rotateStoppedEvent;
        private SerializedProperty scaleStartedEvent;
        private SerializedProperty scaleStoppedEvent;
        private SerializedProperty translateStartedEvent;
        private SerializedProperty translateStoppedEvent;

        private SerializedProperty enableConstraints;
        private SerializedProperty constraintManager;

        private SerializedProperty elasticsManager;

        private BoundsControl boundsControl;

        private static bool showBoxConfiguration = false;
        private static bool showScaleHandlesConfiguration = false;
        private static bool showRotationHandlesConfiguration = false;
        private static bool showTranslationHandlesConfiguration = false;
        private static bool showLinksConfiguration = false;
        private static bool showProximityConfiguration = false;
        private static bool constraintsFoldout = true;

        private void OnEnable()
        {
            boundsControl = (BoundsControl)target;

            targetObject = serializedObject.FindProperty("targetObject");
            activationType = serializedObject.FindProperty("activation");
            boundsOverride = serializedObject.FindProperty("boundsOverride");
            boundsCalculationMethod = serializedObject.FindProperty("boundsCalculationMethod");
            flattenAxis = serializedObject.FindProperty("flattenAxis");
            controlPadding = serializedObject.FindProperty("boxPadding");

            smoothingActive = serializedObject.FindProperty("smoothingActive");
            rotateLerpTime = serializedObject.FindProperty("rotateLerpTime");
            scaleLerpTime = serializedObject.FindProperty("scaleLerpTime");

            boxDisplayConfiguration = serializedObject.FindProperty("boxDisplayConfiguration");
            linksConfiguration = serializedObject.FindProperty("linksConfiguration");
            scaleHandlesConfiguration = serializedObject.FindProperty("scaleHandlesConfiguration");
            rotationHandlesConfiguration = serializedObject.FindProperty("rotationHandlesConfiguration");
            translationHandlesConfiguration = serializedObject.FindProperty("translationHandlesConfiguration");
            proximityEffectConfiguration = serializedObject.FindProperty("handleProximityEffectConfiguration");

            hideElementsInHierarchyEditor = serializedObject.FindProperty("hideElementsInInspector");

            rotateStartedEvent = serializedObject.FindProperty("rotateStarted");
            rotateStoppedEvent = serializedObject.FindProperty("rotateStopped");
            scaleStartedEvent = serializedObject.FindProperty("scaleStarted");
            scaleStoppedEvent = serializedObject.FindProperty("scaleStopped");
            translateStartedEvent = serializedObject.FindProperty("translateStarted");
            translateStoppedEvent = serializedObject.FindProperty("translateStopped");

            // constraints
            enableConstraints = serializedObject.FindProperty("enableConstraints");
            constraintManager = serializedObject.FindProperty("constraintsManager");

            // Elastics
            elasticsManager = serializedObject.FindProperty("elasticsManager");
        }

        public override void OnInspectorGUI()
        {
            if (target != null)
            {
                // Notification section - first thing to show in bounds control component
                DrawRigidBodyWarning();

                // Help url
                InspectorUIUtility.RenderHelpURL(target.GetType());

                // Data section
                {
                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(targetObject);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(new GUIContent("Behavior"), EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(activationType);
                    EditorGUILayout.PropertyField(boundsOverride);
                    EditorGUILayout.PropertyField(boundsCalculationMethod);
                    EditorGUILayout.PropertyField(controlPadding);
                    EditorGUILayout.PropertyField(flattenAxis);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(new GUIContent("Smoothing"), EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(smoothingActive);
                    EditorGUILayout.PropertyField(scaleLerpTime);
                    EditorGUILayout.PropertyField(rotateLerpTime);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(new GUIContent("Visuals", "Bounds Control Visual Configurations"), EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
                    using (new EditorGUI.IndentLevelScope())
                    {
                        showBoxConfiguration = InspectorUIUtility.DrawScriptableFoldout<BoxDisplayConfiguration>(boxDisplayConfiguration, 
                                                                                                                 "Box Configuration", 
                                                                                                                 showBoxConfiguration);

                        showScaleHandlesConfiguration = InspectorUIUtility.DrawScriptableFoldout<ScaleHandlesConfiguration>(scaleHandlesConfiguration,
                                                                                                                            "Scale Handles Configuration",
                                                                                                                            showScaleHandlesConfiguration);

                        showRotationHandlesConfiguration = InspectorUIUtility.DrawScriptableFoldout<RotationHandlesConfiguration>(rotationHandlesConfiguration,
                                                                                                                                  "Rotation Handles Configuration",
                                                                                                                                  showRotationHandlesConfiguration);

                        showTranslationHandlesConfiguration = InspectorUIUtility.DrawScriptableFoldout<TranslationHandlesConfiguration>(translationHandlesConfiguration,
                                                                                                                                        "Translation Handles Configuration",
                                                                                                                                        showTranslationHandlesConfiguration);

                        showLinksConfiguration = InspectorUIUtility.DrawScriptableFoldout<LinksConfiguration>(linksConfiguration, 
                                                                                                              "Links Configuration", 
                                                                                                              showLinksConfiguration);

                        showProximityConfiguration = InspectorUIUtility.DrawScriptableFoldout<ProximityEffectConfiguration>(proximityEffectConfiguration, 
                                                                                                                            "Proximity Configuration", 
                                                                                                                            showProximityConfiguration);
                    }

                    EditorGUILayout.Space();

                    
                    //if (enableConstraints == false)
                    //{
                    //    GUI.enabled = false;
                    //}
                    constraintsFoldout = EditorGUILayout.Foldout(constraintsFoldout, "Constraints", true);

                    if (constraintsFoldout)
                    {
                        EditorGUILayout.PropertyField(enableConstraints);
                        GUI.enabled = enableConstraints.boolValue;
                        //EditorGUILayout.LabelField(new GUIContent("Constraints"), EditorStyles.boldLabel);
                        var constraintManagers = boundsControl.gameObject.GetComponents<ConstraintManager>();

                        int selected = 0;

                        string[] options = new string[constraintManagers.Length];

                        int manualSelectionCount = 0;
                        for (int i = 0; i < constraintManagers.Length; ++i)
                        {
                            var manager = constraintManagers[i];
                            if (constraintManager.objectReferenceValue == manager)
                            {
                                selected = i;
                            }

                            // popups will only show unqiue elements
                            // in case of auto selection we don't care which one we're selecting as the behavior will be the same.
                            // in case of manual selection users might want to differentiate which constraintmanager they are referring to.
                            if (manager.AutoConstraintSelection == true)
                            {
                                options[i] = manager.name + " auto selection";
                            }
                            else
                            {
                                manualSelectionCount++;
                                options[i] = manager.name + " manual selection " + manualSelectionCount;
                            }
                        }


                        EditorGUILayout.BeginHorizontal();

                        selected = EditorGUILayout.Popup("Constraint Manager", selected, options, GUILayout.ExpandWidth(true));
                        ConstraintManager selectedConstraintManager = constraintManagers[selected];
                        constraintManager.objectReferenceValue = selectedConstraintManager;
                        if (GUILayout.Button("Go to component"))
                        {
                            EditorGUIUtility.PingObject(selectedConstraintManager);
                            Highlighter.Highlight("Inspector", $"ComponentId: {selectedConstraintManager.GetInstanceID()}");
                            EditorGUIUtility.ExitGUI();
                        }
                        EditorGUILayout.EndHorizontal();


                        //foreach (var constraint in selectedConstraintManager.ProcessedConstraints)
                        //{
                        //    if (constraint != null)
                        //    {
                        //        EditorGUILayout.BeginHorizontal();
                        //        string constraintName = constraint.GetType().Name;
                        //        EditorGUILayout.LabelField(constraintName);
                        //        if (GUILayout.Button("Go to component"))
                        //        {
                        //            Highlighter.Highlight("Inspector", $"{ObjectNames.NicifyVariableName(constraintName)} (Script)");
                        //            EditorGUIUtility.ExitGUI();
                        //        }
                        //        EditorGUILayout.EndHorizontal();
                        //    }
                        //}

                        //EditorGUILayout.PropertyField(constraintManager);
                        //string typeDescription = "Constraint";
                        //GameObject gameObject = boundsControl.gameObject;
                        //if (constraintManager.objectReferenceValue != null)
                        //{
                        //        EditorGUILayout.Space();
                        //        EditorGUILayout.LabelField(new GUIContent("Constraints"), EditorStyles.boldLabel);
                        //        //constraintsFoldout = EditorGUILayout.Foldout(constraintsFoldout, typeDescription + "s", true);

                        //        //if (constraintsFoldout)
                        //        //{
                        //        System.Collections.Generic.List<TransformConstraint> constraints;
                        //        if (selectedConstraintManager.AutoConstraintSelection == true)
                        //        {
                        //            constraints = gameObject.GetComponents<TransformConstraint>().ToList();

                        //        }
                        //        else
                        //        {
                        //            constraints = selectedConstraintManager.AppliedConstraints;
                        //        }

                        //        foreach (var constraint in constraints)
                        //        {
                        //            EditorGUILayout.BeginHorizontal();
                        //            string constraintName = constraint.GetType().Name;
                        //            EditorGUILayout.LabelField(constraintName);
                        //            if (GUILayout.Button("Go to component"))
                        //            {
                        //                Highlighter.Highlight("Inspector", $"{ObjectNames.NicifyVariableName(constraintName)} (Script)");
                        //                EditorGUIUtility.ExitGUI();
                        //            }
                        //            EditorGUILayout.EndHorizontal();
                        //        }

                        //    }


                        //}

                        GUI.enabled = true;
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(new GUIContent("Events", "Bounds Control Events"), EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
                    {
                        EditorGUILayout.PropertyField(rotateStartedEvent);
                        EditorGUILayout.PropertyField(rotateStoppedEvent);
                        EditorGUILayout.PropertyField(scaleStartedEvent);
                        EditorGUILayout.PropertyField(scaleStoppedEvent);
                        EditorGUILayout.PropertyField(translateStartedEvent);
                        EditorGUILayout.PropertyField(translateStoppedEvent);
                    }

                    EditorGUILayout.Space();

                    ElasticsManagerInspector.DrawElasticsManagerLink(elasticsManager, boundsControl.gameObject);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(new GUIContent("Debug", "Bounds Control Debug Section"), EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
                    {
                        EditorGUILayout.PropertyField(hideElementsInHierarchyEditor);
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }

        private void DrawRigidBodyWarning()
        {
            // Check if rigidbody is attached - if so show warning in case input profile is not configured for individual collider raycast
            Rigidbody rigidBody = boundsControl.GetComponent<Rigidbody>();

            if (rigidBody != null)
            {
                MixedRealityInputSystemProfile profile = Microsoft.MixedReality.Toolkit.CoreServices.InputSystem?.InputSystemProfile;
                if (profile != null && profile.FocusIndividualCompoundCollider == false)
                {
                    EditorGUILayout.Space();
                    // Show warning and button to reconfigure profile
                    EditorGUILayout.HelpBox($"When using Bounds Control in combination with Rigidbody 'Focus Individual Compound Collider' must be enabled in Input Profile.", UnityEditor.MessageType.Warning);
                    if (GUILayout.Button($"Enable 'Focus Individual Compound Collider' in Input Profile"))
                    {
                        profile.FocusIndividualCompoundCollider = true;
                    }

                    EditorGUILayout.Space();
                }
            }
        }
    }
}
