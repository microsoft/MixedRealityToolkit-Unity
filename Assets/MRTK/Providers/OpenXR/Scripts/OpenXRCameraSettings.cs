// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Linq;

#if MSFT_OPENXR
using Microsoft.MixedReality.OpenXR;
using UnityEngine.XR.OpenXR;
#endif // MSFT_OPENXR

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

        private bool IsActiveLoader =>
#if MSFT_OPENXR
            LoaderHelpers.IsLoaderActive<OpenXRLoaderBase>();
#else
            false;
#endif // MSFT_OPENXR

#if MSFT_OPENXR
        private static readonly bool IsReprojectionExtensionSupported = OpenXRRuntime.IsExtensionEnabled("XR_MSFT_composition_layer_reprojection_preview");
#endif // MSFT_OPENXR

        private OpenXRCameraSettingsProfile Profile => ConfigurationProfile as OpenXRCameraSettingsProfile;

        /// <inheritdoc />
        public override void Enable()
        {
            if (!IsActiveLoader)
            {
                IsEnabled = false;
                return;
            }

            base.Enable();
            InitializeReprojectionMode();
        }

        #region IMixedRealityCameraSettings

        /// <inheritdoc/>
        public override bool IsOpaque => XRSubsystemHelpers.DisplaySubsystem?.displayOpaque ?? true;

        #endregion IMixedRealityCameraSettings

        /// <summary>
        /// Adds and initializes the reprojection updater component.
        /// </summary>
        private void InitializeReprojectionMode()
        {
            if (IsReprojectionExtensionSupported && Profile != null)
            {
                ReprojectionMode reprojectionMode;

                switch (Profile.ReprojectionMethod)
                {
                    case HolographicReprojectionMethod.Depth:
                        reprojectionMode = ReprojectionMode.Depth;
                        break;
                    case HolographicReprojectionMethod.PlanarFromDepth:
                        reprojectionMode = ReprojectionMode.PlanarFromDepth;
                        break;
                    case HolographicReprojectionMethod.PlanarManual:
                        reprojectionMode = ReprojectionMode.PlanarManual;
                        break;
                    case HolographicReprojectionMethod.OrientationOnly:
                        reprojectionMode = ReprojectionMode.OrientationOnly;
                        break;
                    case HolographicReprojectionMethod.NoReprojection:
                    default:
                        reprojectionMode = ReprojectionMode.NoReprojection;
                        break;
                }

                if (ReprojectionSettings.SupportedReprojectionModes.Contains(reprojectionMode))
                {
                    ReprojectionSettings.ReprojectionMode = reprojectionMode;
                }
            }
        }
    }
}
