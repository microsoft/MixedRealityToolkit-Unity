// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
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
        /// <param name="soundEmittingObject">The GameObject on which the effect is to be applied.</param>
        /// <param name="audioSource">The AudioSource that is emitting the sound.</param>
        /// <remarks>
        /// This method has been deprecated. The prefered method to call is ApplyEffect(GameObject).
        /// </remarks>
        [Obsolete("ApplyEffect(GameObject, AudioSource) as been deprecated. Using ApplyEffect(GameObject) is prefered.")]
        void ApplyEffect(GameObject soundEmittingObject, AudioSource audioSource);

        /// <summary>
        /// Applies an audio effect.
        /// </summary>
        /// <param name="soundEmittingObject">The GameObject on which the effect is to be applied.</param>
        void ApplyEffect(GameObject soundEmittingObject);

        /// <summary>
        /// Removes a previously applied audio effect.
        /// </summary>
        /// <param name="soundEmittingObject">The GameObject from which the effect is to be removed.</param>
        /// <param name="audioSource">The AudioSource that is emitting the sound.</param>
        /// <remarks>
        /// This method has been deprecated. The prefered method to call is ApplyEffect(GameObject).
        /// </remarks>
        [Obsolete("RemoveEffect(GameObject, AudioSource) as been deprecated. Using RemoveEffect(GameObject) is prefered.")]
        void RemoveEffect(GameObject soundEmittingObject, AudioSource audioSource);

        /// <summary>
        /// Removes a previously applied audio effect.
        /// </summary>
        /// <param name="soundEmittingObject">The GameObject from which the effect is to be removed.</param>
        void RemoveEffect(GameObject soundEmittingObject);
    }
}