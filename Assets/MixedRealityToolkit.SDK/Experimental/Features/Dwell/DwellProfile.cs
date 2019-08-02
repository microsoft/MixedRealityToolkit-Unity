// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    public class DwellProfile : ScriptableObject
    {
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

        //[Tooltip("Additional time (not including the timeToTriggerDwellInSec) the user needs to keep looking at the UI to trigger select on it.")]
        //[SerializeField]
        //private bool allowFocusLossAfterDwellStarts

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
    }
}
