// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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

        [Tooltip("Delay in seconds until dwell feedback is started to be shown.")]
        [SerializeField]
        [Range(0, 5)]
        private float dwellStartDelay = 3;

        [Tooltip("Additional time (not including the timeToTriggerDwellInSec) the user needs to keep looking at the UI to trigger select on it.")]
        [SerializeField]
        [Range(0, 20)]
        private float timeToCompleteDwell = 6;

        [Tooltip("Should the system allow for dwell to resume if the pointer exits the target briefly.")]
        [SerializeField]
        private bool allowDwellResume = false;

        [Tooltip("Time in seconds when gaze can fall off the target and come back.")]
        [SerializeField]
        [Range(0, 2)]
        private float timeToAllowDwellResume = 1;

        public InputSourceType DwellPointerType
        {
            get
            {
                return dwellTriggerPointerType;
            }
        }

        public float DwellIntentDelay
        {
            get
            {
                return dwellIntentDelay;
            }
        }

        public float DwellStartDelay
        {
            get
            {
                return dwellStartDelay;
            }
        }

        public float TimeToCompleteDwell
        {
            get
            {
                return timeToCompleteDwell;
            }
        }

        public bool AllowDwellResume
        {
            get
            {
                return allowDwellResume;
            }
        }

        public float TimeToAllowDwellResume
        {
            get
            {
                return timeToAllowDwellResume;
            }
        }
    }
}
