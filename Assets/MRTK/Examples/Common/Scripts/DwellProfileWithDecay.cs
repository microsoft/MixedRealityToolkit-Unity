// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Dwell
{
    /// <summary>
    /// Custom profile for CustomDwellHandler to show the dwell with decay behavior
    /// </summary>
    [MixedRealityServiceProfile(typeof(DwellProfileWithDecay))]
    [CreateAssetMenu(menuName = "Mixed Reality/Toolkit/Profiles/Dwell Profile With Decay", fileName = "DwellProfileWithDecay", order = 100)]
    [Serializable]
    public class DwellProfileWithDecay : DwellProfile
    {
        [Tooltip("Time in seconds when gaze can fall off the target and come back.")]
        [SerializeField]
        [Range(0, 20)]
        private float timeToAllowDwellDecay = 20;

        /// <summary>
        /// Time in seconds when gaze can fall off the target and come back.
        /// </summary>
        public float TimeToAllowDwellDecay
        {
            get => timeToAllowDwellDecay;
            set => timeToAllowDwellDecay = value;
        }
    }
}
