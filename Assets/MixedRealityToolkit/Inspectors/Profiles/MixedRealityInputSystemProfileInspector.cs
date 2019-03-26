// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(MixedRealityInputSystemProfile))]
    public class MixedRealityInputSystemProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent AddProviderContent = new GUIContent("+ Add Data Provider", "Add Data Provider");
        private static readonly GUIContent RemoveProviderContent = new GUIContent("-", "Remove Data Provider");

        private static bool showDataProviders = true;
        private SerializedProperty dataProviderTypes;

        private static bool showFocusProperties = true;
        private SerializedProperty focusProviderType;

        private static bool showPointerProperties = true;
        private SerializedProperty pointerProfile;

        private static bool showActionsProperties = true;
        private SerializedProperty inputActionsProfile;
        private SerializedProperty inputActionRulesProfile;

        private static bool showControllerProperties = true;
        private SerializedProperty enableControllerMapping;
        private SerializedProperty controllerMappingProfile;
        private SerializedProperty controllerVisualizationProfile;

        private static bool showGestureProperties = true;
        private SerializedProperty gesturesProfile;

        private static bool showSpeechCommandsProperties = true;
        private SerializedProperty speechCommandsProfile;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            dataProviderTypes = serializedObject.FindProperty("dataProviderTypes");
            focusProviderType = serializedObject.FindProperty("focusProviderType");
            inputActionsProfile = serializedObject.FindProperty("inputActionsProfile");
            inputActionRulesProfile = serializedObject.FindProperty("inputActionRulesProfile");
            pointerProfile = serializedObject.FindProperty("pointerProfile");
            gesturesProfile = serializedObject.FindProperty("gesturesProfile");
            speechCommandsProfile = serializedObject.FindProperty("speechCommandsProfile");
            controllerMappingProfile = serializedObject.FindProperty("controllerMappingProfile");
            enableControllerMapping = serializedObject.FindProperty("enableControllerMapping");
            controllerVisualizationProfile = serializedObject.FindProperty("controllerVisualizationProfile");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            if (DrawBacktrackProfileButton("Back to Configuration Profile", MixedRealityToolkit.Instance.ActiveProfile))
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Input System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Input System Profile helps developers configure input no matter what platform you're building for.", MessageType.Info);

            CheckProfileLock(target);

            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 160f;

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            bool changed = false;

            EditorGUILayout.Space();
            showDataProviders = EditorGUILayout.Foldout(showDataProviders, "Data Providers", true);
            if (showDataProviders)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    RenderList(dataProviderTypes);
                }
            }

            EditorGUILayout.Space();
            showFocusProperties = EditorGUILayout.Foldout(showFocusProperties, "Focus Settings", true);
            if (showFocusProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(focusProviderType);
                }
            }

            EditorGUILayout.Space();
            showPointerProperties = EditorGUILayout.Foldout(showPointerProperties, "Pointer Settings", true);
            if (showPointerProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(pointerProfile);
                }
            }

            EditorGUILayout.Space();
            showActionsProperties = EditorGUILayout.Foldout(showActionsProperties, "Action Settings", true);
            if (showActionsProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(inputActionsProfile);
                    changed |= RenderProfile(inputActionRulesProfile);
                }
            }

            EditorGUILayout.Space();
            showControllerProperties = EditorGUILayout.Foldout(showControllerProperties, "Controller Settings", true);
            if (showControllerProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(enableControllerMapping);
                    changed |= RenderProfile(controllerMappingProfile);
                    changed |= RenderProfile(controllerVisualizationProfile, true, typeof(IMixedRealityControllerVisualizer));
                }
            }

            EditorGUILayout.Space();
            showGestureProperties = EditorGUILayout.Foldout(showGestureProperties, "Gesture Settings", true);
            if (showGestureProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(gesturesProfile);
                }
            }

            EditorGUILayout.Space();
            showSpeechCommandsProperties = EditorGUILayout.Foldout(showSpeechCommandsProperties, "Speech Command Settings", true);
            if (showSpeechCommandsProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(speechCommandsProfile);
                }
            }

            if (!changed)
            {
                changed |= EditorGUI.EndChangeCheck();
            }

            EditorGUIUtility.labelWidth = previousLabelWidth;
            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
            }
        }

        private static void RenderList(SerializedProperty list)
        {
            EditorGUILayout.Space();
            using (new EditorGUILayout.VerticalScope())
            {
                if (GUILayout.Button(AddProviderContent, EditorStyles.miniButton))
                {
                    list.arraySize += 1;
                    SerializedProperty dataProvider = list.GetArrayElementAtIndex(list.arraySize - 1);
                }

                GUILayout.Space(12f);

                if (list == null || list.arraySize == 0)
                {
                    EditorGUILayout.HelpBox("The Mixed Reality Input System requires one or more data providers.", MessageType.Warning);
                    return;
                }

                for (int i = 0; i < list.arraySize; i++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {

                        // TODO: need to display this with name(?), priority and type (NO profile)
                        SerializedProperty dataProvider = list.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(dataProvider, GUIContent.none);

                        if (GUILayout.Button(RemoveProviderContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                        {
                            list.DeleteArrayElementAtIndex(i);
                        }
                    }
                }
            }
        }
    }
}