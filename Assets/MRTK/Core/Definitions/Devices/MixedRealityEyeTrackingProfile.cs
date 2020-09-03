// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Eye Tracking Profile", fileName = "MixedRealityEyeTrackingProfile", order = (int)CreateProfileMenuItemIndices.EyeTracking)]
    [MixedRealityServiceProfile(requiredTypes: new Type[] { typeof(IMixedRealityEyeGazeDataProvider), typeof(IMixedRealityEyeSaccadeProvider) })]
    public class MixedRealityEyeTrackingProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("Use smoothed eye tracking signal.")]
        private bool smoothEyeTracking = false;

        /// <summary>
        /// Use smoothed eye tracking signal.
        /// </summary>
        public bool SmoothEyeTracking => smoothEyeTracking;
    }
}