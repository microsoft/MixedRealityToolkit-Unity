// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.SpatialSound.Effects
{
    /// <summary>
    /// Interface that is implemented by any class that wishes to influence how an audio source sounds.
    /// </summary>
    public interface IAudioInfluencer
    {
        /// <summary>
        /// Applies an audio effect.
        /// </summary>
        /// <param name="soundEmittingObject">The GameObject on which the effect is to be applied.</param>
        void ApplyEffect(GameObject soundEmittingObject);

        /// <summary>
        /// Removes a previously applied audio effect.
        /// </summary>
        /// <param name="soundEmittingObject">The GameObject from which the effect is to be removed.</param>
        void RemoveEffect(GameObject soundEmittingObject);
    }
}