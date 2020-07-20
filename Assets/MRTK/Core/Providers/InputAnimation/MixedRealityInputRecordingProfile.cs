// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Settings for recording input animation assets.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Input Recording Profile", fileName = "MixedRealityInputRecordingProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    [MixedRealityServiceProfile(typeof(IMixedRealityInputRecordingService))]
    public class MixedRealityInputRecordingProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("Minimum movement of hand joints to record a keyframe")]
        private float jointPositionThreshold = 0.001f;
        public float JointPositionThreshold => jointPositionThreshold;

        [SerializeField]
        [Tooltip("Minimum movement of hand joints to record a keyframe")]
        private float jointRotationThreshold = 0.02f;
        public float JointRotationThreshold => jointRotationThreshold;

        [SerializeField]
        [Tooltip("Minimum movement of the camera to record a keyframe")]
        private float cameraPositionThreshold = 0.002f;
        public float CameraPositionThreshold => cameraPositionThreshold;

        [SerializeField]
        [Tooltip("Minimum rotation angle of the camera to record a keyframe")]
        private float cameraRotationThreshold = 0.02f;
        public float CameraRotationThreshold => cameraRotationThreshold;
    }
}