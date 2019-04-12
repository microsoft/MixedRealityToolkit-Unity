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
        private List<Camera> customCameras = new List<Camera>();

        public SceneTransitionService(IMixedRealityServiceRegistrar registrar,  string name,  uint priority,  BaseMixedRealityProfile profile) : base(registrar, name, priority, profile) 
		{
            sceneTransitionServiceProfile = (SceneTransitionServiceProfile)profile;
		}

        public override void Initialize()
        {
         
        }

        public override void Enable()
        {
            CreateProgressIndicator();
        }

        public override void Update()
        {

        }

        public override void Disable()
        {
            CleanUpProgressIndicator();
        }

        public override void Destroy()
        {
            CleanUpProgressIndicator();
        }

        public void SetCustomFadeCameras(IEnumerable<Camera> customCameras)
        {
            this.customCameras.AddRange(customCameras);
        }

        public async Task TransitionToScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool awaitConfirmationForTransitionOut = false)
        {
            if (TransitionInProgress)
            {
                throw new System.Exception("Attempting to transition to scene when transition is already in progress.");
            }

            Scene targetScene = SceneManager.GetSceneByName(sceneName);

            TransitionInProgress = true;
            AwaitingConfirmation = false;

            if (sceneTransitionServiceProfile.UseFadeColor)
            {
                // Fade out before proceeding
                await cameraFader.FadeOutAsync(
                    sceneTransitionServiceProfile.FadeOutTime, 
                    sceneTransitionServiceProfile.FadeColor, 
                    GatherFadeTargetCameras());
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

            // If the user has requested that we wait before exiting the transition
            // don't proceed with transition out until we receive confirmation
            if (awaitConfirmationForTransitionOut)
            {
                AwaitingConfirmation = true;
                while (AwaitingConfirmation)
                {
                    await Task.Yield();
                }
            }

            if (progressIndicator.State != ProgressIndicatorState.Closed)
            {
                // Deactivate the progress indicator and wait for it to spin down
                await progressIndicator.CloseAsync();
            }

            if (cameraFader.State != CameraFaderState.Clear)
            {
                // Wait for camera to fade out
                await cameraFader.FadeIn(sceneTransitionServiceProfile.FadeInTime);
            }

            // We're done!
            TransitionInProgress = false;
        }

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
                    if (customCameras.Count == 0)
                        throw new Exception("Attempting to fade custom target cameras but none were supplied. Use SetCustomFadeCameras prior to calling TransitionToScene.");

                    targetCameras.AddRange(customCameras);
                    break;
            }

            return targetCameras;
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

        private void CreateProgressIndicator()
        {
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
            // Deactivate it immediately
            progressIndicatorObject.SetActive(false);
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
    }
}