// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityPhysics = UnityEngine.Physics;

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// Class which supports components implementing <see cref="IAudioInfluencer"/>.
    /// </summary>
    /// <remarks>
    /// AudioInfluencerController requires an <see href="https://docs.unity3d.com/ScriptReference/AudioSource.html">AudioSource</see> component.
    /// If one is not attached, it will be added automatically.
    /// <para>
    /// Each sound playing game object should have an AudioInfluencerController
    /// attached in order to have its audio properly influenced.
    /// </para>
    /// </remarks>
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    [AddComponentMenu("MRTK/Audio/Audio Influencer Controller")]
    public class AudioInfluencerController : MonoBehaviour
    {
        /// <summary>
        /// Frequency below the nominal range of human hearing.
        /// </summary>
        /// <remarks>
        /// This frequency can be used to set a high pass filter to allow all
        /// human audible frequencies through the filter.
        /// </remarks>
        public static readonly float NeutralLowFrequency = 10.0f;

        /// <summary>
        /// Frequency above the nominal range of human hearing.
        /// </summary>
        /// <remarks>
        /// This frequency can be used to set a low pass filter to allow all
        /// human audible frequencies through the filter.
        /// </remarks>
        public static readonly float NeutralHighFrequency = 22000.0f;

        /// <summary>
        /// The source of the audio.
        /// </summary>
        private AudioSource audioSource;

        /// <summary>
        /// Time, in seconds, between audio influence updates.
        /// </summary>
        /// <remarks>
        /// The UpdateInterval range is between 0.0 and 1.0, inclusive. The default
        /// value is 0.25.
        /// <para>
        /// A value of 0.0f indicates that updates occur every frame.
        /// </para>
        /// </remarks>
        [Tooltip("Time, in seconds, between audio influence updates. 0 indicates to update every frame.")]
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
        /// Maximum distance, in meters, to look when attempting to find the user and
        /// any influencers.
        /// </summary>
        /// <remarks>
        /// The MaxDistance range is 1.0 to 50.0, inclusive. The default value is 20.0.
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
        /// Setting this value too high may have a negative impact on the performance of your
        /// experience.
        /// </summary>
        /// <remarks>
        /// <para>MaxObjects can only be set in the Unity Inspector.
        /// The MaxObjects range is 1 to 25, inclusive.
        /// The default value is 10.</para>
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
        /// The initial volume level of the audio source.
        /// </summary>
        private float initialAudioSourceVolume;

        /// <summary>
        /// The hits returned by Physics.RaycastAll
        /// </summary>
        private RaycastHit[] hits;

        /// <summary>
        /// The collection of applied audio effects.
        /// </summary>
        private List<IAudioInfluencer> currentEffects = new List<IAudioInfluencer>();

        private float nativeLowPassCutoffFrequency;

        /// <summary>
        /// Gets or sets the native low pass cutoff frequency for the
        /// sound emitter.
        /// </summary>
        public float NativeLowPassCutoffFrequency
        {
            get { return nativeLowPassCutoffFrequency; }
            set { nativeLowPassCutoffFrequency = value; }
        }

        private float nativeHighPassCutoffFrequency;

        /// <summary>
        /// Gets or sets the native high pass cutoff frequency for the
        /// sound emitter.
        /// </summary>
        public float NativeHighPassCutoffFrequency
        {
            get { return nativeHighPassCutoffFrequency; }
            set { nativeHighPassCutoffFrequency = value; }
        }

        private List<IAudioInfluencer> effectsToApply = null;
        private List<IAudioInfluencer> effectsToRemove = null;

        private float nextUpdate = 0f;

        private void Awake()
        {
            effectsToApply = new List<IAudioInfluencer>(maxObjects);
            effectsToRemove = new List<IAudioInfluencer>(maxObjects);

            audioSource = GetComponent<AudioSource>();

            initialAudioSourceVolume = audioSource.volume;

            // Get initial values that the sound designer / developer 
            // may have applied to this game object
            AudioLowPassFilter lowPassFilter = gameObject.GetComponent<AudioLowPassFilter>();
            nativeLowPassCutoffFrequency = (lowPassFilter != null) ? lowPassFilter.cutoffFrequency : NeutralHighFrequency;
            AudioHighPassFilter highPassFilter = gameObject.GetComponent<AudioHighPassFilter>();
            nativeHighPassCutoffFrequency = (highPassFilter != null) ? highPassFilter.cutoffFrequency : NeutralLowFrequency;

            // Preallocate the array that will be used to collect RaycastHit structures.
            hits = new RaycastHit[maxObjects];

            // Initialize our update time.
            nextUpdate = Time.time;
        }

        private void Update()
        {
            // Audio influences are generally not updated every frame.
            if (Time.time < nextUpdate) { return; }

            audioSource.volume = initialAudioSourceVolume;

            // Apply audio influencers to the audio source.
            ApplyEffects();

            // Remove the audio influencers that no longer apply.
            RemoveEffects();

            currentEffects.Clear();
            currentEffects.AddRange(effectsToApply);

            nextUpdate += UpdateInterval;
        }

        /// <summary>
        /// Applies the effects specified by the collection of audio influencers.
        /// </summary>
        private void ApplyEffects()
        {
            effectsToApply.Clear();
            UpdateActiveInfluencerCollection();

            foreach (IAudioInfluencer influencer in effectsToApply)
            {
                influencer.ApplyEffect(gameObject);
            }
        }

        /// <summary>
        /// Removes the effects applied by specified audio influencers.
        /// </summary>
        private void RemoveEffects()
        {
            effectsToRemove.Clear();

            for (int i = 0; i < currentEffects.Count; i++)
            {
                IAudioInfluencer audioInfluencer = currentEffects[i];

                // Find influencers that are no longer in line of sight,
                // have been destroyed, or have been disabled
                if (!effectsToApply.Contains(audioInfluencer) ||
                    !audioInfluencer.TryGetMonoBehaviour(out MonoBehaviour mbPrev) ||
                    !mbPrev.isActiveAndEnabled)
                {
                    effectsToRemove.Add(audioInfluencer);
                }
            }

            foreach (IAudioInfluencer influencer in effectsToRemove)
            {
                influencer.RemoveEffect(gameObject);
            }
        }

        /// <summary>
        /// Finds the IAudioInfluencer objects that are to be applied to the audio source.
        /// </summary>
        private void UpdateActiveInfluencerCollection()
        {
            Transform cameraTransform = Camera.main.transform;

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
                    effectsToApply.Add(influencer);
                }
            }
        }
    }
}
