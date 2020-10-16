// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;

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
        "MixedRealityToolkit.Providers")]
    public abstract class BaseWindowsMixedRealityCameraSettings : BaseCameraSettingsProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cameraSystem">The instance of the camera system which is managing this provider.</param>
        /// <param name="name">Friendly name of the provider.</param>
        /// <param name="priority">Provider priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The provider's configuration profile.</param>
        protected BaseWindowsMixedRealityCameraSettings(
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
        private static readonly bool isTryGetViewConfigurationSupported = Windows.Utilities.WindowsApiChecker.IsMethodAvailable(
            "Windows.Graphics.Holographic",
            "HolographicDisplay",
            "TryGetViewConfiguration");
#endif // WINDOWS_UWP

        private WindowsMixedRealityReprojectionUpdater reprojectionUpdater = null;

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
            if (reprojectionUpdater == null && Profile != null)
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


        /// <inheritdoc />
        public bool ReadingModeEnabled
        {
            get
            {
                ProjectionOverride projectionOverride;
                if (TryGetProjectionOverrideComponent(out projectionOverride, false))
                {
                    return projectionOverride.ReadingModeEnabled;
                }
                return false;
            }
            set
            {
                ProjectionOverride projectionOverride;
                if (TryGetProjectionOverrideComponent(out projectionOverride, true))
                {
                    projectionOverride.ReadingModeEnabled = value;
                }
            }
        }

        /// <summary>
        /// Helper to get the <see cref="ProjectionOverride"> component that's attached to the main camera
        /// </summary>
        /// <param name="projectionOverride">The <see cref="ProjectionOverride"> component if there is one</param>
        /// <param name="createIfAbsent">Create the <see cref="ProjectionOverride"> if it's not there</param>
        /// <returns>
        /// false if there was no ProjectionOverride component and we didn't create one
        /// true if the out param projectionOverride is set to the ProjectionOverride attached to the main camera
        /// </returns>
        private bool TryGetProjectionOverrideComponent(out ProjectionOverride projectionOverride, bool createIfAbsent)
        {
            projectionOverride = CameraCache.Main.GetComponent<ProjectionOverride>();
            if (projectionOverride != null)
            {
                return true;
            }

            if (!createIfAbsent)
            {
                return false;
            }

            projectionOverride = CameraCache.Main.EnsureComponent<ProjectionOverride>();
            return true;
        }

        #endregion IMixedRealityCameraSettings
    }
}
