// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

#if ARFOUNDATION_PRESENT
using UnityEngine.SpatialTracking;
#endif // ARFOUNDATION_PRESENT

namespace Microsoft.MixedReality.Toolkit.Experimental.UnityAR
{
    /// <summary>
    /// Class that performs conversions between Unity's AR enum values and the provider's
    /// custom enums.
    /// </summary>
    public static class ArEnumConversion
    {
#if ARFOUNDATION_PRESENT
        /// <summary>
        /// Converts from an <see cref="ArTrackedPose"/> to a Unity tracked pose value.
        /// </summary>
        /// <param name="pose">Value to convert.</param>
        /// <returns>
        /// Unity tracked pose equivalent of the <see cref="ArTrackedPose"/>.
        /// </returns>
        public static TrackedPoseDriver.TrackedPose ToUnityTrackedPose(ArTrackedPose pose)
        {
            switch (pose)
            {
                case ArTrackedPose.Center:
                    return TrackedPoseDriver.TrackedPose.Center;
                case ArTrackedPose.ColorCamera:
                    return TrackedPoseDriver.TrackedPose.ColorCamera;
                case ArTrackedPose.Head:
                    return TrackedPoseDriver.TrackedPose.Head;
                case ArTrackedPose.LeftEye:
                    return TrackedPoseDriver.TrackedPose.LeftEye;
                case ArTrackedPose.LeftPose:
                    return TrackedPoseDriver.TrackedPose.LeftPose;
                case ArTrackedPose.RightEye:
                    return TrackedPoseDriver.TrackedPose.RightEye;
                case ArTrackedPose.RightPose:
                    return TrackedPoseDriver.TrackedPose.RightPose;
                default:
                    // Unknown pose, pass the value through.
                    return (TrackedPoseDriver.TrackedPose)((int)pose);
            }
        }

        /// <summary>
        /// Converts from an <see cref="ArTrackingType"/> to a Unity tracking type value.
        /// </summary>
        /// <param name="trackingType">Value to convert.</param>
        /// <returns>
        /// Unity tracking type equivalent of the <see cref="ArTrackingType"/>.
        /// </returns>
        public static TrackedPoseDriver.TrackingType ToUnityTrackingType(ArTrackingType trackingType)
        {
            switch (trackingType)
            {
                case ArTrackingType.Position:
                    return TrackedPoseDriver.TrackingType.PositionOnly;
                case ArTrackingType.Rotation:
                    return TrackedPoseDriver.TrackingType.RotationOnly;
                case ArTrackingType.RotationAndPosition:
                    return TrackedPoseDriver.TrackingType.RotationAndPosition;
                default:
                    // Unknown type, pass the value through.
                    return (TrackedPoseDriver.TrackingType)((int)trackingType);
            }
        }

        /// <summary>
        /// Converts from an <see cref="ArUpdateType"/> to a Unity update type value.
        /// </summary>
        /// <param name="updateType">Value to convert.</param>
        /// <returns>
        /// Unity update type equivalent of the <see cref="ArUpdateType"/>.
        /// </returns>
        public static TrackedPoseDriver.UpdateType ToUnityUpdateType(ArUpdateType updateType)
        {
            switch (updateType)
            {
                case ArUpdateType.BeforeRender:
                    return TrackedPoseDriver.UpdateType.BeforeRender;
                case ArUpdateType.Update:
                    return TrackedPoseDriver.UpdateType.Update;
                case ArUpdateType.UpdateAndBeforeRender:
                    return TrackedPoseDriver.UpdateType.UpdateAndBeforeRender;
                default:
                    // Unknown type, pass the value through.
                    return (TrackedPoseDriver.UpdateType)((int)updateType);
            }
        }
#endif // ARFOUNDATION_PRESENT
    }
}
