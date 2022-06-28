// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// An audio effect that limits the frequencies of a sound to a defined range.
    /// </summary>
    /// <remarks>
    /// For the best results, also attach an <see cref="AudioInfluencerController"/>
    /// to the sound source. This will ensure that the proper frequencies will be restored
    /// when audio influencers are used in the scene.
    /// </remarks>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioLowPassFilter))]
    [RequireComponent(typeof(AudioHighPassFilter))]
    [DisallowMultipleComponent]
    [AddComponentMenu("MRTK/Audio/Audio Bandpass Effect")]
    public class AudioBandPassEffect : MonoBehaviour
    {
        [Tooltip("The filter to apply to the audio source.")]
        [SerializeField]
        private AudioBandPassFilter filter;

        /// <summary>
        /// The filter to apply to the audio source.
        /// </summary>
        public AudioBandPassFilter Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        /// <summary>
        /// The audio influencer controller that will be updated when filter settings
        /// are changed.
        /// </summary>
        [SerializeField]
        [HideInInspector]   // The inspector will already have a reference to the object, this avoids duplication.
        private AudioInfluencerController influencerController = null;

        /// <summary>
        /// The filters used to simulate the source quality.
        /// </summary>
        private AudioLowPassFilter lowPassFilter;
        private AudioHighPassFilter highPassFilter;

        private void Awake()
        {
            lowPassFilter = gameObject.GetComponent<AudioLowPassFilter>();
            highPassFilter = gameObject.GetComponent<AudioHighPassFilter>();
            influencerController = gameObject.GetComponent<AudioInfluencerController>();

            ApplyFilter();
        }

        private AudioBandPassFilter previousFilter = null;

        private void Update()
        {
            if (previousFilter == Filter) { return; }

            // If we have an attached AudioInfluencerController, we must let it know
            // about our filter settings change, otherwise other effects may not behave
            // as expected.
            if (influencerController != null)
            {
                influencerController.NativeLowPassCutoffFrequency = Filter.LowFrequencyCutoff;
                influencerController.NativeHighPassCutoffFrequency = Filter.HighFrequencyCutoff;
            }

            ApplyFilter();
        }

        /// <summary>
        /// Sets the desired frequency range on the sound source.
        /// </summary>
        private void ApplyFilter()
        {
            lowPassFilter.cutoffFrequency = Filter.HighFrequencyCutoff;
            highPassFilter.cutoffFrequency = Filter.LowFrequencyCutoff;

            previousFilter = Filter;
        }
    }
}
