// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;

#if UNITY_WSA
using UnityEngine.XR.WSA;
#endif // UNITY_WSA

#if WINDOWS_UWP
using Windows.Foundation.Metadata;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Camera settings provider for use with Windows Mixed Reality.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityCameraSystem),
        SupportedPlatforms.WindowsUniversal,
        "Windows Mixed Reality Camera Settings",
        "WindowsMixedReality/Profiles/DefaultWindowsMixedRealityCameraSettingsProfile.asset",
        "MixedRealityToolkit.Providers")]
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

        /// <inheritdoc/>
        public override void Enable()
        {
            base.Enable();
            InitializeReprojectionUpdater();
        }

        /// <inheritdoc/>
        public override void Disable()
        {
            UninitializeReprojectionUpdater();
            base.Disable();
        }

        #region IMixedRealityCameraSettings

        private WindowsMixedRealityCameraSettingsProfile Profile => ConfigurationProfile as WindowsMixedRealityCameraSettingsProfile;

#if WINDOWS_UWP
        private static readonly bool isTryGetViewConfigurationSupported = ApiInformation.IsMethodPresent("Windows.Graphics.Holographic.HolographicDisplay", "TryGetViewConfiguration");
#endif // WINDOWS_UWP

        private WindowsMixedRealityReprojectionUpdater reprojectionUpdater = null;

        /// <inheritdoc/>
        public override bool IsOpaque =>
#if UNITY_WSA
            HolographicSettings.IsDisplayOpaque;
#else
            false;
#endif

        public override void ApplyConfiguration()
        {
            base.ApplyConfiguration();

#if WINDOWS_UWP
            if (Profile != null
                && Profile.RenderFromPVCameraForMixedRealityCapture
                && isTryGetViewConfigurationSupported)
            {
                // If the default display has configuration for a PhotoVideoCamera, we want to enable it
                global::Windows.Graphics.Holographic.HolographicViewConfiguration viewConfiguration = global::Windows.Graphics.Holographic.HolographicDisplay.GetDefault()?.TryGetViewConfiguration(global::Windows.Graphics.Holographic.HolographicViewConfigurationKind.PhotoVideoCamera);
                if (viewConfiguration != null)
                {
                    viewConfiguration.IsEnabled = true;
                }
            }
#endif // WINDOWS_UWP
        }

        /// <summary>
        /// Adds and initializes the reprojection updater component.
        /// </summary>
        private void InitializeReprojectionUpdater()
        {
            if (reprojectionUpdater == null)
            {
                reprojectionUpdater = CameraCache.Main.EnsureComponent<WindowsMixedRealityReprojectionUpdater>();
                reprojectionUpdater.ReprojectionMethod = Profile.ReprojectionMethod;
            }
        }

        /// <summary>
        /// Uninitializes and removes the reprojection updater component.
        /// </summary>
        private void UninitializeReprojectionUpdater()
        {
            if (reprojectionUpdater != null)
            {
                UnityObjectExtensions.DestroyObject(reprojectionUpdater);
                reprojectionUpdater = null;
            }
        }

        #endregion IMixedRealityCameraSettings
    }
}
