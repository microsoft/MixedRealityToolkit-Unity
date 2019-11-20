// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    /// <summary>
    /// Implements a movement logic that uses the model of angular rotations along a sphere whose 
    /// radius varies. The angle to move by is computed by looking at how much the hand changes
    /// relative to a pivot point (slightly below and in front of the head).
    /// 
    /// Usage:
    /// When a manipulation starts, call Setup.
    /// Call Update any time to update the move logic and get a new rotation for the object.
    /// </summary>
    public class TwoHandMoveLogic
    {
        private float pointerRefDistance;

        private bool pointerPosIndependentOfHead = true;

        private Vector3 pointerLocalGrabPoint;
        private Vector3 objectLocalGrabPoint;
        private Vector3 pointerToObject;

        /// <summary>
        /// Setup function
        /// </summary>
        public void Setup(MixedRealityPose pointerCentroidPose, Vector3 grabCentroid, MixedRealityPose objectPose, Vector3 objectScale)
        {
            Vector3 headPosition = CameraCache.Main.transform.position;            
            pointerRefDistance = Vector3.Distance(pointerCentroidPose.Position, headPosition);
            pointerPosIndependentOfHead = pointerRefDistance != 0;
            
            Quaternion worldToPointerRotation = Quaternion.Inverse(pointerCentroidPose.Rotation);
            pointerLocalGrabPoint = worldToPointerRotation * (grabCentroid - pointerCentroidPose.Position);

            objectLocalGrabPoint = Quaternion.Inverse(objectPose.Rotation) * (grabCentroid - objectPose.Position);
            objectLocalGrabPoint = objectLocalGrabPoint.Div(objectScale);

            pointerToObject = objectPose.Position - pointerCentroidPose.Position;
        }

        /// <summary>
        /// Update the rotation based on input.
        /// </summary>
        /// <returns>A Vector3 describing the desired position</returns>
        public Vector3 Update(MixedRealityPose pointerCentroidPose, Quaternion objectRotation, Vector3 objectScale, bool isNearMode, bool usePointerRotation, MovementConstraintType movementConstraint)
        {
            if (!isNearMode || usePointerRotation)
            {
                Vector3 headPosition = CameraCache.Main.transform.position;
                float distanceRatio = 1.0f;

                if (pointerPosIndependentOfHead && movementConstraint != MovementConstraintType.FixDistanceFromHead)
                {
                    // Compute how far away the object should be based on the ratio of the current to original hand distance
                    var currentHandDistance = Vector3.Magnitude(pointerCentroidPose.Position - headPosition);
                    distanceRatio = currentHandDistance / pointerRefDistance;
                }

                Vector3 scaledGrabToObject = Vector3.Scale(objectLocalGrabPoint, objectScale);
                Vector3 adjustedPointerToGrab = (pointerLocalGrabPoint * distanceRatio);
                adjustedPointerToGrab = pointerCentroidPose.Rotation * adjustedPointerToGrab;

                return adjustedPointerToGrab - objectRotation * scaledGrabToObject + pointerCentroidPose.Position;
            }
            else
            {
                return pointerCentroidPose.Position + pointerToObject;
            }
        }
    }
}
