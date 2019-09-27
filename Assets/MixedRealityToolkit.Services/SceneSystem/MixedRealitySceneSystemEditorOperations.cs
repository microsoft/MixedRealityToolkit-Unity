// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
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
    public partial class MixedRealitySceneSystem : BaseCoreSystem, IMixedRealitySceneSystem, IMixedRealitySceneSystemEditor
    {

#if UNITY_EDITOR

        /// <summary>
        /// Detects asset modifications.
        /// Used to detect when lighting cache may be out of date.
        /// </summary>
        internal sealed class FileModificationWarning : UnityEditor.AssetModificationProcessor
        {
            public static HashSet<string> ModifiedAssetPaths = new HashSet<string>();

            public static string[] OnWillSaveAssets(string[] paths)
            {
                foreach (string path in paths) { ModifiedAssetPaths.Add(path); }
                return paths;
            }
        }

        /// <summary>
        /// Returns the manager scene found in profile.
        /// </summary>
        public SceneInfo ManagerScene => profile.ManagerScene;

        /// <summary>
        /// Returns all lighting scenes found in profile.
        /// </summary>
        public SceneInfo[] LightingScenes => contentTracker.SortedLightingScenes;

        /// <summary>
        /// Returns all content scenes found in profile.
        /// </summary>
        public SceneInfo[] ContentScenes => contentTracker.SortedContentScenes;

        /// <summary>
        /// Returns all content tags found in profile scenes.
        /// </summary>
        public IEnumerable<string> ContentTags => profile.ContentTags;

        // Cache these so we're not looking them up constantly
        private EditorBuildSettingsScene[] cachedBuildScenes = new EditorBuildSettingsScene[0];

        // These get set to dirty based on what events have been recieved from the editor
        private bool activeSceneDirty = false;
        private bool buildSettingsDirty = false;
        private bool heirarchyDirty = false;
        // These get set based on what our update methods are doing in response to events
        private bool updatingSettingsOnEditorChanged = false;
        private bool updatingCachedLightingSettings = false;
        // Checking for the manager scene via root game objects is very expensive
        // So only do it once in a while
        private const float lightingUpdateInterval = 5f;
        private const float managerSceneInstanceCheckInterval = 2f;
        private const int editorApplicationUpdateTickInterval = 10;
        private float managerSceneInstanceCheckTime;
        private int editorApplicationUpdateTicks;

        // Used to track which instance of the service we're using (for debugging purposes)
        private static int instanceIDCount;
        private int instanceID = -1;

        #region public editor methods

        /// <summary>
        /// Singly loads next content scene (if available) and unloads all other content scenes.
        /// Useful for inspectors.
        /// </summary>
        public void EditorLoadNextContent(bool wrap = false)
        {
            string contentSceneName;
            if (contentTracker.GetNextContent(wrap, out contentSceneName))
            {
                foreach (SceneInfo contentScene in ContentScenes)
                {
                    if (contentScene.Name == contentSceneName)
                    {
                        EditorSceneUtils.LoadScene(contentScene, false, out Scene scene);
                    }
                    else
                    {
                        EditorSceneUtils.UnloadScene(contentScene, false);
                    }
                }
            }

            contentTracker.RefreshLoadedContent();
        }

        /// <summary>
        /// Singly loads previous content scene (if available) and unloads all other content scenes.
        /// Useful for inspectors.
        /// </summary>
        public void EditorLoadPrevContent(bool wrap = false)
        {
            string contentSceneName;
            if (contentTracker.GetPrevContent(wrap, out contentSceneName))
            {
                foreach (SceneInfo contentScene in ContentScenes)
                {
                    if (contentScene.Name == contentSceneName)
                    {
                        EditorSceneUtils.LoadScene(contentScene, false, out Scene scene);
                    }
                    else
                    {
                        EditorSceneUtils.UnloadScene(contentScene, false);
                    }
                }
            }

            contentTracker.RefreshLoadedContent();
        }

        #endregion

        #region Initialization / Teardown

        private void EditorOnInitialize()
        {
            instanceID = instanceIDCount++;

            EditorSubscribeToEvents();
            cachedBuildScenes = EditorBuildSettings.scenes;

            activeSceneDirty = true;
            buildSettingsDirty = true;
            heirarchyDirty = true;

            EditorCheckForChanges();
        }

        private void EditorOnEnable()
        {
            EditorSubscribeToEvents();
            cachedBuildScenes = EditorBuildSettings.scenes;

            activeSceneDirty = true;
            buildSettingsDirty = true;
            heirarchyDirty = true;

            EditorCheckForChanges();
        }

        private void EditorOnDisable()
        {
            EditorUnsubscribeFromEvents();
        }

        private void EditorOnDestroy()
        {
            EditorUnsubscribeFromEvents();
        }

        private void EditorSubscribeToEvents()
        {
            EditorApplication.projectChanged += EditorApplicationProjectChanged;
            EditorApplication.hierarchyChanged += EditorApplicationHeirarcyChanged;
            EditorApplication.update += EditorApplicationUpdate;

            EditorSceneManager.newSceneCreated += EditorSceneManagerNewSceneCreated;
            EditorSceneManager.sceneOpened += EditorSceneManagerSceneOpened;
            EditorSceneManager.sceneClosed += EditorSceneManagerSceneClosed;

            EditorBuildSettings.sceneListChanged += EditorSceneListChanged;
        }

        private void EditorUnsubscribeFromEvents()
        {
            EditorApplication.projectChanged -= EditorApplicationProjectChanged;
            EditorApplication.hierarchyChanged -= EditorApplicationHeirarcyChanged;
            EditorApplication.update -= EditorApplicationUpdate;

            EditorSceneManager.newSceneCreated += EditorSceneManagerNewSceneCreated;
            EditorSceneManager.sceneOpened -= EditorSceneManagerSceneOpened;
            EditorSceneManager.sceneClosed -= EditorSceneManagerSceneClosed;

            EditorBuildSettings.sceneListChanged -= EditorSceneListChanged;
        }

        #endregion

        #region update triggers from editor events

        private void EditorApplicationUpdate()
        {
            editorApplicationUpdateTicks++;
            if (editorApplicationUpdateTicks > editorApplicationUpdateTickInterval)
            {
                activeSceneDirty = true;
                heirarchyDirty = true;
                editorApplicationUpdateTicks = 0;
                EditorCheckForChanges();
            }
        }

        private void EditorApplicationHeirarcyChanged()
        {
            activeSceneDirty = true;
            heirarchyDirty = true;
        }

        private void EditorApplicationProjectChanged()
        {
            buildSettingsDirty = true;
        }

        private void EditorSceneListChanged()
        {
            buildSettingsDirty = true;

            EditorCheckForChanges();
        }

        private void EditorSceneManagerSceneClosed(Scene scene) { activeSceneDirty = true; }

        private void EditorSceneManagerSceneOpened(Scene scene, OpenSceneMode mode) { activeSceneDirty = true; }

        private void EditorSceneManagerNewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode) { activeSceneDirty = true; }

        #endregion

        /// <summary>
        /// Checks the state of service and profile based on changes made in editor and reacts accordingly.
        /// </summary>
        private async void EditorCheckForChanges()
        {
            if (!MixedRealityToolkit.IsInitialized || !MixedRealityToolkit.Instance.HasActiveProfile || !MixedRealityToolkit.Instance.ActiveProfile.IsSceneSystemEnabled)
            {
                return;
            }

            if (updatingSettingsOnEditorChanged || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
            {   // Make sure we don't double up on our updates via events we trigger during updates
                return;
            }

            if (updatingCachedLightingSettings)
            {   // This is a long operation, don't interrupt it
                return;
            }

            // Update cached lighting settings, if the profile has requested it
            if (profile.EditorLightingCacheUpdateRequested)
            {
                updatingCachedLightingSettings = true;
                updatingSettingsOnEditorChanged = true;

                await EditorUpdateCachedLighting();

                updatingSettingsOnEditorChanged = false;
                updatingCachedLightingSettings = false;
                // This is an async operation which may take a while to execute
                // So exit when we're done - we'll pick up where we left off next time
                heirarchyDirty = true;
                return;
            }

            updatingSettingsOnEditorChanged = true;

            // Update editor settings

            if (FileModificationWarning.ModifiedAssetPaths.Count > 0)
            {
                EditorCheckIfCachedLightingOutOfDate();
                FileModificationWarning.ModifiedAssetPaths.Clear();
            }

            if (buildSettingsDirty)
            {
                buildSettingsDirty = false;

                EditorUpdateBuildSettings();
            }

            if (activeSceneDirty || heirarchyDirty)
            {
                heirarchyDirty = false;
                activeSceneDirty = false;

                EditorUpdateManagerScene();
                EditorUpdateLightingScene(heirarchyDirty);
                EditorUpdateContentScenes(activeSceneDirty);

                contentTracker.RefreshLoadedContent();
            }

            EditorUtility.SetDirty(profile);

            updatingSettingsOnEditorChanged = false;
        }
        
        /// <summary>
        /// Checks whether any of the save dates on our lighting scenes are later than the save date of our cached lighting data.
        /// </summary>
        private void EditorCheckIfCachedLightingOutOfDate()
        {            
            DateTime cachedLightingTimestamp = profile.GetEarliestLightingCacheTimestamp();
            bool outOfDate = false;

            foreach (SceneInfo lightingScene in profile.LightingScenes)
            {
                if (FileModificationWarning.ModifiedAssetPaths.Contains(lightingScene.Path))
                {
                    string lightingScenePath = System.IO.Path.Combine(Application.dataPath.Replace("/Assets", ""), lightingScene.Path);
                    DateTime lightingSceneTimestamp = System.IO.File.GetLastWriteTime(lightingScenePath);
                    if (lightingSceneTimestamp > cachedLightingTimestamp)
                    {
                        outOfDate = true;
                        break;
                    }
                }
            }

            if (outOfDate)
            {
                profile.SetLightingCacheDirty();
            }
        }

        /// <summary>
        /// Loads all lighting scenes, extracts their lighting data, then caches that data in the profile.
        /// </summary>
        private async Task EditorUpdateCachedLighting()
        {
            // Clear out our lighting cache
            profile.ClearLightingCache();
            profile.EditorLightingCacheUpdateRequested = false;

            SceneInfo defaultLightingScene = profile.DefaultLightingScene;

            foreach (SceneInfo lightingScene in profile.LightingScenes)
            {
                // Load all our lighting scenes
                Scene scene;
                EditorSceneUtils.LoadScene(lightingScene, false, out scene);
            }

            // Wait for a moment so all loaded scenes have time to get set up
            await Task.Delay(100);

            foreach (SceneInfo lightingScene in profile.LightingScenes)
            {
                Scene scene;
                EditorSceneUtils.GetSceneIfLoaded(lightingScene, out scene);
                EditorSceneUtils.SetActiveScene(scene);

                SerializedObject lightingSettingsObject;
                SerializedObject renderSettingsObject;
                EditorSceneUtils.GetLightingAndRenderSettings(out lightingSettingsObject, out renderSettingsObject);

                // Copy the serialized objects into new structs
                RuntimeLightingSettings lightingSettings = default(RuntimeLightingSettings);
                RuntimeRenderSettings renderSettings = default(RuntimeRenderSettings);
                RuntimeSunlightSettings sunlightSettings = default(RuntimeSunlightSettings);

                lightingSettings = SerializedObjectUtils.CopySerializedObjectToStruct<RuntimeLightingSettings>(lightingSettingsObject, lightingSettings, "m_");
                renderSettings = SerializedObjectUtils.CopySerializedObjectToStruct<RuntimeRenderSettings>(renderSettingsObject, renderSettings, "m_");

                // Extract sunlight settings based on sunlight object
                SerializedProperty sunProperty = renderSettingsObject.FindProperty("m_Sun");
                if (sunProperty == null)
                {
                    Debug.LogError("Sun settings may not be available in this version of Unity.");
                }
                else
                {
                    Light sunLight = (Light)sunProperty.objectReferenceValue;

                    if (sunLight != null)
                    {
                        sunlightSettings.UseSunlight = true;
                        sunlightSettings.Color = sunLight.color;
                        sunlightSettings.Intensity = sunLight.intensity;

                        Vector3 eulerAngles = sunLight.transform.eulerAngles;
                        sunlightSettings.XRotation = eulerAngles.x;
                        sunlightSettings.YRotation = eulerAngles.y;
                        sunlightSettings.ZRotation = eulerAngles.z;
                    }
                }

                profile.SetLightingCache(lightingScene, lightingSettings, renderSettings, sunlightSettings);
            }
        }

        /// <summary>
        /// Ensures that if a content scene is loaded, that scene is set active, rather than a lighting or manager scene.
        /// </summary>
        private void EditorUpdateContentScenes(bool activeSceneDirty)
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
        private void EditorUpdateManagerScene()
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
                        return;
                    }

                    if (!foundToolkitInstance)
                    {
                        GameObject mrtkGo = new GameObject("MixedRealityToolkit");
                        MixedRealityToolkit toolkitInstance = mrtkGo.AddComponent<MixedRealityToolkit>();

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

        /// <summary>
        /// If a lighting scene is being used, this ensures that at least one lighting scene is loaded in editor.
        /// </summary>
        private void EditorUpdateLightingScene(bool heirarchyDirty)
        {
            if (!profile.UseLightingScene || !profile.EditorManageLoadedScenes)
            {
                return;
            }

            if (string.IsNullOrEmpty(ActiveLightingScene))
            {
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
                                EditorEnforceLightingSceneTypes(scene);
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

        /// <summary>
        /// Ensures that only approved component types are present in lighting scenes.
        /// </summary>
        private void EditorEnforceLightingSceneTypes(Scene scene)
        {
            if (EditorSceneManager.sceneCount == 1)
            {   // There's nowhere to move invalid objects to.
                return;
            }

            List<Component> violations = new List<Component>();
            if (EditorSceneUtils.EnforceSceneComponents(scene, profile.PermittedLightingSceneComponentTypes, violations))
            {
                Scene targetScene = default(Scene);
                for (int i = 0; i < EditorSceneManager.sceneCount; i++)
                {
                    targetScene = EditorSceneManager.GetSceneAt(i);
                    if (targetScene.path != scene.path)
                    {   // We'll move invalid items to this scene
                        break;
                    }
                }

                if (!targetScene.IsValid() || !targetScene.isLoaded)
                {   // Something's gone wrong - don't proceed
                    return;
                }

                HashSet<Transform> rootObjectsToMove = new HashSet<Transform>();
                foreach (Component component in violations)
                {
                    rootObjectsToMove.Add(component.transform.root);
                }

                List<string> rootObjectNames = new List<string>();
                // Build a list of root objects so they know what's being moved
                foreach (Transform rootObject in rootObjectsToMove)
                {
                    rootObjectNames.Add(rootObject.name);
                }

                EditorUtility.DisplayDialog(
                    "Invalid components found in " + scene.name,
                    "Only lighting-related componets are permitted. The following gameobjects will be moved to another scene:\n\n"
                    + String.Join("\n", rootObjectNames)
                    + "\n\nTo disable this warning, un-check 'EditorEnforceLightingSceneTypes' in your SceneSystem profile.", "OK");

                try
                {
                    foreach (Transform rootObject in rootObjectsToMove)
                    {
                        EditorSceneManager.MoveGameObjectToScene(rootObject.gameObject, targetScene);
                    }

                    EditorGUIUtility.PingObject(rootObjectsToMove.FirstOrDefault());
                } 
                catch (Exception)
                {
                    // This can happen if the move object operation fails. No big deal, we'll try again next time.
                    return;
                }
            }
        }

        /// <summary>
        /// Adds all scenes from profile into build settings.
        /// </summary>
        private void EditorUpdateBuildSettings()
        {
            if (!profile.EditorManageBuildSettings)
            {   // Nothing to do here
                return;
            }

            if (profile.UseManagerScene)
            {
                if (EditorSceneUtils.AddSceneToBuildSettings(
                    profile.ManagerScene,
                    cachedBuildScenes,
                    EditorSceneUtils.BuildIndexTarget.First))
                {
                    cachedBuildScenes = EditorBuildSettings.scenes;
                }
            }

            foreach (SceneInfo contentScene in profile.ContentScenes)
            {
                if (EditorSceneUtils.AddSceneToBuildSettings(
                    contentScene,
                    cachedBuildScenes, 
                    EditorSceneUtils.BuildIndexTarget.None))
                {
                    cachedBuildScenes = EditorBuildSettings.scenes;
                }
            }

            if (profile.UseLightingScene)
            {
                foreach (SceneInfo lightingScene in profile.LightingScenes)
                {   // Make sure ALL lighting scenes are added to build settings
                    if (EditorSceneUtils.AddSceneToBuildSettings(
                        lightingScene, 
                        cachedBuildScenes,
                        EditorSceneUtils.BuildIndexTarget.Last))
                    {
                        cachedBuildScenes = EditorBuildSettings.scenes;
                    }
                }
            }

            EditorCheckForSceneNameDuplicates();
        }

        /// <summary>
        /// Ensures that there are no scenes in build settings with duplicate names.
        /// If any are found, a resolve duplicates window is launched.
        /// </summary>
        private void EditorCheckForSceneNameDuplicates()
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