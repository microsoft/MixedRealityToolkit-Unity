// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Visualization;
using Microsoft.MixedReality.Toolkit.Core.Devices.UnityInput;
using Microsoft.MixedReality.Toolkit.Core.Extensions;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityVisualizationProfile))]
    public class MixedRealityVisualizationProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent ControllerAddButtonContent = new GUIContent("+ Add a New Controller Definition");
        private static readonly GUIContent ControllerMinusButtonContent = new GUIContent("-", "Remove Controller Definition");

        private static readonly GUIContent[] HandednessSelections =
        {
            new GUIContent("Left Hand"),
            new GUIContent("Right Hand"),
        };

        private SerializedProperty renderMotionControllers;
        private SerializedProperty visualizationType;
        private SerializedProperty useDefaultModels;
        private SerializedProperty globalLeftHandModel;
        private SerializedProperty globalRightHandModel;
        private SerializedProperty visualizationSettings;

        private MixedRealityVisualizationProfile thisProfile;

        private float defaultLabelWidth;
        private float defaultFieldWidth;

        private void OnEnable()
        {
            defaultLabelWidth = EditorGUIUtility.labelWidth;
            defaultFieldWidth = EditorGUIUtility.fieldWidth;

            if (!CheckMixedRealityManager(false))
            {
                return;
            }

            thisProfile = target as MixedRealityVisualizationProfile;

            renderMotionControllers = serializedObject.FindProperty("renderMotionControllers");
            visualizationType = serializedObject.FindProperty("visualizationType");
            useDefaultModels = serializedObject.FindProperty("useDefaultModels");
            globalLeftHandModel = serializedObject.FindProperty("globalLeftHandModel");
            globalRightHandModel = serializedObject.FindProperty("globalRightHandModel");
            visualizationSettings = serializedObject.FindProperty("visualizationSettings");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!CheckMixedRealityManager())
            {
                return;
            }

            if (!MixedRealityManager.Instance.ActiveProfile.IsInputSystemEnabled)
            {
                EditorGUILayout.HelpBox("No input system is enabled, or you need to specify the type in the main configuration profile.", MessageType.Error);

                if (GUILayout.Button("Back to Configuration Profile"))
                {
                    Selection.activeObject = MixedRealityManager.Instance.ActiveProfile;
                }

                return;
            }

            if (GUILayout.Button("Back to Input Profile"))
            {
                Selection.activeObject = MixedRealityManager.Instance.ActiveProfile.InputSystemProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Controller Visualizations", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Define all the custom controller visualizations you'd like to use for each controller type when they're rendered in the scene.", MessageType.Info);
            serializedObject.Update();

            EditorGUIUtility.labelWidth = 168f;
            EditorGUILayout.PropertyField(renderMotionControllers);

            if (renderMotionControllers.boolValue)
            {
                EditorGUILayout.PropertyField(visualizationType);
                EditorGUILayout.PropertyField(useDefaultModels);

                EditorGUI.BeginChangeCheck();
                var leftHandModelPrefab = globalLeftHandModel.objectReferenceValue as GameObject;
                leftHandModelPrefab = EditorGUILayout.ObjectField(new GUIContent(globalLeftHandModel.displayName, "Note: If the default model is not found, the fallback is the global left hand model."), leftHandModelPrefab, typeof(GameObject), false) as GameObject;

                if (EditorGUI.EndChangeCheck() && CheckVisualizer(leftHandModelPrefab))
                {
                    globalLeftHandModel.objectReferenceValue = leftHandModelPrefab;
                }

                var rightHandModelPrefab = globalRightHandModel.objectReferenceValue as GameObject;
                EditorGUI.BeginChangeCheck();
                rightHandModelPrefab = EditorGUILayout.ObjectField(new GUIContent(globalRightHandModel.displayName, "Note: If the default model is not found, the fallback is the global right hand model."), rightHandModelPrefab, typeof(GameObject), false) as GameObject;

                if (EditorGUI.EndChangeCheck() && CheckVisualizer(rightHandModelPrefab))
                {
                    globalRightHandModel.objectReferenceValue = rightHandModelPrefab;
                }

                EditorGUIUtility.labelWidth = defaultLabelWidth;

                RenderControllerList(visualizationSettings);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderControllerList(SerializedProperty controllerList)
        {
            if (thisProfile.VisualizationSettings.Length != controllerList.arraySize) { return; }

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
                thisProfile.VisualizationSettings[index].ControllerType.Type = typeof(GenericJoystickController);
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
                mixedRealityControllerMappingDescription.stringValue = thisProfile.VisualizationSettings[i].ControllerType.Type.Name.ToProperCase();
                serializedObject.ApplyModifiedProperties();
                var mixedRealityControllerHandedness = controllerSetting.FindPropertyRelative("handedness");
                EditorGUILayout.LabelField($"{mixedRealityControllerMappingDescription.stringValue.ToProperCase()} {((Handedness)mixedRealityControllerHandedness.intValue).ToString().ToProperCase()} Hand");

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

                var handednessValue = mixedRealityControllerHandedness.intValue - 1;

                // Reset in case it was set to something other than left or right.
                if (handednessValue < 0 || handednessValue > 1) { handednessValue = 0; }

                EditorGUI.BeginChangeCheck();
                handednessValue = EditorGUILayout.IntPopup(new GUIContent(mixedRealityControllerHandedness.displayName, mixedRealityControllerHandedness.tooltip), handednessValue, HandednessSelections, null);

                if (EditorGUI.EndChangeCheck())
                {
                    mixedRealityControllerHandedness.intValue = handednessValue + 1;
                }

                EditorGUILayout.PropertyField(controllerSetting.FindPropertyRelative("useDefaultModel"));

                var overrideModel = controllerSetting.FindPropertyRelative("overrideModel");

                var overrideModelPrefab = overrideModel.objectReferenceValue as GameObject;
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

            if (PrefabUtility.GetPrefabType(modelPrefab) != PrefabType.Prefab)
            {
                Debug.LogWarning("Assigned GameObject must be a prefab");
                return false;
            }

            var componentList = modelPrefab.GetComponentsInChildren<IMixedRealityVisualizer>();

            if (componentList == null || componentList.Length == 0)
            {
                if (thisProfile.VisualizationType != null &&
                    thisProfile.VisualizationType.Type != null)
                {
                    modelPrefab.AddComponent(thisProfile.VisualizationType.Type);
                }
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