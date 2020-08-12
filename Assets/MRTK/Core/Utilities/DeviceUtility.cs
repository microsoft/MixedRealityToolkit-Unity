// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if !UNITY_2020_1_OR_NEWER
using UnityEngine.XR;
#endif // UNITY_2019_3_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Helps with getting data from or about the current HMD or other XR device.
    /// </summary>
    public static class DeviceUtility
    {
        /// <summary>
        /// If an HMD is present and running.
        /// </summary>
        public static bool IsPresent
        {
            get
            {
#if UNITY_2019_3_OR_NEWER
                if (XRSubsystemHelpers.DisplaySubsystem != null)
                {
                    return true;
                }
#endif // UNITY_2019_3_OR_NEWER

#if UNITY_2020_1_OR_NEWER
                return false;
#else
                return XRDevice.isPresent;
#endif // UNITY_2020_1_OR_NEWER
            }
        }
    }
}
