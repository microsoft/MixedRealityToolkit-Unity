// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.SpatialSound.Extensions
{
    /// <summary>
    /// A shortcut to assign a clip to an AudioSource component and play the source
    /// </summary>
    public static class AudioSourcePlayClipExtension
    {
        public static void PlayClip(this AudioSource source, UnityEngine.AudioClip clip, bool loop = false)
        {
            source.clip = clip;
            source.loop = loop;
            source.Play();
        }
    }
}