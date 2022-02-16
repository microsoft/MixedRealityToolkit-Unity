//
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
//
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// Component that plays sounds to communicate the state of a pinch slider
    /// </summary>
    [RequireComponent(typeof(PinchSlider))]
    [AddComponentMenu("Scripts/MRTK/SDK/SliderSounds")]
    public class SliderSounds : MonoBehaviour
    {
        [SerializeField]
        private bool playSoundsOnlyOnInteract = false;

        [Header("Audio Clips")]
        [SerializeField]
        [Tooltip("Sound to play when interaction with slider starts")]
        private AudioClip interactionStartSound = null;
        [SerializeField]
        [Tooltip("Sound to play when interaction with slider ends")]
        private AudioClip interactionEndSound = null;

        [Header("Tick Notch Sounds")]

        [SerializeField]
        [Tooltip("Whether to play 'tick tick' sounds as the slider passes notches")]
        private bool playTickSounds = true;

        [SerializeField]
        [Tooltip("Whether to line up the 'tick tick' sounds with slider step divisions when those are in use")]
        private bool alignWithStepSlider = true;

        [SerializeField]
        [Tooltip("Sound to play when slider passes a notch")]
        private AudioClip passNotchSound = null;

        [Range(0, 1)]
        [SerializeField]
        private float tickEvery = 0.1f;

        [SerializeField]
        private float startPitch = 0.75f;

        [SerializeField]
        private float endPitch = 1.25f;

        [SerializeField]
        private float minSecondsBetweenTicks = 0.01f;

        #region Private members
        private PinchSlider slider;

        // Check to see if the slider is being interacted with
        private bool isInteracting;

        // Play sound when passing through slider notches
        private float accumulatedDeltaSliderValue = 0;
        private float lastSoundPlayTime;

        private AudioSource grabReleaseAudioSource = null;
        private AudioSource passNotchAudioSource = null;
        #endregion

        private void Start()
        {
            if (grabReleaseAudioSource == null)
            {
                grabReleaseAudioSource = gameObject.AddComponent<AudioSource>();
            }
            if (passNotchAudioSource == null)
            {
                passNotchAudioSource = gameObject.AddComponent<AudioSource>();
            }

            slider = GetComponent<PinchSlider>();
            if (alignWithStepSlider && slider.UseSliderStepDivisions)
            {
                tickEvery = 1.0f / slider.SliderStepDivisions;
            }
            slider.OnInteractionStarted.AddListener(OnInteractionStarted);
            slider.OnInteractionEnded.AddListener(OnInteractionEnded);
            slider.OnValueUpdated.AddListener(OnValueUpdated);
        }

        private void OnValueUpdated(SliderEventData eventData)
        {
            if (!(playSoundsOnlyOnInteract && !isInteracting) && playTickSounds && passNotchAudioSource != null && passNotchSound != null)
            {
                float delta = eventData.NewValue - eventData.OldValue;
                accumulatedDeltaSliderValue += Mathf.Abs(delta);
                var now = Time.timeSinceLevelLoad;
                if (accumulatedDeltaSliderValue >= tickEvery && now - lastSoundPlayTime > minSecondsBetweenTicks)
                {
                    passNotchAudioSource.pitch = Mathf.Lerp(startPitch, endPitch, eventData.NewValue);
                    if (passNotchAudioSource.isActiveAndEnabled)
                    {
                        passNotchAudioSource.PlayOneShot(passNotchSound);
                    }

                    accumulatedDeltaSliderValue = 0;
                    lastSoundPlayTime = now;
                }
            }
        }

        private void OnInteractionEnded(SliderEventData arg0)
        {
            isInteracting = false;
            if (interactionEndSound != null && grabReleaseAudioSource != null && grabReleaseAudioSource.isActiveAndEnabled)
            {
                grabReleaseAudioSource.PlayOneShot(interactionEndSound);
            }
        }

        private void OnInteractionStarted(SliderEventData arg0)
        {
            isInteracting = true;
            if (interactionStartSound != null && grabReleaseAudioSource != null && grabReleaseAudioSource.isActiveAndEnabled)
            {
                grabReleaseAudioSource.PlayOneShot(interactionStartSound);
            }
        }
    }


}