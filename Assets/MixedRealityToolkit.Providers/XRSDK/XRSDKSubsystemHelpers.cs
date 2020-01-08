// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.XR;

#if XR_MANAGEMENT_ENABLED
using UnityEngine.XR.Management;
#endif //XR_MANAGEMENT_ENABLED

namespace Microsoft.MixedReality.Toolkit.XRSDK
{
    public static class XRSDKSubsystemHelpers
    {
        private static XRInputSubsystem inputSubsystem;
        public static XRInputSubsystem InputSubsystem
        {
            get
            {
#if XR_MANAGEMENT_ENABLED
                if (inputSubsystem == null &&
                    XRGeneralSettings.Instance != null &&
                    XRGeneralSettings.Instance.Manager != null &&
                    XRGeneralSettings.Instance.Manager.activeLoader != null)
                {
                    inputSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>();
                }
#endif //XR_MANAGEMENT_ENABLED

                return inputSubsystem;
            }
        }
    }
}
