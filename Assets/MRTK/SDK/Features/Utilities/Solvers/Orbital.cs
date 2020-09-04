// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// Provides a solver that follows the TrackedObject/TargetTransform in an orbital motion.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/Orbital")]
    public class Orbital : Solver
    {
        [SerializeField]
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
        [Tooltip("XYZ offset for this object oriented with the TrackedObject/TargetTransform's forward. Mixing local and world offsets is not recommended. Local offsets are applied before world offsets.")]
        private Vector3 localOffset = new Vector3(0, -1, 1);

        /// <summary>
        /// XYZ offset for this object in relation to the TrackedObject/TargetTransform.
        /// </summary>
        /// <remarks>
        /// Mixing local and world offsets is not recommended.
        /// </remarks>
        public Vector3 LocalOffset
        {
            get { return localOffset; }
            set { localOffset = value; }
        }

        [SerializeField]
        [Tooltip("XYZ offset for this object in worldspace, best used with the YawOnly orientationType. Mixing local and world offsets is not recommended. Local offsets are applied before world offsets.")]
        private Vector3 worldOffset = Vector3.zero;

        /// <summary>
        /// XYZ offset for this object in worldspace, best used with the YawOnly orientationType.
        /// </summary>
        /// <remarks>
        /// Mixing local and world offsets is not recommended.
        /// </remarks>
        public Vector3 WorldOffset
        {
            get { return worldOffset; }
            set { worldOffset = value; }
        }

        [SerializeField]
        [FormerlySerializedAs(oldName: "useAngleSteppingForWorldOffset")]
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
        [Tooltip("The division of steps this object can tether to. Higher the number, the more snapping steps.")]
        private int tetherAngleSteps = 6;

        /// <summary>
        /// The division of steps this object can tether to. Higher the number, the more snapping steps.
        /// </summary>
        public int TetherAngleSteps
        {
            get { return tetherAngleSteps; }
            set
            {
                tetherAngleSteps = Mathf.Clamp(value, 2, 24);
            }
        }

        /// <inheritdoc />
        public override void SolverUpdate()
        {
            Vector3 desiredPos = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.position : Vector3.zero;

            Quaternion targetRot = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.rotation : Quaternion.Euler(0, 1, 0);
            Quaternion yawOnlyRot = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);
            desiredPos = desiredPos + (SnapToTetherAngleSteps(targetRot) * LocalOffset);
            desiredPos = desiredPos + (SnapToTetherAngleSteps(yawOnlyRot) * WorldOffset);

            Quaternion desiredRot = CalculateDesiredRotation(desiredPos);

            GoalPosition = desiredPos;
            GoalRotation = desiredRot;
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