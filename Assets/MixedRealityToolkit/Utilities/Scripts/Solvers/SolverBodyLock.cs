// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using UnityEngine;

namespace MixedRealityToolkit.Utilities.Solvers
{
    /// <summary>
    /// SolverBodyLock provides a solver that follows the TrackedObject/TargetTransform. Adjusting "LerpTime"
    /// properties changes how quickly the object moves to the TracketObject/TargetTransform's position.
    /// </summary>
    public class SolverBodyLock : Solver
    {
        #region public enums
        public enum OrientationReference
        {
            /// <summary>
            /// Orient towards SolverHandler's tracked object or TargetTransform
            /// </summary>
            Default,
            /// <summary>
            /// Orient toward Camera.main instead of SolverHandler's properties.
            /// </summary>
            CameraFacing,
        }
        #endregion


        #region public members
        [Tooltip("The desired orientation of this object. Default sets the object to face the TrackedObject/TargetTransform. CameraFacing sets the object to always face the user.")]
        public OrientationReference Orientation = OrientationReference.Default;
        [Tooltip("XYZ offset for this object in relation to the TrackedObject/TargetTransform")]
        public Vector3 offset;
        [Tooltip("RotationTether snaps the object to be in front of TrackedObject regardless of the object's local rotation.")]
        public bool RotationTether = false;
        [Tooltip("TetherAngleSteps is the divison of steps this object can tether to. Higher the number, the more snapple steps.")]
        [Range(4, 12)]
        public int TetherAngleSteps = 6;
        #endregion

        public override void SolverUpdate()
        {
            Vector3 desiredPos = base.solverHandler.TransformTarget != null ? base.solverHandler.TransformTarget.position + offset : Vector3.zero;
            Quaternion desiredRot = Quaternion.identity;

            if (RotationTether)
            {
                float targetYRotation = GetOrientationRef().eulerAngles.y;
                float tetherYRotation = desiredRot.eulerAngles.y;
                float angleDiff = Mathf.DeltaAngle(targetYRotation, tetherYRotation);
                float tetherAngleLimit = 360f / TetherAngleSteps;

                if (Mathf.Abs(angleDiff) > tetherAngleLimit)
                {
                    int numSteps = Mathf.RoundToInt(targetYRotation / tetherAngleLimit);
                    tetherYRotation = numSteps * tetherAngleLimit;
                }

                desiredRot = Quaternion.Euler(0f, tetherYRotation, 0f);
            }

            desiredPos = solverHandler.TransformTarget.position + (desiredRot * offset);

            this.GoalPosition = desiredPos;
            this.GoalRotation = desiredRot;

            UpdateWorkingPosToGoal();
            UpdateWorkingRotToGoal();
        }

        private Transform GetOrientationRef()
        {
            if (Orientation == OrientationReference.CameraFacing)
            {
                return CameraCache.Main.transform;
            }
            return solverHandler.TransformTarget;
        }
    }
}