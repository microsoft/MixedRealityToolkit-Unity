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
        /// <summary>
        /// Indicator buttons to show input simulation state in the viewport
        /// </summary>
        public GameObject IndicatorsPrefab => indicatorsPrefab;

        [Header("Common Input")]
        [SerializeField]
        [Tooltip("Sensitivity when using the mouse for rotation")]
        [FormerlySerializedAs("defaultMouseSensitivity")]
        private float mouseRotationSensitivity = 0.1f;
        /// <summary>
        /// Sensitivity when using the mouse for rotation
        /// </summary>
        public float MouseRotationSensitivity => mouseRotationSensitivity;
        [SerializeField]
        [Tooltip("Mouse Movement X-axis")]
        private string mouseX = "Mouse X"; 
        /// <summary>
        /// Mouse Movement X-axis
        /// </summary>
        public string MouseX => mouseX;
        [SerializeField]
        [Tooltip("Mouse Movement Y-axis")]
        private string mouseY = "Mouse Y";
        /// <summary>
        /// Mouse Movement Y-axis
        /// </summary>
        public string MouseY => mouseY;
        [SerializeField]
        [Tooltip("Mouse Scroll Wheel")]
        private string mouseScroll = "Mouse ScrollWheel";
        /// <summary>
        /// Mouse Scroll Wheel
        /// </summary>
        public string MouseScroll => mouseScroll;
        [SerializeField]
        [Tooltip("Maximum time interval for double press")]
        private float doublePressTime = 0.4f;
        /// <summary>
        /// Maximum time interval for double press
        /// </summary>
        public float DoublePressTime => doublePressTime;

        [Header("Camera Control")]
        [SerializeField]
        [Tooltip("Enable manual camera control")]
        private bool isCameraControlEnabled = true;
        /// <summary>
        /// Enable manual camera control
        /// </summary>
        public bool IsCameraControlEnabled => isCameraControlEnabled;
        [SerializeField]
        [Tooltip("Additional rotation factor after input smoothing has been applied")]
        [FormerlySerializedAs("extraMouseSensitivityScale")]
        private float mouseLookSpeed = 3.0f;
        /// <summary>
        /// Additional rotation factor after input smoothing has been applied
        /// </summary>
        public float MouseLookSpeed => mouseLookSpeed;
        [SerializeField]
        [Tooltip("Controls how mouse look control is activated")]
        private KeyBinding mouseLookButton = KeyBinding.FromMouseButton(KeyBinding.MouseButton.Right);
        /// <summary>
        /// Controls how mouse look control is activated
        /// </summary>
        public KeyBinding MouseLookButton => mouseLookButton;
        [SerializeField]
        [Tooltip("Toggle mouse look on with with the mouse look button, press escape to release")]
        private bool mouseLookToggle = false;
        /// <summary>
        /// Toggle mouse look on with with the mouse look button, press escape to release
        /// </summary>
        public bool MouseLookToggle => mouseLookToggle;
        [SerializeField]
        [Tooltip("Invert the vertical rotation")]
        private bool isControllerLookInverted = true;
        /// <summary>
        /// Invert the vertical rotation
        /// </summary>
        public bool IsControllerLookInverted => isControllerLookInverted;

        [SerializeField]
        [Tooltip("Camera movement mode")]
        private InputSimulationControlMode currentControlMode = InputSimulationControlMode.Fly;
        /// <summary>
        /// Camera movement mode
        /// </summary>
        public InputSimulationControlMode CurrentControlMode => currentControlMode;
        [SerializeField]
        [Tooltip("Key to speed up camera movement")]
        private KeyBinding fastControlKey = KeyBinding.FromKey(KeyCode.RightControl);
        /// <summary>
        /// Key to speed up camera movement
        /// </summary>
        public KeyBinding FastControlKey => fastControlKey;
        [SerializeField]
        [Tooltip("Slow camera translation speed")]
        private float controlSlowSpeed = 0.1f;
        /// <summary>
        /// Slow camera translation speed
        /// </summary>
        public float ControlSlowSpeed => controlSlowSpeed;
        [SerializeField]
        [Tooltip("Fast camera translation speed")]
        private float controlFastSpeed = 1.0f;
        /// <summary>
        /// Fast camera translation speed
        /// </summary>
        public float ControlFastSpeed => controlFastSpeed;

        // Input axes  to coordinate with the Input Manager (Project Settings -> Input)

        // Horizontal movement string for keyboard and left stick of game controller
        [SerializeField]
        [Tooltip("Horizontal movement Axis ")]
        private string moveHorizontal = "Horizontal";
        /// <summary>
        /// Horizontal movement Axis 
        /// </summary>
        public string MoveHorizontal => moveHorizontal;
        // Vertical movement string for keyboard and left stick of game controller 
        [SerializeField]
        [Tooltip("Vertical movement Axis ")]
        private string moveVertical = "Vertical";
        /// <summary>
        /// Vertical movement Axis 
        /// </summary>
        public string MoveVertical => moveVertical;
        [SerializeField]
        [Tooltip("Up/Down movement Axis ")]
        private string moveUpDown = "UpDown";
        /// <summary>
        /// Up/Down movement Axis 
        /// </summary>
        public string MoveUpDown => moveUpDown;
        // Look horizontal string for right stick of game controller
        // The right stick has no default settings in the Input Manager and will need to be setup for a game controller to look
        [SerializeField]
        [Tooltip("Look Horizontal Axis - Right Stick On Controller")]
        private string lookHorizontal = ControllerMappingLibrary.AXIS_4;
        /// <summary>
        /// Look Horizontal Axis - Right Stick On Controller
        /// </summary>
        public string LookHorizontal => lookHorizontal;
        // Look vertical string for right stick of game controller
        [SerializeField]
        [Tooltip("Look Vertical Axis - Right Stick On Controller ")]
        private string lookVertical = ControllerMappingLibrary.AXIS_5;
        /// <summary>
        /// Look Vertical Axis - Right Stick On Controller 
        /// </summary>
        public string LookVertical => lookVertical;

        [Header("Eye Simulation")]
        [SerializeField]
        [Tooltip("Enable eye simulation")]
        private bool simulateEyePosition = false;
        /// <summary>
        /// Enable eye simulation
        /// </summary>
        public bool SimulateEyePosition => simulateEyePosition;

        [Header("Hand Simulation")]
        [SerializeField]
        [Tooltip("Enable hand simulation")]
        [FormerlySerializedAs("handSimulationMode")]
        private HandSimulationMode defaultHandSimulationMode = HandSimulationMode.Articulated;
        /// <summary>
        /// Enable hand simulation
        /// </summary>
        public HandSimulationMode DefaultHandSimulationMode => defaultHandSimulationMode;

        [Header("Hand Control Settings")]
        [SerializeField]
        [Tooltip("Key to toggle persistent mode for the left hand")]
        private KeyBinding toggleLeftHandKey = KeyBinding.FromKey(KeyCode.T);
        /// <summary>
        /// Key to toggle persistent mode for the left hand
        /// </summary>
        public KeyBinding ToggleLeftHandKey => toggleLeftHandKey;
        [SerializeField]
        [Tooltip("Key to toggle persistent mode for the right hand")]
        private KeyBinding toggleRightHandKey = KeyBinding.FromKey(KeyCode.Y);
        /// <summary>
        /// Key to toggle persistent mode for the right hand
        /// </summary>
        public KeyBinding ToggleRightHandKey => toggleRightHandKey;
        [SerializeField]
        [Tooltip("Time after which uncontrolled hands are hidden")]
        private float handHideTimeout = 0.2f;
        /// <summary>
        /// Time after which uncontrolled hands are hidden
        /// </summary>
        public float HandHideTimeout => handHideTimeout;
        [SerializeField]
        [Tooltip("Key to manipulate the left hand")]
        private KeyBinding leftHandManipulationKey = KeyBinding.FromKey(KeyCode.LeftShift);
        /// <summary>
        /// Key to manipulate the left hand
        /// </summary>
        public KeyBinding LeftHandManipulationKey => leftHandManipulationKey;
        [SerializeField]
        [Tooltip("Key to manipulate the right hand")]
        private KeyBinding rightHandManipulationKey = KeyBinding.FromKey(KeyCode.Space);
        /// <summary>
        /// Key to manipulate the right hand
        /// </summary>
        public KeyBinding RightHandManipulationKey => rightHandManipulationKey;
        [SerializeField]
        [Tooltip("Additional rotation factor after input smoothing has been applied")]
        private float mouseHandRotationSpeed = 6.0f;
        /// <summary>
        /// Additional rotation factor after input smoothing has been applied
        /// </summary>
        public float MouseHandRotationSpeed => mouseHandRotationSpeed;
        [SerializeField]
        [Tooltip("Controls how hand rotation is activated")]
        private KeyBinding handRotateButton = KeyBinding.FromKey(KeyCode.LeftControl);
        /// <summary>
        /// Controls how hand rotation is activated
        /// </summary>
        public KeyBinding HandRotateButton => handRotateButton;

        [Header("Hand Gesture Settings")]
        [SerializeField]
        [Tooltip("Hand joint pose on first show or reset")]
        private ArticulatedHandPose.GestureId defaultHandGesture = ArticulatedHandPose.GestureId.Open;
        /// <summary>
        /// Hand joint pose on first show or reset
        /// </summary>
        public ArticulatedHandPose.GestureId DefaultHandGesture => defaultHandGesture;
        [SerializeField]
        [Tooltip("Hand joint pose when pressing the left mouse button")]
        private ArticulatedHandPose.GestureId leftMouseHandGesture = ArticulatedHandPose.GestureId.Pinch;
        /// <summary>
        /// Hand joint pose when pressing the left mouse button
        /// </summary>
        public ArticulatedHandPose.GestureId LeftMouseHandGesture => leftMouseHandGesture;
        [SerializeField]
        [Tooltip("Hand joint pose when pressing the middle mouse button")]
        private ArticulatedHandPose.GestureId middleMouseHandGesture = ArticulatedHandPose.GestureId.None;
        /// <summary>
        /// Hand joint pose when pressing the middle mouse button
        /// </summary>
        public ArticulatedHandPose.GestureId MiddleMouseHandGesture => middleMouseHandGesture;
        [SerializeField]
        [Tooltip("Hand joint pose when pressing the right mouse button")]
        private ArticulatedHandPose.GestureId rightMouseHandGesture = ArticulatedHandPose.GestureId.None;
        /// <summary>
        /// Hand joint pose when pressing the right mouse button
        /// </summary>
        public ArticulatedHandPose.GestureId RightMouseHandGesture => rightMouseHandGesture;
        [SerializeField]
        [Tooltip("Gesture interpolation per second")]
        private float handGestureAnimationSpeed = 8.0f;
        /// <summary>
        /// Gesture interpolation per second
        /// </summary>
        public float HandGestureAnimationSpeed => handGestureAnimationSpeed;

        [SerializeField]
        [Tooltip("Time until hold gesture starts")]
        private float holdStartDuration = 0.5f;
        /// <summary>
        /// Time until hold gesture starts
        /// </summary>
        public float HoldStartDuration => holdStartDuration;
        [SerializeField]
        [Tooltip("The total amount of input source movement that needs to happen to start navigation")]
        [UnityEngine.Serialization.FormerlySerializedAs("manipulationStartThreshold")]
        private float navigationStartThreshold = 0.03f;
        /// <summary>
        /// The total amount of input source movement that needs to happen to start navigation
        /// </summary>
        public float NavigationStartThreshold => navigationStartThreshold;

        [Header("Hand Placement Settings")]
        [SerializeField]
        [Tooltip("Default distance of the hand from the camera")]
        private float defaultHandDistance = 0.5f;
        /// <summary>
        /// Default distance of the hand from the camera
        /// </summary>
        public float DefaultHandDistance => defaultHandDistance;
        [SerializeField]
        [Tooltip("Depth change when scrolling the mouse wheel")]
        private float handDepthMultiplier = 0.03f;
        /// <summary>
        /// Depth change when scrolling the mouse wheel
        /// </summary>
        public float HandDepthMultiplier => handDepthMultiplier;
        [SerializeField]
        [Tooltip("Apply random offset to the hand position")]
        private float handJitterAmount = 0.0f;
        /// <summary>
        /// Apply random offset to the hand position
        /// </summary>
        public float HandJitterAmount => handJitterAmount;
    }
}