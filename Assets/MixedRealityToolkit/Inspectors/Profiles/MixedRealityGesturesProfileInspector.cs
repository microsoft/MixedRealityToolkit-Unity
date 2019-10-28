// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(MixedRealityGesturesProfile))]
    public class MixedRealityGesturesProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent MinusButtonContent = new GUIContent("-", "Remove defined Gesture");
        private static readonly GUIContent AddButtonContent = new GUIContent("+ Add a New defined Gesture");
        private static readonly GUIContent DescriptionContent = new GUIContent("Description", "The human readable description of the Gesture.");
        private static readonly GUIContent GestureTypeContent = new GUIContent("Gesture Type", "The type of Gesture that will trigger the action.");
        private static readonly GUIContent ActionContent = new GUIContent("Action", "The action to trigger when a Gesture is recognized.");

        private const string ProfileTitle = "Gesture Settings";
        private const string ProfileDescription = "This gesture map is any and all movements of part the user's body, especially a hand or the head, that raise actions through the input system.\n\n" +
                "Note: Defined controllers can look up the list of gestures and raise the events based on specific criteria.";

        private SerializedProperty gestures;
        private SerializedProperty windowsManipulationGestureSettings;
        private SerializedProperty useRailsNavigation;
        private SerializedProperty windowsNavigationGestureSettings;
        private SerializedProperty windowsRailsNavigationGestures;
        private SerializedProperty windowsGestureAutoStart;

        private MixedRealityGesturesProfile thisProfile;
        private static GUIContent[] allGestureLabels;
        private static int[] allGestureIds;
        private static GUIContent[] actionLabels = Array.Empty<GUIContent>();
        private static int[] actionIds = Array.Empty<int>();
        private bool isInitialized = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            isInitialized = false;

            gestures = serializedObject.FindProperty("gestures");
            windowsManipulationGestureSettings = serializedObject.FindProperty("manipulationGestures");
            useRailsNavigation = serializedObject.FindProperty("useRailsNavigation");
            windowsNavigationGestureSettings = serializedObject.FindProperty("navigationGestures");
            windowsRailsNavigationGestures = serializedObject.FindProperty("railsNavigationGestures");
            windowsGestureAutoStart = serializedObject.FindProperty("windowsGestureAutoStart");

            thisProfile = target as MixedRealityGesturesProfile;
            Debug.Assert(thisProfile != null);

            UpdateGestureLabels();

            if (!IsProfileInActiveInstance() 
                || MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
            {
                return;
            }

            var inputActions = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions;
            actionLabels = inputActions.Select(action => new GUIContent(action.Description)).Prepend(new GUIContent("None")).ToArray();
            actionIds = inputActions.Select(action => (int)action.Id).Prepend(0).ToArray();

            isInitialized = true;
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                    MixedRealityToolkit.Instance.HasActiveProfile &&
                   MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile != null &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.GesturesProfile;
        }

        private void UpdateGestureLabels()
        {
            var allGestureTypeNames = Enum.GetNames(typeof(GestureInputType));

            var tempIds = new List<int>();
            var tempContent = new List<GUIContent>();

            for (int i = 0; i < allGestureTypeNames.Length; i++)
            {
                if (allGestureTypeNames[i].Equals("None") ||
                    thisProfile.Gestures.All(mapping => !allGestureTypeNames[i].Equals(mapping.GestureType.ToString())))
                {
                    tempContent.Add(new GUIContent(allGestureTypeNames[i]));
                    tempIds.Add(i);
                }
            }

            allGestureIds = tempIds.ToArray();
            allGestureLabels = tempContent.ToArray();
        }

        public override void OnInspectorGUI()
        {
            if (!RenderProfileHeader(ProfileTitle, ProfileDescription, target, isInitialized, BackProfileType.Input))
            {
                return;
            }

            CheckMixedRealityInputActions();

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target), false))
            {
                serializedObject.Update();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Windows Gesture Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(windowsManipulationGestureSettings);
                EditorGUILayout.PropertyField(windowsNavigationGestureSettings);
                EditorGUILayout.PropertyField(useRailsNavigation);
                EditorGUILayout.PropertyField(windowsRailsNavigationGestures);
                EditorGUILayout.PropertyField(windowsGestureAutoStart);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Defined Recognizable Gestures", EditorStyles.boldLabel);

                RenderList(gestures);

                serializedObject.ApplyModifiedProperties();
            }
        }

        private void RenderList(SerializedProperty list)
        {
            // Disable gestures list if we could not initialize successfully
            using (new GUIEnabledWrapper(isInitialized, false))
            {
                EditorGUILayout.Space();
                using (new EditorGUILayout.VerticalScope())
                {
                    if (InspectorUIUtility.RenderIndentedButton(AddButtonContent, EditorStyles.miniButton))
                    {
                        list.arraySize += 1;
                        var speechCommand = list.GetArrayElementAtIndex(list.arraySize - 1);
                        var keyword = speechCommand.FindPropertyRelative("description");
                        keyword.stringValue = string.Empty;
                        var gestureType = speechCommand.FindPropertyRelative("gestureType");
                        gestureType.intValue = (int)GestureInputType.None;
                        var action = speechCommand.FindPropertyRelative("action");
                        var actionId = action.FindPropertyRelative("id");
                        actionId.intValue = 0;
                        var actionDescription = action.FindPropertyRelative("description");
                        actionDescription.stringValue = string.Empty;
                        var actionConstraint = action.FindPropertyRelative("axisConstraint");
                        actionConstraint.intValue = 0;
                    }

                    if (list == null || list.arraySize == 0)
                    {
                        EditorGUILayout.HelpBox("Define a new Gesture.", MessageType.Warning);
                        UpdateGestureLabels();
                        return;
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var labelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 24f;
                        EditorGUILayout.LabelField(DescriptionContent, GUILayout.ExpandWidth(true));
                        EditorGUILayout.LabelField(GestureTypeContent, GUILayout.Width(80f));
                        EditorGUILayout.LabelField(ActionContent, GUILayout.Width(64f));
                        EditorGUILayout.LabelField(string.Empty, GUILayout.Width(24f));
                        EditorGUIUtility.labelWidth = labelWidth;
                    }

                    var inputActions = GetInputActions();

                    for (int i = 0; i < list.arraySize; i++)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            SerializedProperty gesture = list.GetArrayElementAtIndex(i);
                            var keyword = gesture.FindPropertyRelative("description");
                            var gestureType = gesture.FindPropertyRelative("gestureType");
                            var action = gesture.FindPropertyRelative("action");
                            var actionId = action.FindPropertyRelative("id");
                            var actionDescription = action.FindPropertyRelative("description");
                            var actionConstraint = action.FindPropertyRelative("axisConstraint");

                            EditorGUILayout.PropertyField(keyword, GUIContent.none, GUILayout.ExpandWidth(true));

                            Debug.Assert(allGestureLabels.Length == allGestureIds.Length);

                            var gestureLabels = new GUIContent[allGestureLabels.Length + 1];
                            var gestureIds = new int[allGestureIds.Length + 1];

                            gestureLabels[0] = new GUIContent(((GestureInputType)gestureType.intValue).ToString());
                            gestureIds[0] = gestureType.intValue;

                            for (int j = 0; j < allGestureLabels.Length; j++)
                            {
                                gestureLabels[j + 1] = allGestureLabels[j];
                                gestureIds[j + 1] = allGestureIds[j];
                            }

                            EditorGUI.BeginChangeCheck();
                            gestureType.intValue = EditorGUILayout.IntPopup(GUIContent.none, gestureType.intValue, gestureLabels, gestureIds, GUILayout.Width(80f));

                            if (EditorGUI.EndChangeCheck())
                            {
                                serializedObject.ApplyModifiedProperties();
                                UpdateGestureLabels();
                            }

                            EditorGUI.BeginChangeCheck();

                            actionId.intValue = EditorGUILayout.IntPopup(GUIContent.none, actionId.intValue, actionLabels, actionIds, GUILayout.Width(64f));

                            if (EditorGUI.EndChangeCheck())
                            {
                                MixedRealityInputAction inputAction = MixedRealityInputAction.None;
                                int idx = actionId.intValue - 1;
                                if (idx >= 0 && idx < inputActions.Length)
                                {
                                    inputAction = inputActions[idx];
                                }

                                actionDescription.stringValue = inputAction.Description;
                                actionConstraint.enumValueIndex = (int)inputAction.AxisConstraint;
                                serializedObject.ApplyModifiedProperties();
                            }

                            if (GUILayout.Button(MinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                            {
                                list.DeleteArrayElementAtIndex(i);
                                serializedObject.ApplyModifiedProperties();
                                UpdateGestureLabels();
                            }
                        }
                    }
                }
            }
        }

        private static MixedRealityInputAction[] GetInputActions()
        {
            if (!MixedRealityToolkit.IsInitialized ||
                !MixedRealityToolkit.Instance.HasActiveProfile ||
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile == null ||
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
            {
                return Array.Empty<MixedRealityInputAction>();
            }

            return MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions;
        }
    }
}