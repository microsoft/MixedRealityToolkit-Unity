// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Audio
{
    /// <summary>
    /// Defines an audio bandpass (frequencies between a lower and upper value
    /// are preserved) filter.
    /// </summary>
    [CreateAssetMenu(fileName = "BandPassFilter.asset", menuName = "MRTK/Audio/Bandpass Filter")]
    public class AudioBandPassFilter : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The frequency below which sound will not be heard")]
        [Range(10f, 22000f)]
        private float lowFrequencyCutoff = 10f;

        /// <summary>
        /// The frequency below which sound will not be heard.
        /// </summary>
        public float LowFrequencyCutoff
        {
            get => lowFrequencyCutoff;
            set => lowFrequencyCutoff = value;
        }

        [SerializeField]
        [Tooltip("The frequency above which sound will not be heard")]
        [Range(10f, 22000f)]
        private float highFrequencyCutoff = 22000f;

        /// <summary>
        /// The frequency above which sound will not be heard.
        /// </summary
        public float HighFrequencyCutoff
        {
            get => highFrequencyCutoff;
            set => highFrequencyCutoff = value;
        }
    }
}
