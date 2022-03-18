// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

#if UNITY_2019_3_OR_NEWER
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
#if XR_MANAGEMENT_ENABLED
using UnityEngine.XR.Management;
using UnityEditor.XR.Management;
#endif // XR_MANAGEMENT_ENABLED
#endif // UNITY_2019_3_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Utilities that abstract XR settings functionality
    /// </summary>
    public static class XRSettingsUtilities
    {
        /// <summary>
        /// Is either LegacyXR pipeline or XRSDK pipeline enabled?
        /// </summary>
        public static bool XREnabled => XRSDKEnabled || LegacyXREnabled;

        /// <summary>
        /// Checks whether the XRSDK pipeline is properly set up to enable XR.
        /// </summary>
        /// <remarks>Returns true if one or more XR SDK plugins are enabled.</remarks>
        public static bool XRSDKEnabled
        {
            get
            {
#if XR_MANAGEMENT_ENABLED
                return XRSDKLoadersOfCurrentBuildTarget.Count > 0;
#else
                return false;
#endif // XR_MANAGEMENT_ENABLED
            }
        }

        /// <summary>
        /// Checks whether the Microsoft OpenXR plugin is present in the project.
        /// </summary>
        public static bool MicrosoftOpenXRPresent
        {
            get
            {
#if MSFT_OPENXR
                return true;
#else
                return false;
#endif // MSFT_OPENXR
            }
        }

        /// <summary>
        /// Checks whether the Microsoft OpenXR plugin is enabled.
        /// </summary>
        /// <remarks>Returns true if the Microsoft OpenXR plugin is present and the OpenXR plugin is enabled.</remarks>
        public static bool MicrosoftOpenXREnabled
        {
            get
            {
#if MSFT_OPENXR
                return XRSDKLoadersOfCurrentBuildTarget.Any(p => p.name.Contains("Open XR"));
#else
                return false;
#endif // UNITY_2019_3_OR_NEWER
            }
        }

        /// <summary>
        /// Checks whether the Unity OpenXR plugin is enabled.
        /// </summary>
        public static bool OpenXREnabled
        {
            get
            {
#if XR_MANAGEMENT_ENABLED
                return XRSDKLoadersOfCurrentBuildTarget.Any(p => p.name.Contains("Open XR"));
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

            set
            {
#if UNITY_2020_2_OR_NEWER
                Debug.LogWarning("This set operation doesn't have any effect as Legacy XR has been removed in Unity 2020!");
#else
#pragma warning disable 0618
                PlayerSettings.virtualRealitySupported = value;
#pragma warning restore 0618
#endif // !UNITY_2020_2_OR_NEWER
            }
        }

        /// <summary>
        /// Checks if an XR SDK plugin is installed that disables legacy XR. Returns false if so.
        /// Also returns false in Unity 2020 and above where Legacy XR has been removed.
        /// </summary>
        public static bool LegacyXRAvailable
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

        /// <summary>
        /// Checks whether the Unity XR Management plugin is present in the project.
        /// </summary>
        public static bool XRManagementPresent
        {
            get
            {
#if XR_MANAGEMENT_ENABLED
                return true;
#else
                return false;
#endif // XR_MANAGEMENT_ENABLED
            }
        }

#if UNITY_2019_3_OR_NEWER && !UNITY_2020_2_OR_NEWER
        private static bool? isXRSDKSuppressingLegacyXR = null;
        
        static XRSettingsUtilities()
        {
            // Called when packages are installed or uninstalled
            EditorApplication.projectChanged += EditorApplication_projectChanged;
        }

        /// <summary>
        /// Checks whether any imported XRSDK plugin is incompatible with Legacy XR so that Legacy XR must remain disabled.
        /// </summary>
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
#endif // UNITY_2019_3_OR_NEWER && !UNITY_2020_2_OR_NEWER

#if XR_MANAGEMENT_ENABLED
        /// <summary>
        /// Retrieves the enabled XRSDK XR loaders (plugins) for the current build target
        /// </summary>
        private static IReadOnlyList<XRLoader> XRSDKLoadersOfCurrentBuildTarget
        {
            get
            {
                BuildTargetGroup currentBuildTarget = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                XRGeneralSettings settingsOfCurrentTarget = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(currentBuildTarget);
                if (settingsOfCurrentTarget != null && settingsOfCurrentTarget.AssignedSettings != null)
                {
#pragma warning disable CS0618 // Suppressing the warning to support xr management plugin 3.x and 4.x
                    return settingsOfCurrentTarget.AssignedSettings.loaders;
#pragma warning restore CS0618
                }
                else
                {
                    return System.Array.Empty<XRLoader>();
                }
            }
        }
#endif // XR_MANAGEMENT_ENABLED

        /// <summary>
        /// Checks if an XR SDK plugin is installed that disables legacy XR. Returns false if so.
        /// Also returns false in Unity 2020 and above where Legacy XR has been removed.
        /// </summary>
        [System.Obsolete("Call LegacyXRAvailable instead.")]
        public static bool IsLegacyXRActive => LegacyXRAvailable;
    }
}
