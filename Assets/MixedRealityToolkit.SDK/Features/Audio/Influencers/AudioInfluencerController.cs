// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityPhysics = UnityEngine.Physics;

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// Class which supports components implementing <see cref="Microsoft.MixedReality.Toolkit.Audio.IAudioInfluencer"/> being used with audio sources.
    /// </summary>
    /// <remarks>
    /// AudioInfluencerController requires an <see href="https://docs.unity3d.com/ScriptReference/AudioSource.html">AudioSource</see> component. If one is not attached, it will be added automatically.
    /// Each sound playing game object needs to have an AudioInfluencerController attached in order to have it's audio influenced.
    /// </remarks>
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class AudioInfluencerController : MonoBehaviour
    {
        /// <summary>
        /// Frequency below the nominal range of human hearing.
        /// </summary>
        /// <remarks>
        /// This frequency can be used to set a high pass filter to allow all
        /// audible frequencies through the filter.
        /// </remarks>
        public static readonly float NeutralLowFrequency = 10.0f;

        /// <summary>
        /// Frequency above the nominal range of human hearing.
        /// </summary>
        /// <remarks>
        /// This frequency can be used to set a low pass filter to allow all
        /// audible frequencies through the filter.
        /// </remarks>
        public static readonly float NeutralHighFrequency = 22000.0f;

        /// <summary>
        /// Time, in seconds, between audio influence updates.
        /// </summary>
        /// <remarks>
        /// The UpdateInterval range is between 0.0 and 1.0, inclusive.
        /// The default value is 0.25.
        /// A value of 0.0f indicates that updates occur every frame.
        /// </remarks>
        [Tooltip("Time, in seconds, between audio influence updates.  0 indicates to update every frame.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float updateInterval = 0.25f;
        public float UpdateInterval
        {
            get { return updateInterval; }
            set
            {
                updateInterval = Mathf.Clamp(value, 0.0f, 1.0f);
            }
        }

        /// <summary>
        /// Maximum distance, in meters, to look when attempting to find the user and any influencers.
        /// </summary>
        /// <remarks>
        /// The MaxDistance range is 1.0 to 50.0, inclusive.
        /// The default value is 20.0.
        /// </remarks>
        [Tooltip("Maximum distance, in meters, to look when attempting to find the user and any influencers.")]
        [Range(1.0f, 50.0f)]
        [SerializeField]
        private float maxDistance = 20.0f;
        public float MaxDistance
        {
            get { return maxDistance; }
            set
            {
                maxDistance = Mathf.Clamp(value, 1.0f, 50.0f);
            }
        }

        /// <summary>
        /// Maximum number of objects that will be considered when looking for influencers. 
        /// Setting this value too high may have a negative impact on the performance of your experience.
        /// </summary>
        /// <remarks>
        /// MaxObjects can only be set in the Unity Inspector.
        /// The MaxObjects range is 1 to 25, inclusive.
        /// The default value is 10.
        /// </remarks>
        [Tooltip("Maximum number of objects that will be considered when looking for influencers.")]
        [Range(1, 25)]
        [SerializeField]
        private int maxObjects = 10;

        /// <summary>
        /// Time of last audio processing update. 
        /// </summary>
        private DateTime lastUpdate = DateTime.MinValue;

        /// <summary>
        /// The source of the audio.
        /// </summary>
        [SerializeField]
        private AudioSource audioSource;

        /// <summary>
        /// The initial volume level of the audio source.
        /// </summary>
        private float initialAudioSourceVolume;

        /// <summary>
        /// The hits returned by Physics.RaycastAll
        /// </summary>
        private RaycastHit[] hits;

        /// <summary>
        /// The collection of previously applied audio influencers.
        /// </summary>
        private List<IAudioInfluencer> previousInfluencers = new List<IAudioInfluencer>();

        /// <summary>
        /// Potential effects manipulated by an audio influencer.
        /// </summary>
        private AudioLowPassFilter lowPassFilter;
        private AudioHighPassFilter highPassFilter;

        private float nativeLowPassCutoffFrequency;

        /// <summary>
        /// Gets or sets the native low pass cutoff frequency for the
        /// sound emitter.
        /// </summary>
        public float NativeLowPassCutoffFrequency
        {
            get { return nativeLowPassCutoffFrequency; }
            set { value = nativeLowPassCutoffFrequency; }
        }

        private float nativeHighPassCutoffFrequency;

        /// <summary>
        /// Gets or sets the native high pass cutoff frequency for the
        /// sound emitter.
        /// </summary>
        public float NativeHighPassCutoffFrequency
        {
            get { return nativeHighPassCutoffFrequency; }
            set { value = nativeHighPassCutoffFrequency; }
        }

        private void Awake()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            initialAudioSourceVolume = audioSource.volume;

            // Get optional filters (and initial values) that the sound designer / developer 
            // may have applied to this game object
            lowPassFilter = gameObject.GetComponent<AudioLowPassFilter>();
            nativeLowPassCutoffFrequency = (lowPassFilter != null) ? lowPassFilter.cutoffFrequency : NeutralHighFrequency;
            highPassFilter = gameObject.GetComponent<AudioHighPassFilter>();
            nativeHighPassCutoffFrequency = (highPassFilter != null) ? highPassFilter.cutoffFrequency : NeutralLowFrequency;

            // Preallocate the array that will be used to collect RaycastHit structures.
            hits = new RaycastHit[maxObjects];
        }

        private void Update() 
        {
            DateTime now = DateTime.UtcNow;

            // Audio influences are not updated every frame.
            if ((UpdateInterval * 1000.0f) <= (now - lastUpdate).TotalMilliseconds)
            {
                audioSource.volume = initialAudioSourceVolume;

                // Get the audio influencers that should apply to the audio source.
                List<IAudioInfluencer> influencers = GetInfluencers();
                for (int i = 0; i < influencers.Count; i++)
                {
                    // Apply the influencer's effect.
                    influencers[i].ApplyEffect(gameObject);
                }

                // Find and remove the audio influencers that are to be removed from the audio source.
                List<IAudioInfluencer> influencersToRemove = new List<IAudioInfluencer>();
                for (int i = 0; i < previousInfluencers.Count; i++)
                {
                    MonoBehaviour mbPrev = previousInfluencers[i] as MonoBehaviour;

                    // Remove influencers that are no longer in line of sight
                    // OR
                    // Have been disabled
                    if (!influencers.Contains(previousInfluencers[i]) ||
                        ((mbPrev != null) && !mbPrev.isActiveAndEnabled))
                    {
                        influencersToRemove.Add(previousInfluencers[i]);
                    }
                }
                RemoveInfluencers(influencersToRemove);

                previousInfluencers = influencers;
                lastUpdate = now;
            }
        }

        /// <summary>
        /// Removes the effects applied by specified audio influencers.
        /// </summary>
        /// <param name="influencers">Collection of IAudioInfluencer objects on which to remove the effect.</param>
        private void RemoveInfluencers(List<IAudioInfluencer> influencers)
        {
            for (int i = 0; i < influencers.Count; i++)
            {
                influencers[i].RemoveEffect(gameObject);
            }
        }

        /// <summary>
        /// Finds the IAudioInfluencer objects that are to be applied to the audio source.
        /// </summary>
        /// <returns>Collection of IAudioInfluencers between the user and the game object.</returns>
        private List<IAudioInfluencer> GetInfluencers()
        {
            List<IAudioInfluencer> influencers = new List<IAudioInfluencer>();
            Transform cameraTransform = CameraCache.Main.transform;

            // Influencers take effect only when between the emitter and the user.
            // Perform a raycast from the user toward the object.
            Vector3 direction = (gameObject.transform.position - cameraTransform.position).normalized;
            float distance = Vector3.Distance(cameraTransform.position, gameObject.transform.position);

            int count = UnityPhysics.RaycastNonAlloc(cameraTransform.position,
                                                direction,
                                                hits,
                                                distance,
                                                UnityPhysics.DefaultRaycastLayers,
                                                QueryTriggerInteraction.Ignore);
            
            for (int i = 0; i < count; i++)
            {
                IAudioInfluencer influencer = hits[i].collider.gameObject.GetComponentInParent<IAudioInfluencer>();
                if (influencer != null)
                {
                    influencers.Add(influencer);
                }
            }

            return influencers;
        }
    }
}