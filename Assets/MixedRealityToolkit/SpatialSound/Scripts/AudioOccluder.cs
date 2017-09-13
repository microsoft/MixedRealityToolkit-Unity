// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Class that implements IAudioInfluencer to provide an occlusion effect.
    /// </summary>
    public class AudioOccluder : MonoBehaviour, IAudioInfluencer
    {
        /// <summary>
        /// Frequency above the nominal range of human hearing. Applying this frequency to the filter will have no perceived impact on the audio source.
        /// </summary>
        private readonly float NeutralFrequency = 22000f;

        [Tooltip("Frequency above which sound will not be heard.")]
        [Range(10.0f, 22000.0f)]
        public float CutoffFrequency = 5000.0f;
        
        [Tooltip("Percentage of the audio source volume that will be heard after applying occlusion.")]
        [Range(0.0f, 1.0f)]
        public float VolumePassThrough = 1.0f;

        // Update is not used, but is kept so that this component can be enabled/disabled.
        private void Update() 
        { }

        /// <summary>
        /// Applies the audio effect.
        /// </summary>
        /// <param name="emitter">The GameObject on which the effect is to be applied.</param>
        /// <param name="audioSource">The AudioSource that will be impacted by the effect.</param>
        public void ApplyEffect(GameObject emitter,
                                AudioSource audioSource)
        {
            if (!isActiveAndEnabled)
            { return; }

            if (audioSource == null)
            {
                Debug.LogWarning("The specified emitter does not have an attached AudioSource component.");
                return;
            }

            // Audio occlusion is performed using a low pass filter.                
            AudioLowPassFilter lowPass = emitter.GetComponent<AudioLowPassFilter>();
            if (lowPass == null)
            { 
                lowPass = emitter.AddComponent<AudioLowPassFilter>();
            }
            lowPass.enabled = true;

            // In the real world, chaining multiple low-pass filters will result in the 
            // lowest of the cutoff frequencies being the highest pitches heard.
            lowPass.cutoffFrequency = Mathf.Min(lowPass.cutoffFrequency, CutoffFrequency);

            // Unlike the cutoff frequency, volume pass-through is cumulative.
            audioSource.volume *= VolumePassThrough;
        }

        /// <summary>
        /// Removes the previously applied audio effect.
        /// </summary>
        /// <param name="emitter">The GameObject from which the effect is to be removed.</param>
        /// <param name="audioSource">The AudioSource that will be impacted by the effect.</param>
        public void RemoveEffect(GameObject emitter,
                                AudioSource audioSource)
        {
            // Note: The audioSource parameter is unused.

            // Audio occlusion is performed using a low pass filter.                
            AudioLowPassFilter lowPass = emitter.GetComponent<AudioLowPassFilter>();
            if (lowPass == null) { return; }
            lowPass.cutoffFrequency = NeutralFrequency;
            lowPass.enabled = false;

            // Note: Volume attenuation is reset in the emitter.
        }
    }
}