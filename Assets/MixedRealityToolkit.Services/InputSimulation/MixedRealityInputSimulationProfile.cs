// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Simulated Input Profile", fileName = "MixedRealityInputSimulationProfile", order = (int)CreateProfileMenuItemIndices.InputSimulation)]
    [MixedRealityServiceProfile(typeof(IInputSimulationService))]
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/InputSimulation/InputSimulationService.html")]
    public class MixedRealityInputSimulationProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("Indicator buttons to show input simulation state in the viewport")]
        private GameObject indicatorsPrefab = null;
        public GameObject IndicatorsPrefab => indicatorsPrefab;

        [SerializeField]
        [Tooltip("Maximum time interval for double press")]
        private float doublePressTime = 0.4f;
        public float DoublePressTime => doublePressTime;

        [Header("Camera Control")]
        [SerializeField]
        [Tooltip("Enable manual camera control")]
        private bool isCameraControlEnabled = true;
        public bool IsCameraControlEnabled => isCameraControlEnabled;

        [SerializeField]
        [FormerlySerializedAs("defaultMouseSensitivity")]
        private float mouseRotationSensitivity = 0.1f;
        public float MouseRotationSensitivity => mouseRotationSensitivity;
        [SerializeField]
        [FormerlySerializedAs("extraMouseSensitivityScale")]
        private float extraMouseRotationScale = 3.0f;
        public float ExtraMouseRotationScale => extraMouseRotationScale;
        [SerializeField]
        [Tooltip("Controls how mouse look control is activated")]
        private KeyBinding mouseLookButton = KeyBinding.FromMouseButton(KeyBinding.MouseButton.Right);
        public KeyBinding MouseLookButton => mouseLookButton;
        [SerializeField]
        [Tooltip("Toggle mouse look on with with the mouse look button, press escape to release")]
        private bool mouseLookToggle = false;
        public bool MouseLookToggle => mouseLookToggle;
        [SerializeField]
        private bool isControllerLookInverted = true;
        public bool IsControllerLookInverted => isControllerLookInverted;

        [SerializeField]
        private InputSimulationControlMode currentControlMode = InputSimulationControlMode.Fly;
        public InputSimulationControlMode CurrentControlMode => currentControlMode;
        [SerializeField]
        private KeyBinding fastControlKey = KeyBinding.FromKey(KeyCode.RightControl);
        public KeyBinding FastControlKey => fastControlKey;
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
        [SerializeField]
        [Tooltip("Up/Down movement Axis ")]
        private string moveUpDown = "UpDown";
        public string MoveUpDown => moveUpDown;
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
        [SerializeField]
        [Tooltip("Mouse Scroll Wheel")]
        private string mouseScroll = "Mouse ScrollWheel";
        public string MouseScroll => mouseScroll;
        // Look horizontal string for right stick of game controller
        // The right stick has no default settings in the Input Manager and will need to be setup for a game controller to look
        [SerializeField]
        [Tooltip("Look Horizontal Axis - Right Stick On Controller")]
        private string lookHorizontal = ControllerMappingLibrary.AXIS_4;
        public string LookHorizontal => lookHorizontal;
        // Look vertical string for right stick of game controller
        [SerializeField]
        [Tooltip("Look Vertical Axis - Right Stick On Controller ")]
        private string lookVertical = ControllerMappingLibrary.AXIS_5;
        public string LookVertical => lookVertical;

        [Header("Eye Simulation")]
        [SerializeField]
        [Tooltip("Enable eye simulation")]
        private bool simulateEyePosition = false;
        public bool SimulateEyePosition => simulateEyePosition;

        [Header("Hand Simulation")]
        [SerializeField]
        [Tooltip("Enable hand simulation")]
        private HandSimulationMode defaultHandSimulationMode = HandSimulationMode.Articulated;
        public HandSimulationMode DefaultHandSimulationMode => defaultHandSimulationMode;

        [Header("Hand Control Settings")]
        [SerializeField]
        [Tooltip("Key to toggle persistent mode for the left hand")]
        private KeyBinding toggleLeftHandKey = KeyBinding.FromKey(KeyCode.T);
        public KeyBinding ToggleLeftHandKey => toggleLeftHandKey;
        [SerializeField]
        [Tooltip("Key to toggle persistent mode for the right hand")]
        private KeyBinding toggleRightHandKey = KeyBinding.FromKey(KeyCode.Y);
        public KeyBinding ToggleRightHandKey => toggleRightHandKey;
        [SerializeField]
        [Tooltip("Time after which uncontrolled hands are hidden")]
        private float handHideTimeout = 0.2f;
        public float HandHideTimeout => handHideTimeout;
        [SerializeField]
        [Tooltip("Key to manipulate the left hand")]
        private KeyBinding leftHandManipulationKey = KeyBinding.FromKey(KeyCode.LeftShift);
        public KeyBinding LeftHandManipulationKey => leftHandManipulationKey;
        [SerializeField]
        [Tooltip("Key to manipulate the right hand")]
        private KeyBinding rightHandManipulationKey = KeyBinding.FromKey(KeyCode.Space);
        public KeyBinding RightHandManipulationKey => rightHandManipulationKey;
        [SerializeField]
        [Tooltip("Controls how mouse look control is activated")]
        private KeyBinding handRotateButton = KeyBinding.FromKey(KeyCode.LeftControl);
        public KeyBinding HandRotateButton => handRotateButton;

        [Header("Hand Gesture Settings")]
        [SerializeField]
        private ArticulatedHandPose.GestureId defaultHandGesture = ArticulatedHandPose.GestureId.Open;
        public ArticulatedHandPose.GestureId DefaultHandGesture => defaultHandGesture;
        [SerializeField]
        private ArticulatedHandPose.GestureId leftMouseHandGesture = ArticulatedHandPose.GestureId.Pinch;
        public ArticulatedHandPose.GestureId LeftMouseHandGesture => leftMouseHandGesture;
        [SerializeField]
        private ArticulatedHandPose.GestureId middleMouseHandGesture = ArticulatedHandPose.GestureId.None;
        public ArticulatedHandPose.GestureId MiddleMouseHandGesture => middleMouseHandGesture;
        [SerializeField]
        private ArticulatedHandPose.GestureId rightMouseHandGesture = ArticulatedHandPose.GestureId.None;
        public ArticulatedHandPose.GestureId RightMouseHandGesture => rightMouseHandGesture;
        [SerializeField]
        [Tooltip("Gesture interpolation per second")]
        private float handGestureAnimationSpeed = 8.0f;
        public float HandGestureAnimationSpeed => handGestureAnimationSpeed;

        [SerializeField]
        [Tooltip("Time until hold gesture starts")]
        private float holdStartDuration = 0.5f;
        public float HoldStartDuration => holdStartDuration;
        [SerializeField]
        [Tooltip("The total amount of input source movement that needs to happen to start navigation")]
        [UnityEngine.Serialization.FormerlySerializedAs("manipulationStartThreshold")]
        private float navigationStartThreshold = 0.03f;
        public float NavigationStartThreshold => navigationStartThreshold;

        [Header("Hand Placement Settings")]
        [SerializeField]
        [Tooltip("Default distance of the hand from the camera")]
        private float defaultHandDistance = 0.5f;
        public float DefaultHandDistance => defaultHandDistance;
        [SerializeField]
        [Tooltip("Depth change when scrolling the mouse wheel")]
        private float handDepthMultiplier = 0.03f;
        public float HandDepthMultiplier => handDepthMultiplier;
        [SerializeField]
        [Tooltip("Apply random offset to the hand position")]
        private float handJitterAmount = 0.0f;
        public float HandJitterAmount => handJitterAmount;
    }
}