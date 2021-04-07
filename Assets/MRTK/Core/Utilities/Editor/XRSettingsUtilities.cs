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
    public enum ConfigurationStage
    {
        Init = 0,
        SelectXRSDKPlugin = 100,
        InstallMSOpenXR = 101,
        InstallBuiltinPlugin = 102,
        ProjectConfiguration = 200,
        ImportTMP = 300,
        ShowExamples = 400,
        Done = 500
    };
    /// <summary>
    /// Utilities that abstract XR settings functionality
    /// </summary>
    public static class XRSettingsUtilities
    {
        /// <summary>
        /// Is either LegacyXR pipeline or XRSDK pipeline enabled?
        /// </summary>
        public static bool XREnabled
        {
            get
            {
                return XRSDKEnabled || LegacyXREnabled;
            }
        }


        /// <summary>
        /// Gets or sets the legacy virtual reality supported property in the player settings.
        /// </summary>
        /// <remarks>Returns true if legacy XR is disabled due to XR SDK in Unity 2019.3+.</remarks>
        public static bool XRSDKEnabled
        {
            get
            {
#if UNITY_2019_3_OR_NEWER
                return IsXRSDKSuppressingLegacyXR;
#else
                return false;
#endif // UNITY_2019_3_OR_NEWER
            }
        }


        /// <summary>
        /// Gets or sets the legacy virtual reality supported property in the player settings.
        /// </summary>
        /// <remarks>Returns true if legacy XR is disabled due to XR SDK in Unity 2019.3+.</remarks>
        public static bool MicrosoftOpenXREnabled
        {
            get
            {
#if UNITY_2019_3_OR_NEWER
                return IsXRSDKSuppressingLegacyXR;
#else
                return false;
#endif // UNITY_2019_3_OR_NEWER
            }
        }

        /// <summary>
        /// Gets or sets (in Unity 2019.4 or earlier) the legacy virtual reality supported property in the player settings.
        /// </summary>
        public static bool LegacyXREnabled
        {
            get
            {
#if UNITY_2020_2_OR_NEWER
                return false;
#else
#pragma warning disable 0618
                return PlayerSettings.virtualRealitySupported;
#pragma warning restore 0618
#endif // UNITY_2020_2_OR_NEWER
            }

#if !UNITY_2020_2_OR_NEWER
            set
            {
                // Ensure compatibility with the pre-2019.3 XR architecture for customers / platforms
                // with legacy requirements.
#pragma warning disable 0618
                PlayerSettings.virtualRealitySupported = value;
#pragma warning restore 0618
            }
#endif // !UNITY_2020_2_OR_NEWER
        }
        
        /// <summary>
        /// Checks if an XR SDK plug-in is installed that disables legacy VR. Returns false if so.
        /// </summary>
        public static bool IsLegacyXRAvailable
        {
            get
            {
#if UNITY_2020_2_OR_NEWER
                return false;
#elif UNITY_2019_3_OR_NEWER
                return !IsXRSDKSuppressingLegacyXR;
#else
                return true;
#endif // UNITY_2019_3_OR_NEWER
            }
        }

#if UNITY_2019_3_OR_NEWER
        private static bool? isXRSDKSuppressingLegacyXR = null;
        
        static XRSettingsUtilities()
        {
            // Called when packages are installed or uninstalled
            EditorApplication.projectChanged += EditorApplication_projectChanged;
        }
        
        private static bool IsXRSDKSuppressingLegacyXR
        {
            get
            {
                if (!isXRSDKSuppressingLegacyXR.HasValue)
                {
                    isXRSDKSuppressingLegacyXR = false;

                    List<XRDisplaySubsystemDescriptor> descriptors = new List<XRDisplaySubsystemDescriptor>();
                    SubsystemManager.GetSubsystemDescriptors(descriptors);

                    foreach (XRDisplaySubsystemDescriptor displayDescriptor in descriptors)
                    {
                        if (displayDescriptor.disablesLegacyVr)
                        {
                            isXRSDKSuppressingLegacyXR = true;
                            break;
                        }
                    }
                }

                return isXRSDKSuppressingLegacyXR.HasValue && isXRSDKSuppressingLegacyXR.Value;
            }
        }

        /// <summary>
        /// Called when packages are installed or uninstalled, to toggle a new check on XR SDK package installation status.
        /// </summary>
        private static void EditorApplication_projectChanged() => isXRSDKSuppressingLegacyXR = null;
#endif // UNITY_2019_3_OR_NEWER
        
        [System.Obsolete("Call GetDefaultMappings(Handedness) instead.")]
        public static bool IsLegacyXRActive => IsLegacyXRAvailable;
    }
}
