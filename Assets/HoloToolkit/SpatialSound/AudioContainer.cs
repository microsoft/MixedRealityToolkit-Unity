// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// The AudioContainer class is sound container for an AudioEvent. It also specifies the rules of how to 
    /// play back the contained AudioClips.
    /// </summary>
    [Serializable]
    public class AudioContainer
    {
        [Tooltip("The type of the audio container.")]
        public AudioContainerType containerType = AudioContainerType.Random;
        
        public bool looping = false;
        public float loopTime = 0;      
        public UAudioClip[] sounds = null;
        public float crossfadeTime = 0f;
        public int currentClip = 0;
    }
}