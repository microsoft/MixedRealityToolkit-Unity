// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.UX
{
    /// <summary>
    /// Component that plays sounds to communicate the state of a pinch slider
    /// </summary>
    [RequireComponent(typeof(Slider))]
    [AddComponentMenu("MRTK/UX/Slider Sounds")]
    public class SliderSounds : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Whether sounds play during any changes in value or only during interactions")]
        private bool playSoundsOnlyOnInteract = false;

        /// <summary>
        /// Whether sounds play during any changes in value or only during interactions
        /// </summary>
        public bool PlaySoundsOnlyOnInteract { get => playSoundsOnlyOnInteract; set => playSoundsOnlyOnInteract = value; }


        [Header("Audio Clips")]
        [SerializeField]
        [Tooltip("Sound to play when interaction with slider starts")]
        private AudioClip interactionStartSound = null;

        /// <summary>
        /// Sound to play when interaction with slider starts
        /// </summary>
        public AudioClip InteractionStartSound { get => interactionStartSound; set => interactionStartSound = value; }

        [SerializeField]
        [Tooltip("Sound to play when interaction with slider ends")]
        private AudioClip interactionEndSound = null;

        /// <summary>
        /// Sound to play when interaction with slider ends
        /// </summary>
        public AudioClip InteractionEndSound { get => interactionEndSound; set => interactionEndSound = value; }



        [Header("Tick Notch Sounds")]
        [SerializeField]
        [Tooltip("Whether to play 'tick tick' sounds as the slider passes notches")]
        private bool playTickSounds = true;

        /// <summary>
        /// Whether to play 'tick tick' sounds as the slider passes notches
        /// </summary>
        public bool PlayTickSounds { get => playTickSounds; set => playTickSounds = value; }

        [SerializeField]
        [Tooltip("Whether to line up the 'tick tick' sounds with slider step divisions when those are in use")]
        private bool alignWithStepSlider = true;

        /// <summary>
        /// Whether to line up the 'tick tick' sounds with slider step divisions when those are in use
        /// </summary>
        public bool AlignWithStepSlider { get => alignWithStepSlider; set => alignWithStepSlider = value; }

        [SerializeField]
        [Tooltip("Sound to play when slider passes a notch")]
        private AudioClip passNotchSound = null;

        /// <summary>
        /// Sound to play when slider passes a notch
        /// </summary>
        public AudioClip PassNotchSound { get => passNotchSound; set => passNotchSound = value; }

        [Range(0, 1)]
        [SerializeField]
        [Tooltip("The interval required for the passNotchSound to play")]
        private float tickEvery = 0.1f;

        /// <summary>
        /// The interval required for the passNotchSound to play
        /// </summary>
        public float TickEvery { get => tickEvery; set => tickEvery = value; }

        [SerializeField]
        [Tooltip("The starting pitch for the passNotchSound")]
        private float startPitch = 0.75f;

        /// <summary>
        /// The starting pitch for the passNotchSound
        /// </summary>
        public float StartPitch { get => startPitch; set => startPitch = value; }

        [SerializeField]
        [Tooltip("The ending pitch for the passNotchSound")]
        private float endPitch = 1.25f;

        /// <summary>
        /// The ending pitch for the passNotchSound
        /// </summary>
        public float EndPitch { get => endPitch; set => endPitch = value; }

        [SerializeField]
        [Tooltip("The min time required to pass before playing the passNotchSound again")]
        private float minSecondsBetweenTicks = 0.01f;

        /// <summary>
        /// The min time required to pass before playing the passNotchSound again
        /// </summary>
        public float MinSecondsBetweenTicks { get => minSecondsBetweenTicks; set => minSecondsBetweenTicks = value; }


        #region Private members
        private Slider slider;

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

            slider = GetComponent<Slider>();
            if (alignWithStepSlider && slider.UseSliderStepDivisions)
            {
                tickEvery = 1.0f / slider.SliderStepDivisions;
            }

            slider.firstSelectEntered.AddListener(OnInteractionStarted);
            slider.lastSelectExited.AddListener(OnInteractionEnded);
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

        private void OnInteractionStarted(SelectEnterEventArgs args)
        {
            isInteracting = true;
            if (interactionStartSound != null && grabReleaseAudioSource != null && grabReleaseAudioSource.isActiveAndEnabled)
            {
                grabReleaseAudioSource.PlayOneShot(interactionStartSound);
            }
        }

        private void OnInteractionEnded(SelectExitEventArgs args)
        {
            isInteracting = false;
            if (interactionEndSound != null && grabReleaseAudioSource != null && grabReleaseAudioSource.isActiveAndEnabled)
            {
                grabReleaseAudioSource.PlayOneShot(interactionEndSound);
            }
        }
    }


}