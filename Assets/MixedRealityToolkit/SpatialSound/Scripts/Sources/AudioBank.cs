// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.SpatialSound.Sources
{
    /// <summary>
    /// The audio bank contains a list of audio events that can be loaded into a UAudioManager for playback.
    /// </summary>
    /// <typeparam name="T">Type of event this bank supports</typeparam>
    public abstract class AudioBank<T> : ScriptableObject
    {
        public T[] Events;
    }
}