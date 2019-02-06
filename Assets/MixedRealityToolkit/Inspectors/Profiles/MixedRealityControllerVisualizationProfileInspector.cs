// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Extensions;
using Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Providers.UnityInput;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityControllerVisualizationProfile))]
    public class MixedRealityControllerVisualizationProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent ControllerAddButtonContent = new GUIContent("+ Add a New Controller Definition");
        private static readonly GUIContent ControllerMinusButtonContent = new GUIContent("-", "Remove Controller Definition");

        private static readonly GUIContent[] HandednessSelections =
        {
            new GUIContent("Left Hand"),
            new GUIContent("Right Hand"),
        };

        private static bool showVisualizationProperties = true;
        private SerializedProperty renderMotionControllers;
        private SerializedProperty controllerVisualizationType;

        private static bool showModelProperties = true;
        private SerializedProperty useDefaultModels;
        private SerializedProperty globalLeftHandModel;
        private SerializedProperty globalRightHandModel;

        private static bool showControllerDefinitions = true;
        private SerializedProperty controllerVisualizationSettings;

        private MixedRealityControllerVisualizationProfile thisProfile;

        private float defaultLabelWidth;
        private float defaultFieldWidth;

        protected override void OnEnable()
        {
            base.OnEnable();

            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultFieldWidth = EditorGUIUtility.fieldWidth;

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            thisProfile = target as MixedRealityControllerVisualizationProfile;

            renderMotionControllers = serializedObject.FindProperty("renderMotionControllers");
            controllerVisualizationType = serializedObject.FindProperty("controllerVisualizationType");
            useDefaultModels = serializedObject.FindProperty("useDefaultModels");
            globalLeftHandModel = serializedObject.FindProperty("globalLeftHandModel");
            globalRightHandModel = serializedObject.FindProperty("globalRightHandModel");
            controllerVisualizationSettings = serializedObject.FindProperty("controllerVisualizationSettings");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            if (!MixedRealityToolkit.Instance.ActiveProfile.IsInputSystemEnabled)
            {
                EditorGUILayout.HelpBox("No input system is enabled, or you need to specify the type in the main configuration profile.", MessageType.Error);

                if (GUILayout.Button("Back to Configuration Profile"))
                {
                    Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile;
                }

                return;
            }

            if (GUILayout.Button("Back to Input Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Controller Visualizations", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Define all the custom controller visualizations you'd like to use for each controller type when they're rendered in the scene.\n\n" +
                                    "Global settings are the default fallback, and any specific controller definitions take precedence.", MessageType.Info);
            serializedObject.Update();

            CheckProfileLock(target);

            EditorGUIUtility.labelWidth = 168f;

            EditorGUILayout.Space();
            showVisualizationProperties = EditorGUILayout.Foldout(showVisualizationProperties, "Visualization Settings", true);
            if (showVisualizationProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(renderMotionControllers);

                    EditorGUILayout.PropertyField(controllerVisualizationType);

                    if (thisProfile.ControllerVisualizationType == null ||
                        thisProfile.ControllerVisualizationType.Type == null)
                    {
                        EditorGUILayout.HelpBox("A controller visualization type must be defined!", MessageType.Error);
                    }
                }
            }

            var leftHandModelPrefab = globalLeftHandModel.objectReferenceValue as GameObject;
            var rightHandModelPrefab = globalRightHandModel.objectReferenceValue as GameObject;

            EditorGUILayout.Space();
            showModelProperties = EditorGUILayout.Foldout(showModelProperties, "Controller Model Settings", true);
            if (showModelProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(useDefaultModels);

                    if (useDefaultModels.boolValue && (leftHandModelPrefab != null || rightHandModelPrefab != null))
                    {
                        EditorGUILayout.HelpBox("When default models are used, the global left and right hand models will only be used if the default models cannot be loaded from the driver.", MessageType.Warning);
                    }

                    EditorGUI.BeginChangeCheck();
                    leftHandModelPrefab = EditorGUILayout.ObjectField(new GUIContent(globalLeftHandModel.displayName, "Note: If the default model is not found, the fallback is the global left hand model."), leftHandModelPrefab, typeof(GameObject), false) as GameObject;

                    if (EditorGUI.EndChangeCheck() && CheckVisualizer(leftHandModelPrefab))
                    {
                        globalLeftHandModel.objectReferenceValue = leftHandModelPrefab;
                    }

                    EditorGUI.BeginChangeCheck();
                    rightHandModelPrefab = EditorGUILayout.ObjectField(new GUIContent(globalRightHandModel.displayName, "Note: If the default model is not found, the fallback is the global right hand model."), rightHandModelPrefab, typeof(GameObject), false) as GameObject;

                    if (EditorGUI.EndChangeCheck() && CheckVisualizer(rightHandModelPrefab))
                    {
                        globalRightHandModel.objectReferenceValue = rightHandModelPrefab;
                    }
                }
            }

            EditorGUIUtility.labelWidth = defaultLabelWidth;

            EditorGUILayout.Space();
            showControllerDefinitions = EditorGUILayout.Foldout(showControllerDefinitions, "Controller Definitions", true);
            if (showControllerDefinitions)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    RenderControllerList(controllerVisualizationSettings);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderControllerList(SerializedProperty controllerList)
        {
            if (thisProfile.ControllerVisualizationSettings.Length != controllerList.arraySize) { return; }

            EditorGUILayout.Space();

            if (GUILayout.Button(ControllerAddButtonContent, EditorStyles.miniButton))
            {
                controllerList.InsertArrayElementAtIndex(controllerList.arraySize);
                var index = controllerList.arraySize - 1;
                var controllerSetting = controllerList.GetArrayElementAtIndex(index);
                var mixedRealityControllerMappingDescription = controllerSetting.FindPropertyRelative("description");
                mixedRealityControllerMappingDescription.stringValue = typeof(GenericJoystickController).Name;
                var mixedRealityControllerHandedness = controllerSetting.FindPropertyRelative("handedness");
                mixedRealityControllerHandedness.intValue = 1;
                serializedObject.ApplyModifiedProperties();
                thisProfile.ControllerVisualizationSettings[index].ControllerType.Type = typeof(GenericJoystickController);
                return;
            }

            for (int i = 0; i < controllerList.arraySize; i++)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();

                EditorGUIUtility.labelWidth = 64f;
                EditorGUIUtility.fieldWidth = 64f;
                var controllerSetting = controllerList.GetArrayElementAtIndex(i);
                var mixedRealityControllerMappingDescription = controllerSetting.FindPropertyRelative("description");
                bool hasValidType = thisProfile.ControllerVisualizationSettings[i].ControllerType != null &&
                                    thisProfile.ControllerVisualizationSettings[i].ControllerType.Type != null;

                mixedRealityControllerMappingDescription.stringValue = hasValidType
                    ? thisProfile.ControllerVisualizationSettings[i].ControllerType.Type.Name.ToProperCase()
                    : "Undefined Controller";

                serializedObject.ApplyModifiedProperties();
                var mixedRealityControllerHandedness = controllerSetting.FindPropertyRelative("handedness");
                EditorGUILayout.LabelField($"{mixedRealityControllerMappingDescription.stringValue} {((Handedness)mixedRealityControllerHandedness.intValue).ToString().ToProperCase()} Hand");

                EditorGUIUtility.fieldWidth = defaultFieldWidth;
                EditorGUIUtility.labelWidth = defaultLabelWidth;

                if (GUILayout.Button(ControllerMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    controllerList.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(controllerSetting.FindPropertyRelative("controllerType"));

                if (!hasValidType)
                {
                    EditorGUILayout.HelpBox("A controller type must be defined!", MessageType.Error);
                }

                var handednessValue = mixedRealityControllerHandedness.intValue - 1;

                // Reset in case it was set to something other than left or right.
                if (handednessValue < 0 || handednessValue > 1) { handednessValue = 0; }

                EditorGUI.BeginChangeCheck();
                handednessValue = EditorGUILayout.IntPopup(new GUIContent(mixedRealityControllerHandedness.displayName, mixedRealityControllerHandedness.tooltip), handednessValue, HandednessSelections, null);

                if (EditorGUI.EndChangeCheck())
                {
                    mixedRealityControllerHandedness.intValue = handednessValue + 1;
                }

                var overrideModel = controllerSetting.FindPropertyRelative("overrideModel");
                var overrideModelPrefab = overrideModel.objectReferenceValue as GameObject;

                var controllerUseDefaultModelOverride = controllerSetting.FindPropertyRelative("useDefaultModel");
                EditorGUILayout.PropertyField(controllerUseDefaultModelOverride);

                if (controllerUseDefaultModelOverride.boolValue && overrideModelPrefab != null)
                {
                    EditorGUILayout.HelpBox("When default model is used, the override model will only be used if the default model cannot be loaded from the driver.", MessageType.Warning);
                }

                EditorGUI.BeginChangeCheck();
                overrideModelPrefab = EditorGUILayout.ObjectField(new GUIContent(overrideModel.displayName, "If no override model is set, the global model is used."), overrideModelPrefab, typeof(GameObject), false) as GameObject;

                if (EditorGUI.EndChangeCheck() && CheckVisualizer(overrideModelPrefab))
                {
                    overrideModel.objectReferenceValue = overrideModelPrefab;
                }

                EditorGUI.indentLevel--;
            }
        }

        private bool CheckVisualizer(GameObject modelPrefab)
        {
            if (modelPrefab == null) { return true; }

            if (PrefabUtility.GetPrefabAssetType(modelPrefab) == PrefabAssetType.NotAPrefab)
            {
                Debug.LogWarning("Assigned GameObject must be a prefab.");
                return false;
            }

            var componentList = modelPrefab.GetComponentsInChildren<IMixedRealityControllerVisualizer>();

            if (componentList == null || componentList.Length == 0)
            {
                if (thisProfile.ControllerVisualizationType != null &&
                    thisProfile.ControllerVisualizationType.Type != null)
                {
                    modelPrefab.AddComponent(thisProfile.ControllerVisualizationType.Type);
                    return true;
                }

                Debug.LogError("No controller visualization type specified!");
            }
            else if (componentList.Length == 1)
            {
                return true;
            }
            else if (componentList.Length > 1)
            {
                Debug.LogWarning("Found too many IMixedRealityControllerVisualizer components on your prefab. There can only be one.");
            }

            return false;
        }
    }
}