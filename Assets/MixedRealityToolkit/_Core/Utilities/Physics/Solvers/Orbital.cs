// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Physics.Solvers
{
    /// <summary>
    /// Provides a solver that follows the TrackedObject/TargetTransform in an orbital motion.
    /// </summary>
    public class Orbital : Solver
    {
        public enum Orientation
        {
            /// <summary>
            /// Use the tracked object's pitch, yaw, and roll
            /// </summary>
            FollowTrackedObject,
            /// <summary>
            /// Face toward the tracked object
            /// </summary>
            FaceTrackedObject,
            /// <summary>
            /// Orient towards SolverHandler's tracked object or TargetTransform
            /// </summary>
            YawOnly,
            /// <summary>
            /// Leave the object's rotation alone
            /// </summary>
            Unmodified,
            /// <summary>
            /// Orient toward Camera.main instead of SolverHandler's properties.
            /// </summary>
            CameraFacing,
            /// <summary>
            /// Align parallel to the direction the camera is facing 
            /// </summary>
            CameraAligned,
        }

        [SerializeField]
        [Tooltip("The desired orientation of this object. Default sets the object to face the TrackedObject/TargetTransform. CameraFacing sets the object to always face the user.")]
        private Orientation orientationType = Orientation.FollowTrackedObject;

        /// <summary>
        /// The desired orientation of this object.
        /// </summary>
        /// <remarks>
        /// Default sets the object to face the TrackedObject/TargetTransform. CameraFacing sets the object to always face the user.
        /// </remarks>
        public Orientation OrientationType
        {
            get { return orientationType; }
            set { orientationType = value; }
        }

        [SerializeField]
        [Tooltip("XYZ offset for this object in relation to the TrackedObject/TargetTransform. Mixing local and world offsets is not recommended.")]
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
        [Tooltip("XYZ offset for this object in worldspace, best used with the YawOnly orientationType. Mixing local and world offsets is not recommended.")]
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
        [Tooltip("Lock the rotation to a specified number of steps around the tracked object.")]
        private bool useAngleSteppingForWorldOffset = false;

        /// <summary>
        /// Lock the rotation to a specified number of steps around the tracked object.
        /// </summary>
        public bool UseAngleSteppingForWorldOffset
        {
            get { return useAngleSteppingForWorldOffset; }
            set { useAngleSteppingForWorldOffset = value; }
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
                if (value < 2)
                {
                    tetherAngleSteps = 2;
                }
                else if (value > 24)
                {
                    tetherAngleSteps = 24;
                }
                else
                {
                    tetherAngleSteps = value;
                }
            }
        }

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

            UpdateWorkingPositionToGoal();
            UpdateWorkingRotationToGoal();
        }


        private Quaternion SnapToTetherAngleSteps(Quaternion rotationToSnap)
        {
            if (!UseAngleSteppingForWorldOffset)
            {
                return rotationToSnap;
            }

            float stepAngle = 360f / tetherAngleSteps;
            int numberOfSteps = Mathf.RoundToInt(SolverHandler.TransformTarget.transform.localEulerAngles.y / stepAngle);

            float newAngle = stepAngle * numberOfSteps;

            return Quaternion.Euler(rotationToSnap.eulerAngles.x, newAngle, rotationToSnap.eulerAngles.z);
        }

        private Quaternion CalculateDesiredRotation(Vector3 desiredPos)
        {
            Quaternion desiredRot = Quaternion.identity;

            switch (orientationType)
            {
                case Orientation.YawOnly:
                    float targetYRotation;

                    if (SolverHandler.TransformTarget != null)
                    {
                        targetYRotation = SolverHandler.TransformTarget.eulerAngles.y;
                    }
                    else
                    {
                        targetYRotation = 1;
                    }

                    desiredRot = Quaternion.Euler(0f, targetYRotation, 0f);
                    break;
                case Orientation.Unmodified:
                    desiredRot = transform.rotation;
                    break;
                case Orientation.CameraAligned:
                    desiredRot = CameraCache.Main.transform.rotation;
                    break;
                case Orientation.FaceTrackedObject:
                    desiredRot = Quaternion.LookRotation(SolverHandler.TransformTarget.position - desiredPos);
                    break;
                case Orientation.CameraFacing:
                    desiredRot = Quaternion.LookRotation(CameraCache.Main.transform.position - desiredPos);
                    break;
                case Orientation.FollowTrackedObject:
                    desiredRot = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.rotation : Quaternion.identity;
                    break;
                default:
                    Debug.LogError($"Invalid OrientationType for Orbital Solver on {gameObject.name}");
                    break;
            }

            if (UseAngleSteppingForWorldOffset)
            {
                desiredRot = SnapToTetherAngleSteps(desiredRot);
            }

            return desiredRot;
        }
    }
}