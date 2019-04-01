// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Simulated Input Profile", fileName = "MixedRealityInputSimulationProfile", order = (int)CreateProfileMenuItemIndices.InputSimulation)]
    [MixedRealityServiceProfile(typeof(InputSimulationService))]
    public class MixedRealityInputSimulationProfile : BaseMixedRealityProfile
    {
        [Header("Camera Control")]
        [SerializeField]
        [Tooltip("Enable manual camera control")]
        private bool isCameraControlEnabled = true;
        public bool IsCameraControlEnabled => isCameraControlEnabled;

        [SerializeField]
        private float extraMouseSensitivityScale = 3.0f;
        public float ExtraMouseSensitivityScale => extraMouseSensitivityScale;
        [SerializeField]
        private float defaultMouseSensitivity = 0.1f;
        public float DefaultMouseSensitivity => defaultMouseSensitivity;
        [SerializeField]
        [Tooltip("Controls how mouse look control is activated.")]
        private InputSimulationMouseButton mouseLookButton = InputSimulationMouseButton.Right;
        public InputSimulationMouseButton MouseLookButton => mouseLookButton;
        [SerializeField]
        private bool isControllerLookInverted = true;
        public bool IsControllerLookInverted => isControllerLookInverted;

        [SerializeField]
        private InputSimulationControlMode currentControlMode = InputSimulationControlMode.Fly;
        public InputSimulationControlMode CurrentControlMode => currentControlMode;
        [SerializeField]
        private KeyCode fastControlKey = KeyCode.RightControl;
        public KeyCode FastControlKey => fastControlKey;
        [SerializeField]
        private float controlSlowSpeed = 0.1f;
        public float ControlSlowSpeed => controlSlowSpeed;
        [SerializeField]
        private float controlFastSpeed = 1.0f;
        public float ControlFastSpeed => controlFastSpeed;

        // Input axes  to coordinate with the Input Manager (Project Settings -> Input)

        // Horizontal movement string for keyboard and left stick of game controller
        [SerializeField]
        [Tooltip("Horizontal movement Axis ")]
        private string moveHorizontal = "Horizontal";
        public string MoveHorizontal => moveHorizontal;
        // Vertical movement string for keyboard and left stick of game controller 
        [SerializeField]
        [Tooltip("Vertical movement Axis ")]
        private string moveVertical = "Vertical";
        public string MoveVertical => moveVertical;
        // Mouse movement string for the x-axis
        [SerializeField]
        [Tooltip("Mouse Movement X-axis")]
        private string mouseX = "Mouse X"; 
        public string MouseX => mouseX;
        // Mouse movement string for the y-axis
        [SerializeField]
        [Tooltip("Mouse Movement Y-axis")]
        private string mouseY = "Mouse Y";
        public string MouseY => mouseY;
        // Look horizontal string for right stick of game controller
        // The right stick has no default settings in the Input Manager and will need to be setup for a game controller to look
        [SerializeField]
        [Tooltip("Look Horizontal Axis - Right Stick On Controller")]
        private string lookHorizontal = "LookHorizontal";
        public string LookHorizontal => lookHorizontal;
        // Look vertical string for right stick of game controller
        [SerializeField]
        [Tooltip("Look Vertical Axis - Right Stick On Controller ")]
        private string lookVertical = "LookVertical";
        public string LookVertical => lookVertical;

        [Header("Eye Simulation")]
        [SerializeField]
        [Tooltip("Enable eye simulation")]
        private bool simulateEyePosition = false;
        public bool SimulateEyePosition => simulateEyePosition;

        [Header("Hand Simulation")]
        [SerializeField]
        [Tooltip("Enable hand simulation")]
        private HandSimulationMode handSimulationMode = HandSimulationMode.Articulated;
        public HandSimulationMode HandSimulationMode => handSimulationMode;

        [Header("Hand Control Settings")]
        [SerializeField]
        [Tooltip("Key to toggle persistent mode for the left hand")]
        private KeyCode toggleLeftHandKey = KeyCode.T;
        public KeyCode ToggleLeftHandKey => toggleLeftHandKey;
        [SerializeField]
        [Tooltip("Key to toggle persistent mode for the right hand")]
        private KeyCode toggleRightHandKey = KeyCode.Y;
        public KeyCode ToggleRightHandKey => toggleRightHandKey;
        [SerializeField]
        [Tooltip("Time after which uncontrolled hands are hidden")]
        private float handHideTimeout = 0.2f;
        public float HandHideTimeout => handHideTimeout;
        [SerializeField]
        [Tooltip("Key to manipulate the left hand")]
        private KeyCode leftHandManipulationKey = KeyCode.LeftShift;
        public KeyCode LeftHandManipulationKey => leftHandManipulationKey;
        [SerializeField]
        [Tooltip("Key to manipulate the right hand")]
        private KeyCode rightHandManipulationKey = KeyCode.Space;
        public KeyCode RightHandManipulationKey => rightHandManipulationKey;

        [Header("Hand Gesture Settings")]
        [SerializeField]
        private SimulatedHandPose.GestureId defaultHandGesture = SimulatedHandPose.GestureId.Open;
        public SimulatedHandPose.GestureId DefaultHandGesture => defaultHandGesture;
        [SerializeField]
        private SimulatedHandPose.GestureId leftMouseHandGesture = SimulatedHandPose.GestureId.Pinch;
        public SimulatedHandPose.GestureId LeftMouseHandGesture => leftMouseHandGesture;
        [SerializeField]
        private SimulatedHandPose.GestureId middleMouseHandGesture = SimulatedHandPose.GestureId.None;
        public SimulatedHandPose.GestureId MiddleMouseHandGesture => middleMouseHandGesture;
        [SerializeField]
        private SimulatedHandPose.GestureId rightMouseHandGesture = SimulatedHandPose.GestureId.None;
        public SimulatedHandPose.GestureId RightMouseHandGesture => rightMouseHandGesture;
        [SerializeField]
        [Tooltip("Gesture interpolation per second")]
        private float handGestureAnimationSpeed = 8.0f;
        public float HandGestureAnimationSpeed => handGestureAnimationSpeed;

        [SerializeField]
        [Tooltip("Time until hold gesture starts")]
        private float holdStartDuration = 0.5f;
        public float HoldStartDuration => holdStartDuration;
        [SerializeField]
        [Tooltip("The total amount of input source movement that needs to happen to start a manipulation")]
        private float manipulationStartThreshold = 0.03f;
        public float ManipulationStartThreshold => manipulationStartThreshold;

        [Header("Hand Placement Settings")]
        [SerializeField]
        [Tooltip("Default distance of the hand from the camera")]
        private float defaultHandDistance = 0.5f;
        public float DefaultHandDistance => defaultHandDistance;
        [SerializeField]
        [Tooltip("Depth change when scrolling the mouse wheel")]
        private float handDepthMultiplier = 0.1f;
        public float HandDepthMultiplier => handDepthMultiplier;
        [SerializeField]
        [Tooltip("Apply random offset to the hand position")]
        private float handJitterAmount = 0.0f;
        public float HandJitterAmount => handJitterAmount;

        [Header("Hand Rotation Settings")]
        [SerializeField]
        [Tooltip("Key to turn the hand clockwise")]
        private KeyCode yawHandCWKey = KeyCode.E;
        public KeyCode YawHandCWKey => yawHandCWKey;
        [SerializeField]
        [Tooltip("Key to turn the hand counter-clockwise")]
        private KeyCode yawHandCCWKey = KeyCode.Q;
        public KeyCode YawHandCCWKey => yawHandCCWKey;
        [SerializeField]
        [Tooltip("Key to pitch the hand upward")]
        private KeyCode pitchHandCWKey = KeyCode.F;
        public KeyCode PitchHandCWKey => pitchHandCWKey;
        [SerializeField]
        [Tooltip("Key to pitch the hand downward")]
        private KeyCode pitchHandCCWKey = KeyCode.R;
        public KeyCode PitchHandCCWKey => pitchHandCCWKey;
        [SerializeField]
        [Tooltip("Key to roll the hand right")]
        private KeyCode rollHandCWKey = KeyCode.X;
        public KeyCode RollHandCWKey => rollHandCWKey;
        [SerializeField]
        [Tooltip("Key to roll the hand left")]
        private KeyCode rollHandCCWKey = KeyCode.Z;
        public KeyCode RollHandCCWKey => rollHandCCWKey;
        [SerializeField]
        [Tooltip("Angle per second when rotating the hand")]
        private float handRotationSpeed = 100.0f;
        public float HandRotationSpeed => handRotationSpeed;
    }
}