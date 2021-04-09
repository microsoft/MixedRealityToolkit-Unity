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
#if UNITY_2019_3_OR_NEWER && XR_MANAGEMENT_ENABLED
                if (MicrosoftOpenXREnabled)
                {
                    return true;
                }
                else if (XRSDKLoadersOfCurrentBuildTarget.Count == 1 && XRSDKLoadersOfCurrentBuildTarget[0].name.Contains("Open XR"))
                {
                    return false;
                }
                else
                {
                    return XRSDKLoadersOfCurrentBuildTarget.Count > 0;
                }
#else
                return false;
#endif // UNITY_2019_3_OR_NEWER && XR_MANAGEMENT_ENABLED
            }
        }


        /// <summary>
        /// Gets or sets the legacy virtual reality supported property in the player settings.
        /// </summary>
        /// <remarks>Returns true if legacy XR is disabled due to XR SDK in Unity 2019.3+.</remarks>
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
        /// Gets or sets the legacy virtual reality supported property in the player settings.
        /// </summary>
        /// <remarks>Returns true if legacy XR is disabled due to XR SDK in Unity 2019.3+.</remarks>
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

        /// <summary>
        /// Gets or sets the legacy virtual reality supported property in the player settings.
        /// </summary>
        /// <remarks>Returns true if legacy XR is disabled due to XR SDK in Unity 2019.3+.</remarks>
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
        /// Is either LegacyXR pipeline or XRSDK pipeline enabled?
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

        [System.Obsolete("Call IsLegacyXRAvailable instead.")]
        public static bool IsLegacyXRActive => IsLegacyXRAvailable;
    }
}
