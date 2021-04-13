// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

#if UNITY_2019_3_OR_NEWER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System.Linq;
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
        public static bool XREnabled
        {
            get
            {
                return XRSDKEnabled || LegacyXREnabled;
            }
        }


        /// <summary>
        /// Checks whether the XRSDK pipeline is properly set up to enable XR.
        /// </summary>
        /// <remarks>
        /// <para>Returns true if using the Unity OpenXR plugin along with the Microsoft plugin or using the built-in plugins.
        /// Returns false if the only enabled plugin is Unity OpenXR and the Microsoft one is not present, or there is no enabled plugin.</para>
        /// </remarks>
        public static bool XRSDKEnabled
        {
            get
            {
#if UNITY_2019_3_OR_NEWER && XR_MANAGEMENT_ENABLED
                return XRSDKLoadersOfCurrentBuildTarget.Count > 0;
#else
                return false;
#endif // UNITY_2019_3_OR_NEWER && XR_MANAGEMENT_ENABLED
            }
        }


        /// <summary>
        /// Checks whether the Microsoft OpenXR plugin is present in the project.
        /// </summary>
        public static bool MicrosoftOpenXRPresent
        {
            get
            {
#if UNITY_2020_2_OR_NEWER && MSFT_OPENXR
                return true;
#else
                return false;
#endif // UNITY_2019_3_OR_NEWER
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
#if UNITY_2020_2_OR_NEWER && MSFT_OPENXR
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
#if UNITY_2020_2_OR_NEWER && XR_MANAGEMENT_ENABLED
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
                Debug.LogWarning("This set operation has not effect as Legacy XR has been removed in Unity 2020!");
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

#if UNITY_2019_3_OR_NEWER
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
#pragma warning disable CS0618 // Suppressing the warning to support xr management plugin 3.x and 4.x
                return settingsOfCurrentTarget.AssignedSettings.loaders;
#pragma warning restore CS0618
            }
        }
#endif // XR_MANAGEMENT_ENABLED
#endif // UNITY_2019_3_OR_NEWER

        /// <summary>
        /// Checks if an XR SDK plugin is installed that disables legacy XR. Returns false if so.
        /// Also returns false in Unity 2020 and above where Legacy XR has been removed.
        /// </summary>
        [System.Obsolete("Call LegacyXRAvailable instead.")]
        public static bool IsLegacyXRActive => LegacyXRAvailable;
    }
}
