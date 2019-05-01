// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// The default implementation of the <see cref="Microsoft.MixedReality.Toolkit.SceneSystem.IMixedRealitySceneSystem"/>
    /// This file handles the editor-oriented parts of the service.
    /// </summary>
    public partial class MixedRealitySceneSystem : BaseCoreSystem, IMixedRealitySceneSystem
    {
#if UNITY_EDITOR
        private bool updatingSettingsOnEditorChanged = false;
        private const float lightingUpdateInterval = 5f;
        private const float editorSettingsUpdateInterval = 2.5f;
        private float timeSinceLastEditorSettingsUpdate = 0;
        private float timeSinceLastLightingUpdate = 0;
        private List<string> buildSceneNames = new List<string>();

        private void OnEditorInitialize()
        {
            // Subscribe to editor events
            EditorApplication.playModeStateChanged += EditorApplicationPlayModeStateChanged;
            EditorApplication.projectChanged += EditorApplicationProjectChanged;
            EditorApplication.hierarchyChanged += EditorApplicationHeirarcyChanged;
            EditorApplication.update += EditorApplicationUpdate;

            EditorSceneManager.newSceneCreated += EditorSceneManagerNewSceneCreated;
            EditorSceneManager.sceneOpened += EditorSceneManagerSceneOpened;
            EditorSceneManager.sceneClosed += EditorSceneManagerSceneClosed;
            EditorSceneManager.activeSceneChangedInEditMode += EditorSceneManagerActiveSceneChangedInEditMode;

            UpdateBuildSettings();
        }

        #region update triggers from editor events

        private void EditorApplicationPlayModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                    UpdateSettingsOnEditorChanged(false);
                    break;
            }
        }

        private void EditorSceneManagerActiveSceneChangedInEditMode(Scene prevActiveScene, Scene newActiveScene)
        {
            // Don't change the active scene in this case.
            // Otherwise we may usurp the creation of a new scene
            // and steal its newly created camera and directional light
            UpdateSettingsOnEditorChanged(false);
        }

        private void EditorApplicationHeirarcyChanged()
        {
            UpdateSettingsOnEditorChanged(true);
        }

        private void EditorApplicationProjectChanged()
        {
            UpdateSettingsOnEditorChanged(true);
        }

        private void EditorApplicationUpdate()
        {
            if (Time.realtimeSinceStartup - timeSinceLastEditorSettingsUpdate > editorSettingsUpdateInterval)
            {
                timeSinceLastEditorSettingsUpdate = Time.realtimeSinceStartup;
                UpdateSettingsOnEditorChanged(true);
            }
        }

        private void EditorSceneManagerSceneClosed(Scene scene)
        {
            UpdateSettingsOnEditorChanged(true);
        }

        private void EditorSceneManagerSceneOpened(Scene scene, OpenSceneMode mode)
        {
            UpdateSettingsOnEditorChanged(true);
        }

        private void EditorSceneManagerNewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            UpdateSettingsOnEditorChanged(true);
        }

        #endregion

        private void UpdateSettingsOnEditorChanged(bool updateActiveScene = true)
        {
            if (updatingSettingsOnEditorChanged || EditorApplication.isPlaying || EditorApplication.isCompiling)
            {   // Make sure we don't double up on our updates via events we trigger during updates
                return;
            }

            updatingSettingsOnEditorChanged = true;

            UpdateBuildSettings();
            UpdateManagerScene();
            UpdateLightingScene(updateActiveScene);

            updatingSettingsOnEditorChanged = false;
        }

        private void UpdateManagerScene()
        {
            if (profile.UseManagerScene)
            {
                LoadSceneInEditor(profile.ManagerSceneObject, true, out Scene scene);
            }
        }

        private void UpdateLightingScene(bool updateActiveScene)
        {
            if (profile.UseLightingScene && updateActiveScene)
            {
                // Only update this once in a while to avoid being obnoxious
                if (Time.realtimeSinceStartup - timeSinceLastLightingUpdate > lightingUpdateInterval)
                {
                    timeSinceLastLightingUpdate = Time.realtimeSinceStartup;
                    Scene contentScene;
                    if (LoadSceneInEditor(profile.LightingSceneObject, false, out contentScene))
                    {
                        CopyLightingSettingsToActiveScene(contentScene);
                    }
                }
            }
        }

        private void UpdateBuildSettings()
        {
            if (profile.UseManagerScene)
            {
                AddSceneToBuildSettings(profile.ManagerSceneObject, true);
            }

            if (profile.UseLightingScene)
            {
                AddSceneToBuildSettings(profile.LightingSceneObject, false);
            }

            CheckBuildSettings();
        }

        /// <summary>
        /// Checks build settings for possible errors and displays warnings.
        /// Also modifies current profile's scene names property.
        /// </summary>
        private void CheckBuildSettings()
        {
            // Make sure our profile's build scene names are accurate
            SerializedObject profileObject = new SerializedObject(profile);
            SerializedProperty buildSceneNamesProperty = profileObject.FindProperty("buildSceneNames");

            GetSceneNamesFromBuildSettings(buildSceneNames);

            // Make sure the array size is correct before proceeding
            if (buildSceneNamesProperty.arraySize != buildSceneNames.Count)
            {
                buildSceneNamesProperty.arraySize = buildSceneNames.Count;
                profileObject.ApplyModifiedProperties();
            }

            for (int i = 0; i < buildSceneNames.Count; i++)
            {
                SerializedProperty element = buildSceneNamesProperty.GetArrayElementAtIndex(i);
                element.stringValue = buildSceneNames[i];
            }
            profileObject.ApplyModifiedProperties();

            // Find any duplicate names in our list
            // Duplicate names can complicate loading / unloading of scenes
            Dictionary<string, List<int>> duplicates = new Dictionary<string, List<int>>();
            bool foundDuplicates = false;
            for (int i = 0; i < buildSceneNames.Count; i++)
            {
                List<int> indexes;
                if (duplicates.TryGetValue(buildSceneNames[i], out indexes))
                {
                    indexes.Add(i);
                    foundDuplicates = true;
                }
                else
                {
                    duplicates.Add(buildSceneNames[i], new List<int> { i });
                }
            }

            // If we found any display our duplicates dialog
            if (foundDuplicates)
            {
                // If it's already open, don't display
                if (!ResolveDuplicateScenesWindow.IsOpen)
                {
                    ResolveDuplicateScenesWindow window = EditorWindow.GetWindow<ResolveDuplicateScenesWindow>("Fix Duplicate Scene Names");
                    window.ResolveDuplicates(duplicates, buildSceneNames);
                }
            }
            else if (ResolveDuplicateScenesWindow.IsOpen)
            {
                // If we fixed the issue without the window, close the window
                ResolveDuplicateScenesWindow.Instance.Close();
            }
        }

        /// <summary>
        /// Adds scene to build settings.
        /// </summary>
        /// <param name="sceneObject">Scene object reference.</param>
        /// <param name="setAsFirst">Sets as first scene to be loaded.</param>
        private static void AddSceneToBuildSettings(UnityEngine.Object sceneObject, bool setAsFirst = false)
        {
            long localID;
            string managerGuidString;
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sceneObject, out managerGuidString, out localID);
            GUID sceneGuid = new GUID(managerGuidString);

            // See if / where the scene exists in build settings
            int buildIndex = -1;
            for (int i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
            {
                if (EditorBuildSettings.scenes[i].guid == sceneGuid)
                {
                    buildIndex = i;
                    break;
                }
            }

            if (buildIndex < 0)
            {
                // It doesn't exist in the build settings, add it now
                Debug.LogWarning("Adding " + sceneObject.name + " to build settings now.");

                List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                if (setAsFirst)
                {   // Add it to index 0
                    scenes.Insert(0, new EditorBuildSettingsScene(sceneGuid, true));
                }
                else
                {   // Just add it to the end
                    scenes.Add(new EditorBuildSettingsScene(sceneGuid, true));
                }

                EditorBuildSettings.scenes = scenes.ToArray();
            }
            else if (setAsFirst && buildIndex != 0)
            {
                // If it does exist, but isn't in the right spot, move it now
                Debug.LogWarning("Scene '" + sceneObject.name + "' was not first in build order. Changing build settings now.");

                List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
                scenes.RemoveAt(buildIndex);
                scenes.Insert(0, new EditorBuildSettingsScene(sceneGuid, true));

                EditorBuildSettings.scenes = scenes.ToArray();
            }
        }

        /// <summary>
        /// Attempts to load scene in editor using a scene object reference.
        /// </summary>
        /// <param name="sceneObject">Scene object reference.</param>
        /// <param name="setAsFirst">Whether to set first in the heirarchy window.</param>
        /// <param name="editorScene">The loaded scene.</param>
        /// <returns>True if successful.</returns>
        private static bool LoadSceneInEditor(UnityEngine.Object sceneObject, bool setAsFirst, out Scene editorScene)
        {
            editorScene = default(Scene);

            try
            {
                editorScene = EditorSceneManager.GetSceneByName(sceneObject.name);
                if (!editorScene.isLoaded)
                {
                    string scenePath = AssetDatabase.GetAssetOrScenePath(sceneObject);
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                }

                if (setAsFirst && EditorSceneManager.sceneCount >= 1)
                {   // Move the scene to first in order in the heirarchy
                    Scene nextScene = EditorSceneManager.GetSceneAt(0);
                    EditorSceneManager.MoveSceneBefore(editorScene, nextScene);
                }
            }
            catch (InvalidOperationException)
            {
                // This can happen if we're trying to load immediately upon recompilation.
                return false;
            }
            catch (ArgumentException)
            {
                // This can happen if the scene is an invalid scene and we try to SetActive.
                return false;
            }

            return true;
        }

        /// <summary>
        /// Copies the lighting settings from the lighting scene to the active scene
        /// </summary>
        /// <param name="lightingScene"></param>
        private static void CopyLightingSettingsToActiveScene(Scene lightingScene)
        {
            // Store the active scene on entry
            Scene activeSceneOnEnter = EditorSceneManager.GetActiveScene();

            // No need to do anything
            if (activeSceneOnEnter == lightingScene)
                return;

            SerializedObject sourceLightmapSettings;
            SerializedObject sourceRenderSettings;

            try
            {
                // Set the active scene to the lighting scene
                EditorSceneManager.SetActiveScene(lightingScene);
                // If we can't get the source settings for some reason, abort
                if (!GetLightmapAndRenderSettings(out sourceLightmapSettings, out sourceRenderSettings))
                {
                    return;
                }
            }
            catch (InvalidOperationException)
            {
                // This can happen if the lighting scene is invalid
                return;
            }

            bool madeChanges = false;

            try
            {
                // Set active scene back to the active scene on enter
                EditorSceneManager.SetActiveScene(activeSceneOnEnter);
                SerializedObject targetLightmapSettings;
                SerializedObject targetRenderSettings;

                if (GetLightmapAndRenderSettings(out targetLightmapSettings, out targetRenderSettings))
                {
                    madeChanges |= CopySerializedObject(sourceLightmapSettings, targetLightmapSettings);
                    madeChanges |= CopySerializedObject(sourceRenderSettings, targetRenderSettings);
                }
            }
            catch (InvalidOperationException)
            {
                // This can happen if the current active scene is invalid
                return;
            }

            if (madeChanges)
            {
                Debug.LogWarning("Changed lighting settings in scene " + activeSceneOnEnter.name + " to match lighting scene " + lightingScene.name);
                EditorSceneManager.MarkSceneDirty(activeSceneOnEnter);
            }
        }

        private static void GetSceneNamesFromBuildSettings(List<string> sceneNames)
        {
            sceneNames.Clear();
            for (int i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
            {
                sceneNames.Add(GetSceneNameFromScenePath(SceneUtility.GetScenePathByBuildIndex(i)));
            }
        }

        private static bool GetLightmapAndRenderSettings(out SerializedObject lightmapSettings, out SerializedObject renderSettings)
        {
            lightmapSettings = null;
            renderSettings = null;

            UnityEngine.Object lightmapSettingsObject = null;
            UnityEngine.Object renderSettingsObject = null;

            try
            {
                // Use reflection to get the serialized objects of lightmap settings and render settings
                Type lightmapSettingsType = typeof(LightmapEditorSettings);
                Type renderSettingsType = typeof(RenderSettings);

                lightmapSettingsObject = (UnityEngine.Object)lightmapSettingsType.GetMethod("GetLightmapSettings", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
                renderSettingsObject = (UnityEngine.Object)renderSettingsType.GetMethod("GetRenderSettings", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
            }
            catch (Exception)
            {
                Debug.LogWarning("Couldn't get lightmap or render settings. This version of Unity may not support this operation.");
                return false;
            }

            if (lightmapSettingsObject == null)
            {
                Debug.LogWarning("Couldn't get lightmap settings object");
                return false;
            }

            if (renderSettingsObject == null)
            {
                Debug.LogWarning("Couldn't get render settings object");
                return false;
            }

            // Store the settings in serialized objects
            lightmapSettings = new SerializedObject(lightmapSettingsObject);
            renderSettings = new SerializedObject(renderSettingsObject);
            return true;
        }

        private static bool CopySerializedObject(SerializedObject source, SerializedObject target)
        {
            bool madeChanges = false;
            SerializedProperty sourceProp = source.GetIterator();
            while (sourceProp.NextVisible(true))
            {
                switch (sourceProp.name)
                {
                    // This is an odd case where the value is constantly modified, resulting in constant changes.
                    // It's not apparent how this affects rendering.
                    case "m_IndirectSpecularColor":
                        continue;

                    default:
                        break;
                }

                madeChanges |= target.CopyFromSerializedPropertyIfDifferent(sourceProp);
            }

            if (madeChanges)
            {
                target.ApplyModifiedPropertiesWithoutUndo();
            }

            return madeChanges;
        }
#endif
    }
}