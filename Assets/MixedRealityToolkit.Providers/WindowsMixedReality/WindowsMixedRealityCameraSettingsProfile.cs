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
        [Tooltip("Whether to use photo/video camera rendering for Mixed Reality Capture on Windows.")]
        private bool renderFromPVCameraForMixedRealityCapture = true;

        /// <summary>
        /// Whether to use photo/video camera rendering for Mixed Reality Capture on Windows.
        /// </summary>
        /// <remarks>
        /// If true, the platform will provide an additional HolographicCamera to the app when the user takes a mixed reality capture photo or video.
        /// This HolographicCamera provides view matrices corresponding to the photo/video camera location, and it provides projection matrices using the photo/video camera field of view.
        /// </remarks>
        public bool RenderFromPVCameraForMixedRealityCapture => renderFromPVCameraForMixedRealityCapture;
    }
}
