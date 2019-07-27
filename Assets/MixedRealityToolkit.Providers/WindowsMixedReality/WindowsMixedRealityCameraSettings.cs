// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;

#if UNITY_WSA
using UnityEngine.XR.WSA;
#endif // UNITY_WSA

#if WINDOWS_UWP
using Windows.Graphics.Holographic;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Camera settings provider for use with Windows Mixed Reality.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityCameraSystem),
        SupportedPlatforms.WindowsUniversal,
        "Windows Mixed Reality Camera Settings")]
    public class WindowsMixedRealityCameraSettings : BaseCameraSettingsProvider
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

        private WindowsMixedRealityCameraSettingsProfile Profile => ConfigurationProfile as WindowsMixedRealityCameraSettingsProfile;

        /// <inheritdoc/>
        public override bool IsOpaque =>
#if UNITY_WSA
            HolographicSettings.IsDisplayOpaque;
#else
            false;
#endif

#if WINDOWS_UWP
        public override void ApplyConfiguration()
        {
            base.ApplyConfiguration();

            if (Profile != null &&
                Profile.RenderFromPVCameraForMixedRealityCapture &&
                global::Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.Graphics.Holographic.HolographicDisplay", "TryGetViewConfiguration"))
            {
                // If the default display has configuration for a PhotoVideoCamera, we want to enable it
                HolographicViewConfiguration viewConfiguration = HolographicDisplay.GetDefault()?.TryGetViewConfiguration(HolographicViewConfigurationKind.PhotoVideoCamera);
                if (viewConfiguration != null)
                {
                    viewConfiguration.IsEnabled = true;
                }
            }
        }
#endif // WINDOWS_UWP

        #endregion IMixedRealityCameraSettings
    }
}
