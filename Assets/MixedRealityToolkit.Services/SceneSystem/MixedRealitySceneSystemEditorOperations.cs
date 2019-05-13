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
        // These get set to dirty based on what we're doing in editor
        private bool activeSceneDirty = false;
        private bool buildSettingsDirty = false;
        private bool heirarchyDirty = false;
        // Cache these so we're not looking them up constantly
        private EditorBuildSettingsScene[] cachedBuildScenes = new EditorBuildSettingsScene[0];

        private enum BuildIndexTarget
        {
            First,
            None,
            Last,
        }

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

            UpdateBuildSettings();
        }

        #region update triggers from editor events

        private void EditorApplicationUpdate()
        {
            CheckForChanges();
        }

        private void EditorApplicationPlayModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                    heirarchyDirty = true;
                    buildSettingsDirty = true;
                    break;
            }
        }

        private void EditorApplicationHeirarcyChanged()
        {
            heirarchyDirty = true;
            activeSceneDirty = true;
        }

        private void EditorApplicationProjectChanged()
        {
            buildSettingsDirty = true;
        }

        private void EditorSceneManagerSceneClosed(Scene scene)
        {
            activeSceneDirty = true;
        }

        private void EditorSceneManagerSceneOpened(Scene scene, OpenSceneMode mode)
        {
            activeSceneDirty = true;
        }

        private void EditorSceneManagerNewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            activeSceneDirty = true;
        }

        #endregion

        private void CheckForChanges()
        {
            if (updatingSettingsOnEditorChanged || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
            {   // Make sure we don't double up on our updates via events we trigger during updates
                return;
            }

            cachedBuildScenes = EditorBuildSettings.scenes;

            updatingSettingsOnEditorChanged = true;

            // Update editor settings
            if (buildSettingsDirty)
            {
                UpdateBuildSettings();
            }

            if (activeSceneDirty || heirarchyDirty)
            {
                UpdateManagerScene();
                UpdateLightingScene(activeSceneDirty);
            }

            updatingSettingsOnEditorChanged = false;

            buildSettingsDirty = false;
            heirarchyDirty = false;
            activeSceneDirty = false;
        }

        /// <summary>
        /// If a manager scene is being used, this loads the scene in editor and ensures that an instance of the MRTK has been added to it.
        /// </summary>
        private void UpdateManagerScene()
        {
            if (!profile.UseManagerScene)
            {   // Nothing to do here.
                return;
            }

            if (LoadSceneInEditor(profile.ManagerScene.Asset, true, out Scene scene))
            {
                // Check for an MRTK instance
                bool foundToolkitInstance = false;
                foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                {
                    MixedRealityToolkit instance = rootGameObject.GetComponent<MixedRealityToolkit>();
                    if (instance != null)
                    {
                        foundToolkitInstance = true;
                        break;
                    }
                }

                if (!foundToolkitInstance)
                {
                    Debug.LogWarning("Didn't find a MixedRealityToolkit instance in your manager scene. Creating one now.");
                    GameObject mrtkGo = new GameObject("MixedRealityToolkit");
                    mrtkGo.AddComponent<MixedRealityToolkit>();
                    SceneManager.MoveGameObjectToScene(mrtkGo, scene);
                }
            }
            else
            {
                Debug.Log("Couldn't load manager scene!");
            }
        }

        private void UpdateLightingScene(bool updateActiveScene)
        {
            if (profile.UseLightingScene && updateActiveScene)
            {
                // Only update this once in a while to avoid being obnoxious
                foreach (SceneInfo lightingScene in profile.LightingScenes)
                {   // Make sure ALL lighting scenes are added to build settings
                    if (lightingScene.Name == lightingSceneName)
                    {
                        if (LoadSceneInEditor(lightingScene.Asset, false, out Scene editorScene) && updateActiveScene)
                        {
                            try
                            {
                                EditorSceneManager.SetActiveScene(editorScene);
                            }
                            catch (ArgumentException)
                            {
                                // This can happen if we try to set an invalid scene active.
                            }
                        }
                    }
                    else
                    {
                        UnloadSceneInEditor(lightingScene.Asset);
                    }
                }
            }
        }

        private void UpdateBuildSettings()
        {
            bool changedScenes = false;

            if (profile.UseManagerScene)
            {
                changedScenes |= AddSceneToBuildSettings(profile.ManagerScene.Asset, BuildIndexTarget.First);
            }

            foreach (SceneInfo contentScene in profile.ContentScenes)
            {
                changedScenes |= AddSceneToBuildSettings(contentScene.Asset, BuildIndexTarget.None);
            }

            if (profile.UseLightingScene)
            {
                foreach (SceneInfo lightingScene in profile.LightingScenes)
                {   // Make sure ALL lighting scenes are added to build settings
                    changedScenes |= AddSceneToBuildSettings(lightingScene.Asset, BuildIndexTarget.Last);
                }
            }

            if (changedScenes)
            {   // If we made changes, cache the build scenes again
                cachedBuildScenes = EditorBuildSettings.scenes;
            }

            CheckBuildSettingsForDuplicates();
        }

        /// <summary>
        /// Checks build settings for possible errors and displays warnings.
        /// Also modifies current profile's content scene names property.
        /// </summary>
        private void CheckBuildSettingsForDuplicates()
        {
            // Find any duplicate names in our lists
            // Duplicate names can complicate loading / unloading of scenes
            Dictionary<string, List<int>> duplicates = new Dictionary<string, List<int>>();
            List<SceneInfo> allScenes = new List<SceneInfo>();
            bool foundDuplicates = false;
            List<int> indexes = null;

            foreach (SceneInfo sceneInfo in profile.LightingScenes)
            {
                if (sceneInfo.IsEmpty)
                {   // Don't bother with empty scenes, they'll be handled elsewhere.
                    continue;
                }

                allScenes.Add(sceneInfo);

                if (duplicates.TryGetValue(sceneInfo.Name, out indexes))
                {
                    indexes.Add(sceneInfo.BuildIndex);
                    foundDuplicates = true;
                }
                else
                {
                    duplicates.Add(sceneInfo.Name, new List<int> { sceneInfo.BuildIndex });
                }
            }

            foreach (SceneInfo sceneInfo in profile.ContentScenes)
            {
                if (sceneInfo.IsEmpty)
                {   // Don't bother with empty scenes, they'll be handled elsewhere.
                    continue;
                }

                allScenes.Add(sceneInfo);

                if (duplicates.TryGetValue(sceneInfo.Name, out indexes))
                {
                    indexes.Add(sceneInfo.BuildIndex);
                    foundDuplicates = true;
                }
                else
                {
                    duplicates.Add(sceneInfo.Name, new List<int> { sceneInfo.BuildIndex });
                }
            }

            if (profile.UseManagerScene)
            {
                allScenes.Add(profile.ManagerScene);

                if (duplicates.TryGetValue(profile.ManagerScene.Name, out indexes))
                {
                    indexes.Add(profile.ManagerScene.BuildIndex);
                    foundDuplicates = true;
                }
                else
                {
                    duplicates.Add(profile.ManagerScene.Name, new List<int> { profile.ManagerScene.BuildIndex });
                }
            }

            // If we found any display our duplicates dialog
            if (foundDuplicates)
            {
                // If it's already open, don't display
                if (!ResolveDuplicateScenesWindow.IsOpen)
                {
                    ResolveDuplicateScenesWindow window = EditorWindow.GetWindow<ResolveDuplicateScenesWindow>("Fix Duplicate Scene Names");
                    window.ResolveDuplicates(duplicates, allScenes);
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
        private static bool AddSceneToBuildSettings(UnityEngine.Object sceneObject, BuildIndexTarget buildIndexTarget = BuildIndexTarget.None)
        {
            if (sceneObject == null)
            {   // Can't add a null scene to build settings
                return false;
            }

            long localID;
            string managerGuidString;
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sceneObject, out managerGuidString, out localID);
            GUID sceneGuid = new GUID(managerGuidString);

            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            // See if / where the scene exists in build settings
            int buildIndex = GetSceneBuildIndex(sceneGuid, scenes);

            if (buildIndex < 0)
            {
                // It doesn't exist in the build settings, add it now
                switch (buildIndexTarget)
                {
                    case BuildIndexTarget.First:
                        // Add it to index 0
                        scenes.Insert(0, new EditorBuildSettingsScene(sceneGuid, true));
                        break;

                    case BuildIndexTarget.None:
                    default:
                        // Just add it to the end
                        scenes.Add(new EditorBuildSettingsScene(sceneGuid, true));
                        break;
                }

                EditorBuildSettings.scenes = scenes.ToArray();
                return true;
            }
            else
            {
                switch (buildIndexTarget)
                {
                    // If it does exist, but isn't in the right spot, move it now
                    case BuildIndexTarget.First:
                        if (buildIndex != 0)
                        {
                            Debug.LogWarning("Scene '" + sceneObject.name + "' was not first in build order. Changing build settings now.");

                            scenes.RemoveAt(buildIndex);
                            scenes.Insert(0, new EditorBuildSettingsScene(sceneGuid, true));
                            EditorBuildSettings.scenes = scenes.ToArray();
                        }
                        return true;

                    case BuildIndexTarget.Last:
                        if (buildIndex != EditorSceneManager.sceneCountInBuildSettings - 1)
                        {
                            scenes.RemoveAt(buildIndex);
                            scenes.Insert(scenes.Count - 1, new EditorBuildSettingsScene(sceneGuid, true));
                            EditorBuildSettings.scenes = scenes.ToArray();
                        }
                        return true;

                    case BuildIndexTarget.None:
                    default:
                        // Do nothing
                        return false;

                }
            }
        }

        private static int GetSceneBuildIndex(GUID sceneGUID, List<EditorBuildSettingsScene> scenes)
        {
            int buildIndex = -1;
            int sceneCount = 0;
            for (int i = 0; i < scenes.Count; i++)
            {
                if (scenes[i].guid == sceneGUID)
                {
                    buildIndex = sceneCount;
                    break;
                }

                if (scenes[i].enabled)
                {
                    sceneCount++;
                }
            }

            return buildIndex;
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
            catch (NullReferenceException)
            {
                // This can happen if the scene object is null.
                return false;
            }
            catch (MissingReferenceException)
            {
                // This can happen if the scene object is null.
                return false;
            }

            return true;
        }

        private static bool UnloadSceneInEditor(UnityEngine.Object sceneObject)
        {
            Scene editorScene = default(Scene);

            try
            {
                editorScene = EditorSceneManager.GetSceneByName(sceneObject.name);
                if (editorScene.isLoaded)
                {
                    EditorSceneManager.CloseScene(editorScene, false);
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
            catch (NullReferenceException)
            {
                // This can happen if the scene object is null.
                return false;
            }
            catch (MissingReferenceException)
            {
                // This can happen if the scene object is null.
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

        private static List<string> GetContentSceneNamesFromBuildSettings(IEnumerable<SceneInfo> lightingScenes, SceneInfo managerScene)
        {
            List<string> sceneNames = new List<string>();
            HashSet<string> nonContentSceneNames = new HashSet<string>();
            foreach (SceneInfo scene in lightingScenes)
            {
                nonContentSceneNames.Add(scene.Name);
            }
            nonContentSceneNames.Add(managerScene.Name);

            for (int i = 0; i < EditorSceneManager.sceneCountInBuildSettings; i++)
            {
                string sceneName = GetSceneNameFromScenePath(SceneUtility.GetScenePathByBuildIndex(i));
                if (!nonContentSceneNames.Contains(sceneName))
                {
                    sceneNames.Add(sceneName);
                }
            }

            return sceneNames;
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