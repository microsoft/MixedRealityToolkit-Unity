// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioLowPassFilter))]
    [RequireComponent(typeof(AudioHighPassFilter))]
    [DisallowMultipleComponent]
    public class AudioLoFiEffect : MonoBehaviour
    {
        public AudioSourceQuality SourceQuality;

        private FilterSettings filterSettings;

        private AudioLowPassFilter lowPassFilter;
        private AudioHighPassFilter highPassFilter;

        private Dictionary<AudioSourceQuality, FilterSettings> sourceQualityFilterSettings =
            new Dictionary<AudioSourceQuality, FilterSettings>();

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
            FilterSettings fs = sourceQualityFilterSettings[SourceQuality];
            if (fs != filterSettings)
            {
                // If we have an attached AudioInfluencerManager, we need to let it know
                // about our filter settings change.
                AudioInfluencerManager influencerManager = gameObject.GetComponent<AudioInfluencerManager>();
                if (influencerManager != null)
                {
                    influencerManager.SetNativeLowPassCutoffFrequency(fs.LowPassCutoff);
                    influencerManager.SetNativeHighPassCutoffFrequency(fs.HighPassCutoff);
                }

                filterSettings = fs;
                lowPassFilter.cutoffFrequency = filterSettings.LowPassCutoff;
                highPassFilter.cutoffFrequency = filterSettings.HighPassCutoff;
            }
        }

        private void LoadQualityFilterSettings()
        {
            if (sourceQualityFilterSettings.Keys.Count > 0) { return; }

            sourceQualityFilterSettings.Add(
                AudioSourceQuality.FullRange,
                new FilterSettings(10, 22000));
            sourceQualityFilterSettings.Add(
                AudioSourceQuality.NarrowBandTelephone,
                new FilterSettings(300, 3400));
            sourceQualityFilterSettings.Add(
                AudioSourceQuality.WideBandTelephone,
                new FilterSettings(50, 7000));
            sourceQualityFilterSettings.Add(
                AudioSourceQuality.AmRadio,
                new FilterSettings(40, 5000));
            sourceQualityFilterSettings.Add(
                AudioSourceQuality.FmRadio,
                new FilterSettings(30, 15000));
        }

        public enum AudioSourceQuality
        {
            /// <summary>
            /// 
            /// </summary>
            NarrowBandTelephone,

            /// <summary>
            /// 
            /// </summary>
            WideBandTelephone,

            /// <summary>
            /// 
            /// </summary>
            AmRadio,

            /// <summary>
            /// 
            /// </summary>
            FmRadio,

            /// <summary>
            /// The Full range quality covers the entire range of human hearing.
            /// Any reduction of quality is a result of the AudioSpeakerQuality selection.
            /// </summary>
            FullRange,

            // todo
            ///// <summary>
            ///// 
            ///// </summary>
            //Custom
        }

        /// <summary>
        /// 
        /// </summary>
        public struct FilterSettings
        {
            /// <summary>
            /// 
            /// </summary>
            public float LowPassCutoff
            { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public float HighPassCutoff
            { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="highPassCutoff"></param>
            /// <param name="lowPassCutoff"></param>
            public FilterSettings(
                float highPassCutoff,
                float lowPassCutoff)
            {
                HighPassCutoff = highPassCutoff;
                LowPassCutoff = lowPassCutoff;
            }

            public static bool operator ==(FilterSettings a, FilterSettings b)
            {
                return a.Equals(b);
            }

            public static bool operator !=(FilterSettings a, FilterSettings b)
            {
                return !(a.Equals(b));
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                if (!(obj is FilterSettings))
                {
                    return false;
                }

                FilterSettings other = (FilterSettings)obj;
                if ((other.LowPassCutoff != LowPassCutoff) ||
                    (other.HighPassCutoff != HighPassCutoff))
                {
                    return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                string s = string.Format(
                    "[FilterSettings] Low: {0}, High: {1}",
                    LowPassCutoff,
                    HighPassCutoff);

                return s.GetHashCode();
            }
        }
    }
}
