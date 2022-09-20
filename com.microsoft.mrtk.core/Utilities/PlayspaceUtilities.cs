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
        /// The XROrigin's CameraFloorOffsetObject transform, which all input devices
        /// are tracked relative to. AR trackables like spatial anchors are not tracked
        /// relative to this transform; see OriginTransform instead.
        /// </summary>
        /// <remarks>
        /// The XROrigin tracks input devices (head, hands, etc) relative to the
        /// CameraFloorOffsetObject. This offset object is set based on the TrackingOriginMode;
        /// in Device mode, head and hands poses are reported relative to some arbitrary point
        /// in space, whereas in Floor mode, the poses are reported relative to the precalibrated
        /// floor height/origin. Such a distinction does not exist for other AR trackables like
        /// spatial anchors or SR meshes; they are always reported relative to the XROrigin's true origin.
        /// </remarks>
        public static Transform OriginOffsetTransform => XROrigin.CameraFloorOffsetObject.transform;

        /// <summary>
        /// The origin transform used for trackables like spatial anchors and SR meshes.
        /// Will always be equal to the XROrigin's origin base transform. This may not necessarily
        /// be the origin/coordinate space that the user's head and hands are reported in;
        /// use the <see cref="OriginOffsetTransform"/> when working with the user's head or input
        /// device poses.
        /// </summary>
        /// <remarks>
        /// Input devices such as the HMD, controllers, or hands, depending on the XROrigin's 
        /// TrackingOriginMode, will report their poses as either relative to the precalibrated
        /// floor height (in the runtime) or relative to some arbitrary point in space (generally,
        /// when the device was first initialized.) Therefore, their poses will not necessarily
        /// be relative to the <see cref="OriginTransform"/>, but will always be relative to the
        /// <see cref="OriginOffsetTransform"/>.
        /// </remarks>
        public static Transform OriginTransform => XROrigin.Origin.transform;

        /// <summary>
        /// Given a raw joint pose in floor-offset-relative space, transform the pose into
        /// absolute Unity world coordinates.
        /// </summary>
        /// <remarks>
        /// Internally, this uses <see cref="OriginOffsetTransform"/>, as input devices
        /// are reported relative to the floor offset of the rig.
        /// </remarks>
        public static HandJointPose TransformJointPose(HandJointPose playspaceLocalJoint)
        {
            // Null-checking Unity objects can be expensive. Caching this here cuts two null checks into one.
            // Here, we use OriginOffsetTransform, because joint poses are reported local to the floor offset.
            Transform referenceTransform = OriginOffsetTransform;
            return new HandJointPose(
                referenceTransform.TransformPoint(playspaceLocalJoint.Position),
                referenceTransform.rotation * playspaceLocalJoint.Rotation,
                playspaceLocalJoint.Radius
            );
        }
    }
}
