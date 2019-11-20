// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using UnityEditor;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Editor;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
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

        private SerializedProperty renderMotionControllers;
        private SerializedProperty defaultControllerVisualizationType;

        private SerializedProperty useDefaultModels;
        private SerializedProperty globalLeftHandedControllerModel;
        private SerializedProperty globalRightHandedControllerModel;
        private SerializedProperty globalLeftHandModel;
        private SerializedProperty globalRightHandModel;

        private static bool showControllerDefinitions = true;
        private SerializedProperty controllerVisualizationSettings;

        private MixedRealityControllerVisualizationProfile thisProfile;

        private float defaultLabelWidth;
        private float defaultFieldWidth;

        private const string ProfileTitle = "Controller Visualization Settings";
        private const string ProfileDescription = "Define all the custom controller visualizations you'd like to use for each controller type when they're rendered in the scene.\n\n" +
                                    "Global settings are the default fallback, and any specific controller definitions take precedence.";

        protected override void OnEnable()
        {
            base.OnEnable();

            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultFieldWidth = EditorGUIUtility.fieldWidth;

            thisProfile = target as MixedRealityControllerVisualizationProfile;

            renderMotionControllers = serializedObject.FindProperty("renderMotionControllers");
            defaultControllerVisualizationType = serializedObject.FindProperty("defaultControllerVisualizationType");
            useDefaultModels = serializedObject.FindProperty("useDefaultModels");
            globalLeftHandedControllerModel = serializedObject.FindProperty("globalLeftControllerModel");
            globalRightHandedControllerModel = serializedObject.FindProperty("globalRightControllerModel");
            globalLeftHandModel = serializedObject.FindProperty("globalLeftHandVisualizer");
            globalRightHandModel = serializedObject.FindProperty("globalRightHandVisualizer");
            controllerVisualizationSettings = serializedObject.FindProperty("controllerVisualizationSettings");
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.Input);

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                EditorGUILayout.LabelField("Visualization Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(renderMotionControllers);

                    EditorGUILayout.PropertyField(defaultControllerVisualizationType);

                    if (thisProfile.DefaultControllerVisualizationType == null ||
                        thisProfile.DefaultControllerVisualizationType.Type == null)
                    {
                        EditorGUILayout.HelpBox("A default controller visualization type must be defined!", MessageType.Error);
                    }
                }

                var leftHandModelPrefab = globalLeftHandedControllerModel.objectReferenceValue as GameObject;
                var rightHandModelPrefab = globalRightHandedControllerModel.objectReferenceValue as GameObject;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Controller Model Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(useDefaultModels);

                    if (useDefaultModels.boolValue && (leftHandModelPrefab != null || rightHandModelPrefab != null))
                    {
                        EditorGUILayout.HelpBox("When default models are used, an attempt is made to obtain controller models from the platform sdk. The global left and right models are only shown if no model can be obtained.", MessageType.Warning);
                    }

                    EditorGUI.BeginChangeCheck();
                    leftHandModelPrefab = EditorGUILayout.ObjectField(new GUIContent(globalLeftHandedControllerModel.displayName, "Note: If the default model is not found, the fallback is the global left hand model."), leftHandModelPrefab, typeof(GameObject), false) as GameObject;

                    if (EditorGUI.EndChangeCheck() && CheckVisualizer(leftHandModelPrefab))
                    {
                        globalLeftHandedControllerModel.objectReferenceValue = leftHandModelPrefab;
                    }

                    EditorGUI.BeginChangeCheck();
                    rightHandModelPrefab = EditorGUILayout.ObjectField(new GUIContent(globalRightHandedControllerModel.displayName, "Note: If the default model is not found, the fallback is the global right hand model."), rightHandModelPrefab, typeof(GameObject), false) as GameObject;

                    if (EditorGUI.EndChangeCheck() && CheckVisualizer(rightHandModelPrefab))
                    {
                        globalRightHandedControllerModel.objectReferenceValue = rightHandModelPrefab;
                    }

                    EditorGUILayout.PropertyField(globalLeftHandModel);
                    EditorGUILayout.PropertyField(globalRightHandModel);
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
        }
        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile != null &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.ControllerVisualizationProfile;
        }

        private void RenderControllerList(SerializedProperty controllerList)
        {
            if (thisProfile.ControllerVisualizationSettings.Length != controllerList.arraySize) { return; }

            EditorGUILayout.Space();

            if (InspectorUIUtility.RenderIndentedButton(ControllerAddButtonContent, EditorStyles.miniButton))
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

                var controllerSetting = controllerList.GetArrayElementAtIndex(i);
                var mixedRealityControllerMappingDescription = controllerSetting.FindPropertyRelative("description");
                bool hasValidType = thisProfile.ControllerVisualizationSettings[i].ControllerType != null &&
                                    thisProfile.ControllerVisualizationSettings[i].ControllerType.Type != null;

                mixedRealityControllerMappingDescription.stringValue = hasValidType
                    ? thisProfile.ControllerVisualizationSettings[i].ControllerType.Type.Name.ToProperCase()
                    : "Undefined Controller";

                serializedObject.ApplyModifiedProperties();
                var mixedRealityControllerHandedness = controllerSetting.FindPropertyRelative("handedness");
                EditorGUILayout.LabelField($"{mixedRealityControllerMappingDescription.stringValue} {((Handedness)mixedRealityControllerHandedness.intValue).ToString().ToProperCase()} Hand", EditorStyles.boldLabel);

                if (GUILayout.Button(ControllerMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    controllerList.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(controllerSetting.FindPropertyRelative("controllerType"));
                EditorGUILayout.PropertyField(controllerSetting.FindPropertyRelative("controllerVisualizationType"));

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
                if (thisProfile.DefaultControllerVisualizationType != null &&
                    thisProfile.DefaultControllerVisualizationType.Type != null)
                {
                    modelPrefab.AddComponent(thisProfile.DefaultControllerVisualizationType.Type);
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