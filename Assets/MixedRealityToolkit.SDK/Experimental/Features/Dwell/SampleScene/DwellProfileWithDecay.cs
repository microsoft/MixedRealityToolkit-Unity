
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    [MixedRealityServiceProfile(typeof(DwellProfileWithDecay))]
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Dwell Profile With Decay", fileName = "DwellProfileWithDecay", order = 100)]
    [Serializable]
    public class DwellProfileWithDecay : DwellProfile
    {
        [Tooltip("Should the system allow for dwell to resume if the pointer exits the target briefly.")]
        [SerializeField]
        private bool allowDwellDecayOnCancel = false;

        [Tooltip("Time in seconds when gaze can fall off the target and come back.")]
        [SerializeField]
        [Range(0, 60)]
        private float timeToAllowDwellDecay = 30;

        public bool AllowDwellDecayOnCancel
        {
            get
            {
                return allowDwellDecayOnCancel;
            }
        }

        public float TimeToAllowDwellDecay
        {
            get
            {
                return timeToAllowDwellDecay;
            }
        }
    }
}
