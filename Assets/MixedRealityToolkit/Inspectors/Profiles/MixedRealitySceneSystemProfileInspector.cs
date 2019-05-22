// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information. 

using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealitySceneSystemProfile))]
    public class MixedRealitySceneSystemProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        const float DragAreaWidth = 0f;
        const float DragAreaHeight = 30f;
        const float DragAreaOffset = 10;

        private static string managerSceneContent = 
            "The Manager scene is loaded first and remains loaded for the duration of the app. Only one Manager scene is ever loaded, and no scene operation will ever unload it.";

        private static string lightingSceneContent =
            "The Lighting scene controls lighting settings such as ambient light, skybox and sun direction. A Lighting scene's content is restricted to lights, light probes and reflection probes. A default lighting scene is loaded on initialization. Only one lighting scene will ever be loaded at a time.";

        private static string contentSceneContent =
            "Content scenes are everything else. You can load and unload any number of content scenes in any combination, and their content is unrestricted.";

        private static bool showEditorProperties = true;
        private SerializedProperty editorManageBuildSettings;
        private SerializedProperty editorManagerLoadedScenes;
        private SerializedProperty editorEnforceSceneOrder;
        private SerializedProperty editorEnforceLightingSceneTypes;

        private static bool showManagerProperties = true;
        private SerializedProperty useManagerScene;
        private SerializedProperty managerScene;

        private static bool showContentProperties = true;
        private SerializedProperty contentScenes;

        private static bool showLightingProperties = true;
        private SerializedProperty useLightingScene;
        private SerializedProperty defaultLightingSceneIndex;
        private SerializedProperty lightingScenes;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityToolkit.IsInitialized)
            {
                return;
            }

            editorManageBuildSettings = serializedObject.FindProperty("editorManageBuildSettings");
            editorManagerLoadedScenes = serializedObject.FindProperty("editorManagerLoadedScenes");
            editorEnforceSceneOrder = serializedObject.FindProperty("editorEnforceSceneOrder");
            editorEnforceLightingSceneTypes = serializedObject.FindProperty("editorEnforceLightingSceneTypes");

            useManagerScene = serializedObject.FindProperty("useManagerScene");
            managerScene = serializedObject.FindProperty("managerScene");

            useLightingScene = serializedObject.FindProperty("useLightingScene");
            defaultLightingSceneIndex = serializedObject.FindProperty("defaultLightingSceneIndex");
            lightingScenes = serializedObject.FindProperty("lightingScenes");

            contentScenes = serializedObject.FindProperty("contentScenes");
        }

        private void OnSelecteContentScene(ReorderableList list)
        {
            Debug.Log("Selecting");
        }

        private void DrawContentSceneElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SceneInfoDrawer.DrawProperty(rect, contentScenes.GetArrayElementAtIndex(index), GUIContent.none, isActive, isFocused);
        }

        public override void OnInspectorGUI()
        {
            if (!MixedRealityToolkit.IsInitialized)
            {
                return;
            }

            RenderMRTKLogo();

            MixedRealityInspectorUtility.CheckMixedRealityConfigured(true);

            serializedObject.Update();

            EditorGUILayout.Space();

            MixedRealitySceneSystemProfile profile = (MixedRealitySceneSystemProfile)target;

            EditorGUILayout.Space();
            showEditorProperties = EditorGUILayout.Foldout(showEditorProperties, "Editor Settings", true);
            if (showEditorProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(editorManageBuildSettings);
                    EditorGUILayout.PropertyField(editorManagerLoadedScenes);
                    EditorGUILayout.PropertyField(editorEnforceSceneOrder);
                    EditorGUILayout.PropertyField(editorEnforceLightingSceneTypes);
                    EditorGUILayout.Space();
                }
            }

            showManagerProperties = EditorGUILayout.Foldout(showManagerProperties, "Manager Scene Settings", true);

            if (showManagerProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.HelpBox(managerSceneContent, MessageType.Info);

                    // Disable the tag field since we're drawing manager scenes
                    SceneInfoDrawer.DrawTagProperty = false;
                    EditorGUILayout.PropertyField(useManagerScene);

                    if (useManagerScene.boolValue && profile.ManagerScene.IsEmpty && !Application.isPlaying)
                    {
                        EditorGUILayout.HelpBox("You haven't created a manager scene yet. Click the button below to create one.", MessageType.Warning);
                        var buttonRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(new GUILayoutOption[] { }));
                        if (GUI.Button(buttonRect, "Create Manager Scene", EditorStyles.miniButton))
                        {
                            // Create a new manager scene and add it to build settings
                            SceneInfo newManagerScene = EditorSceneUtils.CreateAndSaveScene("ManagerScene");
                            SerializedObjectUtils.SetStructValue<SceneInfo>(managerScene, newManagerScene);
                            EditorSceneUtils.AddSceneToBuildSettings(newManagerScene, EditorBuildSettings.scenes, EditorSceneUtils.BuildIndexTarget.First);
                        }
                        EditorGUILayout.Space();
                    }

                    if (useManagerScene.boolValue)
                    {
                        EditorGUILayout.PropertyField(managerScene, includeChildren: true);
                    }
                }
            }

            EditorGUILayout.Space();
            showLightingProperties = EditorGUILayout.Foldout(showLightingProperties, "Lighting Scene Settings", true);
            if (showLightingProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.HelpBox(lightingSceneContent, MessageType.Info);

                    EditorGUILayout.PropertyField(useLightingScene);

                    if (useLightingScene.boolValue && profile.NumLightingScenes < 1 && !Application.isPlaying)
                    {
                        EditorGUILayout.HelpBox("You haven't created a lighting scene yet. Click the button below to create one.", MessageType.Warning);
                        var buttonRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(new GUILayoutOption[] { }));
                        if (GUI.Button(buttonRect, "Create Lighting Scene", EditorStyles.miniButton))
                        {
                            // Create a new lighting scene and add it to build settings
                            SceneInfo newLightingScene = EditorSceneUtils.CreateAndSaveScene("LightingScene");
                            // Create an element in the array
                            lightingScenes.arraySize = 1;
                            serializedObject.ApplyModifiedProperties();
                            SerializedObjectUtils.SetStructValue<SceneInfo>(lightingScenes.GetArrayElementAtIndex(0), newLightingScene);
                            EditorSceneUtils.AddSceneToBuildSettings(newLightingScene, EditorBuildSettings.scenes, EditorSceneUtils.BuildIndexTarget.Last);
                        }
                        EditorGUILayout.Space();
                    }

                    if (useLightingScene.boolValue)
                    {
                        // Disable the tag field since we're drawing lighting scenes
                        SceneInfoDrawer.DrawTagProperty = false;

                        if (profile.NumLightingScenes > 0)
                        {
                            string[] lightingSceneNames = profile.LightingScenes.Select(l => l.Name).ToArray<string>();
                            defaultLightingSceneIndex.intValue = EditorGUILayout.Popup("Default Lighting Scene", defaultLightingSceneIndex.intValue, lightingSceneNames);

                        }

                        EditorGUILayout.PropertyField(lightingScenes, includeChildren: true);
                        //DrawSceneInfoDragAndDrop(lightingScenes);
                    }
                }
            }

            EditorGUILayout.Space();
            showContentProperties = EditorGUILayout.Foldout(showContentProperties, "Content Scene Settings", true);
            if (showContentProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.HelpBox(contentSceneContent, MessageType.Info);

                    // Enable the tag field since we're drawing content scenes
                    SceneInfoDrawer.DrawTagProperty = true;
                    EditorGUILayout.PropertyField(contentScenes, includeChildren: true);
                    //DrawSceneInfoDragAndDrop(contentScenes);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSceneInfoDragAndDrop(SerializedProperty arrayProperty)
        {
            if (!Application.isPlaying)
            {
                Rect dropArea = GUILayoutUtility.GetRect(DragAreaWidth, DragAreaHeight, GUILayout.ExpandWidth(true));
                dropArea.width -= DragAreaOffset * 2;
                dropArea.x += DragAreaOffset;
                GUI.Box(dropArea, "Drag-and-drop new " + arrayProperty.displayName, EditorStyles.helpBox);

                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        if (!dropArea.Contains(Event.current.mousePosition))
                        {
                            break;
                        }

                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        if (Event.current.type == EventType.DragPerform)
                        {
                            SerializedProperty arrayElement = null;
                            SerializedProperty assetProperty = null;

                            DragAndDrop.AcceptDrag();
                            foreach (Object draggedObject in DragAndDrop.objectReferences)
                            {
                                if (draggedObject.GetType() != typeof(SceneAsset))
                                {   // Skip anything that isn't a scene asset
                                    continue;
                                }

                                bool isDuplicate = false;
                                for (int i = 0; i < arrayProperty.arraySize; i++)
                                {
                                    arrayElement = arrayProperty.GetArrayElementAtIndex(i);
                                    assetProperty = arrayElement.FindPropertyRelative("Asset");
                                    if (assetProperty.objectReferenceValue != null && assetProperty.objectReferenceValue == draggedObject)
                                    {   
                                        isDuplicate = true;
                                        break;
                                    }
                                }

                                if (isDuplicate)
                                {   // Skip any duplicates
                                    Debug.LogWarning("Skipping " + draggedObject.name + " - it's already in the " + arrayProperty.displayName + " list.");
                                    continue;
                                }
                                
                                // Create the new element at 0
                                arrayProperty.InsertArrayElementAtIndex(0);
                                arrayProperty.serializedObject.ApplyModifiedProperties();

                                // Get the new element and assign the dragged object
                                arrayElement = arrayProperty.GetArrayElementAtIndex(0);
                                assetProperty = arrayElement.FindPropertyRelative("Asset");
                                assetProperty.objectReferenceValue = draggedObject;
                                arrayProperty.serializedObject.ApplyModifiedProperties();

                                // Move the new element to end of list
                                arrayProperty.MoveArrayElement(0, arrayProperty.arraySize - 1);
                                arrayProperty.serializedObject.ApplyModifiedProperties();
                            }
                        }
                        break;
                }
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            return MixedRealityToolkit.IsInitialized
                && MixedRealityToolkit.Instance.HasActiveProfile
                && MixedRealityToolkit.Instance.ActiveProfile.CameraProfile == this;
        }
    }
}
