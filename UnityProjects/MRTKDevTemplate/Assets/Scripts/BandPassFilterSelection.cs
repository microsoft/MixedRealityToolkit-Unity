// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Audio;
using TMPro;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// Example script that changes the audio fidelity of the specified emitter and
    /// updates an on-screen caption to indicate which filter is being used.
    /// </summary>
    [AddComponentMenu("MRTK/Examples/Bandpass Filter Selection")]
    public class BandPassFilterSelection : MonoBehaviour
    {
        [SerializeField]
        private GameObject audioEmitter = null;

        [SerializeField]
        private AudioBandPassFilter filter = null;

        [SerializeField]
        private TMP_Text caption = null;

        private AudioBandPassEffect effect = null;

        private void Start()
        {
            effect = audioEmitter.EnsureComponent<AudioBandPassEffect>();
        }
        
        public void SetFilter()
        {
            effect.Filter = filter;
            caption.text = $"{filter.name} ({filter.LowFrequencyCutoff}Hz - {filter.HighFrequencyCutoff}Hz)";
        }
    }
}
