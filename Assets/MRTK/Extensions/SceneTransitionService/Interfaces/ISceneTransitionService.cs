// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    public interface ISceneTransitionService : IMixedRealityExtensionService
    {
        /// <summary>
        /// Called when transition starts.
        /// This is called at the beginning of a transition, not at the beginning of a scene load.
        /// For scene load events, we recommend using IMixedRealitySceneSystem.
        /// </summary>
        Action OnTransitionStarted { get; set; }

        /// <summary>
        /// Called when transition ends.
        /// This is called at the end of a transition, not at the end of a scene load.
        /// For scene load events, we recommend using IMixedRealitySceneSystem.
        /// </summary>
        Action OnTransitionCompleted { get; set; }

        /// <summary>
        /// Whether to use a fade color during transitions.
        /// </summary>
        bool UseFadeColor { get; set; }

        /// <summary>
        /// The color to use when fading out.
        /// </summary>
        Color FadeColor { get; set; }

        /// <summary>
        /// The default time in seconds for fade in to complete.
        /// </summary>
        float FadeInTime { get; set; }

        /// <summary>
        /// The default time in seconds for fade out to complete.
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
        /// From 0 to 1
        /// </summary>
        float TransitionProgress { get; }

        /// <summary>
        /// Fades out, enables progress indicator, execute scene operations in order, disables progress indicator, fades back in
        /// </summary>
        /// <param name="sceneOperations">A set of tasks from the Scene System.</param>
        /// <param name="fadeOutTime">Overrides the default FadeOutTIme value.</param>
        /// <param name="fadeInTime">Overrides the default FadeInTime value.</param>
        /// <param name="progressIndicator">If null, default progress indicator prefab will be used (or none if default is disabled in profile)</param>
        Task DoSceneTransition(IEnumerable<Func<Task>> sceneOperations, float fadeOutTime, float fadeInTime, IProgressIndicator progressIndicator = null);

        /// <summary>
        /// Fades out, enables progress indicator, execute scene operations in order, disables progress indicator, fades back in
        /// </summary>
        /// <param name="sceneOperations">A set of tasks from the Scene System.</param>
        /// <param name="progressIndicator">If null, default progress indicator prefab will be used (or none if default is disabled in profile)</param>
        Task DoSceneTransition(IEnumerable<Func<Task>> sceneOperations, IProgressIndicator progressIndicator = null);

        /// <summary>
        /// Fades out, enables progress indicator, executes scene op 1, executes scene op 2, disables progress indicator, fades back in
        /// </summary>
        Task DoSceneTransition(Func<Task> sceneOp1, Func<Task> sceneOp2, IProgressIndicator progressIndicator = null);

        /// <summary>
        /// Fades out, enables progress indicator, execute scene operation, disables progress indicator, fades back in
        /// </summary>
        /// <param name="sceneOperations">A set of tasks from the Scene System.</param>
        /// <param name="progressIndicator">If null, default progress indicator prefab will be used (or none if default is disabled in profile)</param>
        Task DoSceneTransition(Func<Task> sceneOperation, IProgressIndicator progressIndicator = null);

        /// <summary>
        /// If FadeTargets is set to custom, you will need to provide a custom set of cameras for fading using this function PRIOR to calling DoSceneTransition.
        /// </summary>
        void SetCustomFadeTargetCameras(IEnumerable<Camera> customFadeTargetCameras);

        /// <summary>
        /// Fades target cameras out to color. Can be used independently of scene transitions provided no transition is taking place. Uses default FadeOutTime.
        /// </summary>
        Task FadeOut();

        /// <summary>
        /// Fades target cameras in. Instant fade-out will occur if fade state is not opaque. Can be used independently of scene transitions provided no transition is taking place. Uses default FadeInTime.
        /// </summary>
        Task FadeIn();

        /// <summary>
        /// Fades target cameras out to color. Can be used independently of scene transitions provided no transition is taking place.
        /// </summary>
        Task FadeOut(float fadeOutTime);

        /// <summary>
        /// Fades target cameras in. Instant fade-out will occur if fade state is not opaque. Can be used independently of scene transitions provided no transition is taking place.
        /// </summary>
        Task FadeIn(float fadeInTime);

        /// <summary>
        /// Instantiates the default progress indicator and returns its main transform. Can be used independently of scene transitions provided no transition is taking place.
        /// </summary>
        Transform ShowDefaultProgressIndicator();

        /// <summary>
        /// Hides the default progress indicator. Task completes when hide animation is done. Can be used independently of scene transitions provided no transition is taking place.
        /// </summary>
        Task HideProgressIndicator();

        /// <summary>
        /// Sets progress to value from 0-1. If no progress indicator exists, has no effect.
        /// </summary>
        void SetProgressValue(float progress);

        /// <summary>
        /// Sets the message on displayed progress indicator. If no progress indicator exists, has no effect.
        /// </summary>
        void SetProgressMessage(string message);
    }
}
