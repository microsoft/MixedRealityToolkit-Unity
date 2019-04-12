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

        private SceneTransitionServiceProfile sceneTransitionServiceProfile;
        private GameObject progressIndicatorObject;
        private IProgressIndicator progressIndicator;
        private ICameraFader cameraFader;
        private List<Camera> customFadeTargetCameras = new List<Camera>();

        public SceneTransitionService(IMixedRealityServiceRegistrar registrar,  string name,  uint priority,  BaseMixedRealityProfile profile) : base(registrar, name, priority, profile) 
		{
            sceneTransitionServiceProfile = (SceneTransitionServiceProfile)profile;
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

        public async Task TransitionToScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool awaitConfirmationForTransitionOut = false)
        {
            if (TransitionInProgress)
            {
                throw new Exception("Attempting to transition to scene when transition is already in progress.");
            }

            Scene targetScene = SceneManager.GetSceneByName(sceneName);
            if (!targetScene.IsValid())
            {
                throw new Exception("Can't load invalid scene " + sceneName);
            }

            if (targetScene.isLoaded && mode != LoadSceneMode.Single)
            {
                throw new Exception("Attempting to additively load already-loaded scene " + sceneName);
            }

            TransitionInProgress = true;
            AwaitingConfirmation = false;

            CreateProgressIndicator();
            CreateCameraFader();

            if (sceneTransitionServiceProfile.UseFadeColor)
            {
                List<Camera> fadeTargetCameras = GatherFadeTargetCameras();
                // Fade out before proceeding
                await cameraFader.FadeOutAsync(
                    sceneTransitionServiceProfile.FadeOutTime, 
                    sceneTransitionServiceProfile.FadeColor,
                    fadeTargetCameras);
            }

            if (sceneTransitionServiceProfile.UseProgressIndicator)
            {
                // Activate the progress indicator and wait for it to spin up
                await progressIndicator.OpenAsync();
            }

            float startTime = Time.time;
            while (Time.time < startTime + 5f)
            {
                progressIndicator.Progress = (Time.time - startTime) / 5;
                await Task.Yield();
            }

            // Load the scene
            AsyncOperation sceneLoadOp = SceneManager.LoadSceneAsync(sceneName, mode);
            sceneLoadOp.allowSceneActivation = true;
            while (!sceneLoadOp.isDone)
            {
                progressIndicator.Progress = sceneLoadOp.progress;
                await Task.Yield();
            }

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
                await cameraFader.FadeInAsync(sceneTransitionServiceProfile.FadeInTime);
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

        public void SetProgressMessage(string message)
        {
            if (!TransitionInProgress)
            {
                Debug.LogWarning("No transition in progress. This action will have no effect.");
            }

            progressIndicator.Message = message;
        }

        #endregion

        #region private methods

        private List<Camera> GatherFadeTargetCameras()
        {
            List<Camera> targetCameras = new List<Camera>();

            switch (sceneTransitionServiceProfile.FadeTargets)
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
                Debug.LogWarning("No progress indicator prefab found in profile.");
                return;
            }

            progressIndicatorObject = GameObject.Instantiate(sceneTransitionServiceProfile.ProgressIndicatorPrefab);
            progressIndicator = (IProgressIndicator)progressIndicatorObject.GetComponent(typeof(IProgressIndicator));

            if (progressIndicator == null)
            {
                Debug.LogError("Progress indicator prefab doesn't have a script implementing IProgressIndicator.");
                return;
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

        #endregion
    }
}