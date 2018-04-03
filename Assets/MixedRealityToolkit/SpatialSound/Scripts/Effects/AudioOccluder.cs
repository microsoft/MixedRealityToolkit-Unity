// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.SpatialSound.Effects
{
    /// <summary>
    /// Class that implements IAudioInfluencer to provide an occlusion effect.
    /// </summary>
    /// <remarks>
    /// Ensure that all sound emitting objects have an attached AudioInfluencerController. 
    /// Failing to do so will result in the desired effect not being applied to the sound.
    /// </remarks>
    public class AudioOccluder : MonoBehaviour, IAudioInfluencer
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
        [SerializeField]
        private float cutoffFrequency = 5000.0f;
        public float CutoffFrequency
        {
            get { return cutoffFrequency; }
            set
            {
                // set cutoffFrequency and enforce the specified range
                if (value < 10.0f)
                {
                    cutoffFrequency = 10.0f;
                }
                else if (value > 22000.0f)
                {
                    cutoffFrequency = 22000.0f;
                }
                else
                {
                    cutoffFrequency = value;
                }
            }
        }

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
        [SerializeField]
        private float volumePassThrough = 1.0f;
        public float VolumePassThrough
        {
            get { return volumePassThrough; }
            set
            {
                // set cutoffFrequency and enforce the specified range
                if (value < 10.0f)
                {
                    volumePassThrough = 10.0f;
                }
                else if (value > 22000.0f)
                {
                    volumePassThrough = 22000.0f;
                }
                else
                {
                    volumePassThrough = value;
                }
            }
        }

        /// <summary>
        //  / Update is not used, but is kept so that this component can be enabled/disabled.
        /// </summary>
        private void Update() { }

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

            float neutralFrequency = AudioInfluencerController.NeutralHighFrequency;
            AudioInfluencerController influencerController = soundEmittingObject.GetComponent<AudioInfluencerController>();
            if (influencerController != null)
            {
                neutralFrequency = influencerController.GetNativeLowPassCutoffFrequency();
            }

            lowPass.cutoffFrequency = neutralFrequency;
            lowPass.enabled = false;

            // Note: Volume attenuation is reset in the AudioInfluencerController, attached to the sound emitting object.
        }
    }
}