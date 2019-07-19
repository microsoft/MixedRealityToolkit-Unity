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
        private readonly MovementConstraintType movementConstraint;

        private float pointerRefDistance;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="movementConstraint"></param>
        public TwoHandMoveLogic(MovementConstraintType movementConstraint)
        {
            this.movementConstraint = movementConstraint;
        }

        Vector3 pointerToGrab;
        Vector3 grabToObject;
        Quaternion worldToPointerRotation;
        Vector3 originalScale;

        public void Setup(MixedRealityPose pointerCentroidPose, Vector3 grabCentroid, Vector3 objectPosition, Vector3 objectScale)
        {
            Vector3 headPosition = CameraCache.Main.transform.position;
            
            pointerRefDistance = Vector3.Distance(pointerCentroidPose.Position, headPosition);
            
            worldToPointerRotation = Quaternion.Inverse(pointerCentroidPose.Rotation);
            pointerToGrab = worldToPointerRotation * (grabCentroid - pointerCentroidPose.Position);
            grabToObject = worldToPointerRotation * (objectPosition - grabCentroid);
            originalScale = objectScale;
        }

        public Vector3 Update(MixedRealityPose pointerCentroidPose, Vector3 objectScale, bool usePointerRotation, bool isTwoHand)
        {
            Vector3 headPosition = CameraCache.Main.transform.position;
            float distanceRatio = 1.0f;

            if (movementConstraint != MovementConstraintType.FixDistanceFromHead)
            {
                // Compute how far away the object should be based on the ratio of the current to original hand distance
                var currentHandDistance = Vector3.Magnitude(pointerCentroidPose.Position - headPosition);
                distanceRatio = currentHandDistance / pointerRefDistance;
            }

            Vector3 scaledGrabToObject =  Vector3.Scale(grabToObject, worldToPointerRotation * objectScale.Div(originalScale));
            Vector3 adjustedPointerToObject = (pointerToGrab * distanceRatio) + scaledGrabToObject;
            if (isTwoHand || usePointerRotation)
            {
                adjustedPointerToObject = pointerCentroidPose.Rotation * adjustedPointerToObject;
            }

            return adjustedPointerToObject + pointerCentroidPose.Position;
        }
    }
}
