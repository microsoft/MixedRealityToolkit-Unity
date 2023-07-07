// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Input.Simulation
{
    /// <summary>
    /// Settings used to configure the eye gaze simulation.
    /// </summary>
    [Serializable]
    public class EyeGazeSimulationSettings
    {
        /// <summary>
        /// Indicates the current state of the latched tracking toggle for the
        /// simulated eye gaze device.
        /// </summary>
        internal bool ToggleState { get; set; } = false;

        [SerializeField]
        [Tooltip("Indicates whether or not eye gaze is to be simulated.")]
        private bool simulationEnabled = true;

        /// <summary>
        /// Indicates whether or not eye gaze is to be simulated.
        /// </summary>
        public bool SimulationEnabled
        {
            get => simulationEnabled;
            set => simulationEnabled = value;
        }

        [SerializeField]
        [Tooltip("Indicates whether or not eyes are currently being tracked.")]
        private bool isTracked = true;

        /// <summary>
        /// Indicates whether or not eyes are currently being tracked.
        /// </summary>
        public bool IsTracked
        {
            get => isTracked;
            set => isTracked = value;
        }

        [SerializeField]
        [Tooltip("Offset, from the HMD origin, at which eye gaze originates.")]
        private Vector3 eyeOriginOffset = new Vector3(0f, -0.2f, 0f);

        /// <summary>
        /// Offset, from the HMD origin, at which eye gaze originates.
        /// </summary>
        public Vector3 EyeOriginOffset
        {
            get => eyeOriginOffset;
            set => eyeOriginOffset = value;
        }

        /* todo: needed?
        [SerializeField]
        [Tooltip("Should eye gaze look be smoothed?")]
        private bool isLookSmoothed = true;

        /// <summary>
        /// Should eye gaze look be smoothed?
        /// </summary>
        /// <remarks>
        /// Enabling smoothing can result in the eye gaze 'gliding' to a stop
        /// when look controls are released.
        /// </remarks>
        public bool IsLookSmoothed
        {
            get => isLookSmoothed;
            set => isLookSmoothed = value;
        }
        */

        [SerializeField]
        [Tooltip("The input action to use to look along the horizontal axis.")]
        private InputActionReference lookHorizontal;

        /// <summary>
        /// The input action to use to look along the horizontal axis.
        /// </summary>
        public InputActionReference LookHorizontal
        {
            get => lookHorizontal;
            set => lookHorizontal = value;
        }

        [SerializeField]
        [Tooltip("The input action to use to look along the vertical axis.")]
        private InputActionReference lookVertical;

        /// <summary>
        /// The input action to use to look along the vertical axis.
        /// </summary>
        public InputActionReference LookVertical
        {
            get => lookVertical;
            set => lookVertical = value;
        }

        /// <summary>
        /// The lowest sensitivity for eye tracking simulation.
        /// </summary>
        public const float MinimumSensitivity = 0.1f;

        /// <summary>
        /// The highest sensitivity for eye tracking simulation.
        /// </summary>
        public const float MaximumSensitivity = 5f;

        [SerializeField]
        [Tooltip("Multiplier applied to control the sensitivity of the simulated eye gaze.")]
        [Range(MinimumSensitivity, MaximumSensitivity)]
        private float sensitivity = 1.0f;

        /// <summary>
        /// Multiplier applied to control the sensitivity of the simulated eye gaze.
        /// </summary>
        public float Sensitivity
        {
            get => sensitivity;
            set
            {
                if ((value < MinimumSensitivity) ||
                    (value > MaximumSensitivity))
                {
                    Debug.LogWarning($"{nameof(Sensitivity)} out of range, altering to be {MinimumSensitivity} <= value <= {MaximumSensitivity}.");
                }

                sensitivity = Mathf.Clamp(value, MinimumSensitivity, MaximumSensitivity);
            }
        }
    }
}
