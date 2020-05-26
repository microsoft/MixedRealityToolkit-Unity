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
        /// If true, the platform will provide an additional HolographicCamera to the app when a mixed reality capture photo or video is taken.
        /// This HolographicCamera provides view matrices corresponding to the photo/video camera location, and it provides projection matrices using the photo/video camera field of view.
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
    }
}
