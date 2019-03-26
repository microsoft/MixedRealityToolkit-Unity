// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEngine;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness.Editor
{
    [CustomEditor(typeof(MixedRealitySpatialAwarenessSystemProfile))]
    public class MixedRealitySpatialAwarenessSystemProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent AddObserverContent = new GUIContent("+ Add Spatial Observer", "Add Spatial Observer");
        private static readonly GUIContent RemoveObserverContent = new GUIContent("-", "Remove Spatial Observer");

        private static bool showObservers = true;
        private SerializedProperty observerConfigurations;

        private bool[] observerFoldouts;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            observerConfigurations = serializedObject.FindProperty("observerConfigurations");

            observerFoldouts = new bool[observerConfigurations.arraySize];
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            // todo: find good way to update...
            if (DrawBacktrackProfileButton("Back to Configuration Profile", MixedRealityToolkit.Instance.ActiveProfile))
            {
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Spatial Awareness System Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Spatial Awareness System profile allows developers to configure cross-platform environmental awareness.", MessageType.Info);

            CheckProfileLock(target);

            serializedObject.Update();

            EditorGUILayout.Space();
            showObservers = EditorGUILayout.Foldout(showObservers, "Observers", true);
            if (showObservers)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    RenderList(observerConfigurations);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void RenderList(SerializedProperty list)
        {
            EditorGUILayout.Space();

            bool changed = false;

            using (new EditorGUILayout.VerticalScope())
            {
                if (GUILayout.Button(AddObserverContent, EditorStyles.miniButton))
                {
                    list.InsertArrayElementAtIndex(list.arraySize);
                    SerializedProperty observer = list.GetArrayElementAtIndex(list.arraySize - 1);

                    SerializedProperty providerName = observer.FindPropertyRelative("componentName");
                    providerName.stringValue = $"New spatial observer {list.arraySize - 1}";

                    SerializedProperty runtimePlatform = observer.FindPropertyRelative("runtimePlatform");
                    runtimePlatform.intValue = -1;

                    // todo - profile

                    serializedObject.ApplyModifiedProperties();

                    // todo
                    //Utilities.SystemType providerType = ((MixedRealityInputSystemProfile)serializedObject.targetObject).DataProviderConfigurations[list.arraySize - 1].ComponentType;
                    //providerType.Type = null;

                    observerFoldouts = new bool[list.arraySize];
                    return;
                }

                GUILayout.Space(12f);

                if (list == null || list.arraySize == 0)
                {
                    EditorGUILayout.HelpBox("The Mixed Reality Spatial Awareness System requires one or more observers.", MessageType.Warning);
                    return;
                }

                for (int i = 0; i < list.arraySize; i++)
                {
                    SerializedProperty observer = list.GetArrayElementAtIndex(i);
                    SerializedProperty observerName = observer.FindPropertyRelative("componentName");
                    SerializedProperty observerType = observer.FindPropertyRelative("componentType");
                    SerializedProperty runtimePlatform = observer.FindPropertyRelative("runtimePlatform");
                    // todo: profile

                    using (new EditorGUILayout.VerticalScope())
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            observerFoldouts[i] = EditorGUILayout.Foldout(observerFoldouts[i], observerName.stringValue, true);

                            if (GUILayout.Button(RemoveObserverContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                            {
                                list.DeleteArrayElementAtIndex(i);
                                serializedObject.ApplyModifiedProperties();
                                changed = true;
                                break;
                            }

                        }

                        if (observerFoldouts[i] || RenderAsSubProfile)
                        {
                            using (new EditorGUI.IndentLevelScope())
                            {
                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.PropertyField(observerType);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    serializedObject.ApplyModifiedProperties();
                                    // todo
                                    //System.Type type = ((MixedRealityInputSystemProfile)serializedObject.targetObject).DataProviderConfigurations[i].ComponentType.Type;
                                    //ApplyDataProviderConfiguration(type, providerName, runtimePlatform);
                                }

                                EditorGUI.BeginChangeCheck();
                                EditorGUILayout.PropertyField(runtimePlatform);
                                changed |= EditorGUI.EndChangeCheck();
                            }

                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                }

                if (changed)
                {
                    // todo: find different way to handle...
                    // EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(MixedRealityToolkit.Instance.ActiveProfile);
                }
            }
        }
    }
}