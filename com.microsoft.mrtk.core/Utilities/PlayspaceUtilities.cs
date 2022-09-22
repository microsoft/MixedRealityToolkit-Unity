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
        /// The first detected XROrigin in the current scene.
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
        
        [ObsoleteAttribute("For input device poses (head, hands, controllers), use OriginOffsetTransform. " +
                           "For trackables like spatial anchors or SR meshes, use OriginTransform.")]
        public static Transform ReferenceTransform;

        /// <summary>
        /// The XROrigin's CameraFloorOffsetObject transform, which all trackables
        /// should be tracked relative to (and parented to). On devices that have a 
        /// floor (Guardian, Chaperone, etc), this will be identical to the RigTransform. On
        /// devices with no floor (HoloLens, etc) this may have a vertical offset to represent
        /// the user's height. This is the transform that should be used for all tracked objects,
        /// including input devices, the user's head, controllers, hand joints, spatial anchors,
        /// meshes, and planes.
        /// </summary>
        /// <remarks>
        /// The XROrigin places trackables relative to the
        /// CameraFloorOffsetObject. This offset object is set based on the TrackingOriginMode;
        /// in Device mode, head and hands poses are reported relative to some arbitrary point
        /// in space, whereas in Floor mode, the poses are reported relative to the precalibrated
        /// floor height/origin.
        /// </remarks>
        public static Transform OriginOffsetTransform => XROrigin.CameraFloorOffsetObject.transform;

        /// <summary>
        /// The rig's physical origin. This is used for locomotion, colliders, avatars, or other
        /// properties that should reflect the user's floor. The transform will always be at the user's
        /// physical floor elevation. Locomotion scripts should use this transform to physically
        /// move the user throughout the world. When teleporting the user, compare this transform's
        /// position to the camera's position to determine the relative teleport target offset, or
        /// use <see cref="XROrigin.CameraInOriginSpacePos"/>.
        /// </summary>
        /// <seealso cref="OriginOffsetTransform"/>
        public static Transform RigTransform => XROrigin.Origin.transform;

        /// <summary>
        /// Transforms a <see cref="Pose"/> from OpenXR scene-origin-space to Unity world-space.
        /// Uses the XROrigin's CameraFloorOffsetObject transform.
        /// </summary>
        public static Pose TransformPose(Pose pose)
        {
            Transform origin = OriginOffsetTransform;
            return new Pose(
                origin.TransformPoint(pose.position),
                origin.rotation * pose.rotation
            );
        }

        /// <summary>
        /// Transforms a <see cref="HandJointPose"/> from OpenXR scene-origin-space to Unity world-space.
        /// </summary>
        /// <remarks>
        /// Internally, this uses <see cref="OriginOffsetTransform"/>, as input devices
        /// are reported relative to the floor offset of the rig.
        /// </remarks>
        public static HandJointPose TransformJointPose(HandJointPose joint)
        {
            // Null-checking Unity objects can be expensive. Caching this here cuts two null checks into one.
            // Here, we use OriginOffsetTransform, because joint poses are reported local to the floor offset.
            Transform origin = OriginOffsetTransform;
            return new HandJointPose(
                origin.TransformPoint(joint.Position),
                origin.rotation * joint.Rotation,
                joint.Radius
            );
        }
    }
}
