// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Eye Tracking Profile", fileName = "MixedRealityEyeTrackingProfile", order = (int)CreateProfileMenuItemIndices.EyeTracking)]
    [MixedRealityServiceProfile(typeof(IMixedRealityEyeGazeDataProvider))]
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