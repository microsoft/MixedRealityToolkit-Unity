// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Interface that is implemented by any class that wishes to influence how an audio source sounds.
    /// </summary>
    public interface IAudioInfluencer
    {
        /// <summary>
        /// Applies an audio effect.
        /// </summary>
        /// <param name="emitter">The GameObject on which the effect is to be applied.</param>
        /// <param name="audioSource">The AudioSource that will be impacted by the effect.</param>
        void ApplyEffect(GameObject emitter,
                        AudioSource audioSource);

        /// <summary>
        /// Removes a previously applied audio effect.
        /// </summary>
        /// <param name="emitter">The GameObject from which the effect is to be removed.</param>
        /// <param name="audioSource">The AudioSource that will be impacted by the effect.</param>
        void RemoveEffect(GameObject emitter,
                        AudioSource audioSource);
    }
}