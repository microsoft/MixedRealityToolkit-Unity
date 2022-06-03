// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.XR.CoreUtils;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Utilities for retrieving the XR rig's offset transform, for use in transforming tracked object coordinates.
    /// </summary>
    public static class PlayspaceUtilities
    {
        private static Transform referenceTransform;

        /// <summary>
        /// The transform of XR offset, which all tracked objects are relative to.
        /// </summary>
        public static Transform ReferenceTransform
        {
            get
            {
                if (referenceTransform == null)
                {
                    // Unfortunately, the XROrigin has no singleton property.
                    // Instead, we can find it through the main camera.
                    var rig = CameraCache.Main.GetComponentInParent<XROrigin>();
                    Debug.Assert(rig != null, "PlayspaceUtilities requires the use of an XROrigin. Check if your main camera is a child of an XROrigin.");

                    if (rig != null)
                    {
                        referenceTransform = rig.CameraFloorOffsetObject.transform;
                    }
                }

                return referenceTransform;
            }
        }

        public static HandJointPose TransformJointPose(HandJointPose playspaceLocalJoint)
        {
            // Null-checking Unity objects can be expensive. Caching this here cuts two null checks into one.
            Transform referenceTransform = ReferenceTransform;
            return new HandJointPose(
                referenceTransform.TransformPoint(playspaceLocalJoint.Position),
                referenceTransform.rotation * playspaceLocalJoint.Rotation,
                playspaceLocalJoint.Radius
            );
        }
    }
}
