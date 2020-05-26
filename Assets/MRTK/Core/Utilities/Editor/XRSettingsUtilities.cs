// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

#if UNITY_2019_3_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
#endif // UNITY_2019_3_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Utilities that abstract XR settings functionality so that the MRTK need not know which
    /// implementation is being used.
    /// </summary>
    public static class XRSettingsUtilities
    {
#if UNITY_2019_3_OR_NEWER
        static XRSettingsUtilities()
        {
            // Called when packages are installed or uninstalled
            EditorApplication.projectChanged += EditorApplication_projectChanged;
        }
#endif // UNITY_2019_3_OR_NEWER

        /// <summary>
        /// Gets or sets the legacy virtual reality supported property in the player settings.
        /// </summary>
        /// <remarks>Returns true if legacy XR is disabled due to XR SDK in Unity 2019.3+.</remarks>
        public static bool LegacyXREnabled
        {
            get
            {
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements. Returns true if legacy XR is disabled due to XR SDK.
#pragma warning disable 0618
                return !IsLegacyXRActive || PlayerSettings.virtualRealitySupported;
#pragma warning restore 0618
            }

            set
            {
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                PlayerSettings.virtualRealitySupported = value;
#pragma warning restore 0618
            }
        }

#if UNITY_2019_3_OR_NEWER
        private static bool? isLegacyXRActive = null;
#endif // UNITY_2019_3_OR_NEWER

        /// <summary>
        /// Checks if an XR SDK plug-in is installed that disables legacy VR. Returns false if so.
        /// </summary>
        public static bool IsLegacyXRActive
        {
            get
            {
#if UNITY_2019_3_OR_NEWER
                if (!isLegacyXRActive.HasValue)
                {
                    isLegacyXRActive = true;

                    List<ISubsystemDescriptor> descriptors = new List<ISubsystemDescriptor>();
                    SubsystemManager.GetAllSubsystemDescriptors(descriptors);

                    foreach (ISubsystemDescriptor descriptor in descriptors)
                    {
                        if (descriptor is XRDisplaySubsystemDescriptor displayDescriptor)
                        {
                            if (displayDescriptor.disablesLegacyVr)
                            {
                                isLegacyXRActive = false;
                                break;
                            }
                        }
                    }
                }

                return isLegacyXRActive.HasValue && isLegacyXRActive.Value;
#else
                return true;
#endif // UNITY_2019_3_OR_NEWER
            }
        }

#if UNITY_2019_3_OR_NEWER
        /// <summary>
        /// Called when packages are installed or uninstalled, to toggle a new check on XR SDK package installation status.
        /// </summary>
        private static void EditorApplication_projectChanged() => isLegacyXRActive = null;
#endif // UNITY_2019_3_OR_NEWER
    }
}
