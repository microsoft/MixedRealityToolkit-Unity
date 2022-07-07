// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

#if UNITY_OPENXR
using UnityEngine.XR.OpenXR;
#endif // UNTIY_OPENXR

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    /// <summary>
    /// Camera settings provider for use with OpenXR and XR SDK.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityCameraSystem),
        (SupportedPlatforms)(-1),
        "OpenXR Camera Settings",
        "OpenXR/Profiles/DefaultOpenXRCameraSettingsProfile.asset",
        "MixedRealityToolkit.Providers",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.XRSDK)]
    public class OpenXRCameraSettings : BaseCameraSettingsProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cameraSystem">The instance of the camera system which is managing this provider.</param>
        /// <param name="name">Friendly name of the provider.</param>
        /// <param name="priority">Provider priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The provider's configuration profile.</param>
        public OpenXRCameraSettings(
            IMixedRealityCameraSystem cameraSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseCameraSettingsProfile profile = null) : base(cameraSystem, name, priority, profile)
        { }

        private bool? IsActiveLoader =>
#if UNITY_OPENXR
            LoaderHelpers.IsLoaderActive<OpenXRLoaderBase>();
#else
            false;
#endif // UNITY_OPENXR

        private OpenXRCameraSettingsProfile Profile => ConfigurationProfile as OpenXRCameraSettingsProfile;

        private OpenXRReprojectionUpdater reprojectionUpdater = null;

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

            InitializeReprojectionUpdater();

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

        /// <inheritdoc/>
        public override void Disable()
        {
            UninitializeReprojectionUpdater();
            base.Disable();
        }

        #region IMixedRealityCameraSettings

        /// <inheritdoc/>
        public override bool IsOpaque =>
            XRSubsystemHelpers.DisplaySubsystem == null
            || !XRSubsystemHelpers.DisplaySubsystem.running
            || XRSubsystemHelpers.DisplaySubsystem.displayOpaque;

        #endregion IMixedRealityCameraSettings

        /// <summary>
        /// Adds and initializes the reprojection updater component.
        /// </summary>
        private void InitializeReprojectionUpdater()
        {
            if (reprojectionUpdater == null && Profile != null)
            {
                reprojectionUpdater = CameraCache.Main.EnsureComponent<OpenXRReprojectionUpdater>();
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
    }
}
