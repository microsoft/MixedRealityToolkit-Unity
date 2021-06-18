// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    /// <summary>
    /// Camera settings provider for use with Windows Mixed Reality and XR SDK.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityCameraSystem),
        SupportedPlatforms.WindowsUniversal | SupportedPlatforms.WindowsStandalone,
        "XR SDK Windows Mixed Reality Camera Settings",
        "WindowsMixedReality/Shared/Profiles/DefaultWindowsMixedRealityCameraSettingsProfile.asset",
        "MixedRealityToolkit.Providers",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.XRSDK)]
    public class WindowsMixedRealityCameraSettings : BaseWindowsMixedRealityCameraSettings
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cameraSystem">The instance of the camera system which is managing this provider.</param>
        /// <param name="name">Friendly name of the provider.</param>
        /// <param name="priority">Provider priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The provider's configuration profile.</param>
        public WindowsMixedRealityCameraSettings(
            IMixedRealityCameraSystem cameraSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseCameraSettingsProfile profile = null) : base(cameraSystem, name, priority, profile)
        { }

        private bool? IsActiveLoader =>
#if WMR_ENABLED
            LoaderHelpers.IsLoaderActive("Windows MR Loader");
#else
            false;
#endif // WMR_ENABLED

        /// <inheritdoc />
        public override void Enable()
        {
            if (!IsActiveLoader.HasValue)
            {
                IsEnabled = false;
                EnableIfLoaderBecomesActive();
                return;
            }
            else if (!IsActiveLoader.Value)
            {
                IsEnabled = false;
                return;
            }

            base.Enable();
        }

        private async void EnableIfLoaderBecomesActive()
        {
            await new WaitUntil(() => IsActiveLoader.HasValue);
            if (IsActiveLoader.Value)
            {
                Enable();
            }
        }

        #region IMixedRealityCameraSettings

        /// <inheritdoc/>
        public override bool IsOpaque => XRSubsystemHelpers.DisplaySubsystem?.displayOpaque ?? true;

        #endregion IMixedRealityCameraSettings
    }
}
