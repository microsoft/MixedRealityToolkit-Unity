// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
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

        // These are the types of components that are permitted to exist inside a lighting scene
        private static Type[] permittedLightingSceneComponentTypes = new Type[] {
            typeof(Transform),
            typeof(GameObject),
            typeof(Light),
            typeof(ReflectionProbe),
            typeof(LightProbeGroup),
            typeof(LightProbeProxyVolume),
        };

        private bool updatingSettingsOnEditorChanged = false;
        private const float lightingUpdateInterval = 5f;
        private const float managerSceneInstanceCheckInterval = 2f;
        private const int editorApplicationUpdateTickInterval = 5;
        // These get set to dirty based on what we're doing in editor
        private bool activeSceneDirty = false;
        private bool buildSettingsDirty = false;
        private bool heirarchyDirty = false;
        // Cache these so we're not looking them up constantly
        private EditorBuildSettingsScene[] cachedBuildScenes = new EditorBuildSettingsScene[0];
        // Checking for the manager scene via root game objects is very expensive
        // So only do it once in a while
        private float managerSceneInstanceCheckTime;
        private int editorApplicationUpdateTicks;

        private static int instanceIDCount;

        private int instanceID = -1;

        private void OnEditorInitialize()
        {
            instanceID = instanceIDCount++;

            SubscribeToEditorEvents();
            cachedBuildScenes = EditorBuildSettings.scenes;

            activeSceneDirty = true;
            buildSettingsDirty = true;
            heirarchyDirty = true;

            CheckForChanges();
        }

        private void OnEditorEnable()
        {
            SubscribeToEditorEvents();
            cachedBuildScenes = EditorBuildSettings.scenes;

            activeSceneDirty = true;
            buildSettingsDirty = true;
            heirarchyDirty = true;

            CheckForChanges();
        }

        private void OnEditorDisable()
        {
            UnsubscribeToEditorEvents();
        }

        private void OnEditorDestroy()
        {
            UnsubscribeToEditorEvents();
        }

        private void SubscribeToEditorEvents()
        {            
            EditorApplication.playModeStateChanged += EditorApplicationPlayModeStateChanged;
            EditorApplication.projectChanged += EditorApplicationProjectChanged;
            EditorApplication.hierarchyChanged += EditorApplicationHeirarcyChanged;
            EditorApplication.update += EditorApplicationUpdate;

            EditorSceneManager.newSceneCreated += EditorSceneManagerNewSceneCreated;
            EditorSceneManager.sceneOpened += EditorSceneManagerSceneOpened;
            EditorSceneManager.sceneClosed += EditorSceneManagerSceneClosed;
        }

        private void UnsubscribeToEditorEvents()
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
            editorApplicationUpdateTicks++;
            if (editorApplicationUpdateTicks > editorApplicationUpdateTickInterval)
            {
                editorApplicationUpdateTicks = 0;
                activeSceneDirty = true;
                heirarchyDirty = true;
                CheckForChanges();
            }
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
            if (!MixedRealityToolkit.IsInitialized || !MixedRealityToolkit.Instance.HasActiveProfile)
            {
                return;
            }

            if (!MixedRealityToolkit.Instance.ActiveProfile.IsSceneSystemEnabled)
            {
                return;
            }

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
                UpdateLightingScene(heirarchyDirty);
                UpdateContentScenes(activeSceneDirty);
            }

            updatingSettingsOnEditorChanged = false;

            buildSettingsDirty = false;
            heirarchyDirty = false;
            activeSceneDirty = false;
        }

        private void UpdateContentScenes(bool activeSceneDirty)
        {
            if (!profile.UseLightingScene || !profile.EditorManageLoadedScenes)
            {   // Nothing to do here
                return;
            }

            if (!activeSceneDirty)
            {   // Nothing to do here either
                return;
            }

            bool contentSceneIsActive = false;
            SceneInfo firstLoadedContentScene = SceneInfo.Empty;

            foreach (SceneInfo contentScene in profile.ContentScenes)
            {
                Scene scene;
                if (EditorSceneUtils.GetSceneIfLoaded(contentScene, out scene))
                {
                    if (firstLoadedContentScene.IsEmpty)
                    {   // If this is the first loaded content scene we've found, store it for later
                        firstLoadedContentScene = contentScene;
                    }

                    Scene activeScene = EditorSceneManager.GetActiveScene();
                    if (activeScene.name == contentScene.Name)
                    {
                        contentSceneIsActive = true;
                    }
                }
            }

            if (!firstLoadedContentScene.IsEmpty)
            {   // If at least one content scene is loaded
                if (!contentSceneIsActive)
                {   // And that content scene is NOT the active scene
                    // Set that content to be the active scene
                    Scene activeScene;
                    EditorSceneUtils.GetSceneIfLoaded(firstLoadedContentScene, out activeScene);
                    EditorSceneUtils.SetActiveScene(activeScene);
                }
            }
        }

        /// <summary>
        /// If a manager scene is being used, this loads the scene in editor and ensures that an instance of the MRTK has been added to it.
        /// </summary>
        private void UpdateManagerScene()
        {
            if (!profile.UseManagerScene || !profile.EditorManageLoadedScenes)
            {   // Nothing to do here.
                return;
            }

            if (EditorSceneUtils.LoadScene(profile.ManagerScene, true, out Scene scene))
            {
                // If we're managing scene heirarchy, move this to the front
                if (profile.EditorEnforceSceneOrder)
                {
                    Scene currentFirstScene = EditorSceneManager.GetSceneAt(0);
                    if (currentFirstScene.name != scene.name)
                    {
                        EditorSceneManager.MoveSceneBefore(scene, currentFirstScene);
                    }
                }

                if (Time.realtimeSinceStartup > managerSceneInstanceCheckTime)
                {
                    managerSceneInstanceCheckTime = Time.realtimeSinceStartup + managerSceneInstanceCheckInterval;
                    // Check for an MRTK instance
                    bool foundToolkitInstance = false;

                    try
                    {
                        foreach (GameObject rootGameObject in scene.GetRootGameObjects())
                        {
                            MixedRealityToolkit instance = rootGameObject.GetComponent<MixedRealityToolkit>();
                            if (instance != null)
                            {
                                foundToolkitInstance = true;
                                // If we found an instance, and it's not the active instance, we probably want to activate it
                                if (instance != MixedRealityToolkit.Instance)
                                {   // The only exception would be if the new instance has a different profile than the current instance
                                    // If that's the case, we could end up ping-ponging between two sets of manager scenes
                                    if (!instance.HasActiveProfile)
                                    {   // If it doesn't have a profile, set it to our current profile
                                        instance.ActiveProfile = MixedRealityToolkit.Instance.ActiveProfile;
                                    }
                                    else if (instance.ActiveProfile != MixedRealityToolkit.Instance.ActiveProfile)
                                    {
                                        Debug.LogWarning("The active profile of the instance in your manager scene is different from the profile that loaded your scene. This is not recommended.");
                                    }
                                    else
                                    {
                                        Debug.LogWarning("Setting the manager scene MixedRealityToolkit instance to the active instance.");
                                        MixedRealityToolkit.SetActiveInstance(instance);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // This can happen if the scene isn't valid
                        // Not an issue - we'll take care of it on the next update.
                    }

                    if (!foundToolkitInstance)
                    {
                        GameObject mrtkGo = new GameObject("MixedRealityToolkit");
                        MixedRealityToolkit toolkitInstance = mrtkGo.AddComponent<MixedRealityToolkit>();
                        // Set the config profile to use the same profile as the current instance
                        toolkitInstance.ActiveProfile = MixedRealityToolkit.Instance.ActiveProfile;

                        try
                        {
                            SceneManager.MoveGameObjectToScene(mrtkGo, scene);
                            // Set the scene as dirty
                            EditorSceneManager.MarkSceneDirty(scene);
                        }
                        catch (Exception)
                        {
                            // This can happen if the scene isn't valid
                            // Not an issue - we'll take care of it on the next update.
                            // Destroy the new manager
                            GameObject.DestroyImmediate(mrtkGo);
                            return;
                        }

                        MixedRealityToolkit.SetActiveInstance(toolkitInstance);
                        Debug.LogWarning("Didn't find a MixedRealityToolkit instance in your manager scene. Creating one now.");
                    }
                }
            }
            else
            {
                Debug.Log("Couldn't load manager scene!");
            }
        }

        private void UpdateLightingScene(bool heirarchyDirty)
        {
            if (!profile.UseLightingScene || !profile.EditorManageLoadedScenes)
            {
                return;
            }

            if (string.IsNullOrEmpty(ActiveLightingScene))
            {
                Debug.LogWarning("Active lighting scene was empty - setting to default.");
                ActiveLightingScene = profile.DefaultLightingScene.Name;
            }
            else
            {
                foreach (SceneInfo lightingScene in profile.LightingScenes)
                {
                    if (lightingScene.Name == ActiveLightingScene)
                    {
                        Scene scene;
                        if (EditorSceneUtils.LoadScene(lightingScene, false, out scene))
                        {
                            EditorSceneUtils.CopyLightingSettingsToActiveScene(scene);

                            if (profile.EditorEnforceLightingSceneTypes && heirarchyDirty)
                            {
                                List<Component> violations = new List<Component>();
                                if (EditorSceneUtils.EnforceSceneComponents(scene, permittedLightingSceneComponentTypes, violations))
                                {
                                    if (EditorUtility.DisplayDialog("Non-lighting components found", "We found non-lighting components in your lighting scene. To disable this check, un-check 'EditorEnforceLightingSceneTypes' in your SceneSystem profile.", "Destroy", "Cancel"))
                                    {
                                        foreach (Component component in violations)
                                        {
                                            GameObject.DestroyImmediate(component);
                                        }
                                    }
                                }
                            }
                        }

                        if (profile.EditorEnforceSceneOrder)
                        {   // If we're enforcing scene order, make sure this scene comes after the current scene
                            Scene currentFirstScene = EditorSceneManager.GetSceneAt(0);
                            EditorSceneManager.MoveSceneAfter(scene, currentFirstScene);
                        }
                    }
                    else
                    {
                        EditorSceneUtils.UnloadScene(lightingScene, true);
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