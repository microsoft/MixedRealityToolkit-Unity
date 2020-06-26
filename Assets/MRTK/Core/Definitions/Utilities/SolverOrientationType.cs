// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public enum SolverOrientationType
    {
        /// <summary>
        /// Use the tracked object's pitch, yaw, and roll
        /// </summary>
        FollowTrackedObject = 0,
        /// <summary>
        /// Face toward the tracked object
        /// </summary>
        FaceTrackedObject,
        /// <summary>
        /// Orient towards SolverHandler's tracked object or TargetTransform
        /// </summary>
        YawOnly,
        /// <summary>
        /// Leave the object's rotation alone
        /// </summary>
        Unmodified,
        /// <summary>
        /// Orient toward the main camera instead of SolverHandler's properties.
        /// </summary>
        CameraFacing,
        /// <summary>
        /// Align parallel to the direction the camera is facing 
        /// </summary>
        CameraAligned
    }
}