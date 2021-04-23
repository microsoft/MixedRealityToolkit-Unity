// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;

#if UNITY_WSA
using UnityEngine.XR.WSA;
#endif // UNITY_WSA

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Camera settings provider for use with Windows Mixed Reality.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityCameraSystem),
        SupportedPlatforms.WindowsUniversal,
        "Windows Mixed Reality Camera Settings",
        "WindowsMixedReality/Shared/Profiles/DefaultWindowsMixedRealityCameraSettingsProfile.asset",
        "MixedRealityToolkit.Providers",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.LegacyXR)]
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
        public override bool IsOpaque =>
#if UNITY_WSA
            HolographicSettings.IsDisplayOpaque;
#else
            false;
#endif

        #endregion IMixedRealityCameraSettings
    }
}
