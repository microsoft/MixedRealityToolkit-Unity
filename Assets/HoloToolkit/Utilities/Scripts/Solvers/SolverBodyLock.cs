//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HoloToolkit.Unity
{
    public class SolverBodyLock : Solver
    {
        #region public enums
        [HideInInspector]
        public enum OrrientationReference
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
        public OrrientationReference Orrientation = OrrientationReference.Default;
        public Vector3 offset;
        public bool RotationTether = false;
        [Range(4, 12)]
        public int TetherAngleSteps = 6;
        #endregion

        public override void SolverUpdate()
        {
            Vector3 desiredPos = base.solverHandler.TransformTarget != null ? base.solverHandler.TransformTarget.position + offset : Vector3.zero;
            Quaternion desiredRot = Quaternion.identity;

            if (RotationTether)
            {
                float targetYRotation = GetOrreintationRef().eulerAngles.y;
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

        private Transform GetOrreintationRef()
        {
            if(Orrientation == OrrientationReference.CameraFacing)
            {
                return Camera.main.transform;
            }
            return solverHandler.TransformTarget;
        }
    }
}