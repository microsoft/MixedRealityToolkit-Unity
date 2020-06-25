// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    [MixedRealityServiceProfile(typeof(DwellProfile))]
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Dwell Profile", fileName = "DwellProfile", order = 100)]
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

        [Tooltip("Time in seconds when focus can fall off the target and come back to resume an ongoing dwell.This only comes into play after DwellStarted state but before DwellCompleted is invoked.")]
        [SerializeField]
        [Range(0, 20)]
        private float timeToAllowDwellResume = 0;

        public InputSourceType DwellPointerType
        {
            get
            {
                return dwellTriggerPointerType;
            }
        }

        public TimeSpan DwellIntentDelay
        {
            get
            {
                return TimeSpan.FromSeconds(dwellIntentDelay);
            }
        }

        public TimeSpan DwellStartDelay
        {
            get
            {
                return TimeSpan.FromSeconds(dwellStartDelay);
            }
        }

        public TimeSpan TimeToCompleteDwell
        {
            get
            {
                return TimeSpan.FromSeconds(timeToCompleteDwell);
            }
        }

        public TimeSpan TimeToAllowDwellResume
        {
            get
            {
                return TimeSpan.FromSeconds(timeToAllowDwellResume);
            }
        }
    }
}
