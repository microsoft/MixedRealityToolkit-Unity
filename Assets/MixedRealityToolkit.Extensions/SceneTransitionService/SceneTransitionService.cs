// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    [MixedRealityExtensionService(SupportedPlatforms.WindowsStandalone|SupportedPlatforms.MacStandalone|SupportedPlatforms.LinuxStandalone|SupportedPlatforms.WindowsUniversal)]
    public class SceneTransitionService : BaseExtensionService, ISceneTransitionService, IMixedRealityExtensionService
    {
        public bool TransitionInProgress { get; private set; }
        public bool AwaitingConfirmation { get; private set; }

        public Color FadeColor { get; set; }
        public float FadeInTime { get; set; }
        public float FadeOutTime { get; set; }
        public CameraFaderTargets FadeTargets { get; set; }

        private SceneTransitionServiceProfile sceneTransitionServiceProfile;
        private GameObject progressIndicatorObject;
        private IProgressIndicator progressIndicator;
        private ICameraFader cameraFader;
        private List<Camera> customFadeTargetCameras = new List<Camera>();

        public SceneTransitionService(IMixedRealityServiceRegistrar registrar,  string name,  uint priority,  BaseMixedRealityProfile profile) : base(registrar, name, priority, profile) 
		{
            sceneTransitionServiceProfile = (SceneTransitionServiceProfile)profile;

            FadeColor = sceneTransitionServiceProfile.FadeColor;
            FadeInTime = sceneTransitionServiceProfile.FadeInTime;
            FadeOutTime = sceneTransitionServiceProfile.FadeOutTime;
            FadeTargets = sceneTransitionServiceProfile.FadeTargets;
        }

        #region public methods

        public override void Initialize()
        {
         
        }
        
        public override void Update()
        {

        }
        
        public override void Destroy()
        {
            CleanUpProgressIndicator();
            CleanUpCameraFader();
        }

        #endregion

        #region ISceneTransitionService implementation

        public Task TransitionToScene(string sceneToLoad, bool awaitConfirmationForTransitionOut = false)
        {
            return TransitionToScene(new string[] { sceneToLoad }, new string[] { }, awaitConfirmationForTransitionOut);
        }

        public Task TransitionToScene(IEnumerable<string> scenesToLoad, bool awaitConfirmationForTransitionOut = false)
        {
            return TransitionToScene(scenesToLoad, new string[] { }, awaitConfirmationForTransitionOut);
        }

        public async Task TransitionToScene(IEnumerable<string> scenesToLoad, IEnumerable<string> scenesToUnload = null, bool awaitConfirmationForTransitionOut = false)
        {
            if (TransitionInProgress)
            {
                Debug.LogError ("Attempting to transition to scene when transition is already in progress.");
                return;
            }

            TransitionInProgress = true;
            AwaitingConfirmation = false;

            CreateProgressIndicator();
            CreateCameraFader();

            if (sceneTransitionServiceProfile.UseFadeColor)
            {   // Fade out before proceeding
                await cameraFader.FadeOutAsync(FadeOutTime, FadeColor, GatherFadeTargetCameras());
            }

            if (sceneTransitionServiceProfile.UseProgressIndicator)
            {   // Activate the progress indicator and wait for it to spin up
                await progressIndicator.OpenAsync();
            }
                        
            // Validate our scenes
            List<int> validScenesToUnload = new List<int>();
            List<int> validScenesToLoad = new List<int>();

            foreach (string sceneName in scenesToUnload)
            {
                // See if scene exists
                Scene scene;
                int sceneIndex;
                if (!FindScene(sceneName, out scene, out sceneIndex))
                {
                    Debug.LogError("Can't unload invalid scene " + sceneName);
                    TransitionInProgress = false;
                    return;
                }
                else
                {
                    validScenesToUnload.Add(sceneIndex);
                }
            }

            foreach (string sceneName in scenesToLoad)
            {
                // See if scene exists
                Scene scene;
                int sceneIndex;
                if (!FindScene(sceneName, out scene, out sceneIndex))
                {
                    Debug.LogError("Can't load invalid scene " + sceneName);
                    TransitionInProgress = false;
                    return;
                }
                else
                {
                    validScenesToLoad.Add(sceneIndex);
                }
            }

            int totalSceneOps = validScenesToUnload.Count + validScenesToLoad.Count;
            if (totalSceneOps < 1)
            {
                Debug.LogWarning("No valid scenes found to load or unload.");
            }

            float totalProgress = 0;

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

                    totalProgress = 0;
                    for (int i = 0; i < unloadSceneOps.Count; i++)
                    {
                        totalProgress += unloadSceneOps[i].progress;
                        completedAllSceneOps &= unloadSceneOps[i].isDone;
                    }
                    totalProgress = Mathf.Clamp01(totalProgress / totalSceneOps);
                    progressIndicator.Progress = totalProgress;
                    await Task.Yield();
                }
            }

            float totalProgressBeforeLoad = totalProgress;

            // Wait a moment for Unity's scenes to finish unloading
            await Task.Delay(100);

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

                    totalProgress = 0;
                    for (int i = 0; i < loadSceneOps.Count; i++)
                    {
                        totalProgress += loadSceneOps[i].progress;
                        completedAllSceneOps &= loadSceneOps[i].isDone;
                    }
                    totalProgress = Mathf.Clamp01(totalProgressBeforeLoad + (totalProgress / totalSceneOps));
                    progressIndicator.Progress = totalProgress;
                    await Task.Yield();
                }
            }

            // Wait a moment for Unity's scenes to finish loading
            await Task.Delay(100);

            progressIndicator.Progress = 1;

            // If the user has requested that we wait before exiting the transition
            // don't proceed until we receive confirmation
            if (awaitConfirmationForTransitionOut)
            {
                AwaitingConfirmation = true;
                while (AwaitingConfirmation)
                {
                    await Task.Yield();
                }
            }

            // If we used the progress indicator, close it
            if (progressIndicator.State != ProgressIndicatorState.Closed)
            {
                await progressIndicator.CloseAsync();
            }

            // If we used the camera fader to fade out, fade back in
            if (cameraFader.State != CameraFaderState.Clear)
            {
                // Wait for camera to fade out
                await cameraFader.FadeInAsync(FadeInTime);
            }

            // We're done!
            TransitionInProgress = false;
        }

        public void SetCustomFadeTargetCameras(IEnumerable<Camera> customFadeTargetCameras)
        {
            this.customFadeTargetCameras.AddRange(customFadeTargetCameras);
        }

        public void ProceedWithTransition()
        {
            if (!TransitionInProgress)
            {
                Debug.LogWarning("No transition in progress. This action will have no effect.");
            }

            AwaitingConfirmation = false;
        }

        public async Task FadeOut()
        {
            if (TransitionInProgress)
            {
                Debug.LogWarning("A scene transition is already in progress. This would interrupt that transition. Taking no action.");
                return;
            }

            CreateCameraFader();

            switch (cameraFader.State)
            {
                case CameraFaderState.Clear:
                    // Ready to go!
                    break;

                case CameraFaderState.FadingOut:
                    Debug.LogWarning("Already fading out. Taking no action.");
                    break;

                case CameraFaderState.Opaque:
                    Debug.LogWarning("Already faded out. Taking no action.");
                    break;

                case CameraFaderState.FadingIn:
                    while (cameraFader.State == CameraFaderState.FadingIn)
                    {   // Wait until we're done fading in to fade back in
                        await Task.Yield();
                    }
                    break;
            }

            await cameraFader.FadeOutAsync(FadeInTime, FadeColor, GatherFadeTargetCameras());
        }

        public async Task FadeIn()
        {
            if (TransitionInProgress)
            {
                Debug.LogWarning("A scene transition is already in progress. This would interrupt that transition. Taking no action.");
                return;
            }

            CreateCameraFader();

            switch (cameraFader.State)
            {
                case CameraFaderState.Opaque:
                    // Ready to go!
                    break;

                case CameraFaderState.FadingOut:
                    while (cameraFader.State == CameraFaderState.FadingOut)
                    {   // Wait until we're done fading out to fade back in
                        await Task.Yield();
                    }
                    break;

                case CameraFaderState.FadingIn:
                    Debug.LogWarning("Already fading in. Taking no action.");
                    return;

                case CameraFaderState.Clear:
                    // If we haven't faded out yet, do so now - make it instantaneous
                    await cameraFader.FadeOutAsync(0, FadeColor, GatherFadeTargetCameras());
                    break;
            }

            await cameraFader.FadeInAsync(FadeInTime);
        }

        public Transform ShowProgressIndicator()
        {
            if (TransitionInProgress)
            {
                Debug.LogWarning("A scene transition is already in progress. This would interrupt that transition. Taking no action.");
                return null;
            }

            CreateProgressIndicator();

            switch (progressIndicator.State)
            {
                case ProgressIndicatorState.Open:
                case ProgressIndicatorState.Opening:
                    // If it's already open / opening, don't botheer to open again
                    break;

                case ProgressIndicatorState.Closed:
                    // Open it now - don't await result, we want to return the transform promptly 
                    progressIndicator.OpenAsync();
                    break;

                case ProgressIndicatorState.Closing:
                default:
                    // Open it now - don't await result, we want to return the transform promptly
                    progressIndicator.OpenAsync();
                    break;
            }

            return progressIndicator.MainTransform;
        }

        public async Task HideProgressIndicator()
        {
            if (TransitionInProgress)
            {
                Debug.LogWarning("A scene transition is already in progress. This would interrupt that transition. Taking no action.");
                return;
            }

            if (progressIndicator == null)
            {
                // No need to do anything.
                return;
            }

            switch (progressIndicator.State)
            {
                case ProgressIndicatorState.Closed:
                    // No need to do anything.
                    return;

                case ProgressIndicatorState.Closing:
                    while (progressIndicator.State == ProgressIndicatorState.Closing)
                    {   // Wait for progress indicator to be done closing
                        await Task.Yield();
                    }
                    return;

                case ProgressIndicatorState.Open:
                    await progressIndicator.CloseAsync();
                    return;

                case ProgressIndicatorState.Opening:
                    while (progressIndicator.State == ProgressIndicatorState.Opening)
                    {   // Wait for it to be done opening, then close it
                        await Task.Yield();
                    }
                    await progressIndicator.CloseAsync();
                    return;
            }
        }

        public void SetProgressMessage(string message)
        {
            if (progressIndicator == null)
            {
                Debug.LogWarning("Progress Indicator has not been launched. Taking no action.");
            }

            progressIndicator.Message = message;
        }

        public void SetProgressValue(float progress)
        {
            if (progressIndicator == null)
            {
                Debug.LogWarning("Progress Indicator has not been launched. Taking no action.");
            }

            progressIndicator.Progress = progress;
        }

        #endregion

        #region private methods

        private List<Camera> GatherFadeTargetCameras()
        {
            List<Camera> targetCameras = new List<Camera>();

            switch (FadeTargets)
            {
                case CameraFaderTargets.All:
                    // Add every single camera in all scenes
                    targetCameras.AddRange(GameObject.FindObjectsOfType<Camera>());
                    break;

                case CameraFaderTargets.Main:
                    targetCameras.Add(CameraCache.Main);
                    break;

                case CameraFaderTargets.UI:
                    foreach (Canvas canvas in GameObject.FindObjectsOfType<Canvas>())
                    {
                        switch (canvas.renderMode)
                        {
                            case RenderMode.ScreenSpaceCamera:
                            case RenderMode.WorldSpace:
                                if (canvas.worldCamera != null)
                                {
                                    targetCameras.Add(canvas.worldCamera);
                                }
                                break;

                            case RenderMode.ScreenSpaceOverlay:
                            default:
                                break;
                        }
                    }
                    break;

                case CameraFaderTargets.Custom:
                    if (customFadeTargetCameras.Count == 0)
                        throw new Exception("Attempting to fade custom target cameras but none were supplied. Use SetCustomFadeCameras prior to calling TransitionToScene.");

                    targetCameras.AddRange(customFadeTargetCameras);
                    break;
            }

            return targetCameras;
        }

        private void CreateProgressIndicator()
        {
            if (progressIndicatorObject != null)
                return;

            // Do service initialization here.
            if (sceneTransitionServiceProfile.ProgressIndicatorPrefab == null)
            {
                throw new Exception("No progress indicator prefab found in profile.");
            }

            progressIndicatorObject = GameObject.Instantiate(sceneTransitionServiceProfile.ProgressIndicatorPrefab);
            progressIndicator = (IProgressIndicator)progressIndicatorObject.GetComponent(typeof(IProgressIndicator));

            if (progressIndicator == null)
            {
                throw new Exception("Progress indicator prefab doesn't have a script implementing IProgressIndicator.");
            }

            // Ensure progress indicator doesn't get destroyed
            progressIndicatorObject.transform.DontDestroyOnLoad();
        }

        private void CleanUpProgressIndicator()
        {
            if (progressIndicatorObject != null)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(progressIndicatorObject);
                }
                else
                {
                    GameObject.DestroyImmediate(progressIndicatorObject);
                }
            }
        }

        private void CreateCameraFader()
        {
            if (cameraFader != null)
                return;

            cameraFader = (ICameraFader)Activator.CreateInstance(sceneTransitionServiceProfile.CameraFaderType.Type);

            if (cameraFader == null)
            {
                throw new Exception("Couldn't create camera fader of type " + sceneTransitionServiceProfile.CameraFaderType.Type);
            }
        }

        private void CleanUpCameraFader()
        {
            if (cameraFader != null)
            {
                cameraFader.OnDestroy();
                cameraFader = null;
            }
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

        #endregion
    }
}