// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;

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
        "MixedRealityToolkit.Providers")]
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

        #region IMixedRealityCameraSettings

        /// <inheritdoc/>
        public override bool IsOpaque => XRSDKSubsystemHelpers.DisplaySubsystem?.displayOpaque ?? true;

        #endregion IMixedRealityCameraSettings
    }
}
