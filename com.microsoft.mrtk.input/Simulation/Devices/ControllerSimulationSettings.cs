// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Microsoft.MixedReality.Toolkit.Input.GestureTypes;

namespace Microsoft.MixedReality.Toolkit.Input.Simulation
{
    /// <summary>
    /// Setting that control simulation settings for motion controls, including hands.
    /// </summary>
    [Serializable]
    public class ControllerSimulationSettings
    {
        /// <summary>
        /// Indicates the current state of the latched tracking toggle for the
        /// simulated controller.
        /// </summary>
        internal bool ToggleState { get; set; } = false;

        [SerializeField]
        [Tooltip("Indicates how the controller is to be simulated.")]
        private ControllerSimulationMode simulationMode = ControllerSimulationMode.ArticulatedHand;

        /// <summary>
        /// Indicates how the controller is to be simulated.
        /// </summary>
        public ControllerSimulationMode SimulationMode
        {
            get => simulationMode;
            set => simulationMode = value;
        }

        [SerializeField]
        [Tooltip("Indicates how the controller is to be anchored.")]
        private ControllerAnchorPoint anchorPoint = ControllerAnchorPoint.Device;

        /// <summary>
        /// Indicates how the controller is to be simulated.
        /// </summary>
        public ControllerAnchorPoint AnchorPoint
        {
            get => anchorPoint;
            set => anchorPoint = value;
        }

        [SerializeField]
        [Tooltip("The input action used to indicate that the hand's neutral pose should be changed.")]
        private InputActionReference changeNeutralPose;

        /// <summary>
        /// The input action used to indicate that the hand's neutral pose should be changed.
        /// </summary>
        public InputActionReference ChangeNeutralPose
        {
            get => changeNeutralPose;
            set => changeNeutralPose = value;
        }

        [SerializeField]
        [Tooltip("The initial gesture of the simulated hand")]
        private GestureId defaultGesture = GestureId.Open;

        /// <summary>
        /// The initial gesture of the simulated hand.
        /// </summary>
        public GestureId DefaultGesture
        {
            get => defaultGesture;
            set => defaultGesture = value;
        }

        [SerializeField]
        [Tooltip("The gesture of the simulated hand when the 'trigger' button is pressed")]
        private GestureId triggerGesture = GestureId.Pinch;

        /// <summary>
        /// The gesture of the simulated hand during a select action.
        /// </summary>
        public GestureId TriggerGesture
        {
            get => triggerGesture;
            set => triggerGesture = value;
        }

        [SerializeField]
        [Tooltip("The alternative initial Gesture of the simulated hand")]
        private GestureId secondaryDefaultGesture = GestureId.Flat;

        /// <summary>
        /// The alternative initial gesture of the simulated hand.
        /// </summary>
        public GestureId SecondaryDefaultGesture
        {
            get => secondaryDefaultGesture;
            set => secondaryDefaultGesture = value;
        }

        [SerializeField]
        [Tooltip("The alternative gesture of the simulated hand during a select action")]
        private GestureId secondaryTriggerGesture = GestureId.ThumbsUp;

        /// <summary>
        /// The alternative gesture of the simulated hand during a select action.
        /// </summary>
        public GestureId SecondaryTriggerGesture
        {
            get => secondaryTriggerGesture;
            set => secondaryTriggerGesture = value;
        }


        [SerializeField]
        [Tooltip("The initial position, relative to the camera, at which the controller will appear (when tracking is toggled).")]
        private Vector3 defaultPosition = new Vector3(0f, 0f, 0.5f);

        /// <summary>
        /// The initial position, relative to the camera, at which the controller will appear.
        /// </summary>
        public Vector3 DefaultPosition
        {
            get => defaultPosition;
            set => defaultPosition = value;
        }

        [SerializeField]
        [Tooltip("The input action used to indicate that the controller should be tracked.")]
        private InputActionReference track;

        /// <summary>
        /// The input action used to indicate that the controller should be tracked.
        /// </summary>
        public InputActionReference Track
        {
            get => track;
            set => track = value;
        }

        [SerializeField]
        [Tooltip("The input action used to toggle the tracking state of the controller (if toggled on, this overrides the Track property).")]
        private InputActionReference toggle;

        /// <summary>
        /// The input action used to toggle the tracking state of the controller
        /// (if toggled on, this overrides the <see cref="Track"/> property).
        /// </summary>
        public InputActionReference Toggle
        {
            get => toggle;
            set => toggle = value;
        }

        /// <summary>
        /// Minimum sensitivity multiplier for controller depth movement.
        /// </summary>
        public const float MinimumDepthSensitivity = 0.1f;

        /// <summary>
        /// Maximum sensitivity multiplier for controller depth movement.
        /// </summary>
        public const float MaximumDepthSensitivity = 2f;

        [SerializeField]
        [Tooltip("Multiplier controlling the sensitivity of controller depth movement.")]
        [Range(MinimumDepthSensitivity, MaximumDepthSensitivity)]
        private float depthSensitivity = 1f;

        /// <summary>
        /// Multiplier controlling the sensitivity of controller depth movement.
        /// </summary>
        public float DepthSensitivity
        {
            get => depthSensitivity;
            set
            {
                if ((value < MinimumDepthSensitivity) ||
                    (value > MaximumDepthSensitivity))
                {
                    Debug.LogWarning($"{nameof(DepthSensitivity)} out of range, altering to be {MinimumDepthSensitivity} <= value <= {MaximumDepthSensitivity}.");
                }

                depthSensitivity = Mathf.Clamp(value, MinimumDepthSensitivity, MaximumDepthSensitivity);
            }
        }

        #region Move controls

        /// <summary>
        /// The smallest multiplier that can be applied to control the amount of artificial controller jitter.
        /// </summary>
        public const float MinimumJitterStrength = 0f;

        /// <summary>
        /// The largest multiplier that can be applied to control the amount of artificial controller jitter.
        /// </summary>
        public const float MaximumJitterStrength = 1f;

        [SerializeField]
        [Tooltip("The amount of controller jitter to simulate.")]
        [Range(MinimumJitterStrength, MaximumJitterStrength)]
        private float jitterStrength = MinimumJitterStrength;

        /// <summary>
        /// The amount of controller jitter to simulate.
        /// </summary>
        public float JitterStrength
        {
            get => jitterStrength;
            set
            {
                if ((value < MinimumJitterStrength) ||
                    (value > MaximumJitterStrength))
                {
                    Debug.LogWarning($"{nameof(JitterStrength)} out of range, altering the provided value.");
                }

                jitterStrength = Mathf.Clamp(value, MinimumJitterStrength, MaximumJitterStrength);
            }
        }

        [SerializeField]
        [Tooltip("Should controller movement along the depth axis (Z) be smoothed?")]
        private bool isMovementSmoothed = true;

        /// <summary>
        /// Should controller movement along the depth axis (Z) be smoothed?
        /// </summary>
        /// <remarks>
        /// Enabling smoothing can result in the controller 'gliding' to a stop
        /// when movement controls are released.
        /// </remarks>
        public bool IsMovementSmoothed
        {
            get => isMovementSmoothed;
            set => isMovementSmoothed = value;
        }

        [SerializeField]
        [Tooltip("The input action used to move the controller along the depth axis.")]
        private InputActionReference moveDepth;

        /// <summary>
        /// The input action used to move the controller along the depth axis.
        /// </summary>
        public InputActionReference MoveDepth
        {
            get => moveDepth;
            set => moveDepth = value;
        }

        [SerializeField]
        [Tooltip("The input action used to move the controller along the horizontal axis.")]
        private InputActionReference moveHorizontal;

        /// <summary>
        /// The input action used to move the controller along the horizontal axis.
        /// </summary>
        public InputActionReference MoveHorizontal
        {
            get => moveHorizontal;
            set => moveHorizontal = value;
        }

        [SerializeField]
        [Tooltip("The input action used to move the controller along the vertical axis.")]
        private InputActionReference moveVertical;

        /// <summary>
        /// The input action used to move the controller along the vertical axis.
        /// </summary>
        public InputActionReference MoveVertical
        {
            get => moveVertical;
            set => moveVertical = value;
        }

        #endregion Move controls

        #region Rotate controls

        // todo: sensitivity?

        /// <summary>
        /// The mode in which the controller is to be rotated.
        /// </summary>
        public ControllerRotationMode RotationMode { get; set; } = ControllerRotationMode.UserControl;

        [SerializeField]
        [Tooltip("The input action used to simulate rotating the controller around the depth axis.")]
        private InputActionReference roll;

        /// <summary>
        /// The input action used to simulate rotating the controller to rotate around the depth axis.
        /// </summary>
        public InputActionReference Roll
        {
            get => roll;
            set => roll = value;
        }

        [SerializeField]
        [Tooltip("The input action used to simulate rotating the controller around the horizontal axis.")]
        private InputActionReference pitch;

        /// <summary>
        /// The input action used to simulate rotating the controller around the horizontal axis.
        /// </summary>
        public InputActionReference Pitch
        {
            get => pitch;
            set => pitch = value;
        }

        [SerializeField]
        [Tooltip("The input action used to simulate rotating the controller around the vertical axis.")]
        private InputActionReference yaw;

        /// <summary>
        /// The input action used to simulate rotating the controller around the vertical axis.
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

        #region Pose controls

        [SerializeField]
        [Tooltip("The input action used to trigger the controller to rotate to face the camera.")]
        private InputActionReference faceTheCamera;

        /// <summary>
        /// The input action used to trigger the controller to rotate to face the camera.
        /// </summary>
        public InputActionReference FaceTheCamera
        {
            get => faceTheCamera;
            set => value = faceTheCamera;
        }

        /* todo
                /// <summary>
                /// The shortest interval, in seconds, during which a pose transition will occur.
                /// </summary>
                public const float MinimumHandPoseTransitionTime = 0f;

                /// <summary>
                /// The longest interval, in seconds, during which a pose transition will occur.
                /// </summary>
                public const float MaximumHandPoseTransitionTime = 10f;


                [SerializeField]
                [Tooltip("The time, in seconds, to transition from the current to the desired pose.")]
                [Range(MinimumHandPoseTransitionTime, MaximumHandPoseTransitionTime)]
                private float handPoseTransitionTime = 1f;

                /// <summary>
                /// The time, in seconds, to transition from the current to the desired pose.
                /// </summary>
                public float HandPoseTransitionTime
                {
                    get => handPoseTransitionTime;
                    set
                    {
                        if ((value < MinimumHandPoseTransitionTime) ||
                            (value > MaximumHandPoseTransitionTime))
                        {
                            Debug.LogWarning($"{nameof(HandPoseTransitionTime)} out of range, altering to be {MinimumHandPoseTransitionTime} <= value <= {MaximumHandPoseTransitionTime}.");
                        }

                        handPoseTransitionTime = Mathf.Clamp(value, MinimumHandPoseTransitionTime, MaximumHandPoseTransitionTime);
                    }
                }
        */

        #endregion Pose controls

        #region Action controls

        [SerializeField]
        [Tooltip("The input action used to control the state of the trigger axis.")]
        private InputActionReference triggerAxis;

        /// <summary>
        /// The input action used to control the state of the trigger axis.
        /// </summary>
        public InputActionReference TriggerAxis
        {
            get => triggerAxis;
            set => triggerAxis = value;
        }

        [SerializeField]
        [Tooltip("The input action used to control the state of the trigger button.")]
        private InputActionReference triggerButton;

        /// <summary>
        /// The input action used to control the state of the trigger button.
        /// </summary>
        public InputActionReference TriggerButton
        {
            get => triggerButton;
            set => triggerButton = value;
        }

        [SerializeField]
        [Tooltip("The input action used to control the state of the grip axis.")]
        private InputActionReference gripAxis;

        /// <summary>
        /// The input action used to control the state of the grip axis.
        /// </summary>
        public InputActionReference GripAxis
        {
            get => gripAxis;
            set => gripAxis = value;
        }

        [SerializeField]
        [Tooltip("The input action used to control the state of the grip button.")]
        private InputActionReference gripButton;

        /// <summary>
        /// The input action used to control the state of the trigger button.
        /// </summary>
        public InputActionReference GripButton
        {
            get => gripButton;
            set => gripButton = value;
        }

        // todo: more (menu button, touch pads, thumbsticks, etc) to come in the "near" future

        #endregion Action controls

    }

    /// <summary>
    /// The mode in which the controller should be rotated.
    /// </summary>
    public enum ControllerRotationMode
    {
        /// <summary>
        /// The controller rotation is in control of the user.
        /// </summary>
        UserControl = 0,

        /// <summary>
        /// The controller is being / to be rotated to face the camera.
        /// </summary>
        FaceCamera = 1,

        /// <summary>
        /// The controller is being / to be rotated to align with the camera's
        /// forward direction.
        /// </summary>
        CameraAligned = 2
    }
}
