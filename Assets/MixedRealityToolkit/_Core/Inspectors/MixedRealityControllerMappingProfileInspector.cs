// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityControllerMappingProfile))]
    public class MixedRealityControllerMappingProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent ControllerAddButtonContent = new GUIContent("+ Add a New Controller Template");
        private static readonly GUIContent ControllerMinusButtonContent = new GUIContent("-", "Remove Controller Template");
        private static readonly GUIContent InteractionAddButtonContent = new GUIContent("+ Add a New Interaction Mapping");
        private static readonly GUIContent InteractionMinusButtonContent = new GUIContent("-", "Remove Interaction Mapping");
        private static readonly GUIContent AxisTypeContent = new GUIContent("Axis Type", "The axis type of the button, e.g. Analogue, Digital, etc.");
        private static readonly GUIContent ControllerInputTypeContent = new GUIContent("Input Type", "The primary action of the input as defined by the controller SDK.");
        private static readonly GUIContent ActionContent = new GUIContent("Action", "Action to be raised to the Input Manager when the input data has changed.");

        private static bool[] controllerFoldouts;

        private SerializedProperty mixedRealityControllerMappingProfiles;
        private static GUIContent[] actionLabels;
        private static int[] actionIds;

        private SerializedProperty renderMotionControllers;
        private SerializedProperty useDefaultModels;
        private SerializedProperty overrideLeftHandModel;
        private SerializedProperty overrideRightHandModel;

        private void OnEnable()
        {
            if (!CheckMixedRealityManager(false))
            {
                return;
            }

            mixedRealityControllerMappingProfiles = serializedObject.FindProperty("mixedRealityControllerMappingProfiles");
            if (controllerFoldouts == null || controllerFoldouts.Length != mixedRealityControllerMappingProfiles.arraySize)
            {
                controllerFoldouts = new bool[mixedRealityControllerMappingProfiles.arraySize];
            }

            actionLabels = MixedRealityManager.Instance.ActiveProfile.InputActionsProfile.InputActions.Select(action => new GUIContent(action.Description)).Prepend(new GUIContent("None")).ToArray();
            actionIds = MixedRealityManager.Instance.ActiveProfile.InputActionsProfile.InputActions.Select(action => (int)action.Id).Prepend(0).ToArray();

            renderMotionControllers = serializedObject.FindProperty("renderMotionControllers");
            useDefaultModels = serializedObject.FindProperty("useDefaultModels");
            overrideLeftHandModel = serializedObject.FindProperty("overrideLeftHandModel");
            overrideRightHandModel = serializedObject.FindProperty("overrideRightHandModel");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();

            EditorGUILayout.LabelField("Controller Templates", EditorStyles.boldLabel);

            if (!CheckMixedRealityManager())
            {
                return;
            }

            EditorGUILayout.HelpBox("Controller templates define all the controllers your users will be able to use in your application.\n\n" +
                                    "After defining all your Input Actions, you can then wire them up to hardware sensors, controllers, and other input devices.", MessageType.Info);

            serializedObject.Update();

            EditorGUILayout.PropertyField(renderMotionControllers);
            if (renderMotionControllers.boolValue)
            {
                EditorGUILayout.PropertyField(useDefaultModels);
                if (!useDefaultModels.boolValue)
                {
                    EditorGUILayout.PropertyField(overrideLeftHandModel);
                    EditorGUILayout.PropertyField(overrideRightHandModel);
                }
            }

            RenderControllerProfilesList(mixedRealityControllerMappingProfiles);

            serializedObject.ApplyModifiedProperties();
        }

        private static void RenderControllerProfilesList(SerializedProperty list)
        {
            EditorGUILayout.Space();
            GUILayout.BeginVertical();

            if (GUILayout.Button(ControllerAddButtonContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
                var mixedRealityControllerMapping = list.GetArrayElementAtIndex(list.arraySize - 1);
                var mixedRealityControllerMappingId = mixedRealityControllerMapping.FindPropertyRelative("id");
                var mixedRealityControllerMappingDescription = mixedRealityControllerMapping.FindPropertyRelative("description");
                mixedRealityControllerMappingDescription.stringValue = $"New Controller Template {mixedRealityControllerMappingId.intValue = list.arraySize}";
                controllerFoldouts = new bool[list.arraySize];
                return;
            }

            GUILayout.Space(12f);
            GUILayout.BeginVertical();

            if (list == null || list.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Create a new Controller Template.", MessageType.Warning);
            }

            for (int i = 0; i < list?.arraySize; i++)
            {
                GUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();

                var previousLabelWidth = EditorGUIUtility.labelWidth;
                var mixedRealityControllerMapping = list.GetArrayElementAtIndex(i);
                var mixedRealityControllerMappingId = mixedRealityControllerMapping.FindPropertyRelative("id");
                var mixedRealityControllerMappingDescription = mixedRealityControllerMapping.FindPropertyRelative("description");

                EditorGUIUtility.labelWidth = 64f;
                EditorGUILayout.PropertyField(mixedRealityControllerMappingDescription, new GUIContent($"Controller {mixedRealityControllerMappingId.intValue = i + 1}"));
                EditorGUIUtility.labelWidth = previousLabelWidth;

                if (GUILayout.Button(ControllerMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    list.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.EndHorizontal();

                    var controllerType = mixedRealityControllerMapping.FindPropertyRelative("controller");
                    var controllerHandedness = mixedRealityControllerMapping.FindPropertyRelative("handedness");
                    var useDefaultModel = mixedRealityControllerMapping.FindPropertyRelative("defaultModel");
                    var controllerModel = mixedRealityControllerMapping.FindPropertyRelative("overrideModel");
                    var interactionsList = mixedRealityControllerMapping.FindPropertyRelative("interactions");

                    EditorGUI.indentLevel++;
                    EditorGUIUtility.labelWidth = 96f;
                    EditorGUILayout.PropertyField(controllerType);
                    EditorGUILayout.PropertyField(controllerHandedness);
                    EditorGUILayout.PropertyField(useDefaultModel);
                    if (!useDefaultModel.boolValue)
                    {
                        EditorGUILayout.PropertyField(controllerModel);
                    }
                    EditorGUIUtility.labelWidth = previousLabelWidth;

                    controllerFoldouts[i] = EditorGUILayout.Foldout(controllerFoldouts[i], new GUIContent("Interaction Mappings"), true);
                    if (controllerFoldouts[i])
                    {
                        GUILayout.BeginHorizontal();
                        RenderInteractionList(interactionsList);
                        GUILayout.EndHorizontal();
                    }

                    EditorGUI.indentLevel--;
                }

                GUILayout.EndVertical();
                GUILayout.Space(12f);
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        private static void RenderInteractionList(SerializedProperty list)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Space(24f);
            if (GUILayout.Button(InteractionAddButtonContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
                var interaction = list.GetArrayElementAtIndex(list.arraySize - 1);
                var axisType = interaction.FindPropertyRelative("axisType");
                axisType.enumValueIndex = 0;
                var inputType = interaction.FindPropertyRelative("inputType");
                inputType.enumValueIndex = 0;
                var action = interaction.FindPropertyRelative("inputAction");
                var actionId = action.FindPropertyRelative("id");
                actionId.intValue = 0;
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(12f);

            if (list == null || list.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Create a Interaction Mapping.", MessageType.Warning);
                GUILayout.EndVertical();
                return;
            }

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            var prevLabelWidth = EditorGUIUtility.labelWidth;
            var prevFieldWidth = EditorGUIUtility.fieldWidth;
            EditorGUILayout.LabelField("Id", GUILayout.Width(32f));
            EditorGUIUtility.labelWidth = 24f;
            EditorGUIUtility.fieldWidth = 24f;
            EditorGUILayout.LabelField(ControllerInputTypeContent, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(AxisTypeContent, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(ActionContent, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField(string.Empty, GUILayout.Width(24f));
            EditorGUIUtility.labelWidth = prevLabelWidth;
            EditorGUIUtility.fieldWidth = prevFieldWidth;
            GUILayout.EndHorizontal();

            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                SerializedProperty interaction = list.GetArrayElementAtIndex(i);

                EditorGUILayout.LabelField($"{i + 1}", GUILayout.Width(32f));
                EditorGUIUtility.labelWidth = 24f;
                EditorGUIUtility.fieldWidth = 24f;
                var inputType = interaction.FindPropertyRelative("inputType");
                EditorGUILayout.PropertyField(inputType, GUIContent.none, GUILayout.ExpandWidth(true));
                var axisType = interaction.FindPropertyRelative("axisType");
                EditorGUILayout.PropertyField(axisType, GUIContent.none, GUILayout.ExpandWidth(true));
                var action = interaction.FindPropertyRelative("inputAction");
                var actionId = action.FindPropertyRelative("id");
                actionId.intValue = EditorGUILayout.IntPopup(GUIContent.none, CheckValue(actionId.intValue, actionIds.Length), actionLabels, actionIds, GUILayout.ExpandWidth(true));

                if (GUILayout.Button(InteractionMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    list.DeleteArrayElementAtIndex(i);
                }

                EditorGUIUtility.labelWidth = prevLabelWidth;
                EditorGUIUtility.fieldWidth = prevFieldWidth;
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        private static int CheckValue(int value, int against)
        {
            if (value > against)
            {
                value = 0;
            }

            return value;
        }
    }
}