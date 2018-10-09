// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// SolverOrbital provides a solver that offsets from the TrackedObject/TargetTransform. Adjusting "LerpTime"
    /// properties changes how quickly the object moves to the TrackedObject/TargetTransform's position.
    /// </summary>
    public class SolverOrbital : Solver
    {
        public enum OrientationReferenceEnum
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

        #region public members
        [Tooltip("The desired orientation of this object. Default sets the object to face the TrackedObject/TargetTransform. CameraFacing sets the object to always face the user.")]
        [SerializeField]
        private OrientationReferenceEnum orientation = OrientationReferenceEnum.FollowTrackedObject;

        /// <summary>
        /// The desired orientation of this object. Default sets the object to face the TrackedObject/TargetTransform. CameraFacing sets the object to always face the user.
        /// </summary>
        public OrientationReferenceEnum Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        [Tooltip("XYZ offset for this object in relation to the TrackedObject/TargetTransform. Mixing local and world offsets is not recommended.")]
        [SerializeField]
        private Vector3 localOffset = new Vector3(0,-1,1);

        /// <summary>
        /// XYZ offset for this object in relation to the TrackedObject/TargetTransform. Mixing local and world offsets is not recommended.
        /// </summary>
        public Vector3 LocalOffset
        {
            get { return localOffset; }
            set { localOffset = value; }
        }

        [Tooltip("XYZ offset for this object in worldspace, best used with the YawOnly orientation. Mixing local and world offsets is not recommended.")]
        [SerializeField]
        private Vector3 worldOffset = Vector3.zero;

        /// <summary>
        /// XYZ offset for this object in worldspace, best used with the YawOnly orientation. Mixing local and world offsets is not recommended.
        /// </summary>
        public Vector3 WorldOffset
        {
            get { return worldOffset; }
            set { worldOffset = value; }
        }

        [Tooltip("Lock the rotation to a specified number of steps around the tracked object.")]
        [SerializeField]
        private bool useAngleSteppingForWorldOffset = false;

        /// <summary>
        /// Lock the rotation to a specified number of steps around the tracked object.
        /// </summary>
        public bool UseAngleSteppingForWorldOffset
        {
            get { return useAngleSteppingForWorldOffset; }
            set { useAngleSteppingForWorldOffset = value; }
        }

        [Tooltip("TetherAngleSteps is the division of steps this object can tether to. Higher the number, the more snapple steps.")]
        [Range(2, 24)]
        [SerializeField]
        private int tetherAngleSteps = 6;
        /// <summary>
        /// TetherAngleSteps is the division of steps this object can tether to. Higher the number, the more snapple steps.
        /// </summary>
        public int TetherAngleSteps
        {
            get { return tetherAngleSteps;}
            set 
            {
                if (value > 24 || value < 2)
                {
                    Debug.LogError("TetherAngleSteps was set to a size larger than it's max range. Clamping to a valid value.");
                }
                tetherAngleSteps = Mathf.Clamp(value, 2, 24);
            }
        }
        #endregion

        public override void SolverUpdate()
        {
            Vector3 desiredPos = solverHandler.TransformTarget != null ? solverHandler.TransformTarget.position : Vector3.zero;

            Quaternion targetRot = solverHandler.TransformTarget != null ? solverHandler.TransformTarget.rotation : Quaternion.Euler(0, 1, 0);
            Quaternion yawOnlyRot = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);
            desiredPos = desiredPos + (SnapToTetherAngleSteps(targetRot) * LocalOffset);
            desiredPos = desiredPos + (SnapToTetherAngleSteps(yawOnlyRot) * WorldOffset);

            Quaternion desiredRot = CalculateDesiredRotation(desiredPos);
            
            GoalPosition = desiredPos;
            GoalRotation = desiredRot;

            UpdateWorkingPosToGoal();
            UpdateWorkingRotToGoal();
        }


        private Quaternion SnapToTetherAngleSteps(Quaternion RotationToSnap)
        {
            if (!UseAngleSteppingForWorldOffset)
            {
                return RotationToSnap;
            }

            float stepAngle = 360f / TetherAngleSteps;
            int numberOfSteps = Mathf.RoundToInt(solverHandler.TransformTarget.transform.localEulerAngles.y / stepAngle);

            float newAngle = stepAngle * numberOfSteps;

            return Quaternion.Euler(RotationToSnap.eulerAngles.x, newAngle, RotationToSnap.eulerAngles.z);
        }

        private Quaternion CalculateDesiredRotation( Vector3 desiredPos )
        {
            Quaternion desiredRot;

            switch (orientation)
            {
                case OrientationReferenceEnum.YawOnly:
                    float targetYRotation;
                    if (solverHandler.TransformTarget != null)
                    {
                        targetYRotation = solverHandler.TransformTarget.eulerAngles.y;
                    }
                    else
                    {
                        targetYRotation = 1;
                    }
                    desiredRot = Quaternion.Euler(0f, targetYRotation, 0f);
                    break;
                case OrientationReferenceEnum.Unmodified:
                    desiredRot = transform.rotation;
                    break;
                case OrientationReferenceEnum.CameraAligned:
                    desiredRot = CameraCache.Main.transform.rotation;
                    break;
                case OrientationReferenceEnum.FaceTrackedObject:
                    desiredRot = Quaternion.LookRotation(solverHandler.TransformTarget.position - desiredPos);
                    break;
                case OrientationReferenceEnum.CameraFacing:
                    desiredRot = Quaternion.LookRotation(CameraCache.Main.transform.position - desiredPos);
                    break;
                default:
                case OrientationReferenceEnum.FollowTrackedObject:
                    if (solverHandler.TransformTarget != null)
                    {
                        desiredRot = solverHandler.TransformTarget.rotation;
                    }
                    else
                    {
                        desiredRot = Quaternion.identity;
                    }
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