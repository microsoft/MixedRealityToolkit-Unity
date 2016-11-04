// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public class AudioOccluder : MonoBehaviour, IAudioInfluencer
    {
        private readonly float NeutralFrequency = 22000f;

        [Range(10.0f, 22000.0f)]
        public Single CutoffFrequency = 5000.0f;
        
        [Range(0.0f, 1.0f)]
        public Single VolumePassThrough = 1.0f;

        // Update is not used, but is kept so that this component can be enabled/disabled.
        private void Update() 
        { }

        /// <summary>
        /// Applies an audio effect.
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
        /// Removes a previously applied audio effect.
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

            // Volume attenuation is reset in the emitter
        }
    }
}