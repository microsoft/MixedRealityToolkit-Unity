// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if UNITY_2019_3_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
#endif // UNITY_2019_3_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Utilities that abstract XR settings functionality so that the MRTK need not know which
    /// implementation is being used.
    /// </summary>
    public static class XRSettingsUtilities
    {
#if UNITY_2019_3_OR_NEWER && !UNITY_2020_2_OR_NEWER
        private static bool? legacyXRAvailable = null;
#endif // UNITY_2019_3_OR_NEWER && !UNITY_2020_2_OR_NEWER

        /// <summary>
        /// Checks if an XR SDK plug-in is installed that disables legacy VR. Returns false if so.
        /// </summary>
        public static bool LegacyXRAvailable
        {
            get
            {
#if UNITY_2020_2_OR_NEWER
                return false;
#elif UNITY_2019_3_OR_NEWER
                if (!legacyXRAvailable.HasValue)
                {
                    legacyXRAvailable = true;

                    List<XRDisplaySubsystemDescriptor> descriptors = new List<XRDisplaySubsystemDescriptor>();
                    SubsystemManager.GetSubsystemDescriptors(descriptors);

                    foreach (XRDisplaySubsystemDescriptor displayDescriptor in descriptors)
                    {
                        if (displayDescriptor.disablesLegacyVr)
                        {
                            legacyXRAvailable = false;
                            break;
                        }
                    }
                }

                return legacyXRAvailable.HasValue && legacyXRAvailable.Value;
#else
                return true;
#endif // UNITY_2020_2_OR_NEWER
            }
        }
    }
}
