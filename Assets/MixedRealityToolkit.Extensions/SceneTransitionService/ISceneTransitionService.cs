// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    public interface ISceneTransitionService : IMixedRealityExtensionService
    {
        /// <summary>
        /// The color to use when fading out.
        /// </summary>
        Color FadeColor { get; set; }

        /// <summary>
        /// Time in seconds for fade in to complete.
        /// </summary>
        float FadeInTime { get; set; }
        
        /// <summary>
        /// Time in seconds for fade out to complete.
        /// </summary>
        float FadeOutTime { get; set; }

        /// <summary>
        /// Which cameras to target when fading.
        /// </summary>
        CameraFaderTargets FadeTargets { get; set; }

        /// <summary>
        /// True when a scene transition is in progress.
        /// </summary>
        bool TransitionInProgress { get; }

        /// <summary>
        /// True when await confirmation was requested in TransitionToScene
        /// and we're ready to transition out
        /// </summary>
        bool AwaitingConfirmation { get; }

        /// <summary>
        /// Activates transition prefabs, fades out to color, loads the scene, sets progress value to scene load progress, fades in from color, then deactivates transition prefabs.
        /// </summary>
        /// <param name="scenesToLoad"></param>
        /// <param name="scenesToUnload"></param>
        /// <param name="awaitConfirmationForTransitionOut"></param>
        /// <returns></returns>
        Task TransitionToScene(IEnumerable<string> scenesToLoad, IEnumerable<string> scenesToUnload, bool awaitConfirmationForTransitionOut = false);

        /// <summary>
        /// Activates transition prefabs, fades out to color, loads the scene, sets progress value to scene load progress, fades in from color, then deactivates transition prefabs.
        /// </summary>
        /// <param name="scenesToLoad"></param>
        /// <param name="awaitConfirmationForTransitionOut"></param>
        /// <returns></returns>
        Task TransitionToScene(IEnumerable<string> scenesToLoad, bool awaitConfirmationForTransitionOut = false);

        /// <summary>
        /// Activates transition prefabs, fades out to color, loads the scene, sets progress value to scene load progress, fades in from color, then deactivates transition prefabs.
        /// </summary>
        /// <param name="sceneToLoad"></param>
        /// <param name="awaitConfirmationForTransitionOut"></param>
        /// <returns></returns>
        Task TransitionToScene(string sceneToLoad, bool awaitConfirmationForTransitionOut = false);

        /// <summary>
        /// If FadeTargets is set to custom, you will need to provide a custom set of cameras for fading using this function PRIOR to calling TransitionToScene.
        /// </summary>
        /// <param name="customFadeTargetCameras"></param>
        void SetCustomFadeTargetCameras(IEnumerable<Camera> customFadeTargetCameras);

        /// <summary>
        /// Fades target cameras out to color. Can be used independently of scene transitions provided no transition is taking place.
        /// </summary>
        /// <param name="fadeColor"></param>
        /// <param name="targetCameras"></param>
        /// <returns></returns>
        Task FadeOut();

        /// <summary>
        /// Fades target cameras in. Instant fade-out will occur if fade state is not opaque. Can be used independently of scene transitions provided no transition is taking place.
        /// </summary>
        /// <param name="fadeColor"></param>
        /// <param name="targetCameras"></param>
        /// <returns></returns>
        Task FadeIn();

        /// <summary>
        /// Creates a progress indicator and returns its main transform. Can be used independently of scene transitions provided no transition is taking place.
        /// </summary>
        /// <returns></returns>
        Transform ShowProgressIndicator();

        /// <summary>
        /// Hides a progress indicator. Task completes when hide animation is done. Can be used independently of scene transitions provided no transition is taking place.
        /// </summary>
        Task HideProgressIndicator();

        /// <summary>
        /// Sets progress to value from 0-1. If no progress indicator exists, has no effect.
        /// </summary>
        /// <param name="progress"></param>
        void SetProgressValue(float progress);

        /// <summary>
        /// Sets the message on displayed progress indicator. If no progress indicator exists, has no effect.
        /// </summary>
        /// <param name="message"></param>
        void SetProgressMessage(string message);
    }
}