// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.XRSDK
{
    /// <summary>
    /// Camera settings provider for use with Windows Mixed Reality and XR SDK.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityCameraSystem),
        (SupportedPlatforms)(-1),
        "XR SDK Camera Settings")]
    public class GenericXRSDKCameraSettings : BaseCameraSettingsProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cameraSystem">The instance of the camera system which is managing this provider.</param>
        /// <param name="name">Friendly name of the provider.</param>
        /// <param name="priority">Provider priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The provider's configuration profile.</param>
        public GenericXRSDKCameraSettings(
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
