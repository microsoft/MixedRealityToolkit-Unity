// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    /// <summary>
    /// Implements a move logic that will move an object based on the initial position of 
    /// the grab point relative to the pointer and relative to the object, and subsequent
    /// changes to the pointer and the object's rotation
    /// 
    /// Usage:
    /// When a manipulation starts, call Setup.
    /// Call Update any time to update the move logic and get a new rotation for the object.
    /// </summary>
    internal class ManipulationMoveLogic
    {
        private float pointerRefDistance;

        private bool pointerPosIndependentOfHead = true;

        private Vector3 pointerLocalGrabPoint;
        private Vector3 objectLocalGrabPoint;
        private Vector3 grabToObject;

        /// <summary>
        /// Setup function
        /// </summary>
        public void Setup(MixedRealityPose pointerCentroidPose, Vector3 grabCentroid, MixedRealityPose objectPose, Vector3 objectScale)
        {
            pointerRefDistance = GetDistanceToBody(pointerCentroidPose);

            pointerPosIndependentOfHead = pointerRefDistance != 0;

            Quaternion worldToPointerRotation = Quaternion.Inverse(pointerCentroidPose.Rotation);
            pointerLocalGrabPoint = worldToPointerRotation * (grabCentroid - pointerCentroidPose.Position);

            objectLocalGrabPoint = Quaternion.Inverse(objectPose.Rotation) * (grabCentroid - objectPose.Position);
            objectLocalGrabPoint = objectLocalGrabPoint.Div(objectScale);

            grabToObject = objectPose.Position - grabCentroid;
        }

        /// <summary>
        /// Update the position based on input.
        /// </summary>
        /// <returns>A Vector3 describing the desired position</returns>
        public Vector3 Update(MixedRealityPose pointerCentroidPose, Quaternion objectRotation, Vector3 objectScale, bool usePointerRotation)
        {
            float distanceRatio = 1.0f;

            if (pointerPosIndependentOfHead)
            {
                // Compute how far away the object should be based on the ratio of the current to original hand distance
                float currentHandDistance = GetDistanceToBody(pointerCentroidPose);
                distanceRatio = currentHandDistance / pointerRefDistance;
            }

            if (usePointerRotation)
            {
                Vector3 scaledGrabToObject = Vector3.Scale(objectLocalGrabPoint, objectScale);
                Vector3 adjustedPointerToGrab = (pointerLocalGrabPoint * distanceRatio);
                adjustedPointerToGrab = pointerCentroidPose.Rotation * adjustedPointerToGrab;

                return adjustedPointerToGrab - objectRotation * scaledGrabToObject + pointerCentroidPose.Position;
            }
            else
            {
                return pointerCentroidPose.Position + grabToObject + (pointerCentroidPose.Rotation * pointerLocalGrabPoint) * distanceRatio;
            }
        }

        private float GetDistanceToBody(MixedRealityPose pointerCentroidPose)
        {
            // The body is treated as a ray, parallel to the y-axis, where the start is head position.
            // This means that moving your hand down such that is the same distance from the body will
            // not cause the manipulated object to move further away from your hand. However, when you
            // move your hand upward, away from your head, the manipulated object will be pushed away.
            if (pointerCentroidPose.Position.y > CameraCache.Main.transform.position.y)
            {
                return Vector3.Distance(pointerCentroidPose.Position, CameraCache.Main.transform.position);
            }
            else
            {
                Vector2 headPosXZ = new Vector2(CameraCache.Main.transform.position.x, CameraCache.Main.transform.position.z);
                Vector2 pointerPosXZ = new Vector2(pointerCentroidPose.Position.x, pointerCentroidPose.Position.z);

                return Vector2.Distance(pointerPosXZ, headPosXZ);
            }
        }
    }
}
