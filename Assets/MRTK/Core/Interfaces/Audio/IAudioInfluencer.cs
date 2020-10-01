// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// Interface that should be implemented by any class that wishes to influence how an audio source sounds.
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