// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// The default implementation of the <see cref="Microsoft.MixedReality.Toolkit.SceneSystem.IMixedRealitySceneSystem"/>
    /// Because so much of this service's functionality is editor-only, it has been split into a partial class.
    /// This part handles the runtime parts of the service.
    /// </summary>
    [DocLink("http://TEMPORARYURL")]
    public partial class MixedRealitySceneSystem : BaseCoreSystem, IMixedRealitySceneSystem
    {
        /// <summary>
        /// Async load operation progress amount indicating that we're ready to activate a scene.
        /// https://docs.unity3d.com/ScriptReference/AsyncOperation-progress.html
        /// </summary>
        const float SceneActivationLoadProgress = 0.9f;

        /// <summary>
        /// Used by internal load methods to decide which actions to invoke.
        /// </summary>
        private enum SceneType
        {
            Manager = 0,
            Content = 1,
            Lighting = 2,
        }

        public MixedRealitySceneSystem(
            IMixedRealityServiceRegistrar registrar,
            MixedRealitySceneSystemProfile profile) : base(registrar, profile)
        {
            this.profile = profile;
        }

        private MixedRealitySceneSystemProfile profile;

        #region Actions

        /// <inheritdoc />
        public Action<IEnumerable<string>> OnWillLoadContent { get; set; }

        /// <inheritdoc />
        public Action<IEnumerable<string>> OnContentLoaded { get; set; }

        /// <inheritdoc />
        public Action<IEnumerable<string>> OnWillUnloadContent { get; set; }

        /// <inheritdoc />
        public Action<IEnumerable<string>> OnContentUnloaded { get; set; }

        /// <inheritdoc />
        public Action<string> OnWillLoadLighting { get; set; }

        /// <inheritdoc />
        public Action<string> OnLightingLoaded { get; set; }

        /// <inheritdoc />
        public Action<string> OnWillUnloadLighting { get; set; }

        /// <inheritdoc />
        public Action<string> OnLightingUnloaded { get; set; }

        /// <inheritdoc />
        public Action<string> OnWillLoadScene { get; set; }

        /// <inheritdoc />
        public Action<string> OnSceneLoaded { get; set; }

        /// <inheritdoc />
        public Action<string> OnWillUnloadScene { get; set; }

        /// <inheritdoc />
        public Action<string> OnSceneUnloaded { get; set; }

        #endregion

        #region Properties

        /// <inheritdoc />
        public bool SceneOpInProgress { get; private set; }

        /// <inheritdoc />
        public float SceneOpProgress { get; private set; }

        /// <inheritdoc />
        public string ActiveLightingScene { get; private set; } = string.Empty;

        /// <inheritdoc />
        public bool WaitingToProceed { get; private set; } = false;

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName { get; } = "Mixed Reality Scene System";

        /// <summary>
        /// Returns the manager scene found in profile.
        /// </summary>
        public SceneInfo ManagerScene { get { return profile.ManagerScene; } }

        /// <summary>
        /// Returns all lighting scenes found in profile.
        /// </summary>
        public IEnumerable<SceneInfo> LightingScenes { get { return profile.LightingScenes; } }
        
        /// <summary>
        /// Returns all content scenes found in profile.
        /// </summary>
        public IEnumerable<SceneInfo> ContentScenes { get { return profile.ContentScenes; } }

        /// <summary>
        /// Returns all content tags found in profile scenes.
        /// </summary>
        public IEnumerable<string> ContentTags { get { return profile.ContentTags; } }

        #endregion

        #region Service Methods

        /// <inheritdoc />
        public override void Initialize()
        {
            if (profile.UseLightingScene)
            {
                SetLightingScene(profile.DefaultLightingScene.Name, LightingSceneTransitionType.None);
            }

#if UNITY_EDITOR
            OnEditorInitialize();
#endif
        }

        /// <inheritdoc />
        public override void Disable()
        {
#if UNITY_EDITOR
            OnEditorDisable();
#endif
        }

        /// <inheritdoc />
        public override void Update()
        {

        }

        #endregion

        #region Scene Operations

        /// <inheritdoc />
        public async Task LoadContent(string sceneToLoad, LoadSceneMode mode = LoadSceneMode.Additive, SceneActivationToken activationToken = null)
        {
            await LoadContent(new string[] { sceneToLoad });
        }

        /// <inheritdoc />
        public async Task UnloadContent(string sceneToUnload)
        {
            await UnloadContent(new string[] { sceneToUnload });
        }

        /// <inheritdoc />
        public async Task LoadContent(IEnumerable<string> scenesToLoad, LoadSceneMode mode = LoadSceneMode.Additive, SceneActivationToken activationToken = null)
        {
            await LoadScenesInternal(scenesToLoad, SceneType.Content, mode, activationToken);
        }

        /// <inheritdoc />
        public async Task UnloadContent(IEnumerable<string> scenesToUnload)
        {
            await UnloadScenesInternal(scenesToUnload, SceneType.Content);
        }

        /// <inheritdoc />
        public async Task LoadContentByTag(string tag, LoadSceneMode mode = LoadSceneMode.Additive, SceneActivationToken activationToken = null)
        {
            await LoadContent(profile.GetContentSceneNamesByTag(tag));
        }

        /// <inheritdoc />
        public async Task UnloadContentByTag(string tag)
        {
            await UnloadContent(profile.GetContentSceneNamesByTag(tag));
        }

        /// <inheritdoc />
        public bool IsContentLoaded(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.IsValid() && scene.isLoaded;
        }

        /// <inheritdoc />
        public async void SetLightingScene(string newLightingSceneName, LightingSceneTransitionType transitionType = LightingSceneTransitionType.None)
        {
            if (ActiveLightingScene == newLightingSceneName)
            {   // Nothing to do here
                return;
            }

            SceneInfo lightingScene;
            if (!string.IsNullOrEmpty(newLightingSceneName) && !profile.GetLightingSceneObject(newLightingSceneName, out lightingScene))
            {   // Make sure we don't try to load a non-existent scene
                Debug.LogWarning("Couldn't find lighting scene " + newLightingSceneName + " in profile - taking no action.");
                return;
            }

            ActiveLightingScene = newLightingSceneName;

            if (Application.isPlaying)
            {
                List<string> lightingSceneNames = new List<string>();
                // Create a list of lighting scenes to unload
                foreach (SceneInfo lso in LightingScenes)
                {
                    if (lso.Name != newLightingSceneName)
                    {
                        lightingSceneNames.Add(lso.Name);
                    }
                }

                // Load the new lighting scene immediately
                await LoadContent(newLightingSceneName);

                // Unload the other lighting scenes
                await UnloadContent(lightingSceneNames);
            }
        }

        /// <summary>
        /// Internal method to handle scene loads
        /// </summary>
        /// <param name="scenesToLoad"></param>
        /// <param name="mode"></param>
        /// <param name="activationToken"></param>
        /// <returns></returns>
        private async Task LoadScenesInternal(IEnumerable<string> scenesToLoad, SceneType sceneType, LoadSceneMode mode = LoadSceneMode.Additive, SceneActivationToken activationToken = null)
        {
            if (!CanSceneOpProceed(sceneType))
            {
                Debug.LogError("Attempting to perform a scene op when a scene op is already in progress.");
                return;
            }

            if (activationToken != null)
            {   // If we're using an activation token let it know that we're NOT ready to proceed
                activationToken.ReadyToProceed = false;
            }

            SetSceneOpProgress(true, 0, sceneType);

            // Validate our scenes
            List<string> validNames = new List<string>();
            List<int> validIndexes = new List<int>();

            foreach (string sceneName in scenesToLoad)
            {
                // See if scene exists
                Scene scene;
                int sceneIndex;
                if (!SceneUtilities.FindScene(sceneName, out scene, out sceneIndex))
                {
                    Debug.LogError("Can't load invalid scene " + sceneName);
                }
                else
                {
                    validIndexes.Add(sceneIndex);
                    validNames.Add(sceneName);
                }
            }

            int totalSceneOps = validIndexes.Count;
            if (totalSceneOps < 1)
            {
                Debug.LogWarning("No valid scenes found to load.");
                SetSceneOpProgress(false, 1, sceneType);
                return;
            }

            // We're about to load scenes - let everyone know
            InvokeWillLoadActions(validNames, sceneType);

            // Load our scenes
            if (validIndexes.Count > 0)
            {
                List<AsyncOperation> loadSceneOps = new List<AsyncOperation>();
                foreach (int sceneIndex in validIndexes)
                {
                    Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
                    if (scene.isLoaded)
                        continue;

                    AsyncOperation sceneOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
                    // Set this to false initially
                    sceneOp.allowSceneActivation = false;
                    loadSceneOps.Add(sceneOp);
                }

                // Now wait for all async operations to complete
                bool completedAllSceneOps = false;

                while (!completedAllSceneOps)
                {
                    completedAllSceneOps = true;
                    bool readyToProceed = false;

                    // Go through all the load scene ops and see if we're ready to be activated
                    float sceneOpProgress = 0;
                    for (int i = 0; i < loadSceneOps.Count; i++)
                    {   // See if all the load ops are ready for activation
                        if (activationToken != null)
                        {
                            activationToken.ReadyToProceed = true;
                        }

                        readyToProceed |= loadSceneOps[i].progress >= SceneActivationLoadProgress;
                        sceneOpProgress += loadSceneOps[i].progress;
                    }

                    // If ALL the scenes are ready for activation, update scene activation
                    if (readyToProceed)
                    {   // Allow scene activation by default
                        bool allowSceneActivation = true;
                        if (activationToken != null)
                        {
                            activationToken.ReadyToProceed = true;
                            allowSceneActivation = activationToken.AllowSceneActivation;
                        }

                        for (int i = 0; i < loadSceneOps.Count; i++)
                        {   // Now that we're in scene activation stage, check whether the scene is actually done
                            completedAllSceneOps &= loadSceneOps[i].isDone;
                            loadSceneOps[i].allowSceneActivation = allowSceneActivation;
                        }
                    }

                    sceneOpProgress = Mathf.Clamp01(SceneOpProgress / totalSceneOps);

                    SetSceneOpProgress(true, sceneOpProgress, sceneType);

                    await Task.Yield();
                }
            }

            // Wait for all scenes to be fully loaded before proceeding
            bool scenesLoadedAndActivated = false;
            while (!scenesLoadedAndActivated)
            {
                scenesLoadedAndActivated = true;
                foreach (int sceneIndex in validIndexes)
                {
                    Scene scene = SceneManager.GetSceneAt(sceneIndex);
                    scenesLoadedAndActivated &= (scene.IsValid() & scene.isLoaded);
                }
                await Task.Yield();
            }

            // We're done!
            SetSceneOpProgress(false, 1, sceneType);

            InvokeLoadedActions(validNames, sceneType);
        }

        /// <summary>
        /// Internal method to handles scene unloads
        /// </summary>
        /// <param name="scenesToUnload"></param>
        /// <param name="sceneType"></param>
        /// <returns></returns>
        private async Task UnloadScenesInternal(IEnumerable<string> scenesToUnload, SceneType sceneType)
        {
            if (!CanSceneOpProceed(sceneType))
            {
                Debug.LogError("Attempting to perform a scene op when a scene op is already in progress.");
                return;
            }

            SetSceneOpProgress(true, 0, sceneType);

            List<string> validNames = new List<string>();
            List<int> validIndexes = new List<int>();

            foreach (string sceneName in scenesToUnload)
            {
                // See if scene exists
                Scene scene;
                int sceneIndex;
                if (!SceneUtilities.FindScene(sceneName, out scene, out sceneIndex))
                {
                    Debug.LogError("Can't unload invalid scene " + sceneName);
                }
                else
                {
                    validIndexes.Add(sceneIndex);
                    validNames.Add(sceneName);
                }
            }

            int totalSceneOps = validIndexes.Count;
            if (totalSceneOps < 1)
            {
                Debug.LogWarning("No valid scenes found to unload.");
                SetSceneOpProgress(false, 1, sceneType);
                return;
            }

            // Invoke our actions
            InvokeWillUnloadActions(validNames, sceneType);

            // Unload our scenes
            if (validIndexes.Count > 0)
            {
                List<AsyncOperation> unloadSceneOps = new List<AsyncOperation>();
                foreach (int sceneIndex in validIndexes)
                {
                    Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
                    if (!scene.isLoaded)
                        continue;

                    AsyncOperation sceneOp = SceneManager.UnloadSceneAsync(sceneIndex);
                    unloadSceneOps.Add(sceneOp);
                }

                // Now wait for all async operations to complete
                bool completedAllSceneOps = false;
                float sceneOpProgress = 0;
                while (!completedAllSceneOps)
                {
                    completedAllSceneOps = true;
                    sceneOpProgress = 0;
                    for (int i = 0; i < unloadSceneOps.Count; i++)
                    {
                        sceneOpProgress += unloadSceneOps[i].progress;
                        completedAllSceneOps &= unloadSceneOps[i].isDone;
                    }
                    sceneOpProgress = Mathf.Clamp01(SceneOpProgress / totalSceneOps);

                    SetSceneOpProgress(true, sceneOpProgress, sceneType);

                    await Task.Yield();
                }
            }

            // Wait for all scenes to be fully unloaded before proceeding
            bool scenesUnloaded = false;
            while (!scenesUnloaded)
            {
                scenesUnloaded = true;
                foreach (int sceneIndex in validIndexes)
                {
                    Scene scene = SceneManager.GetSceneAt(sceneIndex);
                    scenesUnloaded &= !scene.isLoaded;
                }
                await Task.Yield();
            }

            // We're done!
            SetSceneOpProgress(false, 1, sceneType);

            // Invoke our actions
            InvokeUnloadedActions(validNames, sceneType);          
        }
        
        private void SetSceneOpProgress(bool inProgress, float progress, SceneType sceneType)
        {
            switch (sceneType)
            {
                case SceneType.Manager:
                    // Do nothing
                    break;

                case SceneType.Content:
                    SceneOpInProgress = inProgress;
                    SceneOpProgress = progress;
                    break;

                case SceneType.Lighting:
                    // Do nothing
                    break;
            }
        }

        private bool CanSceneOpProceed(SceneType sceneType)
        {
            switch (sceneType)
            {
                case SceneType.Manager:
                    // Manager scene ops can always proceed
                    return true;

                case SceneType.Content:
                    // Content scene ops can only proceed if another scene op is not in progress
                    return !SceneOpInProgress;

                case SceneType.Lighting:
                    // Lighting scene ops can always proceed
                    return true;

                default:
                    throw new NotImplementedException();
            }
        }

        private void InvokeLoadedActions(List<string> sceneNames, SceneType sceneType)
        {
            foreach (string sceneName in sceneNames)
            {  // Announce scenes individually regardless of type
                OnSceneLoaded?.Invoke(sceneName);
            }

            switch (sceneType)
            {
                case SceneType.Content:
                    // Announce content as a set
                    OnContentLoaded?.Invoke(sceneNames);
                    break;

                case SceneType.Lighting:
                    // We only handle lighting scenes one at a time
                    Debug.Assert(sceneNames.Count == 1);
                    OnLightingLoaded(sceneNames[0]);
                    break;

                default:
                    // Don't announce other types of scenes invidually
                    break;
            }
        }

        private void InvokeWillLoadActions(List<string> sceneNames, SceneType sceneType)
        {
            foreach (string sceneName in sceneNames)
            {   // Announce scenes individually regardless of type
                OnWillLoadScene?.Invoke(sceneName);
            }

            switch (sceneType)
            {
                case SceneType.Content:
                    // Announce content as a set
                    OnWillLoadContent?.Invoke(sceneNames);
                    break;

                case SceneType.Lighting:
                    // We only handle lighting scenes one at a time
                    Debug.Assert(sceneNames.Count == 1);
                    OnWillLoadLighting(sceneNames[0]);
                    break;

                default:
                    // Don't announce other types of scenes invidually
                    break;
            }
        }

        private void InvokeWillUnloadActions(List<string> sceneNames, SceneType sceneType)
        {
            foreach (string sceneName in sceneNames)
            {  // Announce scenes individually regardless of type
                OnWillUnloadScene?.Invoke(sceneName);
            }

            switch (sceneType)
            {
                case SceneType.Content:
                    // Announce content as a set
                    OnWillUnloadContent?.Invoke(sceneNames);
                    break;

                case SceneType.Lighting:
                    // We only handle lighting scenes one at a time
                    Debug.Assert(sceneNames.Count == 1);
                    OnWillUnloadLighting(sceneNames[0]);
                    break;

                default:
                    // Don't announce other types of scenes invidually
                    break;
            }
        }

        private void InvokeUnloadedActions(List<string> sceneNames, SceneType sceneType)
        {
            foreach (string sceneName in sceneNames)
            {  // Announce scenes individually regardless of type
                OnSceneUnloaded?.Invoke(sceneName);
            }

            switch (sceneType)
            {
                case SceneType.Content:
                    // Announce content as a set
                    OnContentUnloaded?.Invoke(sceneNames);
                    break;

                case SceneType.Lighting:
                    // We only handle lighting scenes one at a time
                    Debug.Assert(sceneNames.Count == 1);
                    OnLightingUnloaded(sceneNames[0]);
                    break;

                default:
                    // Don't announce other types of scenes invidually
                    break;
            }
        }
        
        #endregion

        #region Utilities

        /// <inheritdoc />
        public IEnumerable<Scene> GetScenes(IEnumerable<string> sceneNames)
        {
            foreach (string sceneName in sceneNames)
            {
                yield return GetScene(sceneName);
            }
        }

        /// <inheritdoc />
        public Scene GetScene(string sceneName)
        {
            Scene scene = default(Scene);
            SceneUtilities.FindScene(sceneName, out scene, out int sceneIndex);
            return scene;
        }

        #endregion

        #region IEqualityComparer

        /// <inheritdoc />
        bool IEqualityComparer.Equals(object x, object y)
        {
            // There shouldn't be other Boundary Managers to compare to.
            return false;
        }

        /// <inheritdoc />
        int IEqualityComparer.GetHashCode(object obj)
        {
            return Mathf.Abs(SourceName.GetHashCode());
        }

        #endregion
    }

    public static class SceneUtilities
    {
        public static string GetSceneNameFromScenePath(string scenePath)
        {
            return System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }

        public static bool FindScene(string sceneName, out Scene scene, out int sceneIndex)
        {
            scene = default(Scene);
            sceneIndex = -1;
            // This is the only method to get all scenes (including unloaded)
            List<Scene> allScenesInProject = new List<Scene>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                // This absurdity is necessary due to a long-standing Unity bug
                // https://issuetracker.unity3d.com/issues/scenemanager-dot-getscenebybuildindex-dot-name-returns-an-empty-string-if-scene-is-not-loaded

                string pathToScene = SceneUtility.GetScenePathByBuildIndex(i);
                string checkSceneName = System.IO.Path.GetFileNameWithoutExtension(pathToScene);
                if (checkSceneName == sceneName)
                {
                    scene = SceneManager.GetSceneByBuildIndex(i);
                    sceneIndex = i;
                    return true;
                }
            }
            return false;
        }
    }
}