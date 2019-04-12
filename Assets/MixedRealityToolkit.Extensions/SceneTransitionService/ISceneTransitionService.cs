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
        /// True when a transition is in progress.
        /// </summary>
        bool TransitionInProgress { get; }

        /// <summary>
        /// True when await confirmation was requested in TransitionToScene
        /// and we're ready to transition out
        /// </summary>
        bool AwaitingConfirmation { get; }

        /// <summary>
        /// Activates transition prefabs, loads the scene, then deactivates transition prefabs.
        /// If the target scene is not found or is already loaded an exception will be thrown
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="mode"></param>
        /// <param name="awaitConfirmationOnTransitionOut"></param>
        /// <returns></returns>
        Task TransitionToScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool awaitConfirmationOnTransitionOut = false);

        /// <summary>
        /// If FadeTargets is set to custom, you will need to provide a custom set of cameras for fading using this function PRIOR to calling TransitionToScene.
        /// </summary>
        /// <param name="customFadeTargetCameras"></param>
        void SetCustomFadeTargetCameras(IEnumerable<Camera> customFadeTargetCameras);

        /// <summary>
        /// Sets the message on displayed progress indicator.
        /// </summary>
        /// <param name="message"></param>
        void SetProgressMessage(string message);
    }
}