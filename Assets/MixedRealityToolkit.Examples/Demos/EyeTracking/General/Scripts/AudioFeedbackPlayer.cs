// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// AudioFeedbackPlayer eases playing single audio feedback. Good for audio effects.
    /// </summary>
    public class AudioFeedbackPlayer : MonoBehaviour
    {
        /// <summary>
        /// Private audio source that will play the sound.
        /// </summary>
        private AudioSource audioSource;

        public static AudioFeedbackPlayer Instance { get; private set; }

        private void Start()
        {
            // Initialize audio source
            audioSource = SetupAudioSource(gameObject);

            if (Instance == null)
            {
                Instance = this;
            }
        }

        /// <summary>
        /// Ensures an audio source on the GameObject and returns it.
        /// </summary>
        /// <param name="targetGameObject">The GameObject to play the desired audio.</param>
        /// <returns>The AudioSource on the GameObject.</returns>
        public AudioSource SetupAudioSource(GameObject targetGameObject)
        {
            AudioSource audioSource = targetGameObject.EnsureComponent<AudioSource>();

            if (audioSource != null)
            {
                audioSource.playOnAwake = false;
                audioSource.enabled = true;
            }

            return audioSource;
        }

        /// <summary>
        /// Play a sound on the most recently set up GameObject.
        /// </summary>
        /// <param name="audiofx">The AudioClip to play.</param>
        public void PlaySound(AudioClip audiofx)
        {
            // Play audio clip
            if ((audioSource != null) && (audiofx != null))
            {
                audioSource.PlayOneShot(audiofx);
            }
        }
    }
}