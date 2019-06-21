// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// Follow solver positions an element relative in front of the forward axis of the reference.
    /// The element can be loosly constrained (a.k.a. tag-along) so that it doesn't follow until it is too far.
    /// </summary>
    public class Follow : Solver
    {
        [SerializeField]
        [Tooltip("Position lerp multiplier")]
        private float moveToDefaultDistanceLerpTime = 0.1f;

        /// <summary>
        /// Position lerp multiplier.
        /// </summary>
        public float MoveToDefaultDistanceLerpTime
        {
            get { return moveToDefaultDistanceLerpTime; }
            set { moveToDefaultDistanceLerpTime = value; }
        }
        
        [SerializeField]
        [Tooltip("The desired orientation of this object")]
        private SolverOrientationType orientationType = SolverOrientationType.MaintainGoal;

        /// <summary>
        /// The desired orientation of this object.
        /// </summary>
        public SolverOrientationType OrientationType
        {
            get { return orientationType; }
            set { orientationType = value; }
        }

        [SerializeField]
        [Tooltip("Min distance from eye to position element around, i.e. the sphere radius")]
        private float minDistance = 1f;

        /// <summary>
        /// Min distance from eye to position element around, i.e. the sphere radius.
        /// </summary>
        public float MinDistance
        {
            get { return minDistance; }
            set { minDistance = value; }
        }

        [SerializeField]
        [Tooltip("Max distance from eye to element")]
        private float maxDistance = 2f;

        /// <summary>
        /// Max distance from eye to element.
        /// </summary>
        public float MaxDistance
        {
            get { return maxDistance; }
            set { maxDistance = value; }
        }

        [SerializeField]
        [Tooltip("Min distance from eye to position element around, i.e. the sphere radius")]
        private float defaultDistance = 1f;

        /// <summary>
        /// Initial placement distance. Should be between min and max.
        /// </summary>
        public float DefaultDistance
        {
            get { return defaultDistance; }
            set { defaultDistance = value; }
        }

        [SerializeField]
        [Tooltip("The element will stay at least this close to the center of view")]
        private float maxViewHorizontalDegrees = 30f;

        /// <summary>
        /// The element will stay at least this close to the center of view.
        /// </summary>
        public float MaxViewHorizontalDegrees
        {
            get { return maxViewHorizontalDegrees; }
            set { maxViewHorizontalDegrees = value; }
        }

        [SerializeField]
        [Tooltip("The element will stay at least this close to the center of view")]
        private float maxViewVerticalDegrees = 30f;

        /// <summary>
        /// The element will stay at least this close to the center of view.
        /// </summary>
        public float MaxViewVerticalDegrees
        {
            get { return maxViewVerticalDegrees; }
            set { maxViewVerticalDegrees = value; }
        }

        [SerializeField]
        [Tooltip("The element will stay world lock until the angle between the forward vector and vector to the controller is greater then the deadzone")]
        private float orientToControllerDeadzoneDegrees = 60f;

        /// <summary>
        /// The element will stay world lock until the angle between the forward vector and vector to the controller is greater then the deadzone.
        /// </summary>
        public float OrientToControllerDeadzoneDegrees
        {
            get { return orientToControllerDeadzoneDegrees; }
            set { orientToControllerDeadzoneDegrees = value; }
        }

        [SerializeField]
        [Tooltip("Option to ignore angle clamping")]
        private bool ignoreAngleClamp = false;

        /// <summary>
        /// Option to ignore angle clamping.
        /// </summary>
        public bool IgnoreAngleClamp
        {
            get { return ignoreAngleClamp; }
            set { ignoreAngleClamp = value; }
        }

        [SerializeField]
        [Tooltip("Option to ignore distance clamping")]
        private bool ignoreDistanceClamp = false;

        /// <summary>
        /// Option to ignore distance clamping.
        /// </summary>
        public bool IgnoreDistanceClamp
        {
            get { return ignoreDistanceClamp; }
            set { ignoreDistanceClamp = value; }
        }

        [SerializeField]
        [Tooltip("Option to ignore the pitch and roll of the reference target")]
        private bool ignoreReferencePitchAndRoll = false;

        /// <summary>
        /// Option to ignore the pitch and roll of the reference target
        /// </summary>
        public bool IgnoreReferencePitchAndRoll
        {
            get { return ignoreReferencePitchAndRoll; }
            set { ignoreReferencePitchAndRoll = value; }
        }

        [SerializeField]
        [Tooltip("Pitch offset from reference element (relative to Max Distance)")]
        public float pitchOffset = 0;

        /// <summary>
        /// Pitch offset from reference element (relative to MaxDistance).
        /// </summary>
        /// [SerializeField]
        public float PitchOffset
        {
            get { return pitchOffset; }
            set { pitchOffset = value; }
        }

        [SerializeField]
        [Tooltip("Max vertical distance between element and reference")]
        private float verticalMaxDistance = 0.0f;

        /// <summary>
        /// Max vertical distance between element and reference.
        /// </summary>
        public float VerticalMaxDistance
        {
            get { return verticalMaxDistance; }
            set { verticalMaxDistance = value; }
        }

        public void Recenter()
        {
            recenterNextUpdate = true;
        }

        private Vector3 ReferencePosition => SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.position : Vector3.zero;
        private Quaternion ReferenceRotation => SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.rotation : Quaternion.identity;
        private Vector3 PreviousReferencePosition = Vector3.zero;
        private Quaternion PreviousReferenceRotation = Quaternion.identity;
        private bool recenterNextUpdate = true;

        protected override void OnEnable()
        {
            base.OnEnable();
            Recenter();
        }

        /// <inheritdoc />
        public override void SolverUpdate()
        {
            Vector3 refPosition = Vector3.zero;
            Quaternion refRotation = Quaternion.identity;
            Vector3 refForward = Vector3.zero;
            GetReferenceInfo(
                PreviousReferencePosition,
                ReferencePosition,
                ReferenceRotation,
                VerticalMaxDistance,
                ref refPosition,
                ref refRotation,
                ref refForward);

            // Determine the current position of the element
            Vector3 currentPosition = WorkingPosition;
            if (recenterNextUpdate)
            {
                currentPosition = refPosition + refForward * DefaultDistance;
            }

            // Angularly clamp to determine goal direction to place the element
            Vector3 goalDirection = refRotation * Vector3.forward;
            SolverOrientationType orientation = orientationType;
            bool angularClamped = false;
            if (!ignoreAngleClamp && !recenterNextUpdate)
            {
                angularClamped = AngularClamp(
                    refPosition,
                    PreviousReferencePosition,
                    refRotation,
                    PreviousReferenceRotation,
                    currentPosition,
                    IgnoreReferencePitchAndRoll,
                    MaxViewHorizontalDegrees,
                    MaxViewVerticalDegrees,
                    ref goalDirection);

                if (angularClamped)
                {       
                    orientation = SolverOrientationType.FaceTrackedObject;
                }
            }

            // Distance clamp to determine goal position to place the element
            Vector3 goalPosition = currentPosition;
            bool distanceClamped = false;
            if (!ignoreDistanceClamp)
            {
                distanceClamped = DistanceClamp(
                    MinDistance,
                    DefaultDistance,
                    MaxDistance,
                    (PitchOffset != 0),
                    currentPosition,
                    refPosition,
                    goalDirection,
                    angularClamped,
                    ref goalPosition);

                if (distanceClamped)
                {       
                    orientation = SolverOrientationType.FaceTrackedObject;
                }
            }

            // Figure out goal rotation of the element based on orientation setting
            Quaternion goalRotation = Quaternion.identity;
            ComputeOrientation(
                orientation,
                orientToControllerDeadzoneDegrees,
                goalPosition,
                GoalRotation,
                ref goalRotation);

            GoalPosition = goalPosition;
            GoalRotation = goalRotation;

            PreviousReferencePosition = refPosition;
            PreviousReferenceRotation = refRotation;
            recenterNextUpdate = false;

            UpdateWorkingPositionToGoal();
            UpdateWorkingRotationToGoal();
        }

        float AngleBetweenOnXZPlane(Vector3 from, Vector3 to)
        {
            float angle = Mathf.Atan2(to.z, to.x) - Mathf.Atan2(from.z, from.x);
            return SimplifyAngle(angle) * Mathf.Rad2Deg;
        }

        float AngleBetweenOnXYPlane(Vector3 from, Vector3 to)
        {
            float angle = Mathf.Atan2(to.y, to.x) - Mathf.Atan2(from.y, from.x);
            return SimplifyAngle(angle) * Mathf.Rad2Deg;
        }

        float AngleBetweenOnAxis(Vector3 from, Vector3 to, Vector3 axis)
        {
            Quaternion axisQuat = Quaternion.Inverse(Quaternion.LookRotation(axis));
            Vector3 v1 = axisQuat * from;
            Vector3 v2 = axisQuat * to;
            return AngleBetweenOnXYPlane(v1, v2);
        }

        float SimplifyAngle(float angle)
        {
            while (angle > Mathf.PI)
            {
                angle -= 2 * Mathf.PI;
            }

            while (angle < -Mathf.PI)
            {
                angle += 2 * Mathf.PI;
            }

            return angle;
        }

        private bool AngularClamp(
            Vector3 refPosition,
            Vector3 previousRefPosition,
            Quaternion refRotation,
            Quaternion previousRefRotation,
            Vector3 currentPosition,
            bool ignoreVertical,
            float maxHorizontalDegrees,
            float maxVerticalDegrees,
            ref Vector3 refForward)
        {
            Vector3 toTarget = currentPosition - refPosition;
            float currentDistance = toTarget.magnitude;
            if (currentDistance <= 0)
            {
                // No need to clamp
                return false;
            }

            toTarget.Normalize();

            // Start off with a rotation towards the target. If it's within leashing bounds, we can leave it alone.
            Quaternion rotation = Quaternion.LookRotation(toTarget, Vector3.up);

            // This is the meat of the leashing algorithm. The goal is to ensure that the reference's forward
            // vector remains within the bounds set by the leashing parameters. To do this, determine the angles
            // between toTarget and the leashing bounds about the global Y axis and the reference's X axis.
            // If toTarget falls within the leashing bounds, then we don't have to modify it.
            // Otherwise, we apply a correction rotation to bring it within bounds.

            Vector3 currentRefForward = refRotation * Vector3.forward;
            Vector3 refRight = refRotation * Vector3.right;

            bool angularClamped = false;

            // X-axis leashing
            // Leashing around the reference's X axis only makes sense if the reference isn't gravity aligned.
            if (ignoreVertical)
            {
                float angle = AngleBetweenOnAxis(toTarget, currentRefForward, refRight);
                rotation = Quaternion.AngleAxis(angle, refRight) * rotation;
            }
            else
            {
                Vector3 min = Quaternion.AngleAxis(maxVerticalDegrees * 0.5f, refRight) * refForward;
                Vector3 max = Quaternion.AngleAxis(-maxVerticalDegrees * 0.5f, refRight) * refForward;

                float minAngle = AngleBetweenOnAxis(toTarget, min, refRight);
                float maxAngle = AngleBetweenOnAxis(toTarget, max, refRight);

                if (minAngle < 0)
                {
                    rotation = Quaternion.AngleAxis(minAngle, refRight) * rotation;
                    angularClamped = true;
                }
                else if (maxAngle > 0)
                {
                    rotation = Quaternion.AngleAxis(maxAngle, refRight) * rotation;
                    angularClamped = true;
                }
            }

            // Y-axis leashing
            {
                Vector3 min = Quaternion.AngleAxis(-maxHorizontalDegrees * 0.5f, Vector3.up) * refForward;
                Vector3 max = Quaternion.AngleAxis(maxHorizontalDegrees * 0.5f, Vector3.up) * refForward;

                // These are negated because Unity is left-handed
                float minAngle = -AngleBetweenOnXZPlane(toTarget, min);
                float maxAngle = -AngleBetweenOnXZPlane(toTarget, max);

                if (minAngle > 0)
                {
                    rotation = Quaternion.AngleAxis(minAngle, Vector3.up) * rotation;
                    angularClamped = true;
                }
                else if (maxAngle < 0)
                {
                    rotation = Quaternion.AngleAxis(maxAngle, Vector3.up) * rotation;
                    angularClamped = true;
                }
            }

            refForward = rotation * Vector3.forward;
            return angularClamped;
        }

        bool DistanceClamp(
            float minDistance,
            float defaultDistance,
            float maxDistance,
            bool maintainPitch,
            Vector3 currentPosition,
            Vector3 refPosition,
            Vector3 refForward,
            bool interpolateToDefaultDistance,
            ref Vector3 clampedPosition)
        {
            float clampedDistance;
            float currentDistance = Vector3.Distance(currentPosition, refPosition);
            Vector3 direction = refForward;
            if (maintainPitch)
            {
                // If we don't account for pitch offset, the casted object will float up/down as the reference
                // gets closer to it because we will still be casting in the direction of the pitched offset.
                // To fix this, only modify the XZ position of the object.

                Vector3 directionXZ = refForward;
                directionXZ.y = 0;
                directionXZ.Normalize();

                Vector3 refToElementXZ = currentPosition - refPosition;
                refToElementXZ.y = 0;
                float desiredDistanceXZ = refToElementXZ.magnitude;

                Vector3 minDistanceXZVector = refForward * minDistance;
                minDistanceXZVector.y = 0;
                float minDistanceXZ = minDistanceXZVector.magnitude;

                Vector3 maxDistanceXZVector = refForward * maxDistance;
                maxDistanceXZVector.y = 0;
                float maxDistanceXZ = maxDistanceXZVector.magnitude;

                desiredDistanceXZ = Mathf.Clamp(desiredDistanceXZ, minDistanceXZ, maxDistanceXZ);

                if (interpolateToDefaultDistance)
                {
                    Vector3 defaultDistanceXZVector = direction * defaultDistance;
                    defaultDistanceXZVector.y = 0;
                    float defaulltDistanceXZ = defaultDistanceXZVector.magnitude;
                
                    float interpolationRate = Mathf.Min(moveToDefaultDistanceLerpTime * 60.0f * SolverHandler.DeltaTime, 1.0f);
                    desiredDistanceXZ = desiredDistanceXZ + (interpolationRate * (defaulltDistanceXZ - desiredDistanceXZ));
                }

                Vector3 desiredPosition = refPosition + directionXZ * desiredDistanceXZ;
                float desiredHeight = refPosition.y + refForward.y * maxDistance;
                desiredPosition.y = desiredHeight;

                direction = desiredPosition - refPosition;
                clampedDistance = direction.magnitude;
                direction /= clampedDistance;

                clampedDistance = Mathf.Max(minDistance, clampedDistance);
            }
            else
            {
                clampedDistance = currentDistance;

                if (interpolateToDefaultDistance)
                {
                    float interpolationRate = Mathf.Min(moveToDefaultDistanceLerpTime * 60.0f * SolverHandler.DeltaTime, 1.0f);
                    clampedDistance = clampedDistance + (interpolationRate * (defaultDistance - clampedDistance));
                }

                clampedDistance = Mathf.Clamp(clampedDistance, minDistance, maxDistance);
            }

            clampedPosition = refPosition + direction * clampedDistance;

            return Vector3EqualEpsilon(clampedPosition, currentPosition, 0.0001f);
        }

        void ComputeOrientation(
            SolverOrientationType defaultOrientationType,
            float orientToControllerDeadzoneRadians,
            Vector3 goalPosition,
            Quaternion previousGoalRotation,
            ref Quaternion orientation)
        {
            if (defaultOrientationType == SolverOrientationType.MaintainGoal)
            {
                Vector3 nodeToCamera = goalPosition - ReferencePosition;
                float angle = Mathf.Abs(AngleBetweenOnXZPlane(transform.forward,nodeToCamera));
                if (angle > orientToControllerDeadzoneRadians)
                {
                    defaultOrientationType = SolverOrientationType.FaceTrackedObject;
                }
            }
            
            switch (defaultOrientationType)
            {
                case SolverOrientationType.YawOnly:
                    float targetYRotation = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.eulerAngles.y : 0.0f;
                    orientation = Quaternion.Euler(0f, targetYRotation, 0f);
                    break;
                case SolverOrientationType.Unmodified:
                    orientation = transform.rotation;
                    break;
                case SolverOrientationType.CameraAligned:
                    orientation = CameraCache.Main.transform.rotation;
                    break;
                case SolverOrientationType.FaceTrackedObject:
                    orientation = SolverHandler.TransformTarget != null ? Quaternion.LookRotation(goalPosition - ReferencePosition) : Quaternion.identity;
                    break;
                case SolverOrientationType.CameraFacing:
                    orientation = SolverHandler.TransformTarget != null ? Quaternion.LookRotation(goalPosition - CameraCache.Main.transform.position) : Quaternion.identity;
                    break;
                case SolverOrientationType.FollowTrackedObject:
                    orientation = SolverHandler.TransformTarget != null ? ReferenceRotation : Quaternion.identity;
                    break;
                case SolverOrientationType.MaintainGoal:
                    orientation = previousGoalRotation;
                    break;
                default:
                    Debug.LogError($"Invalid OrientationType for Orbital Solver on {gameObject.name}");
                    break;
            }
        }

        void GetReferenceInfo(
            Vector3 previousRefPosition,
            Vector3 currentRefPosition,
            Quaternion currentRefRotation,
            float verticalMaxDistance,
            ref Vector3 refPosition,
            ref Quaternion refRotation,
            ref Vector3 refForward)
        {
            refPosition = currentRefPosition;
            refRotation = currentRefRotation;
            if (IgnoreReferencePitchAndRoll)
            {
                Vector3 forward = currentRefRotation * Vector3.forward;
                forward.y = 0;
                refRotation = Quaternion.LookRotation(forward);
                if (PitchOffset != 0)
                {
                    Vector3 right = refRotation * Vector3.right;
                    forward = Quaternion.AngleAxis(PitchOffset, right) * forward;
                    refRotation = Quaternion.LookRotation(forward);
                }
            }

            refForward = refRotation * Vector3.forward;

            // Apply vertical clamp on reference
            if (!recenterNextUpdate && verticalMaxDistance > 0)
            {
                refPosition.y = Mathf.Clamp(previousRefPosition.y, currentRefPosition.y - verticalMaxDistance, currentRefPosition.y + verticalMaxDistance);
            }
        }

        bool Vector3EqualEpsilon(Vector3 x, Vector3 y, float eps)
        {
            float sqrMagnitude = (x - y).sqrMagnitude;

            return sqrMagnitude > eps;
        }
    }
}