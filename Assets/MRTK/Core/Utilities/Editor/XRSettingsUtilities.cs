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
        public static bool LegacyXREnabled
        {
            get
            {
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                return ShouldLegacyVRBeDisabled || PlayerSettings.virtualRealitySupported;
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
        private static bool? shouldLegacyVRBeDisabled = null;
#endif // UNITY_2019_3_OR_NEWER

        /// <summary>
        /// Checks if an XR SDK plug-in is installed that disables legacy VR.
        /// </summary>
        public static bool ShouldLegacyVRBeDisabled
        {
            get
            {
#if UNITY_2019_3_OR_NEWER
                if (!shouldLegacyVRBeDisabled.HasValue)
                {
                    shouldLegacyVRBeDisabled = false;

                    List<ISubsystemDescriptor> descriptors = new List<ISubsystemDescriptor>();
                    SubsystemManager.GetAllSubsystemDescriptors(descriptors);

                    foreach (ISubsystemDescriptor descriptor in descriptors)
                    {
                        if (descriptor is XRDisplaySubsystemDescriptor displayDescriptor)
                        {
                            if (displayDescriptor.disablesLegacyVr)
                            {
                                shouldLegacyVRBeDisabled = true;
                            }
                        }
                    }
                }

                return shouldLegacyVRBeDisabled.HasValue && shouldLegacyVRBeDisabled.Value;
#else
                return false;
#endif // UNITY_2019_3_OR_NEWER
            }
        }

#if UNITY_2019_3_OR_NEWER
        /// <summary>
        /// Called when packages are installed or uninstalled, to toggle a new check on XR SDK package installation status.
        /// </summary>
        private static void EditorApplication_projectChanged() => shouldLegacyVRBeDisabled = null;
#endif // UNITY_2019_3_OR_NEWER
    }
}
