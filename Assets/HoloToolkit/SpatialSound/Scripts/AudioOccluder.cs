// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Class that implements IAudioInfluencer to provide an occlusion effect.
    /// </summary>
    public class AudioOccluder 
        : MonoBehaviour, 
        IAudioInfluencer
    {
        /// <summary>
        /// Frequency above which sound will not be heard after applying occlusion.
        /// Setting this value to 22000.0 effectively disables the effect.
        /// </summary>
        /// <remarks>
        /// Chaining occluders will result in the lowest of the cutoff frequencies being applied to the sound.
        /// The CutoffFrequency range is 0.0 - 22000.0 (0 - 22kHz), inclusive.
        /// The default value is 5000.0 (5kHz).
        /// </remarks>
        [Tooltip("Frequency above which sound will not be heard after applying occlusion.")]
        [Range(10.0f, 22000.0f)]
        public float CutoffFrequency = 5000.0f;

        /// <summary>
        /// Percentage of the audio source volume that will be heard after applying occlusion.
        /// </summary>
        /// <remarks>
        /// VolumePassThrough is cumulative. It is applied to the current volume of the object at the time
        /// the effect is applied.
        /// The VolumePassThrough range is from 0.0 - 1.0 (0-100%), inclusive.
        /// The default value is 1.0.
        /// </remarks>
        [Tooltip("Percentage of the audio source volume that will be heard after applying occlusion.")]
        [Range(0.0f, 1.0f)]
        public float VolumePassThrough = 1.0f;

        /// <summary>
        /// Applies the audio effect.
        /// </summary>
        /// <param name="soundEmittingObject">The GameObject on which the effect is to be applied.</param>
        public void ApplyEffect(GameObject soundEmittingObject)
        {
            if (!isActiveAndEnabled)
            { return; }

            AudioSource audioSource = soundEmittingObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("The specified emitter does not have an attached AudioSource component.");
                return;
            }

            // Audio occlusion is performed using a low pass filter.                
            AudioLowPassFilter lowPass = soundEmittingObject.GetComponent<AudioLowPassFilter>();
            if (lowPass == null)
            { 
                lowPass = soundEmittingObject.AddComponent<AudioLowPassFilter>();
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
        /// <param name="soundEmittingObject">The GameObject from which the effect is to be removed.</param>
        public void RemoveEffect(GameObject soundEmittingObject)
        {
            // Audio occlusion is performed using a low pass filter.                
            AudioLowPassFilter lowPass = soundEmittingObject.GetComponent<AudioLowPassFilter>();
            if (lowPass == null) { return; }

            float neutralFrequency = AudioInfluencerManager.NeutralHighFrequency;
            AudioInfluencerManager influencerManager = soundEmittingObject.GetComponent<AudioInfluencerManager>();
            if (influencerManager != null)
            {
                neutralFrequency = influencerManager.GetNativeLowPassCutoffFrequency();
            }

            lowPass.cutoffFrequency = neutralFrequency;
            lowPass.enabled = false;

            // Note: Volume attenuation is reset in the AudioInfluencerManager, attached to the sound emitting object.
        }

        // Update is not used, but is kept so that this component can be enabled/disabled.
        private void Update()
        { }
    }
}