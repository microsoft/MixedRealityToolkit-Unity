// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.SceneTransitions
{
    /// <summary>
    /// Basic interface for fading in / out of a color.
    /// </summary>
    public interface ICameraFader
    {
        CameraFaderState State { get; }

        Task FadeOutAsync(float fadeOutTime, Color color, IEnumerable<Camera> targets);
        Task FadeIn(float fadeInTime);
        Task FadeOutAsync(float fadeOutTime, Color fadeColor, object fadeTargets, object customTargets);
    }
}