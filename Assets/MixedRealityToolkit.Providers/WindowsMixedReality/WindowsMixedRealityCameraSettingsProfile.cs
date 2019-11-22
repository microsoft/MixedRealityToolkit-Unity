// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
    /// <summary>
    /// Configuration profile for the Windows Mixed Reality Camera settings provider.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Windows Mixed Reality Camera Settings Profile", fileName = "DefaultWindowsMixedRealityCameraSettingsProfile", order = 100)]
    [MixedRealityServiceProfile(typeof(WindowsMixedRealityCameraSettings))]
    public class WindowsMixedRealityCameraSettingsProfile : BaseCameraSettingsProfile
    {
        [SerializeField]
        [Tooltip("If enabled, will render scene from PV camera projection matrix while MRC is active. This will ensure that holograms, such as hand meshes, remain visibly aligned in the video output.")]
        private bool renderFromPVCameraForMixedRealityCapture = true;

        /// <summary>
        /// Whether to use photo/video camera rendering for Mixed Reality Capture on Windows.
        /// </summary>
        /// <remarks>
        /// If true, the platform will provide an additional HolographicCamera to the app when a mixed reality capture photo or video is taken.
        /// This HolographicCamera provides view matrices corresponding to the photo/video camera location, and it provides projection matrices using the photo/video camera field of view.
        /// </remarks>
        public bool RenderFromPVCameraForMixedRealityCapture => renderFromPVCameraForMixedRealityCapture;
    }
}
