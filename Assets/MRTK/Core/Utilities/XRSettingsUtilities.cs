// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if XR_MANAGEMENT_ENABLED
using UnityEngine.XR.Management;
#endif // XR_MANAGEMENT_ENABLED

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Utilities that abstract XR settings functionality so that the MRTK need not know which
    /// implementation is being used.
    /// </summary>
    public static class XRSettingsUtilities
    {
#if !UNITY_2020_2_OR_NEWER && UNITY_2019_3_OR_NEWER && XR_MANAGEMENT_ENABLED
        private static bool? isXRSDKEnabled = null;
#endif // !UNITY_2020_2_OR_NEWER && UNITY_2019_3_OR_NEWER && XR_MANAGEMENT_ENABLED

        /// <summary>
        /// Checks if an XR SDK plug-in is installed that disables legacy VR. Returns false if so.
        /// </summary>
        public static bool XRSDKEnabled
        {
            get
            {
#if UNITY_2020_2_OR_NEWER
                return true;
#elif UNITY_2019_3_OR_NEWER && XR_MANAGEMENT_ENABLED
                if (!isXRSDKEnabled.HasValue)
                {
                    XRGeneralSettings currentSettings = XRGeneralSettings.Instance;
                    if (currentSettings != null && currentSettings.AssignedSettings != null)
                    {
#pragma warning disable CS0618 // Suppressing the warning to support xr management plugin 3.x and 4.x
                        isXRSDKEnabled = currentSettings.AssignedSettings.loaders.Count > 0;
#pragma warning restore CS0618
                    }
                    else
                    {
                        isXRSDKEnabled = false;
                    }
                }
                return isXRSDKEnabled.Value;
#else
                return false;
#endif // UNITY_2020_2_OR_NEWER
            }
        }
    }
}
