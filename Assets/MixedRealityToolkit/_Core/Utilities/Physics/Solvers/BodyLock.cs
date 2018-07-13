// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Physics.Solvers
{
    /// <summary>
    /// This component provides a solver that follows the TrackedObject/TargetTransform. Adjusting "LerpTime"
    /// properties changes how quickly the object moves to the TrackedObject/TargetTransform's position.
    /// </summary>
    public class BodyLock : Solver
    {
        private enum OrientationType
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

        [SerializeField]
        [Tooltip("The desired orientation of this object. Default sets the object to face the TrackedObject/TargetTransform. CameraFacing sets the object to always face the user.")]
        private OrientationType orientation = OrientationType.Default;

        [SerializeField]
        [Tooltip("XYZ offset for this object in relation to the TrackedObject/TargetTransform")]
        private Vector3 offset;

        [SerializeField]
        [Tooltip("RotationTether snaps the object to be in front of TrackedObject regardless of the object's local rotation.")]
        private bool rotationTether = false;

        [Range(4, 12)]
        [SerializeField]
        [Tooltip("TetherAngleSteps is the division of steps this object can tether to. Higher the number, the more snapple steps.")]
        private int tetherAngleSteps = 6;
        private Transform OrientationReference => orientation == OrientationType.CameraFacing ? CameraCache.Main.transform : SolverHandler.TransformTarget;

        public override void SolverUpdate()
        {
            Quaternion goalRotation = Quaternion.identity;

            if (rotationTether)
            {
                float targetYRotation = OrientationReference.eulerAngles.y;
                float tetherYRotation = goalRotation.eulerAngles.y;
                float deltaAngle = Mathf.DeltaAngle(targetYRotation, tetherYRotation);
                float tetherAngleLimit = 360f / tetherAngleSteps;

                if (Mathf.Abs(deltaAngle) > tetherAngleLimit)
                {
                    int steps = Mathf.RoundToInt(targetYRotation / tetherAngleLimit);
                    tetherYRotation = steps * tetherAngleLimit;
                }

                goalRotation = Quaternion.Euler(0f, tetherYRotation, 0f);
            }

            Vector3 goalPosition = SolverHandler.TransformTarget != null ? SolverHandler.TransformTarget.position + (goalRotation * offset) : Vector3.zero;

            GoalPosition = goalPosition;
            GoalRotation = goalRotation;

            UpdateWorkingPosToGoal();
            UpdateWorkingRotToGoal();
        }
    }
}