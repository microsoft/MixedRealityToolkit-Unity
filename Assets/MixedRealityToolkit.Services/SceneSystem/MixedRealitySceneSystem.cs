// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
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
       
        // Internal scene operation info
        private bool managerSceneOpInProgress;
        private float managerSceneOpProgress;

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
        public bool SceneOperationInProgress { get; private set; } = false;

        /// <inheritdoc />
        public float SceneOperationProgress { get; private set; } = 0;

        /// <inheritdoc />
        public bool LightingOperationInProgress { get; private set; } = false;

        /// <inheritdoc />
        public float LightingOperationProgress { get; private set; } = 0;

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
#if UNITY_EDITOR
            OnEditorInitialize();
#endif

            if (!Application.isPlaying)
            {
                return;
            }

            if (profile.UseManagerScene)
            {
                SetManagerScene(profile.ManagerScene.Name);
            }

            if (profile.UseLightingScene)
            {
                SetLightingScene(profile.DefaultLightingScene.Name, LightingSceneTransitionType.None);
            }
        }

        /// <inheritdoc />
        public override void Enable()
        {
#if UNITY_EDITOR
            OnEditorDisable();
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
        public override void Destroy()
        {
#if UNITY_EDITOR
            OnEditorDestroy();
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
            await LoadScenesInternal(new string[] { sceneToLoad }, SceneType.Content, mode, activationToken);
        }

        /// <inheritdoc />
        public async Task UnloadContent(string sceneToUnload)
        {
            await UnloadScenesInternal(new string[] { sceneToUnload }, SceneType.Content);
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
            await LoadScenesInternal(profile.GetContentSceneNamesByTag(tag), SceneType.Content, mode, activationToken);
        }

        /// <inheritdoc />
        public async Task UnloadContentByTag(string tag)
        {
            await UnloadScenesInternal(profile.GetContentSceneNamesByTag(tag), SceneType.Content);
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

            if (!CanSceneOpProceed(SceneType.Lighting))
            {
                Debug.LogError("Attempting to perform a scene op when a scene op is already in progress.");
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
                await LoadScenesInternal(new string[] { newLightingSceneName }, SceneType.Lighting, LoadSceneMode.Additive);

                // Unload the other lighting scenes
                await UnloadScenesInternal(lightingSceneNames, SceneType.Lighting);
            }
        }

        /// <summary>
        /// Loads the manager scene.
        /// </summary>
        /// <param name="managerSceneName"></param>
        private async void SetManagerScene(string managerSceneName)
        {
            Scene scene = SceneManager.GetSceneByName(managerSceneName);
            if (scene.IsValid() && !scene.isLoaded)
            {   // If the manager scene is already loaded, don't proceed.
                return;
            }

            await LoadScenesInternal(new string[] { managerSceneName }, SceneType.Manager);
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

            // If we're using an activation token let it know that we're NOT ready to proceed
            activationToken?.SetReadyToProceed(false);

            SetSceneOpProgress(true, 0, sceneType);

            // Validate our scenes
            List<string> validNames = new List<string>();
            List<int> validIndexes = new List<int>();

            foreach (string sceneName in scenesToLoad)
            {
                // See if scene exists
                Scene scene;
                int sceneIndex;
                if (!RuntimeSceneUtils.FindScene(sceneName, out scene, out sceneIndex))
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
                    // Set this to true unless we have an activation token
                    sceneOp.allowSceneActivation = (activationToken != null) ? activationToken.AllowSceneActivation : true;
                    loadSceneOps.Add(sceneOp);
                }

                // Now wait for all async operations to complete
                bool completedAllSceneOps = false;

                while (!completedAllSceneOps)
                {
                    completedAllSceneOps = true;
                    bool readyToProceed = false;
                    bool allowSceneActivation = (activationToken != null) ? activationToken.AllowSceneActivation : true;

                    // Go through all the load scene ops and see if we're ready to be activated
                    float sceneOpProgress = 0;
                    for (int i = 0; i < loadSceneOps.Count; i++)
                    {
                        // Set allow scene activation
                        // (This can be set to true by user before ReadyToProceed is set)
                        loadSceneOps[i].allowSceneActivation = allowSceneActivation;

                        if (loadSceneOps[i].isDone)
                        {   // Sometimes if a scene is small enough, progress will get reset to 0 before you even have a chance to check it
                            // This is true EVEN IF you've set allowSceneActivation to false
                            // So use isDone as a failsafe
                            sceneOpProgress += 1;
                        }
                        else
                        {
                            readyToProceed |= loadSceneOps[i].progress >= SceneActivationLoadProgress;
                            sceneOpProgress += loadSceneOps[i].progress;
                            completedAllSceneOps = false;
                        }
                    }

                    // Let the activation know whether we're ready
                    activationToken?.SetReadyToProceed(readyToProceed);

                    sceneOpProgress = Mathf.Clamp01(SceneOperationProgress / totalSceneOps);

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
                    Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
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
                if (!RuntimeSceneUtils.FindScene(sceneName, out scene, out sceneIndex))
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
                    sceneOpProgress = Mathf.Clamp01(SceneOperationProgress / totalSceneOps);

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
                    Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
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
                    managerSceneOpInProgress = inProgress;
                    managerSceneOpProgress = progress;
                    break;

                case SceneType.Content:
                    SceneOperationInProgress = inProgress;
                    SceneOperationProgress = progress;
                    break;

                case SceneType.Lighting:
                    LightingOperationInProgress = inProgress;
                    LightingOperationProgress = progress;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private bool CanSceneOpProceed(SceneType sceneType)
        {
            switch (sceneType)
            {
                case SceneType.Manager:
                    return !managerSceneOpInProgress;

                case SceneType.Content:
                    return !SceneOperationInProgress;

                case SceneType.Lighting:
                    return !LightingOperationInProgress;

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
                    OnLightingLoaded?.Invoke(sceneNames[0]);
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
                    OnWillLoadLighting?.Invoke(sceneNames[0]);
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
                    OnWillUnloadLighting?.Invoke(sceneNames[0]);
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
                    OnLightingUnloaded?.Invoke(sceneNames[0]);
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
            RuntimeSceneUtils.FindScene(sceneName, out scene, out int sceneIndex);
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
}