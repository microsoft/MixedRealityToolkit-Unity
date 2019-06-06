// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Plays back input animation via the input simulation system.
    /// </summary>
    public interface IMixedRealityInputPlaybackService : IMixedRealityInputDeviceManager
    {
        InputAnimation Animation { get; set; }

        bool IsPlaying { get; }

        float LocalTime { get; set; }

        void Play();

        void Stop();

        void Pause();
    }
}
