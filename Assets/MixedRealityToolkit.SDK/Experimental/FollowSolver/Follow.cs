// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities
{
    /// <summary>
    /// Follow solver positions an element in front of the of the tracked target (relative to its local forward axis).
    /// The element can be loosely constrained (a.k.a. tag-along) so that it doesn't follow until the tracked target moves
    /// beyond user defined bounds.
    /// </summary> 
    [AddComponentMenu("Scripts/MRTK/Experimental/Solver/Follow")]
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
            get => moveToDefaultDistanceLerpTime;
            set => moveToDefaultDistanceLerpTime = value;
        }
        
        [SerializeField]
        [Tooltip("The desired orientation of this object")]
        private SolverOrientationType orientationType = SolverOrientationType.FaceTrackedObject;

        /// <summary>
        /// The desired orientation of this object.
        /// </summary>
        public SolverOrientationType OrientationType
        {
            get => orientationType;
            set => orientationType = value;
        }

        [SerializeField]
        [Tooltip("The object will face the tracked object while the object is outside of the distance/direction bounds defined in this component.")]
        private bool faceTrackedObjectWhileClamped = true;

        /// <summary>
        /// The object will face the tracked object while the object is outside of the distance/direction bounds defined in this component.
        /// </summary>
        public bool FaceTrackedObjectWhileClamped
        {
            get => faceTrackedObjectWhileClamped;
            set => faceTrackedObjectWhileClamped = value;
        }

        [SerializeField]
        [Tooltip("Face a user defined transform rather than using the solver orientation type.")]
        private bool faceUserDefinedTargetTransform = false;

        /// <summary>
        /// Face a user defined transform rather than using the solver orientation type.
        /// </summary>
        public bool FaceUserDefinedTargetTransform
        {
            get => faceUserDefinedTargetTransform;
            set => faceUserDefinedTargetTransform = value;
        }

        [SerializeField]
        [Tooltip("Transform this object should face rather than using the solver orientation type.")]
        private Transform targetToFace = null;

        /// <summary>
        /// Transform this object should face rather than using the solver orientation type.
        /// </summary>
        public Transform TargetToFace
        {
            get => targetToFace;
            set => targetToFace = value;
        }

        [SerializeField]
        [EnumFlags]
        [Tooltip("Rotation axes used when facing target.")]
        private AxisFlags pivotAxis = AxisFlags.XAxis | AxisFlags.YAxis | AxisFlags.ZAxis;

        /// <summary>
        /// Rotation axes used when facing target.
        /// </summary>
        public AxisFlags PivotAxis
        {
            get => pivotAxis;
            set => pivotAxis = value;
        }

        [SerializeField]
        [Tooltip("Min distance from eye to position element around, i.e. the sphere radius")]
        private float minDistance = 1f;

        /// <summary>
        /// Min distance from eye to position element around, i.e. the sphere radius.
        /// </summary>
        public float MinDistance
        {
            get => minDistance;
            set => minDistance = value;
        }

        [SerializeField]
        [Tooltip("Max distance from eye to element")]
        private float maxDistance = 2f;

        /// <summary>
        /// Max distance from eye to element.
        /// </summary>
        public float MaxDistance
        {
            get => maxDistance;
            set => maxDistance = value;
        }

        [SerializeField]
        [Tooltip("Default distance from eye to position element around, i.e. the sphere radius")]
        private float defaultDistance = 1f;

        /// <summary>
        /// Initial placement distance. Should be between min and max.
        /// </summary>
        public float DefaultDistance
        {
            get => defaultDistance;
            set => defaultDistance = value;
        }

        [SerializeField]
        [Tooltip("The horizontal angle from the tracked target forward axis to this object will not exceed this value")]
        private float maxViewHorizontalDegrees = 30f;

        /// <summary>
        /// The horizontal angle from the tracked target forward axis to this object will not exceed this value.
        /// </summary>
        public float MaxViewHorizontalDegrees
        {
            get => maxViewHorizontalDegrees;
            set => maxViewHorizontalDegrees = value;
        }

        [SerializeField]
        [Tooltip("The vertical angle from the tracked target forward axis to this object will not exceed this value")]
        private float maxViewVerticalDegrees = 30f;

        /// <summary>
        /// The vertical angle from the tracked target forward axis to this object will not exceed this value.
        /// </summary>
        public float MaxViewVerticalDegrees
        {
            get => maxViewVerticalDegrees;
            set => maxViewVerticalDegrees = value;
        }

        [SerializeField]
        [Tooltip("The element will only reorient when the object is outside of the distance/direction bounds defined in this component.")]
        private bool reorientWhenOutsideParameters = true;

        /// <summary>
        /// The element will only reorient when the object is outside of the distance/direction bounds above.
        /// </summary>
        public bool ReorientWhenOutsideParameters
        {
            get => reorientWhenOutsideParameters;
            set => reorientWhenOutsideParameters = value;
        }

        [SerializeField]
        [Tooltip("The element will not reorient until the angle between the forward vector and vector to the controller is greater then this value")]
        private float orientToControllerDeadzoneDegrees = 60f;

        /// <summary>
        /// The element will not reorient until the angle between the forward vector and vector to the controller is greater then this value.
        /// </summary>
        public float OrientToControllerDeadzoneDegrees
        {
            get => orientToControllerDeadzoneDegrees;
            set => orientToControllerDeadzoneDegrees = value;
        }

        [SerializeField]
        [Tooltip("Option to ignore angle clamping")]
        private bool ignoreAngleClamp = false;

        /// <summary>
        /// Option to ignore angle clamping.
        /// </summary>
        public bool IgnoreAngleClamp
        {
            get => ignoreAngleClamp;
            set => ignoreAngleClamp = value;
        }

        [SerializeField]
        [Tooltip("Option to ignore distance clamping")]
        private bool ignoreDistanceClamp = false;

        /// <summary>
        /// Option to ignore distance clamping.
        /// </summary>
        public bool IgnoreDistanceClamp
        {
            get => ignoreDistanceClamp;
            set => ignoreDistanceClamp = value;
        }

        [SerializeField]
        [Tooltip("Option to ignore the pitch and roll of the reference target")]
        private bool ignoreReferencePitchAndRoll = false;

        /// <summary>
        /// Option to ignore the pitch and roll of the reference target
        /// </summary>
        public bool IgnoreReferencePitchAndRoll
        {
            get => ignoreReferencePitchAndRoll;
            set => ignoreReferencePitchAndRoll = value;
        }

        [SerializeField]
        [Tooltip("Pitch offset from reference element (relative to Max Distance)")]
        public float pitchOffset = 0;

        /// <summary>
        /// Pitch offset from reference element (relative to MaxDistance).
        /// </summary>
        public float PitchOffset
        {
            get => pitchOffset;
            set => pitchOffset = value;
        }

        [SerializeField]
        [Tooltip("Max vertical distance between element and reference")]
        private float verticalMaxDistance = 0.0f;

        /// <summary>
        /// Max vertical distance between element and reference.
        /// </summary>
        public float VerticalMaxDistance
        {
            get => verticalMaxDistance;
            set => verticalMaxDistance = value;
        }

        [SerializeField]
        [Tooltip("Lock the rotation to a specified number of steps around the tracked object.")]
        private bool useAngleStepping = false;

        /// <summary>
        /// Lock the rotation to a specified number of steps around the tracked object.
        /// </summary>
        public bool UseAngleStepping
        {
            get => useAngleStepping;
            set => useAngleStepping = value;
        }

        [Range(2, 24)]
        [SerializeField]
        [Tooltip("The division of steps this object can tether to. Higher the number, the more snapping steps.")]
        private int tetherAngleSteps = 6;

        /// <summary>
        /// The division of steps this object can tether to. Higher the number, the more snapping steps.
        /// </summary>
        public int TetherAngleSteps
        {
            get => tetherAngleSteps;
            set => tetherAngleSteps = Mathf.Clamp(value, 2, 24);
        }

        public void Recenter()
        {
            recenterNextUpdate = true;
        }

        private Vector3 ReferencePosition => SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.position : Vector3.zero;
        private Quaternion ReferenceRotation => SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.rotation : Quaternion.identity;
        private Vector3 PreviousReferencePosition = Vector3.zero;
        private Quaternion PreviousReferenceRotation = Quaternion.identity;
        private Quaternion PreviousGoalRotation = Quaternion.identity;
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

            bool wasClamped = false;

            // Angular clamp to determine goal direction to place the element
            Vector3 goalDirection = refForward;
            if (!ignoreAngleClamp && !recenterNextUpdate)
            {
                wasClamped |= AngularClamp(refPosition, refRotation, currentPosition, ref goalDirection);
            }

            // Distance clamp to determine goal position to place the element
            Vector3 goalPosition = currentPosition;
            if (!ignoreDistanceClamp)
            {
                wasClamped |= DistanceClamp(currentPosition, refPosition, goalDirection, wasClamped, ref goalPosition);
            }

            // Figure out goal rotation of the element based on orientation setting
            Quaternion goalRotation = Quaternion.identity;
            ComputeOrientation(goalPosition, wasClamped, ref goalRotation);

            GoalPosition = goalPosition;
            GoalRotation = goalRotation;
            PreviousGoalRotation = goalRotation;

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
        /// <returns>Angle between project from and to in degrees</returns>
        private float AngleBetweenOnPlane(Vector3 from, Vector3 to, Vector3 normal)
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
        /// <returns>Signed angle between vec and the plane described by normal</returns>
        private float AngleBetweenVectorAndPlane(Vector3 vec, Vector3 normal)
        {
            return 90 - (Mathf.Acos(Vector3.Dot(vec, normal)) * Mathf.Rad2Deg);
        }

        private float SimplifyAngle(float angle)
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

        /// <summary>
        /// This method ensures that the refForward vector remains within the bounds set by the 
        /// leashing parameters. To do this, it determines the angles between toTarget and the reference
        /// local xz and yz planes. If these angles fall within the leashing bounds, then we don't have
        /// to modify refForward. Otherwise, we apply a correction rotation to bring it within bounds.
        /// </summary>
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

        /// <summary>
        /// This method ensures that the distance from clampedPosition to the tracked target remains within 
        /// the bounds set by the leashing parameters. To do this, it clamps the current distance to these
        /// bounds and then uses this clamped distance with refForward to calculate the new position. If
        /// IgnoreReferencePitchAndRoll is true and we have a PitchOffset, we only apply these calculations
        /// for xz.
        /// </summary>
        private bool DistanceClamp(Vector3 currentPosition, Vector3 refPosition, Vector3 refForward, bool interpolateToDefaultDistance, ref Vector3 clampedPosition)
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

            // Apply vertical clamp on reference
            if (VerticalMaxDistance > 0)
            {
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, ReferencePosition.y - VerticalMaxDistance, ReferencePosition.y + VerticalMaxDistance);
            }

            return Vector3EqualEpsilon(clampedPosition, currentPosition, 0.0001f);
        }

        private void ComputeOrientation(Vector3 goalPosition, bool wasClamped, ref Quaternion orientation)
        {
            SolverOrientationType defaultOrientationType = OrientationType;

            if (!wasClamped && reorientWhenOutsideParameters)
            {
                Vector3 nodeToCamera = goalPosition - ReferencePosition;
                float angle = Mathf.Abs(AngleBetweenOnPlane(transform.forward, nodeToCamera, Vector3.up));
                if (angle < OrientToControllerDeadzoneDegrees)
                {
                    orientation = PreviousGoalRotation;
                    return;
                }
            }

            if (FaceUserDefinedTargetTransform)
            {
                Vector3 directionToTarget = TargetToFace != null ? goalPosition - TargetToFace.position : Vector3.zero;
                if (!PivotAxis.HasFlag(AxisFlags.XAxis))
                {
                    directionToTarget.x = 0;
                }
                if (!PivotAxis.HasFlag(AxisFlags.YAxis))
                {
                    directionToTarget.y = 0;
                }
                if (!PivotAxis.HasFlag(AxisFlags.ZAxis))
                {
                    directionToTarget.z = 0;
                }

                if (directionToTarget.sqrMagnitude == 0)
                {
                    orientation = Quaternion.identity;
                    return;
                }

                orientation = Quaternion.LookRotation(directionToTarget);
                return;
            }

            if (wasClamped && FaceTrackedObjectWhileClamped)
            {
                defaultOrientationType = SolverOrientationType.FaceTrackedObject;
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
                default:
                    Debug.LogError($"Invalid OrientationType for Orbital Solver on {gameObject.name}");
                    break;
            }
        }

        private void GetReferenceInfo(ref Vector3 refPosition, ref Quaternion refRotation, ref Vector3 refForward)
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
        }

        private bool Vector3EqualEpsilon(Vector3 x, Vector3 y, float eps)
        {
            float sqrMagnitude = (x - y).sqrMagnitude;

            return sqrMagnitude > eps;
        }
    }
}