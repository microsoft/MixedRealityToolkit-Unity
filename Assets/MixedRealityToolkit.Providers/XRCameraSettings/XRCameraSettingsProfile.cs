// Copyright (c) Microsoft Corporation. All rights reserved.
// Copyright(c) 2019 Takahiro Miyaura
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEngine;
using UnityEngine.SpatialTracking;

namespace Microsoft.MixedReality.Toolkit.CameraSystem
{
    /// <summary>
    /// Configuration profile for the XR Camera setttings provider.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/XR Camera Settings Profile", fileName = "DefaultXRCameraSettingsProfile", order = 100)]
    public class XRCameraSettingsProfile : BaseCameraSettingsProfile
    {
#region Tracked Pose Driver settings

        [SerializeField]
        [Tooltip("The portion of the device from which to read the pose.")]
        private TrackedPoseDriver.TrackedPose poseSource = TrackedPoseDriver.TrackedPose.ColorCamera;

        public TrackedPoseDriver.TrackedPose PoseSource => poseSource;

        [SerializeField]
        [Tooltip("The type of tracking (position and/or rotation) to apply.")]
        private TrackedPoseDriver.TrackingType trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;

        public TrackedPoseDriver.TrackingType TrackingType => trackingType;

        [SerializeField]
        [Tooltip("Specfies when (during Update and/or just before rendering) to update the tracking of the pose.")]
        private TrackedPoseDriver.UpdateType updateType = TrackedPoseDriver.UpdateType.UpdateAndBeforeRender;

        public TrackedPoseDriver.UpdateType UpdateType => updateType;

#endregion Tracked Pose Driver settings
    }
}
