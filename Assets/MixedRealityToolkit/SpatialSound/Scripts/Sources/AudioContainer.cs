// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace MixedRealityToolkit.SpatialSound.Sources
{
    /// <summary>
    /// The AudioContainer class is sound container for an AudioEvent. It also specifies the rules of how to
    /// play back the contained AudioClips.
    /// </summary>
    [Serializable]
    public class AudioContainer
    {
        [Tooltip("The type of the audio container.")]
        public AudioContainerType ContainerType = AudioContainerType.Random;

        public bool Looping = false;
        public float LoopTime = 0;
        public UAudioClip[] Sounds = null;
        public float CrossfadeTime = 0f;
        public int CurrentClip = 0;
    }
}