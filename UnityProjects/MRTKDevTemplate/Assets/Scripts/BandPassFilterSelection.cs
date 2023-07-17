// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
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
        private List<AudioBandPassFilter> filters = new List<AudioBandPassFilter>();

        private AudioBandPassEffect effect = null;

        /// <summary>
        /// A Unity event function that is called on the frame when a script is enabled just before any of the update methods are called the first time.
        /// </summary> 
        private void Start()
        {
            effect = audioEmitter.EnsureComponent<AudioBandPassEffect>();
        }
        
        public void SetFilter(int index)
        {
            effect.Filter = filters[index];
        }
    }
}
