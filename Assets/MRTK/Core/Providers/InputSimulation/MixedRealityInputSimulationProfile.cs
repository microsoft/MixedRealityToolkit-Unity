// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Profiles/Mixed Reality Simulated Input Profile", fileName = "MixedRealityInputSimulationProfile", order = (int)CreateProfileMenuItemIndices.InputSimulation)]
    [MixedRealityServiceProfile(typeof(IInputSimulationService))]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/input-simulation/input-simulation-service")]
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
        [Tooltip("Button pressed to interact with objects")]
        [FormerlySerializedAs("Interaction Button")]
        private KeyBinding interactionButton = KeyBinding.FromMouseButton(KeyBinding.MouseButton.Left);
        /// <summary>
        /// Button pressed to interact with objects
        /// </summary>
        public KeyBinding InteractionButton => interactionButton;
        [SerializeField]
        [Tooltip("Maximum time interval for double press")]
        private float doublePressTime = 0.4f;
        /// <summary>
        /// Maximum time interval for double press
        /// </summary>
        public float DoublePressTime => doublePressTime;
        [SerializeField]
        [Tooltip("Enable hands free input")]
        private bool isHandsFreeInputEnabled = true;
        /// <summary>
        /// Enable hands free input
        /// </summary>
        public bool IsHandsFreeInputEnabled => isHandsFreeInputEnabled;

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
        [Tooltip("Toggle mouse look on with the mouse look button, press escape to release")]
        private bool mouseLookToggle = false;
        /// <summary>
        /// Toggle mouse look on with the mouse look button, press escape to release
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
        [Tooltip("Amount to offset the starting position of the camera from the origin")]
        private Vector3 cameraOriginOffset = Vector3.zero;
        /// <summary>
        /// Amount to offset the starting position of the camera from the origin
        /// </summary>
        public Vector3 CameraOriginOffset => cameraOriginOffset;

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

        /// <summary>
        /// Enable eye gaze simulation
        /// </summary>
        [Obsolete("Check the EyeGazeSimulationMode instead")]
        public bool SimulateEyePosition => defaultEyeGazeSimulationMode != EyeGazeSimulationMode.Disabled;

        [Header("Eye Gaze Simulation")]
        [SerializeField]
        [Tooltip("Enable eye gaze simulation")]
        [FormerlySerializedAs("simulateEyePosition")]
        private EyeGazeSimulationMode defaultEyeGazeSimulationMode = EyeGazeSimulationMode.Disabled;

        /// <summary>
        /// Enable eye gaze simulation
        /// </summary>
        public EyeGazeSimulationMode DefaultEyeGazeSimulationMode => defaultEyeGazeSimulationMode;

        [Header("Controller Simulation")]
        [SerializeField]
        [Tooltip("Enable controller simulation")]
        [FormerlySerializedAs("handSimulationMode")]
        [FormerlySerializedAs("defaultHandSimulationMode")]
        private ControllerSimulationMode defaultControllerSimulationMode = ControllerSimulationMode.ArticulatedHand;
        /// <summary>
        /// Enable controller simulation
        /// </summary>
        public ControllerSimulationMode DefaultControllerSimulationMode => defaultControllerSimulationMode;

        [Header("Controller Control Settings")]
        [SerializeField]
        [Tooltip("Key to toggle persistent mode for the left controller")]
        [FormerlySerializedAs("toggleLeftHandKey")]
        private KeyBinding toggleLeftControllerKey = KeyBinding.FromKey(KeyCode.T);
        /// <summary>
        /// Key to toggle persistent mode for the left controller
        /// </summary>
        public KeyBinding ToggleLeftControllerKey => toggleLeftControllerKey;
        [SerializeField]
        [Tooltip("Key to toggle persistent mode for the right controller")]
        [FormerlySerializedAs("toggleRightHandKey")]
        private KeyBinding toggleRightControllerKey = KeyBinding.FromKey(KeyCode.Y);
        /// <summary>
        /// Key to toggle persistent mode for the right controller
        /// </summary>
        public KeyBinding ToggleRightControllerKey => toggleRightControllerKey;
        [SerializeField]
        [Tooltip("Time after which uncontrolled controllers are hidden")]
        [FormerlySerializedAs("handHideTimeout")]
        private float controllerHideTimeout = 0.2f;
        /// <summary>
        /// Time after which uncontrolled controllers are hidden
        /// </summary>
        public float ControllerHideTimeout => controllerHideTimeout;
        [SerializeField]
        [Tooltip("Key to manipulate the left controller")]
        [FormerlySerializedAs("leftHandManipulationKey")]
        private KeyBinding leftControllerManipulationKey = KeyBinding.FromKey(KeyCode.LeftShift);
        /// <summary>
        /// Key to manipulate the left controller
        /// </summary>
        public KeyBinding LeftControllerManipulationKey => leftControllerManipulationKey;
        [SerializeField]
        [Tooltip("Key to manipulate the right controller")]
        [FormerlySerializedAs("rightHandManipulationKey")]
        private KeyBinding rightControllerManipulationKey = KeyBinding.FromKey(KeyCode.Space);
        /// <summary>
        /// Key to manipulate the right controller
        /// </summary>
        public KeyBinding RightControllerManipulationKey => rightControllerManipulationKey;
        [SerializeField]
        [Tooltip("Additional rotation factor after input smoothing has been applied")]
        [FormerlySerializedAs("mouseHandRotationSpeed")]
        private float mouseControllerRotationSpeed = 6.0f;
        /// <summary>
        /// Additional rotation factor after input smoothing has been applied
        /// </summary>
        public float MouseControllerRotationSpeed => mouseControllerRotationSpeed;
        [SerializeField]
        [Tooltip("Controls how controller rotation is activated")]
        [FormerlySerializedAs("handRotateButton")]
        private KeyBinding controllerRotateButton = KeyBinding.FromKey(KeyCode.LeftControl);
        /// <summary>
        /// Controls how controller rotation is activated
        /// </summary>
        public KeyBinding ControllerRotateButton => controllerRotateButton;

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

        [Header("Controller Placement Settings")]
        [SerializeField]
        [Tooltip("Default distance of the controller from the camera")]
        [FormerlySerializedAs("defaultHandDistance")]
        private float defaultControllerDistance = 0.5f;
        /// <summary>
        /// Default distance of the controller from the camera
        /// </summary>
        public float DefaultControllerDistance => defaultControllerDistance;
        [SerializeField]
        [Tooltip("Depth change when scrolling the mouse wheel")]
        [FormerlySerializedAs("handDepthMultiplier")]
        private float controllerDepthMultiplier = 0.03f;
        /// <summary>
        /// Depth change when scrolling the mouse wheel
        /// </summary>
        public float ControllerDepthMultiplier => controllerDepthMultiplier;
        [SerializeField]
        [Tooltip("Apply random offset to the controller position")]
        [FormerlySerializedAs("handJitterAmount")]
        private float controllerJitterAmount = 0.0f;
        /// <summary>
        /// Apply random offset to the controller position
        /// </summary>
        public float ControllerJitterAmount => controllerJitterAmount;

        [Header("Motion Controller Settings")]
        [SerializeField]
        [Tooltip("Key to simulate a trigger press (select) on the motion controller")]
        private KeyBinding motionControllerTriggerKey = KeyBinding.FromMouseButton(KeyBinding.MouseButton.Left);

        /// <summary>
        /// Key to simulate a trigger press (select) on the motion controller
        /// </summary>
        public KeyBinding MotionControllerTriggerKey => motionControllerTriggerKey;
        [SerializeField]
        [Tooltip("Key to simulate a grab on the motion controller")]
        private KeyBinding motionControllerGrabKey = KeyBinding.FromKey(KeyCode.G);

        /// <summary>
        /// Key to simulate a grab on the motion controller
        /// </summary>
        public KeyBinding MotionControllerGrabKey => motionControllerGrabKey;
        [SerializeField]
        [Tooltip("Key to simulate a menu press on the motion controller")]
        private KeyBinding motionControllerMenuKey = KeyBinding.FromKey(KeyCode.M);

        /// <summary>
        /// Key to simulate a menu press on the motion controller
        /// </summary>
        public KeyBinding MotionControllerMenuKey => motionControllerMenuKey;

        #region Obsolete Properties
        /// <summary>
        /// Enable controller simulation
        /// </summary>
        [Obsolete("Use DefaultControllerSimulationMode instead.")]
        public ControllerSimulationMode DefaultHandSimulationMode => DefaultControllerSimulationMode;
        /// <summary>
        /// Key to toggle persistent mode for the left controller
        /// </summary>
        [Obsolete("Use ToggleLeftControllerKey instead.")]
        public KeyBinding ToggleLeftHandKey => ToggleLeftControllerKey;
        /// <summary>
        /// Key to toggle persistent mode for the right controller
        /// </summary>
        [Obsolete("Use ToggleRightControllerKey instead.")]
        public KeyBinding ToggleRightHandKey => ToggleRightControllerKey;
        /// <summary>
        /// Time after which uncontrolled hands are hidden
        /// </summary>
        [Obsolete("Use ControllerHideTimeout instead.")]
        public float HandHideTimeout => ControllerHideTimeout;
        /// <summary>
        /// Key to manipulate the left hand
        /// </summary>
        [Obsolete("Use LeftControllerManipulationKey instead.")]
        public KeyBinding LeftHandManipulationKey => LeftControllerManipulationKey;
        /// <summary>
        /// Key to manipulate the right hand
        /// </summary>
        [Obsolete("Use RightControllerManipulationKey instead.")]
        public KeyBinding RightHandManipulationKey => RightControllerManipulationKey;
        /// <summary>
        /// Additional rotation factor after input smoothing has been applied
        /// </summary>
        [Obsolete("Use MouseControllerRotationSpeed instead.")]
        public float MouseHandRotationSpeed => MouseControllerRotationSpeed;
        /// <summary>
        /// Controls how hand rotation is activated
        /// </summary>
        [Obsolete("Use ControllerRotateButton instead.")]
        public KeyBinding HandRotateButton => ControllerRotateButton;
        /// <summary>
        /// Default distance of the hand from the camera
        /// </summary>
        [Obsolete("Use DefaultControllerDistance instead.")]
        public float DefaultHandDistance => DefaultControllerDistance;
        /// <summary>
        /// Depth change when scrolling the mouse wheel
        /// </summary>
        [Obsolete("Use ControllerDepthMultiplier instead.")]
        public float HandDepthMultiplier => ControllerDepthMultiplier;
        /// <summary>
        /// Apply random offset to the hand position
        /// </summary>
        [Obsolete("Use ControllerJitterAmount instead.")]
        public float HandJitterAmount => ControllerJitterAmount;
        #endregion
    }
}
