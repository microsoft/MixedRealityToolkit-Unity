// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    /// <summary>
    /// Basic interface for fading in / out a color on a camera.
    /// </summary>
    public interface ICameraFader
    {
        CameraFaderState State { get; }

        /// <summary>
        /// Initializes the camera fader class with a transition profile.
        /// </summary>
        /// <param name="profile">The scene transition service profile.</param>
        void Initialize(SceneTransitionServiceProfile profile);

        /// <summary>
        /// Applies a fade-out effect over time.
        /// </summary>
        /// <param name="fadeOutTime">The duration of the fade</param>
        /// <param name="color">The color of the fade</param>
        /// <param name="targets">Which cameras will receive the effect</param>
        Task FadeOutAsync(float fadeOutTime, Color color, IEnumerable<Camera> targets);

        /// <summary>
        /// Applies a fade-in effect over time. Must be called after FadeOutAsync has completed.
        /// </summary>
        /// <param name="fadeInTime">The duration of the fade</param>
        Task FadeInAsync(float fadeInTime);

        /// <summary>
        /// Used to destroy any assets created.
        /// May be called in middle of a transition.
        /// </summary>
        void OnDestroy();
    }
}