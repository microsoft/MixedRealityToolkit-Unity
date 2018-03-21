// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// An audio effect that limits the frequency range of a sound to simulate
    /// being played over various telephony or radio sources.
    /// </summary>
    /// <remarks>
    /// For the best results, also attach an AudioInfluencerManager to the sound
    /// source. This will ensure that the proper frequencies will be restored
    /// when audio influencers are used in the scene.
    /// </remarks>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioLowPassFilter))]
    [RequireComponent(typeof(AudioHighPassFilter))]
    [DisallowMultipleComponent]
    public class AudioLoFiEffect : MonoBehaviour
    {
        /// <summary>
        /// The quality level of the simulated audio source (ex: AM radio).
        /// </summary>
        [Tooltip("The quality level of the simulated audio source.")]
        [SerializeField]
        private AudioLoFiSourceQuality sourceQuality;
        public AudioLoFiSourceQuality SourceQuality
        {
            get { return sourceQuality; }
            set { sourceQuality = value;  }
        }

        /// <summary>
        /// The audio filter settings that match the selected source quality.
        /// </summary>
        private AudioLoFiFilterSettings filterSettings;

        /// <summary>
        /// The filters used to simulate the source quality.
        /// </summary>
        private AudioLowPassFilter lowPassFilter;
        private AudioHighPassFilter highPassFilter;

        /// <summary>
        /// Collection used to look up the filter settings that match the selected
        /// source quality.
        /// </summary>
        private Dictionary<AudioLoFiSourceQuality, AudioLoFiFilterSettings> sourceQualityFilterSettings =
            new Dictionary<AudioLoFiSourceQuality, AudioLoFiFilterSettings>();

        private void Awake()
        {
            LoadQualityFilterSettings();

            filterSettings = sourceQualityFilterSettings[SourceQuality];

            lowPassFilter = gameObject.GetComponent<AudioLowPassFilter>();
            lowPassFilter.cutoffFrequency = filterSettings.LowPassCutoff;

            highPassFilter = gameObject.GetComponent<AudioHighPassFilter>();
            highPassFilter.cutoffFrequency = filterSettings.HighPassCutoff;
        }

        private void Update()
        {
            AudioLoFiFilterSettings newSettings = sourceQualityFilterSettings[SourceQuality];
            if (newSettings != filterSettings)
            {
                // If we have an attached AudioInfluencerManager, we need to let it know
                // about our filter settings change.
                AudioEmitter influencerManager = gameObject.GetComponent<AudioEmitter>();
                if (influencerManager != null)
                {
                    influencerManager.SetNativeLowPassCutoffFrequency(newSettings.LowPassCutoff);
                    influencerManager.SetNativeHighPassCutoffFrequency(newSettings.HighPassCutoff);
                }

                filterSettings = newSettings;
                lowPassFilter.cutoffFrequency = filterSettings.LowPassCutoff;
                highPassFilter.cutoffFrequency = filterSettings.HighPassCutoff;
            }
        }

        /// <summary>
        /// Populates the source quality filter settings collection.
        /// </summary>
        private void LoadQualityFilterSettings()
        {
            if (sourceQualityFilterSettings.Keys.Count > 0) { return; }

            sourceQualityFilterSettings.Add(
                AudioLoFiSourceQuality.FullRange,
                new AudioLoFiFilterSettings(10, 22000));
            sourceQualityFilterSettings.Add(
                AudioLoFiSourceQuality.NarrowBandTelephony,
                new AudioLoFiFilterSettings(300, 3400));
            sourceQualityFilterSettings.Add(
                AudioLoFiSourceQuality.WideBandTelephony,
                new AudioLoFiFilterSettings(50, 7000));
            sourceQualityFilterSettings.Add(
                AudioLoFiSourceQuality.AmRadio,
                new AudioLoFiFilterSettings(40, 5000));
            sourceQualityFilterSettings.Add(
                AudioLoFiSourceQuality.FmRadio,
                new AudioLoFiFilterSettings(30, 15000));
        }

        /// <summary>
        /// Settings for the filters used to simulate a low fidelity sound source.
        /// </summary>
        /// <remarks>
        /// This struct is soley for the private use of the AudioLoFiEffect class.
        /// </remarks>
        private struct AudioLoFiFilterSettings
        {
            /// <summary>
            /// The frequency below which sound will be heard.
            /// </summary>
            public float LowPassCutoff
            { get; private set; }

            /// <summary>
            /// The frequency above which sound will be heard.
            /// </summary>
            public float HighPassCutoff
            { get; private set; }

            /// <summary>
            /// FilterSettings constructor.
            /// </summary>
            /// <param name="highPassCutoff">High pass filter cutoff frequency.</param>
            /// <param name="lowPassCutoff">Low pass filter cutoff frequency.</param>
            public AudioLoFiFilterSettings(float highPassCutoff, float lowPassCutoff)
            {
                HighPassCutoff = highPassCutoff;
                LowPassCutoff = lowPassCutoff;
            }

            /// <summary>
            /// Checks to see if two FilterSettings objects are equivalent.
            /// </summary>
            /// <returns>True if equivalent, false otherwise.</returns>
            public static bool operator ==(AudioLoFiFilterSettings a, AudioLoFiFilterSettings b)
            {
                return a.Equals(b);
            }

            /// <summary>
            /// Checks to see if two FilterSettings objects are not equivalent.
            /// </summary>
            /// <returns>False if equivalent, true otherwise.</returns>
            public static bool operator !=(AudioLoFiFilterSettings a, AudioLoFiFilterSettings b)
            {
                return !(a.Equals(b));
            }

            /// <summary>
            /// Checks to see if a object is equivalent to this FilterSettings.
            /// </summary>
            /// <returns>True if equivalent, false otherwise.</returns>
            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                if (!(obj is AudioLoFiFilterSettings))
                {
                    return false;
                }

                AudioLoFiFilterSettings other = (AudioLoFiFilterSettings)obj;
                if ((other.LowPassCutoff != LowPassCutoff) ||
                    (other.HighPassCutoff != HighPassCutoff))
                {
                    return false;
                }

                return true;
            }

            /// <summary>
            /// Generates a hash code representing this FilterSettings.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                string s = string.Format(
                    "[AudioLoFiFilterSettings] Low: {0}, High: {1}",
                    LowPassCutoff,
                    HighPassCutoff);

                return s.GetHashCode();
            }
        }
    }

    /// <summary>
    /// Source quality options, used by the AudioLoFiEffect, that match common telephony and
    /// radio broadcast options.
    /// </summary>
    public enum AudioLoFiSourceQuality
    {
        /// <summary>
        /// Narrow frequency range telephony.
        /// </summary>
        NarrowBandTelephony,

        /// <summary>
        /// Wide frequency range telephony.
        /// </summary>
        WideBandTelephony,

        /// <summary>
        /// AM radio.
        /// </summary>
        AmRadio,

        /// <summary>
        /// FM radio.
        /// </summary>
        /// <remarks>
        /// The FM radio frequency is quite wide as it relates to human hearing. While it is
        /// a lower fidelity than FullRange, some users may not hear a difference.
        /// </remarks>
        FmRadio,

        /// <summary>
        /// Full range of human hearing.
        /// </summary>
        /// <remarks>
        /// The frequency range used is a bit wider than that of human
        /// hearing. It closely resembles the range used for audio CDs.
        /// </remarks>
        FullRange
    }
}
