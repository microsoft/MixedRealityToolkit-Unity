// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Physics
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
        private static readonly Vector3 offsetPosition = new Vector3(0, -0.2f, 0);
        private readonly MovementConstraintType movementConstraint;

        private float handRefDistance;
        private float objRefDistance;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rotationConstraint"></param>
        public TwoHandMoveLogic(MovementConstraintType _movementConstraint)
        {
            movementConstraint = _movementConstraint;
        }

        /// <summary>
        /// The initial angle between the hand and the object
        /// </summary>
        private Quaternion gazeAngularOffset;

        private const float DistanceScale = 2f;
        public void Setup(Vector3 startHandPositionMeters, Transform manipulationRoot)
        {
            var newHandPosition = startHandPositionMeters;

            // The pivot is just below and in front of the head.
            var pivotPosition = GetHandPivotPosition();

            objRefDistance = Vector3.Distance(manipulationRoot.position, pivotPosition);
            handRefDistance = Vector3.Distance(newHandPosition, pivotPosition);

            var objDirection = Vector3.Normalize(manipulationRoot.position - pivotPosition);
            var handDirection = Vector3.Normalize(newHandPosition - pivotPosition);

            // We transform the forward vector of the object, the direction of the object, and the direction of the hand
            // to camera space so everything is relative to the user's perspective.
            objDirection = CameraCache.Main.transform.InverseTransformDirection(objDirection);
            handDirection = CameraCache.Main.transform.InverseTransformDirection(handDirection);

            // Store the original rotation between the hand an object
            gazeAngularOffset = Quaternion.FromToRotation(handDirection, objDirection);
        }

        public Vector3 Update(Vector3 centroid, Vector3 manipulationObjectPosition)
        {
            var newHandPosition = centroid;
            var pivotPosition = GetHandPivotPosition();

            // Compute the pivot -> hand vector in camera space
            var newHandDirection = Vector3.Normalize(newHandPosition - pivotPosition);
            newHandDirection = CameraCache.Main.transform.InverseTransformDirection(newHandDirection);

            // The direction the object should face is the current hand direction rotated by the original hand -> object rotation.
            var targetDirection = Vector3.Normalize(gazeAngularOffset * newHandDirection);
            targetDirection = CameraCache.Main.transform.TransformDirection(targetDirection);

            var targetDistance = objRefDistance;

            if(movementConstraint != MovementConstraintType.FixDistanceFromHead)
            {
                // Compute how far away the object should be based on the ratio of the current to original hand distance
                var currentHandDistance = Vector3.Magnitude(newHandPosition - pivotPosition);
                var distanceRatio = currentHandDistance / handRefDistance;
                var distanceOffset = distanceRatio > 0 ? (distanceRatio - 1f) * DistanceScale : 0;
                targetDistance += distanceOffset;
            }

            var newPosition = pivotPosition + (targetDirection * targetDistance);
            var newDistance = Vector3.Distance(newPosition, pivotPosition);

            if (newDistance > 4f)
            {
                newPosition = pivotPosition + Vector3.Normalize(newPosition - pivotPosition) * 4f;
            }

            return newPosition;
        }

        /// <summary>
        /// Get the hand pivot position located a bit lower and behind the camera.
        /// </summary>
        /// <returns>A point that is below and just in front of the head.</returns>
        public static Vector3 GetHandPivotPosition()
        {
            Vector3 pivot = CameraCache.Main.transform.position + offsetPosition - CameraCache.Main.transform.forward * 0.2f; // a bit lower and behind
            return pivot;
        }
    }
}
