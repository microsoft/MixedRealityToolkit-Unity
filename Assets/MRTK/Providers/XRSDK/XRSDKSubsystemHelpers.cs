// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.XR;

#if XR_MANAGEMENT_ENABLED
using UnityEngine.XR.Management;
#endif // XR_MANAGEMENT_ENABLED

namespace Microsoft.MixedReality.Toolkit.XRSDK
{
    /// <summary>
    /// A helper class to provide easier access to active Unity XR SDK subsystems.
    /// </summary>
    public static class XRSDKSubsystemHelpers
    {
        private static XRInputSubsystem inputSubsystem = null;

        /// <summary>
        /// The XR SDK input subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRInputSubsystem InputSubsystem
        {
            get
            {
#if XR_MANAGEMENT_ENABLED
                if (inputSubsystem == null &&
                    ActiveLoader != null)
                {
                    inputSubsystem = ActiveLoader.GetLoadedSubsystem<XRInputSubsystem>();
                }
#endif // XR_MANAGEMENT_ENABLED

                return inputSubsystem;
            }
        }

        private static XRMeshSubsystem meshSubsystem = null;

        /// <summary>
        /// The XR SDK mesh subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRMeshSubsystem MeshSubsystem
        {
            get
            {
#if XR_MANAGEMENT_ENABLED
                if (meshSubsystem == null &&
                    ActiveLoader != null)
                {
                    meshSubsystem = ActiveLoader.GetLoadedSubsystem<XRMeshSubsystem>();
                }
#endif // XR_MANAGEMENT_ENABLED

                return meshSubsystem;
            }
        }

        private static XRDisplaySubsystem displaySubsystem = null;

        /// <summary>
        /// The XR SDK display subsystem for the currently loaded XR plug-in.
        /// </summary>
        public static XRDisplaySubsystem DisplaySubsystem
        {
            get
            {
#if XR_MANAGEMENT_ENABLED
                if (displaySubsystem == null &&
                    ActiveLoader != null)
                {
                    displaySubsystem = ActiveLoader.GetLoadedSubsystem<XRDisplaySubsystem>();
                }
#endif // XR_MANAGEMENT_ENABLED

                return displaySubsystem;
            }
        }

#if XR_MANAGEMENT_ENABLED
        private static XRLoader ActiveLoader
        {
            get
            {
                if (XRGeneralSettings.Instance != null &&
                    XRGeneralSettings.Instance.Manager != null &&
                    XRGeneralSettings.Instance.Manager.activeLoader != null)
                {
                    return XRGeneralSettings.Instance.Manager.activeLoader;
                }

                return null;
            }
        }
#endif // XR_MANAGEMENT_ENABLED
    }
}
