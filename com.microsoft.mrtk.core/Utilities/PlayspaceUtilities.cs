// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Utilities for retrieving the XR rig's offset transform, for use in transforming tracked object coordinates.
    /// </summary>
    public static class PlayspaceUtilities
    {
        private static XROrigin xrOrigin;

        /// <summary>
        /// The first detected XROrigin in the current scene. To transform a trackable's pose
        /// into world space, either use <see cref="XROrigin.CameraFloorOffsetObject"/> or use
        /// the <see cref="TransformPose"/> and <see cref="TransformJointPose"/> methods.
        /// </summary>
        public static XROrigin XROrigin
        {
            get
            {
                if (xrOrigin == null)
                {
                    // Unfortunately, the XROrigin has no singleton property.
                    // Instead, we can find it through the main camera.
                    xrOrigin = Camera.main.GetComponentInParent<XROrigin>();
                    Debug.Assert(xrOrigin != null, "PlayspaceUtilities requires the use of an XROrigin. Check if your main camera is a child of an XROrigin.");
                }

                return xrOrigin;
            }
        }
        
        [ObsoleteAttribute("For transforming trackables poses into worldspace, use XROrigin.CameraFloorOffsetObject.transform.")]
        public static Transform ReferenceTransform => XROrigin.CameraFloorOffsetObject.transform;

        /// <summary>
        /// Transforms a <see cref="Pose"/> from OpenXR scene-origin-space to Unity world-space.
        /// Uses the XROrigin's CameraFloorOffsetObject transform.
        /// </summary>
        public static Pose TransformPose(Pose pose)
        {
            // Null-checking Unity objects can be expensive. Caching this here cuts two null checks into one.
            // Here, we use CameraFloorOffsetObject, because poses are reported local to the floor offset.
            Transform origin = XROrigin.CameraFloorOffsetObject.transform;
            return new Pose(
                origin.TransformPoint(pose.position),
                origin.rotation * pose.rotation
            );
        }

        /// <summary>
        /// Transforms a <see cref="HandJointPose"/> from OpenXR scene-origin-space to Unity world-space.
        /// Uses the XROrigin's CameraFloorOffsetObject transform.
        /// </summary>
        public static HandJointPose TransformPose(HandJointPose pose)
        {
            return new HandJointPose(
                TransformPose(pose.Pose),
                pose.Radius
            );
        }

        /// <summary>
        /// Transforms a <see cref="Pose"/> from Unity world-space to OpenXR scene-origin-space.
        /// Uses the XROrigin's CameraFloorOffsetObject transform.
        /// </summary>
        public static Pose InverseTransformPose(Pose pose)
        {
            // Null-checking Unity objects can be expensive. Caching this here cuts two null checks into one.
            // Here, we use CameraFloorOffsetObject, because poses are reported local to the floor offset.
            Transform origin = XROrigin.CameraFloorOffsetObject.transform;
            return new Pose(
                origin.InverseTransformPoint(pose.position),
                Quaternion.Inverse(origin.rotation) * pose.rotation
            );
        }

        /// <summary>
        /// Transforms a <see cref="HandJointPose"/> from Unity world-space to OpenXR scene-origin-space.
        /// Uses the XROrigin's CameraFloorOffsetObject transform.
        /// </summary>
        public static HandJointPose InverseTransformPose(HandJointPose pose)
        {
            return new HandJointPose(
                InverseTransformPose(pose.Pose),
                pose.Radius
            );
        }
    }
}
