// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    [MixedRealityExtensionService(
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.MacStandalone |
        SupportedPlatforms.LinuxStandalone | SupportedPlatforms.WindowsUniversal,
        "Scene Transition Service",
        "SceneTransitionService/Profiles/DefaultSceneTransitionServiceProfile.asset",
        "MixedRealityToolkit.Extensions",
        true)]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/extensions/scene-transition-service")]
    public class SceneTransitionService : BaseExtensionService, ISceneTransitionService, IMixedRealityExtensionService
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the service.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        [Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public SceneTransitionService(
            IMixedRealityServiceRegistrar registrar,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : this(name, priority, profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public SceneTransitionService(
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(name, priority, profile)
        {
            sceneTransitionServiceProfile = profile as SceneTransitionServiceProfile;
        }

        private const float maxFadeOutTime = 30;
        private const float maxFadeInTime = 30;

        /// <inheritdoc />
        public bool UseFadeColor { get; set; }

        /// <inheritdoc />
        public Color FadeColor { get; set; }

        /// <inheritdoc />
        public float FadeInTime { get; set; }

        /// <inheritdoc />
        public float FadeOutTime { get; set; }

        /// <inheritdoc />
        public CameraFaderTargets FadeTargets { get; set; }

        /// <inheritdoc />
        public Action OnTransitionStarted { get; set; }

        /// <inheritdoc />
        public Action OnTransitionCompleted { get; set; }

        /// <inheritdoc />
        public bool TransitionInProgress { get; set; }

        /// <inheritdoc />
        public float TransitionProgress { get; set; }

        private SceneTransitionServiceProfile sceneTransitionServiceProfile;
        private GameObject progressIndicatorObject;
        private IProgressIndicator defaultProgressIndicator;
        private ICameraFader cameraFader;
        private List<Camera> customFadeTargetCameras = new List<Camera>();

        #region public methods

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();
            UseFadeColor = sceneTransitionServiceProfile.UseFadeColor;
            FadeColor = sceneTransitionServiceProfile.FadeColor;
            FadeInTime = sceneTransitionServiceProfile.FadeInTime;
            FadeOutTime = sceneTransitionServiceProfile.FadeOutTime;
            FadeTargets = sceneTransitionServiceProfile.FadeTargets;
        }

        /// <inheritdoc />
        public override void Enable()
        {
            if (!MixedRealityToolkit.IsSceneSystemEnabled)
            {
                Debug.LogError("This extension requires an active IMixedRealitySceneService.");
            }

            // Call the base here to ensure any early exits do not
            // artificially declare the service as enabled.
            base.Enable();
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            CleanUpDefaultProgressIndicator();
            CleanUpCameraFader();
            base.Destroy();
        }

        #endregion

        #region ISceneTransitionService implementation

        /// <inheritdoc />
        public async Task DoSceneTransition(Func<Task> sceneOperation, IProgressIndicator progressIndicator = null)
        {
            await DoSceneTransition(new Func<Task>[] { sceneOperation }, FadeOutTime, FadeInTime, progressIndicator);
        }

        /// <inheritdoc />
        public async Task DoSceneTransition(Func<Task> sceneOp1, Func<Task> sceneOp2, IProgressIndicator progressIndicator = null)
        {
            await DoSceneTransition(new Func<Task>[] { sceneOp1, sceneOp2 }, FadeOutTime, FadeInTime, progressIndicator);
        }

        /// <inheritdoc />
        public async Task DoSceneTransition(IEnumerable<Func<Task>> sceneOperations, IProgressIndicator progressIndicator = null)
        {
            await DoSceneTransition(sceneOperations, FadeOutTime, FadeInTime, progressIndicator);
        }

        private static readonly ProfilerMarker DoSceneTransitionPerfMarker = new ProfilerMarker("[MRTK] SceneTransitionService.DoSceneTransition");

        /// <inheritdoc />
        public async Task DoSceneTransition(IEnumerable<Func<Task>> sceneOperations, float fadeOutTime, float fadeInTime, IProgressIndicator progressIndicator = null)
        {
            using (DoSceneTransitionPerfMarker.Auto())
            {
                fadeOutTime = Mathf.Clamp(fadeOutTime, 0, maxFadeOutTime);
                fadeInTime = Mathf.Clamp(fadeInTime, 0, maxFadeInTime);

                if (TransitionInProgress)
                {
                    throw new Exception("Attempting to do a transition while one is already in progress.");
                }

                #region Transition begin

                TransitionInProgress = true;
                OnTransitionStarted?.Invoke();

                if (progressIndicator == null && sceneTransitionServiceProfile.UseDefaultProgressIndicator)
                {   // If we haven't been given a progress indicator, and we're supposed to use a default
                    // find / create the default progress indicator
                    CreateDefaultProgressIndicator();
                    progressIndicator = defaultProgressIndicator;
                }

                if (UseFadeColor)
                {
                    await FadeOut(fadeOutTime);
                }

                if (progressIndicator != null)
                {
                    await progressIndicator.OpenAsync();
                }

                #endregion

                #region Task execution

                // Make sure we're on the main thread

                foreach (Func<Task> sceneOperation in sceneOperations)
                {
                    await sceneOperation();
                }

                #endregion

                #region Transition end

                // If we used a progress indicator, close it
                if (progressIndicator != null)
                {
                    await progressIndicator.CloseAsync();
                }


                if (UseFadeColor)
                {
                    await FadeIn(fadeInTime);
                }

                TransitionInProgress = false;
                OnTransitionCompleted?.Invoke();

                #endregion
            }
        }

        /// <inheritdoc />
        public void SetCustomFadeTargetCameras(IEnumerable<Camera> customFadeTargetCameras)
        {
            this.customFadeTargetCameras.Clear();
            this.customFadeTargetCameras.AddRange(customFadeTargetCameras);
        }

        /// <inheritdoc />
        public async Task FadeOut()
        {
            await FadeOut(FadeOutTime);
        }

        /// <inheritdoc />
        public async Task FadeIn()
        {
            await FadeIn(FadeInTime);
        }

        private static readonly ProfilerMarker FadeOutPerfMarker = new ProfilerMarker("[MRTK] SceneTransitionService.FadeOut");

        /// <inheritdoc />
        public async Task FadeOut(float fadeOutTime)
        {
            using (FadeOutPerfMarker.Auto())
            {
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

                await cameraFader.FadeOutAsync(fadeOutTime, FadeColor, GatherFadeTargetCameras());
            }
        }

        private static readonly ProfilerMarker FadeInPerfMarker = new ProfilerMarker("[MRTK] SceneTransitionService.FadeIn");

        /// <inheritdoc />
        public async Task FadeIn(float fadeInTime)
        {
            using (FadeInPerfMarker.Auto())
            {
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

                await cameraFader.FadeInAsync(fadeInTime);
            }
        }

        private static readonly ProfilerMarker ShowDefaultProgressIndicatorPerfMarker = new ProfilerMarker("[MRTK] SceneTransitionService.ShowDefaultProgressIndicator");

        /// <inheritdoc />
        public Transform ShowDefaultProgressIndicator()
        {
            using (ShowDefaultProgressIndicatorPerfMarker.Auto())
            {
                CreateDefaultProgressIndicator();

                switch (defaultProgressIndicator.State)
                {
                    case ProgressIndicatorState.Open:
                    case ProgressIndicatorState.Opening:
                        // If it's already open / opening, don't bother to open again
                        break;

                    case ProgressIndicatorState.Closed:
                        // Open it now - don't await result, we want to return the transform promptly 
                        defaultProgressIndicator.OpenAsync();
                        break;

                    case ProgressIndicatorState.Closing:
                    default:
                        // Open it now - don't await result, we want to return the transform promptly
                        defaultProgressIndicator.OpenAsync();
                        break;
                }

                return defaultProgressIndicator.MainTransform;
            }
        }

        private static readonly ProfilerMarker HideProgressIndicatorPerfMarker = new ProfilerMarker("[MRTK] SceneTransitionService.HideProgressIndicator");

        /// <inheritdoc />
        public async Task HideProgressIndicator()
        {
            if (TransitionInProgress)
            {
                Debug.LogWarning("A scene transition is already in progress. This would interrupt that transition. Taking no action.");
                return;
            }

            if (defaultProgressIndicator == null)
            {
                // No need to do anything.
                return;
            }

            using (HideProgressIndicatorPerfMarker.Auto())
            {
                switch (defaultProgressIndicator.State)
                {
                    case ProgressIndicatorState.Closed:
                        // No need to do anything.
                        return;

                    case ProgressIndicatorState.Closing:
                        while (defaultProgressIndicator.State == ProgressIndicatorState.Closing)
                        {   // Wait for progress indicator to be done closing
                            await Task.Yield();
                        }
                        return;

                    case ProgressIndicatorState.Open:
                        await defaultProgressIndicator.CloseAsync();
                        return;

                    case ProgressIndicatorState.Opening:
                        while (defaultProgressIndicator.State == ProgressIndicatorState.Opening)
                        {   // Wait for it to be done opening, then close it
                            await Task.Yield();
                        }
                        await defaultProgressIndicator.CloseAsync();
                        return;
                }
            }
        }

        /// <inheritdoc />
        public void SetProgressMessage(string message)
        {
            if (defaultProgressIndicator == null)
            {
                Debug.LogWarning("Progress Indicator has not been launched. Taking no action.");
            }

            defaultProgressIndicator.Message = message;
        }

        /// <inheritdoc />
        public void SetProgressValue(float progress)
        {
            if (defaultProgressIndicator == null)
            {
                Debug.LogWarning("Progress Indicator has not been launched. Taking no action.");
            }

            defaultProgressIndicator.Progress = progress;
        }

        #endregion

        #region private methods

        private static readonly ProfilerMarker GatherFadeTargetCamerasPerfMarker = new ProfilerMarker("[MRTK] SceneTransitionService.GatherFrameTargetCameras");

        private List<Camera> GatherFadeTargetCameras()
        {
            using (GatherFadeTargetCamerasPerfMarker.Auto())
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
        }

        private void CreateDefaultProgressIndicator()
        {
            if (defaultProgressIndicator != null)
            {
                return;
            }

            if (sceneTransitionServiceProfile.DefaultProgressIndicatorPrefab == null)
            {
                throw new Exception("No progress indicator prefab found in profile.");
            }

            progressIndicatorObject = GameObject.Instantiate(sceneTransitionServiceProfile.DefaultProgressIndicatorPrefab);
            defaultProgressIndicator = (IProgressIndicator)progressIndicatorObject.GetComponent(typeof(IProgressIndicator));

            if (defaultProgressIndicator == null)
            {
                throw new Exception("Progress indicator prefab doesn't have a script implementing IProgressIndicator.");
            }
        }

        private void CleanUpDefaultProgressIndicator()
        {
            if (progressIndicatorObject != null)
            {
                GameObjectExtensions.DestroyGameObject(progressIndicatorObject);
            }
        }

        private void CreateCameraFader()
        {
            if (cameraFader != null)
            {
                return;
            }

            cameraFader = (ICameraFader)Activator.CreateInstance(sceneTransitionServiceProfile.CameraFaderType.Type);
            cameraFader.Initialize(sceneTransitionServiceProfile);

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
