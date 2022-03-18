// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Camera settings provider for use with Windows Mixed Reality.
    /// </summary>
    public abstract class BaseWindowsMixedRealityCameraSettings : BaseCameraSettingsProvider, IMixedRealityCameraProjectionOverrideProvider
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
            InitializeProjectionOverride();
        }

        /// <inheritdoc/>
        public override void Disable()
        {
            UninitializeReprojectionUpdater();
            UninitializeProjectionOverride();
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
        private ProjectionOverride projectionOverride = null;

        /// <inheritdoc/>
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

        #endregion IMixedRealityCameraSettings

        #region IMixedRealityCameraProjectionOverrideProvider

        /// <summary>
        /// Override the camera's projection matrices for a smaller field of view,
        /// but rendered content will have more detail. See <see href="https://docs.microsoft.com/en-us/hololens/hololens2-display">Reading Mode</see> documentation.
        /// While this will work on all Windows Mixed Reality platforms, this
        /// is primarily useful on HoloLens 2 hardware.
        /// If holograms are not stable, change the Stereo Rendering Mode from
        /// "Single Pass Instanced" to "Multi Pass" to work around a bug in Unity.
        /// </summary>
        public bool IsProjectionOverrideEnabled
        {
            get { return projectionOverride != null && projectionOverride.ReadingModeEnabled; }
            set
            {
                if (value && projectionOverride == null)
                {
                    projectionOverride = CameraCache.Main.EnsureComponent<ProjectionOverride>();
                }

                if (projectionOverride != null)
                {
                    projectionOverride.ReadingModeEnabled = value;
                }
            }
        }

        #endregion IMixedRealityCameraProjectionOverrideProvider

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

        /// <summary>
        /// Adds and initializes the ProjectionOverride component.
        /// </summary>
        private void InitializeProjectionOverride()
        {
            if (projectionOverride == null && Profile != null && Profile.ReadingModeEnabled)
            {
                projectionOverride = CameraCache.Main.EnsureComponent<ProjectionOverride>();
                projectionOverride.ReadingModeEnabled = Profile.ReadingModeEnabled;
            }
        }

        /// <summary>
        /// Uninitializes and removes the ProjectionOverride component.
        /// </summary>
        private void UninitializeProjectionOverride()
        {
            if (projectionOverride != null)
            {
                UnityObjectExtensions.DestroyObject(projectionOverride);
                projectionOverride = null;
            }
        }
    }
}
