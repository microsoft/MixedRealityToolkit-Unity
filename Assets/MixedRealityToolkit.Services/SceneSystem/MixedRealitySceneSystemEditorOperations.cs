// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
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
        // Checking for the manager scene via root game objects is very expensive
        // So only do it once in a while
        private float managerSceneInstanceCheckInterval = 2f;
        private float managerSceneInstanceCheckTime;

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

            cachedBuildScenes = EditorBuildSettings.scenes;
            UpdateBuildSettings();
        }

        private void OnEditorDisable()
        {
            EditorApplication.playModeStateChanged -= EditorApplicationPlayModeStateChanged;
            EditorApplication.projectChanged -= EditorApplicationProjectChanged;
            EditorApplication.hierarchyChanged -= EditorApplicationHeirarcyChanged;
            EditorApplication.update -= EditorApplicationUpdate;

            EditorSceneManager.newSceneCreated += EditorSceneManagerNewSceneCreated;
            EditorSceneManager.sceneOpened -= EditorSceneManagerSceneOpened;
            EditorSceneManager.sceneClosed -= EditorSceneManagerSceneClosed;
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
            //buildSettingsDirty = true;
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

            if (EditorSceneUtils.LoadScene(profile.ManagerScene, true, out Scene scene))
            {
                if (Time.realtimeSinceStartup > managerSceneInstanceCheckTime)
                {
                    managerSceneInstanceCheckTime = Time.realtimeSinceStartup + managerSceneInstanceCheckInterval;
                    // Check for an MRTK instance
                    bool foundToolkitInstance = false;
                    foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                    {
                        MixedRealityToolkit instance = rootGameObject.GetComponent<MixedRealityToolkit>();
                        if (instance != null)
                        {
                            foundToolkitInstance = true;
                            // If we found an instance, and it's not the active instance, activate it now
                            if (instance != MixedRealityToolkit.Instance)
                            {
                                Debug.LogWarning("Setting the manager scene MixedRealityToolkit instance to the active instance.");
                                MixedRealityToolkit.SetActiveInstance(instance);
                            }
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
                bool loaded = false;
                // Only update this once in a while to avoid being obnoxious
                foreach (SceneInfo lightingScene in profile.LightingScenes)
                {   // Make sure ALL lighting scenes are added to build settings
                    if (!loaded && lightingScene.Name == ActiveLightingScene)
                    {
                        if (EditorSceneUtils.LoadScene(lightingScene, false, out Scene editorScene) && updateActiveScene)
                        {
                            EditorSceneUtils.SetActiveScene(editorScene);
                        }
                        loaded = true;
                    }
                    else
                    {
                        EditorSceneUtils.UnloadScene(lightingScene);
                    }
                }
            }
        }

        private void UpdateBuildSettings()
        {
            bool changedScenes = false;

            if (profile.UseManagerScene)
            {
                changedScenes |= EditorSceneUtils.AddSceneToBuildSettings(
                    profile.ManagerScene, 
                    cachedBuildScenes, 
                    EditorSceneUtils.BuildIndexTarget.First);
            }

            foreach (SceneInfo contentScene in profile.ContentScenes)
            {
                changedScenes |= EditorSceneUtils.AddSceneToBuildSettings(
                    contentScene,
                    cachedBuildScenes, 
                    EditorSceneUtils.BuildIndexTarget.None);
            }

            if (profile.UseLightingScene)
            {
                foreach (SceneInfo lightingScene in profile.LightingScenes)
                {   // Make sure ALL lighting scenes are added to build settings
                    changedScenes |= EditorSceneUtils.AddSceneToBuildSettings(
                        lightingScene, 
                        cachedBuildScenes, 
                        EditorSceneUtils.BuildIndexTarget.Last);
                }
            }

            if (changedScenes)
            {   // If we made changes, cache the build scenes again
                cachedBuildScenes = EditorBuildSettings.scenes;
            }

            CheckProfileForDuplicates();
        }

        private void CheckProfileForDuplicates()
        {
            List<SceneInfo> allScenes = new List<SceneInfo>();
            Dictionary<string, List<int>> duplicates = new Dictionary<string, List<int>>();

            foreach (SceneInfo sceneInfo in profile.LightingScenes)
            {
                if (!sceneInfo.IsEmpty)
                {   // Don't bother with empty scenes, they'll be handled elsewhere.
                    allScenes.Add(sceneInfo);
                }
            }

            foreach (SceneInfo sceneInfo in profile.ContentScenes)
            {
                if (!sceneInfo.IsEmpty)
                {   // Don't bother with empty scenes, they'll be handled elsewhere.
                    allScenes.Add(sceneInfo);
                }
            }

            if (profile.UseManagerScene && !profile.ManagerScene.IsEmpty)
            {
                allScenes.Add(profile.ManagerScene);
            }

            if(EditorSceneUtils.CheckBuildSettingsForDuplicates(allScenes, duplicates))
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

#endif
    }
}