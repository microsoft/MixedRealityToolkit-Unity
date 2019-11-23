// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;

#if UNITY_WSA
using UnityEngine.XR.WSA;
#endif // UNITY_WSA

#if WINDOWS_UWP
using Windows.Foundation.Metadata;
#elif UNITY_WSA && DOTNETWINRT_PRESENT
using Microsoft.Windows.Foundation.Metadata;
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

        #region IMixedRealityCameraSettings

        private WindowsMixedRealityCameraSettingsProfile Profile => ConfigurationProfile as WindowsMixedRealityCameraSettingsProfile;

#if UNITY_WSA && DOTNETWINRT_PRESENT
        private readonly Dictionary<uint, bool> cameraIdToSupportsAutoPlanar = new Dictionary<uint, bool>();

        private static readonly bool isDepthReprojectionModeSupported = ApiInformation.IsPropertyPresent("Windows.Graphics.Holographic.HolographicCameraRenderingParameters", "DepthReprojectionMethod");
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

#if WINDOWS_UWP
        private static readonly bool isTryGetViewConfigurationSupported = ApiInformation.IsMethodPresent("Windows.Graphics.Holographic.HolographicDisplay", "TryGetViewConfiguration");
#endif // WINDOWS_UWP

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

#if UNITY_WSA && DOTNETWINRT_PRESENT
            if (Profile != null
                && Profile.ReprojectionMethod == HolographicDepthReprojectionMethod.AutoPlanar
                && isDepthReprojectionModeSupported)
            {
                Microsoft.Windows.Graphics.Holographic.HolographicFrame frame = Input.WindowsMixedRealityUtilities.CurrentHolographicFrame;
                foreach (var cameraPose in frame?.CurrentPrediction.CameraPoses)
                {
                    if (CameraSupportsAutoPlanar(cameraPose.HolographicCamera))
                    {
                        Microsoft.Windows.Graphics.Holographic.HolographicCameraRenderingParameters renderingParams = frame.GetRenderingParameters(cameraPose);
                        renderingParams.DepthReprojectionMethod = Microsoft.Windows.Graphics.Holographic.HolographicDepthReprojectionMethod.AutoPlanar;
                    }
                }
            }
#endif // UNITY_WSA && DOTNETWINRT_PRESENT
        }

#if UNITY_WSA && DOTNETWINRT_PRESENT
        private bool CameraSupportsAutoPlanar(Microsoft.Windows.Graphics.Holographic.HolographicCamera camera)
        {
            bool supportsAutoPlanar;
            if (!cameraIdToSupportsAutoPlanar.TryGetValue(camera.Id, out supportsAutoPlanar))
            {
                foreach (var method in camera.ViewConfiguration.SupportedDepthReprojectionMethods)
                {
                    if (method == Microsoft.Windows.Graphics.Holographic.HolographicDepthReprojectionMethod.AutoPlanar)
                    {
                        supportsAutoPlanar = true;
                        break;
                    }
                }
                cameraIdToSupportsAutoPlanar.Add(camera.Id, supportsAutoPlanar);
            }
            return supportsAutoPlanar;
        }
#endif // UNITY_WSA && DOTNETWINRT_PRESENT

        #endregion IMixedRealityCameraSettings
    }
}
