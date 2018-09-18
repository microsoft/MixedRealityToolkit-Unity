// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityControllerVisualizationProfile))]
    public class MixedRealityControllerVisualizationProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private const string ModelWarningText = "The Controller model you've specified is missing a IMixedRealityControllerPoseSynchronizer component. Without it the model will not synchronize it's pose with the controller data. Would you like to add one now?";

        private SerializedProperty renderMotionControllers;
        private SerializedProperty useDefaultModels;
        private SerializedProperty globalLeftHandModel;
        private SerializedProperty globalRightHandModel;
        private float defaultLabelWidth;

        private void OnEnable()
        {
            defaultLabelWidth = EditorGUIUtility.labelWidth;

            if (!CheckMixedRealityManager(false))
            {
                return;
            }

            renderMotionControllers = serializedObject.FindProperty("renderMotionControllers");
            useDefaultModels = serializedObject.FindProperty("useDefaultModels");
            globalLeftHandModel = serializedObject.FindProperty("globalLeftHandModel");
            globalRightHandModel = serializedObject.FindProperty("globalRightHandModel");
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
            EditorGUILayout.HelpBox("Define all the custom controller visualizations you'd like to use for each controller type", MessageType.Info);
            serializedObject.Update();

            EditorGUIUtility.labelWidth = 152f;
            EditorGUILayout.PropertyField(renderMotionControllers);

            if (renderMotionControllers.boolValue)
            {
                EditorGUILayout.PropertyField(useDefaultModels);

                if (!useDefaultModels.boolValue)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(globalLeftHandModel);
                    EditorGUILayout.PropertyField(globalRightHandModel);

                    if (EditorGUI.EndChangeCheck())
                    {
                        CheckSynchronizer((GameObject)globalLeftHandModel.objectReferenceValue);
                        CheckSynchronizer((GameObject)globalRightHandModel.objectReferenceValue);
                    }
                }
            }

            EditorGUIUtility.labelWidth = defaultLabelWidth;

            serializedObject.ApplyModifiedProperties();
        }

        private static void CheckSynchronizer(GameObject modelPrefab)
        {
            if (modelPrefab == null) { return; }

            var list = modelPrefab.GetComponentsInChildren<IMixedRealityControllerPoseSynchronizer>();

            if (list == null || list.Length == 0)
            {
                if (EditorUtility.DisplayDialog("Warning!", ModelWarningText, "Add Component", "I'll do it Later"))
                {
                    EditorGUIUtility.PingObject(modelPrefab);
                    Selection.activeObject = modelPrefab;
                }
            }
        }
    }
}