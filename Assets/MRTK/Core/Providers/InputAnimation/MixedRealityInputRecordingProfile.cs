// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Settings for recording input animation assets.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality/Toolkit/Profiles/Mixed Reality Input Recording Profile", fileName = "MixedRealityInputRecordingProfile", order = (int)CreateProfileMenuItemIndices.Input)]
    [MixedRealityServiceProfile(typeof(IMixedRealityInputRecordingService))]
    public class MixedRealityInputRecordingProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("The rate at which keyframes are recorded")]
        private float frameRate = 60f;
        public float FrameRate => frameRate;

        [SerializeField]
        [Tooltip("Whether or not to record hand data")]
        private bool recordHandData = true;
        public bool RecordHandData => recordHandData;

        [SerializeField]
        [Tooltip("Minimum movement of hand joints to record a keyframe")]
        private float jointPositionThreshold = 0.001f;
        public float JointPositionThreshold => jointPositionThreshold;

        [SerializeField]
        [Tooltip("Minimum rotation angle (in degrees) of hand joints to record a keyframe")]
        private float jointRotationThreshold = 0.2f;
        public float JointRotationThreshold => jointRotationThreshold;

        [SerializeField]
        [Tooltip("Whether or not to record camera movement")]
        private bool recordCameraPose = true;
        public bool RecordCameraPose => recordCameraPose;

        [SerializeField]
        [Tooltip("Minimum movement of the camera to record a keyframe")]
        private float cameraPositionThreshold = 0.002f;
        public float CameraPositionThreshold => cameraPositionThreshold;

        [SerializeField]
        [Tooltip("Minimum rotation angle (in degrees) of the camera to record a keyframe")]
        private float cameraRotationThreshold = 0.2f;
        public float CameraRotationThreshold => cameraRotationThreshold;

        [SerializeField]
        [Tooltip("Whether or not to record eye gaze")]
        private bool recordEyeGaze = true;
        public bool RecordEyeGaze => recordEyeGaze;

        [SerializeField]
        [Tooltip("Minimum movement of the eye gaze origin to record a keyframe")]
        private float eyeGazeOriginThreshold = 0.002f;
        public float EyeGazeOriginThreshold => eyeGazeOriginThreshold;

        [SerializeField]
        [Tooltip("Minimum rotation angle (in degrees) of the eye gaze to record a keyframe")]
        private float eyeGazeDirectionThreshold = 0.2f;
        public float EyeGazeDirectionThreshold => eyeGazeDirectionThreshold;

        [SerializeField]
        [Tooltip("The size of the partitions used to optimize the input animation after recording. Larger values will reduce animation size, but may increase save time. A value of 0 will disable partitioning entirely")]
        private int partitionSize = 32;
        public int PartitionSize => partitionSize;
    }
}