// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Input.Simulation
{
    /// <summary>
    /// Class containing the camera simulation settings.
    /// </summary>
    [Serializable]
    public class CameraSimulationSettings
    {
        [SerializeField]
        [Tooltip("Indicates whether or not the camera is to be simulated.")]
        private bool simulationEnabled = true;

        /// <summary>
        /// Indicates whether or not the camera is to be simulated.
        /// </summary>
        public bool SimulationEnabled
        {
            get => simulationEnabled;
            set => simulationEnabled = value;
        }

        [SerializeField]
        [Tooltip("Optional offset to apply to the camera during simulation.")]
        private Vector3 originOffset = Vector3.zero;

        /// <summary>
        /// Optional offset to apply to the camera during simulation.
        /// </summary>
        public Vector3 OriginOffset
        {
            get => originOffset;
            set => originOffset = value;
        }

        #region Move controls

        [SerializeField]
        [Tooltip("Should camera movement be smoothed?")]
        private bool isMovementSmoothed = true;

        /// <summary>
        /// Should camera movement be smoothed?
        /// </summary>
        /// <remarks>
        /// Enabling smoothing can result in the camera 'gliding' to a stop
        /// when movement controls are released.
        /// </remarks>
        public bool IsMovementSmoothed
        {
            get => isMovementSmoothed;
            set => isMovementSmoothed = value;
        }

        [SerializeField]
        [Tooltip("The input action used to move the camera along the depth axis.")]
        private InputActionReference moveDepth;

        /// <summary>
        /// The input action used to move the camera along the depth axis.
        /// </summary>
        public InputActionReference MoveDepth
        {
            get => moveDepth;
            set => moveDepth = value;
        }

        [SerializeField]
        [Tooltip("The input action used to move the camera along the horizontal axis.")]
        private InputActionReference moveHorizontal;

        /// <summary>
        /// The input action used to move the camera along the horizontal axis.
        /// </summary>
        public InputActionReference MoveHorizontal
        {
            get => moveHorizontal;
            set => moveHorizontal = value;
        }

        [SerializeField]
        [Tooltip("The input action used to move the camera along the vertical axis.")]
        private InputActionReference moveVertical;

        /// <summary>
        /// The input action used to move the camera along the vertical axis.
        /// </summary>
        public InputActionReference MoveVertical
        {
            get => moveVertical;
            set => moveVertical = value;
        }

        /// <summary>
        /// The smallest multiplier that can be applied to control the speed of camera movement.
        /// </summary>
        public const float MinimumMoveSpeed = 0.1f;

        /// <summary>
        /// The largest multiplier that can be applied to control the speed of camera movement.
        /// </summary>
        public const float MaximumMoveSpeed = 10f;

        [SerializeField]
        [Tooltip("Multiplier applied to control the speed of the simulated camera movement.")]
        [Range(MinimumMoveSpeed, MaximumMoveSpeed)]
        private float moveSpeed = 1.0f;

        /// <summary>
        /// Multiplier applied to control the speed of the simulated camera movement.
        /// </summary>
        public float MoveSpeed
        {
            get => moveSpeed;
            set
            {
                if ((value < MinimumMoveSpeed) ||
                    (value > MaximumMoveSpeed))
                {
                    Debug.LogWarning($"{nameof(MoveSpeed)} out of range, altering the provided value.");
                }

                moveSpeed = Mathf.Clamp(value, MinimumMoveSpeed, MaximumMoveSpeed);
            }
        }

        #endregion Move controls

        #region Rotate controls

        /// <summary>
        /// The lowest sensitivity for the simulated head rotation.
        /// </summary>
        public const float MinimumRotationSensitivity = 0.1f;

        /// <summary>
        /// The highest sensitivity for the simulated head rotation.
        /// </summary>
        public const float MaximumRotationSensitivity = 5f;

        [SerializeField]
        [Tooltip("Multiplier applied to control the sensitivity of the simulated head rotation.")]
        [Range(MinimumRotationSensitivity, MaximumRotationSensitivity)]
        private float rotationSensitivity = 1.0f;

        /// <summary>
        /// Multiplier applied to control the sensitivity of the simulated head rotation.
        /// </summary>
        public float RotationSensitivity
        {
            get => rotationSensitivity;
            set
            {
                if ((value < MinimumRotationSensitivity) ||
                    (value > MaximumRotationSensitivity))
                {
                    Debug.LogWarning($"{nameof(RotationSensitivity)} out of range, altering the provided value.");
                }

                rotationSensitivity = Mathf.Clamp(value, MinimumRotationSensitivity, MaximumRotationSensitivity);
            }
        }

        [SerializeField]
        [Tooltip("The input action used to simulate rotating the user's head around the depth axis.")]
        private InputActionReference roll;

        /// <summary>
        /// The input action used to simulate rotating the user's head to rotate around the depth axis.
        /// </summary>
        public InputActionReference Roll
        {
            get => roll;
            set => roll = value;
        }

        [SerializeField]
        [Tooltip("The input action used to simulate rotating the user's head around the horizontal axis.")]
        private InputActionReference pitch;

        /// <summary>
        /// The input action used to simulate rotating the user's head to rotate around the horizontal axis.
        /// </summary>
        public InputActionReference Pitch
        {
            get => pitch;
            set => pitch = value;
        }

        [SerializeField]
        [Tooltip("The input action used to simulate rotating the user's head around the vertical axis.")]
        private InputActionReference yaw;

        /// <summary>
        /// The input action used to simulate rotating the user's head around the vertical axis.
        /// </summary>
        public InputActionReference Yaw
        {
            get => yaw;
            set => yaw = value;
        }

        [SerializeField]
        [Tooltip("Indicates whether or not the direction of the pitch is to be inverted.")]
        private bool invertPitch;

        /// <summary>
        /// Indicates whether or not the direction of the pitch is to be inverted.
        /// </summary>
        public bool InvertPitch
        {
            get => invertPitch;
            set => invertPitch = value;
        }

        #endregion Rotate controls
    }
}
