// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.CameraSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    [CreateAssetMenu(menuName = "Mixed Reality/Toolkit/Providers/OpenXR/OpenXR Camera Settings Profile", fileName = "OpenXRCameraSettingsProfile", order = 100)]
    [MixedRealityServiceProfile(typeof(OpenXRCameraSettings))]
    public class OpenXRCameraSettingsProfile : BaseCameraSettingsProfile
    {
        [SerializeField]
        [Tooltip("Specifies the default reprojection method for HoloLens 2.")]
        private HolographicReprojectionMethod reprojectionMethod = HolographicReprojectionMethod.Depth;

        /// <summary>
        /// Specifies the default reprojection method for HoloLens 2.
        /// </summary>
        public HolographicReprojectionMethod ReprojectionMethod => reprojectionMethod;
    }
}
