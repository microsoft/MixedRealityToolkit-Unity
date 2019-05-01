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
    public partial class MixedRealitySceneSystem : BaseCoreSystem, IMixedRealitySceneSystem
    {
        public MixedRealitySceneSystem(
            IMixedRealityServiceRegistrar registrar,
            MixedRealitySceneSystemProfile profile) : base(registrar, profile)
        {
            this.profile = profile;
        }

        private MixedRealitySceneSystemProfile profile;

        /// <inheritdoc />
        public bool SceneOpInProgress { get; private set; }

        /// <inheritdoc />
        public float SceneOpProgress { get; private set; }

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

        /// <inheritdoc />
        public uint SourceId { get; } = 0;

        /// <inheritdoc />
        public string SourceName { get; } = "Mixed Reality Scene System";

        public override void Initialize()
        {
#if UNITY_EDITOR
            OnEditorInitialize();
#endif
        }

        public override void Update()
        {

        }

        /// <inheritdoc />
        public async Task LoadScenes(IEnumerable<string> scenesToLoad)
        {
            if (SceneOpInProgress)
            {
                Debug.LogError("Attempting to perform a scene op when a scene op is already in progress.");
                return;
            }

            SceneOpInProgress = true;
            SceneOpProgress = 0;

            // Validate our scenes
            List<int> validScenesToLoad = new List<int>();

            foreach (string sceneName in scenesToLoad)
            {
                // See if scene exists
                Scene scene;
                int sceneIndex;
                if (!FindScene(sceneName, out scene, out sceneIndex))
                {
                    Debug.LogError("Can't load invalid scene " + sceneName);
                    SceneOpInProgress = false;
                    return;
                }
                else
                {
                    validScenesToLoad.Add(sceneIndex);
                }
            }

            int totalSceneOps = validScenesToLoad.Count;
            if (totalSceneOps < 1)
            {
                Debug.LogWarning("No valid scenes found to load.");
            }
            
            // Load our scenes
            if (validScenesToLoad.Count > 0)
            {
                List<AsyncOperation> loadSceneOps = new List<AsyncOperation>();
                foreach (int sceneIndex in validScenesToLoad)
                {
                    Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
                    if (scene.isLoaded)
                        continue;

                    AsyncOperation sceneOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
                    sceneOp.allowSceneActivation = true;
                    loadSceneOps.Add(sceneOp);
                }

                // Now wait for all async operations to complete
                bool completedAllSceneOps = false;
                while (!completedAllSceneOps)
                {
                    completedAllSceneOps = true;

                    SceneOpProgress = 0;
                    for (int i = 0; i < loadSceneOps.Count; i++)
                    {
                        SceneOpProgress += loadSceneOps[i].progress;
                        completedAllSceneOps &= loadSceneOps[i].isDone;
                    }
                    SceneOpProgress = Mathf.Clamp01(SceneOpProgress / totalSceneOps);
                    await Task.Yield();
                }
            }

            // Wait for all scenes to be fully loaded before proceeding
            bool scenesLoadedAndActivated = false;
            while (!scenesLoadedAndActivated)
            {
                scenesLoadedAndActivated = true;
                foreach (int sceneIndex in validScenesToLoad)
                {
                    Scene scene = SceneManager.GetSceneAt(sceneIndex);
                    scenesLoadedAndActivated &= (scene.IsValid() & scene.isLoaded);
                }
                await Task.Yield();
            }

            // We're done!
            SceneOpProgress = 1;
            SceneOpInProgress = false;
        }

        /// <inheritdoc />
        public async Task UnloadScenes(IEnumerable<string> scenesToUnload)
        {
            if (SceneOpInProgress)
            {
                Debug.LogError("Attempting to perform a scene op when a scene op is already in progress.");
                return;
            }

            SceneOpInProgress = true;

            List<int> validScenesToUnload = new List<int>();

            foreach (string sceneName in scenesToUnload)
            {
                // See if scene exists
                Scene scene;
                int sceneIndex;
                if (!FindScene(sceneName, out scene, out sceneIndex))
                {
                    Debug.LogError("Can't unload invalid scene " + sceneName);
                    SceneOpInProgress = false;
                    return;
                }
                else
                {
                    validScenesToUnload.Add(sceneIndex);
                }
            }

            int totalSceneOps = validScenesToUnload.Count;
            if (totalSceneOps < 1)
            {
                Debug.LogWarning("No valid scenes found to unload.");
            }

            // Unload our scenes
            if (validScenesToUnload.Count > 0)
            {
                List<AsyncOperation> unloadSceneOps = new List<AsyncOperation>();
                foreach (int sceneIndex in validScenesToUnload)
                {
                    Scene scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
                    if (!scene.isLoaded)
                        continue;

                    AsyncOperation sceneOp = SceneManager.UnloadSceneAsync(sceneIndex);
                    unloadSceneOps.Add(sceneOp);
                }

                // Now wait for all async operations to complete
                bool completedAllSceneOps = false;
                while (!completedAllSceneOps)
                {
                    completedAllSceneOps = true;

                    SceneOpProgress = 0;
                    for (int i = 0; i < unloadSceneOps.Count; i++)
                    {
                        SceneOpProgress += unloadSceneOps[i].progress;
                        completedAllSceneOps &= unloadSceneOps[i].isDone;
                    }
                    SceneOpProgress = Mathf.Clamp01(SceneOpProgress / totalSceneOps);
                    await Task.Yield();
                }
            }

            // Wait for all scenes to be fully unloaded before proceeding
            bool scenesUnloaded = false;
            while (!scenesUnloaded)
            {
                scenesUnloaded = true;
                foreach (int sceneIndex in validScenesToUnload)
                {
                    Scene scene = SceneManager.GetSceneAt(sceneIndex);
                    scenesUnloaded &= !scene.isLoaded;
                }
                await Task.Yield();
            }

            // We're done!
            SceneOpProgress = 1;
            SceneOpInProgress = false;
        }

        private bool FindScene(string sceneName, out Scene scene, out int sceneIndex)
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

        private static string GetSceneNameFromScenePath(string scenePath)
        {
            return System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }
    }
}