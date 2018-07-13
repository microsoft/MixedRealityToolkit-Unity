// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Physics.Solvers
{
    /// <summary>
    /// RadialViewPoser solver locks a tag-along type object within a view cone
    /// </summary>
    public class RadialView : Solver
    {
        private enum ReferenceDirectionEnum
        {
            /// <summary>
            /// Orient towards head including roll, pitch and yaw
            /// </summary>
            ObjectOriented,
            /// <summary>
            /// Orient toward head but ignore roll
            /// </summary>
            FacingWorldUp,
            /// <summary>
            /// Orient towards the head movement direction
            /// </summary>
            HeadMoveDirection,
            /// <summary>
            /// Orient towards head but remain vertical or gravity aligned
            /// </summary>
            GravityAligned
        }

        [SerializeField]
        [Tooltip("Which direction to position the element relative to: HeadOriented rolls with the head, HeadFacingWorldUp view dir but ignores head roll, and HeadMoveDirection uses the direction the head last moved without roll")]
        private ReferenceDirectionEnum referenceDirection = ReferenceDirectionEnum.FacingWorldUp;

        [SerializeField]
        [Tooltip("Min distance from eye to position element around, i.e. the sphere radius")]
        private float minDistance = 1f;

        [SerializeField]
        [Tooltip("Max distance from eye to element")]
        private float maxDistance = 2f;

        [SerializeField]
        [Tooltip("The element will stay at least this far away from the center of view")]
        private float minViewDegrees = 0f;

        [SerializeField]
        [Tooltip("The element will stay at least this close to the center of view")]
        private float maxViewDegrees = 30f;

        [SerializeField]
        [Tooltip("Apply a different clamp to vertical FOV than horizontal.  Vertical = Horizontal * aspectV")]
        private float aspectV = 1f;

        [SerializeField]
        [Tooltip("Option to ignore angle clamping")]
        private bool ignoreAngleClamp = false;

        [SerializeField]
        [Tooltip("Option to ignore distance clamping")]
        private bool ignoreDistanceClamp = false;

        [SerializeField]
        [Tooltip("If true, element will orient to ReferenceDirection, otherwise it will orient to ref pos (the head is the only option currently)")]
        private bool orientToReferenceDirection = false;

        /// <summary>
        /// Position to the view direction, or the movement direction, or the direction of the view cone.
        /// </summary>
        private Vector3 ReferenceDirection
        {
            get
            {
                Vector3 direction = Vector3.one;

                if (referenceDirection == ReferenceDirectionEnum.HeadMoveDirection)
                {
                    direction = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.forward : Vector3.forward;
                }

                return direction;
            }
        }

        /// <summary>
        /// The up direction to use for orientation.
        /// <remarks>Cone may roll with head, or not.</remarks>
        /// </summary>
        private Vector3 UpReference
        {
            get
            {
                Vector3 upReference = Vector3.up;

                if (referenceDirection == ReferenceDirectionEnum.ObjectOriented)
                {
                    upReference = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.up : Vector3.up;
                }

                return upReference;
            }
        }

        private Vector3 ReferencePoint => SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.position : Vector3.zero;

        /// <inheritdoc />
        public override void SolverUpdate()
        {
            Vector3 goalPosition = WorkingPosition;

            if (ignoreAngleClamp)
            {
                if (ignoreDistanceClamp)
                {
                    goalPosition = transform.position;
                }
                else
                {
                    GetDesiredOrientation_DistanceOnly(ref goalPosition);
                }
            }
            else
            {
                GetDesiredOrientation(ref goalPosition);
            }

            // Element orientation
            Vector3 refDirUp = UpReference;
            Quaternion desiredRot;

            if (orientToReferenceDirection)
            {
                desiredRot = Quaternion.LookRotation(ReferenceDirection, refDirUp);
            }
            else
            {
                Vector3 refPoint = ReferencePoint;
                desiredRot = Quaternion.LookRotation(goalPosition - refPoint, refDirUp);
            }

            // If gravity aligned then zero out the x and z axes on the rotation
            if (referenceDirection == ReferenceDirectionEnum.GravityAligned)
            {
                desiredRot.x = desiredRot.z = 0f;
            }

            GoalPosition = goalPosition;
            GoalRotation = desiredRot;

            UpdateWorkingPositionToGoal();
            UpdateWorkingRotationToGoal();
        }

        /// <summary>
        /// Optimized version of GetDesiredOrientation.
        /// </summary>
        /// <param name="desiredPos"></param>
        private void GetDesiredOrientation_DistanceOnly(ref Vector3 desiredPos)
        {
            // TODO: There should be a different solver for distance constraint.
            // Determine reference locations and directions
            Vector3 refPoint = ReferencePoint;
            Vector3 elementPoint = transform.position;
            Vector3 elementDelta = elementPoint - refPoint;
            float elementDist = elementDelta.magnitude;
            Vector3 elementDir = elementDist > 0 ? elementDelta / elementDist : Vector3.one;

            // Clamp distance too
            float clampedDistance = Mathf.Clamp(elementDist, minDistance, maxDistance);

            if (!clampedDistance.Equals(elementDist))
            {
                desiredPos = refPoint + clampedDistance * elementDir;
            }
        }

        private void GetDesiredOrientation(ref Vector3 desiredPos)
        {
            // Determine reference locations and directions
            Vector3 direction = ReferenceDirection;
            Vector3 upDirection = UpReference;
            Vector3 referencePoint = ReferencePoint;
            Vector3 elementPoint = transform.position;
            Vector3 elementDelta = elementPoint - referencePoint;
            float elementDist = elementDelta.magnitude;
            Vector3 elementDir = elementDist > 0 ? elementDelta / elementDist : Vector3.one;
            float flip = Vector3.Dot(elementDelta, direction);

            // Generate basis: First get axis perpendicular to reference direction pointing toward element
            Vector3 perpendicularDirection = (elementDir - direction);
            perpendicularDirection -= direction * Vector3.Dot(perpendicularDirection, direction);
            perpendicularDirection.Normalize();

            // Calculate the clamping angles, accounting for aspect (need the angle relative to view plane)
            float heightToViewAngle = Vector3.Angle(perpendicularDirection, upDirection);
            float verticalAspectScale = Mathf.Lerp(aspectV, 1f, Mathf.Abs(Mathf.Sin(heightToViewAngle * Mathf.Deg2Rad)));

            // Calculate the current angle
            float currentAngle = Vector3.Angle(elementDir, direction);
            float currentAngleClamped = Mathf.Clamp(currentAngle, minViewDegrees * verticalAspectScale, maxViewDegrees * verticalAspectScale);

            // Clamp distance too, if desired
            float clampedDistance = ignoreDistanceClamp ? elementDist : Mathf.Clamp(elementDist, minDistance, maxDistance);

            // If the angle was clamped, do some special update stuff
            if (flip < 0)
            {
                desiredPos = referencePoint + direction;
            }
            else if (!currentAngle.Equals(currentAngleClamped))
            {
                float angRad = currentAngleClamped * Mathf.Deg2Rad;

                // Calculate new position
                desiredPos = referencePoint + clampedDistance * (direction * Mathf.Cos(angRad) + perpendicularDirection * Mathf.Sin(angRad));
            }
            else if (!clampedDistance.Equals(elementDist))
            {
                // Only need to apply distance
                desiredPos = referencePoint + clampedDistance * elementDir;
            }
        }
    }
}
