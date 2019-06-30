// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// Provides a solver that follows the TrackedObject/TargetTransform in an orbital motion.
    /// </summary>
    public class Orbital2 : Solver
    {
        [SerializeField]
        [Tooltip("The desired object reference ")]
        private ReferenceObjectType referenceObjectType = default;

        [SerializeField]
        [Tooltip("Tracked object to calculate facing direction for. If you want to manually override and use a scene object, use the FaceTarget field.")]
        private TrackedObjectType trackedObjectToFace = default;

        /// <summary>
        /// Tracked object to calculate position and orientation from. If you want to manually override and use a scene object, use the TransformTarget field.
        /// </summary>
        public TrackedObjectType TrackedObjectToFace
        {
            get { return trackedObjectToFace; }
            set { trackedObjectToFace = value; }
        }

        [SerializeField]
        [Tooltip("Manual override for FacedObjectToReference if you want to use a scene object. Leave empty if you want to use head, motion-tracked controllers, or motion-tracked hands.")]
        private Transform faceTarget;

        [SerializeField, HideInInspector]
        [Tooltip("The desired orientation of this object. Default sets the object to face the TrackedObject/TargetTransform. CameraFacing sets the object to always face the user.")]
        private SolverOrientationType orientationType = SolverOrientationType.FollowTrackedObject;

        /// <summary>
        /// The desired orientation of this object.
        /// </summary>
        /// <remarks>
        /// Default sets the object to face the TrackedObject/TargetTransform. CameraFacing sets the object to always face the user.
        /// </remarks>
        public SolverOrientationType OrientationType
        {
            get { return orientationType; }
            set { orientationType = value; }
        }


        [SerializeField]
        [Tooltip("The constraint on the view rotation")]
        private PivotAxis pivotAxis = PivotAxis.Free;

        /// <summary>
        /// The desired axis constraint.
        /// </summary>
        /// <remarks>
        /// Default leaves the object capable of facing the target on all axis.
        /// </remarks>
        public PivotAxis PivotAxis
        {
            get { return pivotAxis; }
            set { pivotAxis = value; }
        }

        [SerializeField]
        [Tooltip("The space in which the XYZ offset is used")]
        private TransformationSpaceType offsetSpace = default;

        /// <summary>
        /// The space in which the XYZ offset is used
        /// </summary>
        public TransformationSpaceType OffsetSpacorbitale
        {
            get { return offsetSpace; }
            set { offsetSpace = value; }
        }

        [SerializeField]
        [Tooltip("XYZ offset for this object in relation to the TrackedObject/TargetTransform")]
        private Vector3 offset = default;

        /// <summary>
        /// XYZ offset for this object in relation to the TrackedObject/TargetTransform.
        /// </summary>
        public Vector3 Offset
        {
            get { return offset; }
            set { offset = value; }
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

        public override void SolverUpdate()
        {
            Vector3 desiredPos = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.position : Vector3.zero;

            Quaternion targetRot;
            switch (referenceObjectType)
            {
                case ReferenceObjectType.TrackedObject:
                    targetRot = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.rotation : Quaternion.Euler(0, 1, 0);
                    break;
                case ReferenceObjectType.BodyPart:
                    switch (trackedObjectToFace)
                    {
                        case TrackedObjectType.Head:
                            if (!)
                            TrackTransform(CameraCache.Main.transform);
                            break;
                        case TrackedObjectType.MotionControllerLeft:
                            Handedness = Handedness.Left;
                            break;
                        case TrackedObjectType.MotionControllerRight:
                            Handedness = Handedness.Right;
                            break;
                        case TrackedObjectType.HandJointLeft:
                            // Set to None, so the underlying ControllerFinder doesn't attach to a controller.
                            // TODO: Make this more generic / configurable for hands vs controllers. Also resolve the duplicate Handedness variables.
                            Handedness = Handedness.None;
                            TrackTransform(RequestEnableHandJoint(Handedness.Left));
                            break;
                        case TrackedObjectType.HandJointRight:
                            Handedness = Handedness.None;
                            TrackTransform(RequestEnableHandJoint(Handedness.Right));
                            break;
                    }
            }

            Quaternion targetRot = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.rotation : Quaternion.Euler(0, 1, 0);
            Quaternion yawOnlyRot = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);
            desiredPos = desiredPos + (SnapToTetherAngleSteps(offsetSpace == TransformationSpaceType.LocalSpace ? targetRot : yawOnlyRot) * Offset);

            Quaternion desiredRot = CalculateDesiredRotation(desiredPos);

            GoalPosition = desiredPos;
            GoalRotation = desiredRot;

            UpdateWorkingPositionToGoal();
            UpdateWorkingRotationToGoal();
        }


        private Quaternion SnapToTetherAngleSteps(Quaternion rotationToSnap)
        {
            if (!UseAngleStepping || SolverHandler.TransformTarget == null)
            {
                return rotationToSnap;
            }

            float stepAngle = 360f / tetherAngleSteps;
            int numberOfSteps = Mathf.RoundToInt(SolverHandler.TransformTarget.transform.eulerAngles.y / stepAngle);

            float newAngle = stepAngle * numberOfSteps;

            return Quaternion.Euler(rotationToSnap.eulerAngles.x, newAngle, rotationToSnap.eulerAngles.z);
        }

        private Quaternion CalculateDesiredRotation(Vector3 desiredPos)
        {
            Quaternion desiredRot = Quaternion.identity;

            switch (orientationType)
            {
                case SolverOrientationType.YawOnly:
                    float targetYRotation = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.eulerAngles.y : 0.0f;
                    desiredRot = Quaternion.Euler(0f, targetYRotation, 0f);
                    break;
                case SolverOrientationType.Unmodified:
                    desiredRot = transform.rotation;
                    break;
                case SolverOrientationType.CameraAligned:
                    desiredRot = CameraCache.Main.transform.rotation;
                    break;
                case SolverOrientationType.FaceTrackedObject:
                    desiredRot = SolverHandler.TransformTarget != null ? Quaternion.LookRotation(SolverHandler.TransformTarget.position - desiredPos) : Quaternion.identity;
                    break;
                case SolverOrientationType.CameraFacing:
                    desiredRot = SolverHandler.TransformTarget != null ? Quaternion.LookRotation(CameraCache.Main.transform.position - desiredPos) : Quaternion.identity;
                    break;
                case SolverOrientationType.FollowTrackedObject:
                    desiredRot = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.rotation : Quaternion.identity;
                    break;
                default:
                    Debug.LogError($"Invalid OrientationType for Orbital Solver on {gameObject.name}");
                    break;
            }

            if (UseAngleStepping)
            {
                desiredRot = SnapToTetherAngleSteps(desiredRot);
            }

            return desiredRot;
        }
    }
}