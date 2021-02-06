// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Configuration profile for the Windows Mixed Reality camera settings provider.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Providers/Windows Mixed Reality/Windows Mixed Reality Camera Settings Profile", fileName = "WindowsMixedRealityCameraSettingsProfile", order = 100)]
    [MixedRealityServiceProfile(typeof(BaseWindowsMixedRealityCameraSettings))]
    public class WindowsMixedRealityCameraSettingsProfile : BaseCameraSettingsProfile
    {
        [SerializeField]
        [Tooltip("If enabled, will render scene from PV camera projection matrix while MRC is active. This will ensure that holograms, such as hand meshes, remain visibly aligned in the video output.")]
        private bool renderFromPVCameraForMixedRealityCapture = false;

        /// <summary>
        /// Whether to use photo/video camera rendering for Mixed Reality Capture on Windows.
        /// </summary>
        /// <remarks>
        /// <para>If true, the platform will provide an additional HolographicCamera to the app when a mixed reality capture photo or video is taken.</para>
        /// <para>This HolographicCamera provides view matrices corresponding to the photo/video camera location, and it provides projection matrices using the photo/video camera field of view.</para>
        /// </remarks>
        public bool RenderFromPVCameraForMixedRealityCapture => renderFromPVCameraForMixedRealityCapture;

        [SerializeField]
        [Tooltip("Specifies the default reprojection method for HoloLens 2. Note: AutoPlanar requires the DotNetWinRT adapter. DepthReprojection is the default if the adapter isn't present.")]
        private HolographicDepthReprojectionMethod reprojectionMethod = HolographicDepthReprojectionMethod.DepthReprojection;

        /// <summary>
        /// Specifies the default reprojection method for HoloLens 2.
        /// </summary>
        /// <remarks>AutoPlanar requires the DotNetWinRT adapter. DepthReprojection is the default if the adapter isn't present.</remarks>
        public HolographicDepthReprojectionMethod ReprojectionMethod => reprojectionMethod;

        [SerializeField]
        [Tooltip("Override the camera's projection matrices for a smaller field of view, but rendered content will have more detail.")]
        private bool readingModeEnabled = false;

        /// <summary>
        /// Override the camera's projection matrices for a smaller field of view,
        /// but rendered content will have more detail. See <see href="https://docs.microsoft.com/en-us/hololens/hololens2-display">Reading Mode</see> documentation.
        /// While this will work on all Windows Mixed Reality platforms, this
        /// is primarily useful on HoloLens 2 hardware.
        /// If holograms are not stable, change the Stereo Rendering Mode from
        /// "Single Pass Instanced" to "Multi Pass" to work around a bug in Unity.
        /// </summary>
        public bool ReadingModeEnabled => readingModeEnabled;
    }
}
