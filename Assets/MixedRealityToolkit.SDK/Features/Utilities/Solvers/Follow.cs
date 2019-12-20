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

        [SerializeField]
        [Tooltip("Lock the rotation to a specified number of steps around the tracked object.")]
        private bool useAngleStepping = false;

        /// <summary>
        /// Lock the rotation to a specified number of steps around the tracked object.
        /// </summary>
        public bool UseAngleStepping
        {
            get { return useAngleStepping; }
            set { useAngleStepping = value; }
        }

        [Range(2, 24)]
        [SerializeField]
        [Tooltip("The division of steps this object can tether to. Higher the number, the more snapple steps.")]
        private int tetherAngleSteps = 6;

        /// <summary>
        /// The division of steps this object can tether to. Higher the number, the more snapple steps.
        /// </summary>
        public int TetherAngleSteps
        {
            get { return tetherAngleSteps; }
            set
            {
                tetherAngleSteps = Mathf.Clamp(value, 2, 24);
            }
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
            GetReferenceInfo(ref refPosition, ref refRotation, ref refForward);

            // Determine the current position of the element
            Vector3 currentPosition = WorkingPosition;
            if (recenterNextUpdate)
            {
                currentPosition = refPosition + refForward * DefaultDistance;
            }

            // Angularly clamp to determine goal direction to place the element
            Vector3 goalDirection = refForward;
            SolverOrientationType orientation = orientationType;
            bool angularClamped = false;
            if (!ignoreAngleClamp && !recenterNextUpdate)
            {
                angularClamped = AngularClamp(refPosition, refRotation, currentPosition, ref goalDirection);

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
                distanceClamped = DistanceClamp(currentPosition, refPosition, goalDirection, angularClamped, ref goalPosition);

                if (distanceClamped)
                {       
                    orientation = SolverOrientationType.FaceTrackedObject;
                }
            }

            // Figure out goal rotation of the element based on orientation setting
            Quaternion goalRotation = Quaternion.identity;
            ComputeOrientation(orientation, goalPosition, ref goalRotation);

            GoalPosition = goalPosition;
            GoalRotation = goalRotation;

            PreviousReferencePosition = refPosition;
            PreviousReferenceRotation = refRotation;
            recenterNextUpdate = false;

            UpdateWorkingPositionToGoal();
            UpdateWorkingRotationToGoal();
        }

        /// <summary>
        /// Projects from and to on to the plane with given normal and gets the
        /// angle between these projected vectors.
        /// </summary>
        /// <returns>angle between project from and to in degrees</returns>
        float AngleBetweenOnPlane(Vector3 from, Vector3 to, Vector3 normal)
        {
            from.Normalize();
            to.Normalize();
            normal.Normalize();

            Vector3 right = Vector3.Cross(normal, from);
            Vector3 forward = Vector3.Cross(right, normal);

            float angle = Mathf.Atan2(Vector3.Dot(to, right), Vector3.Dot(to, forward));

            return SimplifyAngle(angle) * Mathf.Rad2Deg;
        }
        
        /// <summary>
        /// Calculates the angle between vec and a plane described by normal. The angle returned
        /// is signed.
        /// </summary>
        /// <returns>signed angle between vec and the plane described by normal</returns>
        float AngleBetweenVectorAndPlane(Vector3 vec, Vector3 normal)
        {
            return 90 - (Mathf.Acos(Vector3.Dot(vec, normal)) * Mathf.Rad2Deg);
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

        private bool AngularClamp(Vector3 refPosition, Quaternion refRotation, Vector3 currentPosition, ref Vector3 refForward)
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
            Vector3 refUp = refRotation * Vector3.up;

            bool angularClamped = false;

            // X-axis leashing
            // Leashing around the reference's X axis only makes sense if the reference isn't gravity aligned.
            if (IgnoreReferencePitchAndRoll)
            {
                float angle = AngleBetweenOnPlane(toTarget, currentRefForward, refRight);
                rotation = Quaternion.AngleAxis(angle, refRight) * rotation;
            }
            else
            {
                float angle = -AngleBetweenVectorAndPlane(toTarget, refUp);
                float minMaxAngle = MaxViewVerticalDegrees * 0.5f;

                if (angle < -minMaxAngle)
                {
                    rotation = Quaternion.AngleAxis(-minMaxAngle - angle, refRight) * rotation;
                    angularClamped = true;
                }
                else if (angle > minMaxAngle)
                {
                    rotation = Quaternion.AngleAxis(minMaxAngle - angle, refRight) * rotation;
                    angularClamped = true;
                }
            }

            // Y-axis leashing
            if (UseAngleStepping)
            {
                float stepAngle = 360f / tetherAngleSteps;
                int numberOfSteps = Mathf.RoundToInt(SolverHandler.TransformTarget.transform.eulerAngles.y / stepAngle);

                float newAngle = stepAngle * numberOfSteps;

                rotation = Quaternion.Euler(rotation.eulerAngles.x, newAngle, rotation.eulerAngles.z);
            }
            else
            {
                // This is negated because Unity is left-handed
                float angle = AngleBetweenVectorAndPlane(toTarget, refRight);
                float minMaxAngle = MaxViewHorizontalDegrees * 0.5f;

                if (angle < -minMaxAngle)
                {
                    rotation = Quaternion.AngleAxis(-minMaxAngle - angle, Vector3.up) * rotation;
                    angularClamped = true;
                }
                else if (angle > minMaxAngle)
                {
                    rotation = Quaternion.AngleAxis(minMaxAngle - angle, Vector3.up) * rotation;
                    angularClamped = true;
                }
            }

            refForward = rotation * Vector3.forward;
            return angularClamped;
        }

        bool DistanceClamp(Vector3 currentPosition, Vector3 refPosition, Vector3 refForward, bool interpolateToDefaultDistance, ref Vector3 clampedPosition)
        {
            float clampedDistance;
            float currentDistance = Vector3.Distance(currentPosition, refPosition);
            Vector3 direction = refForward;
            if (IgnoreReferencePitchAndRoll && PitchOffset != 0)
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

                Vector3 minDistanceXZVector = refForward * MinDistance;
                minDistanceXZVector.y = 0;
                float minDistanceXZ = minDistanceXZVector.magnitude;

                Vector3 maxDistanceXZVector = refForward * MaxDistance;
                maxDistanceXZVector.y = 0;
                float maxDistanceXZ = maxDistanceXZVector.magnitude;

                desiredDistanceXZ = Mathf.Clamp(desiredDistanceXZ, minDistanceXZ, maxDistanceXZ);

                if (interpolateToDefaultDistance)
                {
                    Vector3 defaultDistanceXZVector = direction * DefaultDistance;
                    defaultDistanceXZVector.y = 0;
                    float defaulltDistanceXZ = defaultDistanceXZVector.magnitude;
                
                    float interpolationRate = Mathf.Min(moveToDefaultDistanceLerpTime * 60.0f * SolverHandler.DeltaTime, 1.0f);
                    desiredDistanceXZ = desiredDistanceXZ + (interpolationRate * (defaulltDistanceXZ - desiredDistanceXZ));
                }

                Vector3 desiredPosition = refPosition + directionXZ * desiredDistanceXZ;
                float desiredHeight = refPosition.y + refForward.y * MaxDistance;
                desiredPosition.y = desiredHeight;

                direction = desiredPosition - refPosition;
                clampedDistance = direction.magnitude;
                direction /= clampedDistance;

                clampedDistance = Mathf.Max(MinDistance, clampedDistance);
            }
            else
            {
                clampedDistance = currentDistance;

                if (interpolateToDefaultDistance)
                {
                    float interpolationRate = Mathf.Min(moveToDefaultDistanceLerpTime * 60.0f * SolverHandler.DeltaTime, 1.0f);
                    clampedDistance = clampedDistance + (interpolationRate * (DefaultDistance - clampedDistance));
                }

                clampedDistance = Mathf.Clamp(clampedDistance, MinDistance, MaxDistance);
            }

            clampedPosition = refPosition + direction * clampedDistance;

            return Vector3EqualEpsilon(clampedPosition, currentPosition, 0.0001f);
        }

        void ComputeOrientation(SolverOrientationType defaultOrientationType, Vector3 goalPosition, ref Quaternion orientation)
        {
            if (defaultOrientationType == SolverOrientationType.MaintainGoal)
            {
                Vector3 nodeToCamera = goalPosition - ReferencePosition;
                float angle = Mathf.Abs(AngleBetweenOnPlane(transform.forward, nodeToCamera, Vector3.up));
                if (angle > orientToControllerDeadzoneDegrees)
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
                    orientation = GoalRotation;
                    break;
                default:
                    Debug.LogError($"Invalid OrientationType for Orbital Solver on {gameObject.name}");
                    break;
            }
        }

        void GetReferenceInfo(ref Vector3 refPosition, ref Quaternion refRotation, ref Vector3 refForward)
        {
            refPosition = ReferencePosition;
            refRotation = ReferenceRotation;
            if (IgnoreReferencePitchAndRoll)
            {
                Vector3 forward = ReferenceRotation * Vector3.forward;
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
            if (!recenterNextUpdate && VerticalMaxDistance > 0)
            {
                refPosition.y = Mathf.Clamp(PreviousReferencePosition.y, ReferencePosition.y - VerticalMaxDistance, ReferencePosition.y + VerticalMaxDistance);
            }
        }

        bool Vector3EqualEpsilon(Vector3 x, Vector3 y, float eps)
        {
            float sqrMagnitude = (x - y).sqrMagnitude;

            return sqrMagnitude > eps;
        }
    }
}