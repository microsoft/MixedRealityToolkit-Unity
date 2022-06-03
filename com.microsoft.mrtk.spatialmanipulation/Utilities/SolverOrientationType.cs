// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Orientation options utilized by solvers.
    /// </summary>
    public enum SolverOrientationType
    {
        /// <summary>
        /// Leave the object's rotation alone
        /// </summary>
        Unmodified = 0,

        /// <summary>
        /// Use the tracked object's pitch, yaw, and roll
        /// </summary>
        FollowTrackedObject = 1,

        /// <summary>
        /// Face toward the tracked object
        /// </summary>
        FaceTrackedObject = 2,

        /// <summary>
        /// Orient towards SolverHandler's tracked object or TargetTransform
        /// </summary>
        YawOnly = 3,

        /// <summary>
        /// Orient toward the main camera instead of SolverHandler's properties.
        /// </summary>
        CameraFacing = 4,

        /// <summary>
        /// Align parallel to the direction the camera is facing 
        /// </summary>
        CameraAligned = 5
    }
}