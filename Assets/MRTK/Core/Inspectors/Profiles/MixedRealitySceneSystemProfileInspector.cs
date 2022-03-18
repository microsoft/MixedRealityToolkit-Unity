// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. 

using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealitySceneSystemProfile))]
    public class MixedRealitySceneSystemProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        const float DragAreaWidth = 0;
        const float DragAreaHeight = 30;
        const float DragAreaOffset = 10;
        const float LightingSceneTypesLabelWidth = 45;

        private const string defaultSceneContent =
            "Default scene system resources were not found.\nIf using custom manager and lighting scenes, this message can be ignored.\nIf not, please see the documentation for more information";

        private const string managerSceneContent =
            "The manager scene is loaded first and remains loaded for the duration of the app. Only one manager scene is ever loaded, and no scene operation will ever unload it.";

        private const string lightingSceneContent =
            "The lighting scene controls lighting settings such as ambient light, skybox and sun direction. A lighting scene's content is restricted based on the types defined in your editor settings. A default lighting scene is loaded on initialization. Only one lighting scene will ever be loaded at a time.";

        private const string contentSceneContent =
            "Content scenes are everything else. You can load and unload any number of content scenes in any combination, and their content is unrestricted.";

        private static bool showEditorProperties = true;
        private const string ShowSceneSystem_Editor_PreferenceKey = "ShowSceneSystem_Editor_PreferenceKey";
        private SerializedProperty editorManageBuildSettings;
        private SerializedProperty editorManageLoadedScenes;
        private SerializedProperty editorEnforceSceneOrder;
        private SerializedProperty editorEnforceLightingSceneTypes;
        private SerializedProperty permittedLightingSceneComponentTypes;

        private static bool showManagerProperties = true;
        private const string ShowSceneSystem_Manager_PreferenceKey = "ShowSceneSystem_Manager_PreferenceKey";
        private SerializedProperty useManagerScene;
        private SerializedProperty managerScene;

        private static bool showContentProperties = true;
        private const string ShowSceneSystem_Content_PreferenceKey = "ShowSceneSystem_Content_PreferenceKey";
        private SerializedProperty contentScenes;

        private static bool showLightingProperties = true;
        private const string ShowSceneSystem_Lighting_PreferenceKey = "ShowSceneSystem_Lighting_PreferenceKey";
        private SerializedProperty useLightingScene;
        private SerializedProperty defaultLightingSceneIndex;
        private SerializedProperty lightingScenes;

        private const string ProfileTitle = "Scene System Settings";
        private const string ProfileDescription = "The Scene System helps developers manage additive scene loading at runtime and in editor.";


        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityToolkit.IsInitialized)
            {
                return;
            }

            editorManageBuildSettings = serializedObject.FindProperty("editorManageBuildSettings");
            editorManageLoadedScenes = serializedObject.FindProperty("editorManageLoadedScenes");
            editorEnforceSceneOrder = serializedObject.FindProperty("editorEnforceSceneOrder");
            editorEnforceLightingSceneTypes = serializedObject.FindProperty("editorEnforceLightingSceneTypes");
            permittedLightingSceneComponentTypes = serializedObject.FindProperty("permittedLightingSceneComponentTypes");

            useManagerScene = serializedObject.FindProperty("useManagerScene");
            managerScene = serializedObject.FindProperty("managerScene");

            useLightingScene = serializedObject.FindProperty("useLightingScene");
            defaultLightingSceneIndex = serializedObject.FindProperty("defaultLightingSceneIndex");
            lightingScenes = serializedObject.FindProperty("lightingScenes");

            contentScenes = serializedObject.FindProperty("contentScenes");
        }

        public override void OnInspectorGUI()
        {
            if (!MixedRealityToolkit.IsInitialized)
            {
                return;
            }

            if (!RenderProfileHeader(ProfileTitle, ProfileDescription, target))
            {
                return;
            }

            MixedRealityInspectorUtility.CheckMixedRealityConfigured(true);

            serializedObject.Update();

            MixedRealitySceneSystemProfile profile = (MixedRealitySceneSystemProfile)target;

            if (!FindDefaultResources())
            {
                EditorGUILayout.HelpBox(defaultSceneContent, MessageType.Info);
            }

            RenderFoldout(ref showEditorProperties, "Editor Settings", () =>
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(editorManageBuildSettings);
                    EditorGUILayout.PropertyField(editorManageLoadedScenes);
                    EditorGUILayout.PropertyField(editorEnforceSceneOrder);
                    EditorGUILayout.PropertyField(editorEnforceLightingSceneTypes);

                    if (editorEnforceLightingSceneTypes.boolValue)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("Below are the component types that will be allowed in lighting scenes. Types not found in this list will be moved to another scene.", MessageType.Info);
                        EditorGUIUtility.labelWidth = LightingSceneTypesLabelWidth;
                        EditorGUILayout.PropertyField(permittedLightingSceneComponentTypes, true);
                        EditorGUIUtility.labelWidth = 0;
                    }
                }
            }, ShowSceneSystem_Editor_PreferenceKey);

            RenderFoldout(ref showManagerProperties, "Manager Scene Settings", () =>
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
                        var buttonRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(System.Array.Empty<GUILayoutOption>()));
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
            }, ShowSceneSystem_Manager_PreferenceKey);

            RenderFoldout(ref showLightingProperties, "Lighting Scene Settings", () =>
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.HelpBox(lightingSceneContent, MessageType.Info);

                    EditorGUILayout.PropertyField(useLightingScene);

                    if (useLightingScene.boolValue && profile.NumLightingScenes < 1 && !Application.isPlaying)
                    {
                        EditorGUILayout.HelpBox("You haven't created a lighting scene yet. Click the button below to create one.", MessageType.Warning);
                        var buttonRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(System.Array.Empty<GUILayoutOption>()));
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
                        EditorGUILayout.Space();

                        if (profile.NumLightingScenes > 0)
                        {
                            if (profile.EditorLightingCacheOutOfDate)
                            {
                                EditorGUILayout.HelpBox("Your cached lighting settings may be out of date. This could result in unexpected appearances at runtime.", MessageType.Warning);
                            }
                            if (InspectorUIUtility.RenderIndentedButton(new GUIContent("Update Cached Lighting Settings"), EditorStyles.miniButton))
                            {
                                profile.EditorLightingCacheUpdateRequested = true;
                            }
                        }
                        EditorGUILayout.Space();
                    }
                }
            }, ShowSceneSystem_Lighting_PreferenceKey);

            RenderFoldout(ref showContentProperties, "Content Scene Settings", () =>
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.HelpBox(contentSceneContent, MessageType.Info);

                    // Enable the tag field since we're drawing content scenes
                    SceneInfoDrawer.DrawTagProperty = true;
                    EditorGUILayout.PropertyField(contentScenes, includeChildren: true);
                }
            }, ShowSceneSystem_Content_PreferenceKey);

            serializedObject.ApplyModifiedProperties();

            // Keep this inspector perpetually refreshed
            EditorUtility.SetDirty(target);
        }

        protected override bool IsProfileInActiveInstance()
        {
            return MixedRealityToolkit.IsInitialized
                && MixedRealityToolkit.Instance.HasActiveProfile
                && MixedRealityToolkit.Instance.ActiveProfile.CameraProfile == this;
        }

        /// <summary>
        /// Used to drag-drop scene objects into scene lists. (Currently unused.)
        /// </summary>
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
                            foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
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

        private void OnSelecteContentScene(ReorderableList list)
        {
            // Do nothing.
        }

        private void DrawContentSceneElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SceneInfoDrawer.DrawProperty(rect, contentScenes.GetArrayElementAtIndex(index), GUIContent.none, isActive, isFocused);
        }

        private const string defaultManagerAssetGuid = "ae7bb08d297fb69408695d8de0962524";
        private Object defaultManagerAsset = null;
        private const string defaultLightingAssetGuid = "7e54e36c44f826c438c95da79f8de638";
        private Object defaultLightingAsset = null;

        private bool FindDefaultResources()
        {
            if ((defaultManagerAsset != null) &&
                (defaultLightingAsset != null))
            {
                return true;
            }

            defaultManagerAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(defaultManagerAssetGuid));
            defaultLightingAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(defaultLightingAssetGuid));

            return ((defaultManagerAsset != null) && (defaultLightingAsset != null));
        }
    }
}
