// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Dwell
{
    /// <summary>
    /// Profile used by dwell handler to configure the various thresholds.
    /// </summary>
    [MixedRealityServiceProfile(typeof(DwellProfile))]
    [CreateAssetMenu(menuName = "Mixed Reality/Toolkit/Profiles/Dwell Profile", fileName = "DwellProfile", order = 100)]
    [Serializable]
    public class DwellProfile : ScriptableObject
    {
        [Tooltip("Pointer type to use for triggering a dwell interaction")]
        [SerializeField]
        private InputSourceType dwellTriggerPointerType = InputSourceType.Head;

        [Tooltip("Delay in seconds until it is determined that the user intends to interact with the target.")]
        [SerializeField]
        [Range(0, 2)]
        private float dwellIntentDelay = 0;

        [Tooltip("Delay in seconds until DwellStarted event is invoked.")]
        [SerializeField]
        [Range(0, 5)]
        private float dwellStartDelay = 0.5f;

        [Tooltip("Additional time in seconds (not including the dwellStartDelay) the user needs to keep looking at the UI to trigger select on it. Raises DwellCompleted event.")]
        [SerializeField]
        [Range(0, 20)]
        private float timeToCompleteDwell = 4;

        [Tooltip("Time in seconds when focus can fall off the target and come back to resume an ongoing dwell. This only comes into play after DwellStarted state but before DwellCompleted is invoked.")]
        [SerializeField]
        [Range(0, 20)]
        private float timeToAllowDwellResume = 0;

        [Tooltip("Whether the dwell timer decays when not in focus. If false, all dwell progress is lost at once after timeToAllowDwellResume elapses. Otherwise, the dwell progress decreases over time. ")]
        [SerializeField]
        private bool decayDwellOverTime = false;


        /// <summary>
        /// Pointer type to use for triggering a dwell interaction
        /// </summary>
        public InputSourceType DwellPointerType
        {
            get => dwellTriggerPointerType;
            set => dwellTriggerPointerType = value;
        }

        /// <summary>
        /// Delay in seconds until it is determined that the user intends to interact with the target.
        /// </summary>
        public float DwellIntentDelay
        {
            get => dwellIntentDelay;
            set => dwellIntentDelay = value;
        }

        /// <summary>
        /// Delay in seconds until DwellStarted event is invoked.
        /// </summary>
        public float DwellStartDelay
        {
            get => dwellStartDelay;
            set => dwellStartDelay = value;
        }

        /// <summary>
        /// Additional time in seconds (not including the dwellStartDelay) the user needs to keep looking at the UI to trigger select on it. Raises DwellCompleted event.
        /// </summary>
        public float TimeToCompleteDwell
        {
            get => timeToCompleteDwell;
            set => timeToCompleteDwell = value;
        }

        /// <summary>
        /// Time in seconds when focus can fall off the target and come back to resume an ongoing dwell. This only comes into play after DwellStarted state but before DwellCompleted is invoked.
        /// </summary>
        public float TimeToAllowDwellResume
        {
            get => timeToAllowDwellResume;
            set => timeToAllowDwellResume = value;
        }

        public bool DecayDwellOverTime
        {
            get => decayDwellOverTime;
            set => decayDwellOverTime = value;
        }
    }
}
