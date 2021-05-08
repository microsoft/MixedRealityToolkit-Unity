// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Utility struct that provides mouse delta in pixels (screen space), normalized viewport coordinates, and world units.
    /// </summary>
    public class MouseDelta
    {
        public Vector3 screenDelta = Vector3.zero;
        public Vector3 viewportDelta = Vector3.zero;
        public Vector3 worldDelta = Vector3.zero;

        /// <summary>
        /// Resets all vector contents to zero vector values
        /// </summary>
        public void Reset()
        {
            screenDelta = Vector3.zero;
            viewportDelta = Vector3.zero;
            worldDelta = Vector3.zero;
        }
    }

    /// <summary>
    /// Service that provides simulated mixed reality input information based on mouse and keyboard input in editor
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsEditor | SupportedPlatforms.MacEditor | SupportedPlatforms.LinuxEditor,
        "Input Simulation Service",
        "Profiles/DefaultMixedRealityInputSimulationProfile.asset",
        "MixedRealityToolkit.SDK",
        true)]
    [HelpURL("https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/input-simulation/input-simulation-service")]
    public class InputSimulationService :
        BaseInputSimulationService,
        IInputSimulationService,
        IMixedRealityEyeGazeDataProvider,
        IMixedRealityCapabilityCheck
    {
        private ManualCameraControl cameraControl = null;
        private SimulatedControllerDataProvider dataProvider = null;

        /// <inheritdoc />
        public ControllerSimulationMode ControllerSimulationMode { get; set; }

        /// <inheritdoc />
        public SimulatedHandData HandDataLeft { get; } = new SimulatedHandData();
        /// <inheritdoc />
        public SimulatedHandData HandDataRight { get; } = new SimulatedHandData();
        /// <inheritdoc />
        private SimulatedHandData HandDataGaze { get; } = new SimulatedHandData();
        /// <inheritdoc />
        public SimulatedMotionControllerData MotionControllerDataLeft { get; } = new SimulatedMotionControllerData();
        /// <inheritdoc />
        public SimulatedMotionControllerData MotionControllerDataRight { get; } = new SimulatedMotionControllerData();

        /// <inheritdoc />
        public bool IsSimulatingControllerLeft => dataProvider != null && dataProvider.IsSimulatingLeft;
        /// <inheritdoc />
        public bool IsSimulatingControllerRight => dataProvider != null && dataProvider.IsSimulatingRight;

        /// <inheritdoc />
        public bool IsAlwaysVisibleControllerLeft
        {
            get { return dataProvider != null && dataProvider.IsAlwaysVisibleLeft; }
            set { if (dataProvider != null) { dataProvider.IsAlwaysVisibleLeft = value; } }
        }

        /// <inheritdoc />
        public bool IsAlwaysVisibleControllerRight
        {
            get { return dataProvider != null && dataProvider.IsAlwaysVisibleRight; }
            set { if (dataProvider != null) { dataProvider.IsAlwaysVisibleRight = value; } }
        }

        /// <inheritdoc />
        public Vector3 ControllerPositionLeft
        {
            get { return dataProvider != null ? dataProvider.InputStateLeft.ViewportPosition : Vector3.zero; }
            set { if (dataProvider != null) { dataProvider.InputStateLeft.ViewportPosition = value; } }
        }

        /// <inheritdoc />
        public Vector3 ControllerPositionRight
        {
            get { return dataProvider != null ? dataProvider.InputStateRight.ViewportPosition : Vector3.zero; }
            set { if (dataProvider != null) { dataProvider.InputStateRight.ViewportPosition = value; } }
        }

        /// <inheritdoc />
        public Vector3 ControllerRotationLeft
        {
            get { return dataProvider != null ? dataProvider.InputStateLeft.ViewportRotation : Vector3.zero; }
            set { if (dataProvider != null) { dataProvider.InputStateLeft.ViewportRotation = value; } }
        }

        /// <inheritdoc />
        public Vector3 ControllerRotationRight
        {
            get { return dataProvider != null ? dataProvider.InputStateRight.ViewportRotation : Vector3.zero; }
            set { if (dataProvider != null) { dataProvider.InputStateRight.ViewportRotation = value; } }
        }

        /// <inheritdoc />
        public void ResetControllerLeft()
        {
            if (dataProvider != null)
            {
                dataProvider.ResetInput(Handedness.Left);
            }
        }

        /// <inheritdoc />
        public void ResetControllerRight()
        {
            if (dataProvider != null)
            {
                dataProvider.ResetInput(Handedness.Right);
            }
        }

        /// <summary>
        /// If true then camera forward direction is used to simulate eye tracking data.    
        /// </summary>
        [Obsolete("Check the EyeGazeSimulationMode instead")]
        public bool SimulateEyePosition
        {
            get
            {
                return EyeGazeSimulationMode != EyeGazeSimulationMode.Disabled;
            }
            set
            {
                EyeGazeSimulationMode = value ? EyeGazeSimulationMode.CameraForwardAxis : EyeGazeSimulationMode.Disabled;
            }
        }

        /// <inheritdoc />
        public EyeGazeSimulationMode EyeGazeSimulationMode { get; set; }

        /// <summary>
        /// If true then keyboard and mouse input are used to simulate controllers.
        /// </summary>
        public bool UserInputEnabled { get; set; } = true;

        /// <summary>
        /// Timestamp of the last controller device update
        /// </summary>
        private long lastControllerUpdateTimestamp = 0;

        /// <summary>
        /// Indicators to show input simulation state in the viewport.
        /// </summary>
        private GameObject indicators;

        /// <summary>
        /// Tracks mouse movement delta information in different coordinate system spaces between updates
        /// </summary>
        private MouseDelta mouseDelta = new MouseDelta();

        private Vector3 lastMousePosition;
        private bool wasFocused;
        private bool wasCursorLocked;

        #region BaseInputDeviceManager Implementation

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public InputSimulationService(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : this(inputSystem, name, priority, profile)
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public InputSimulationService(
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(inputSystem, name, priority, profile) { }

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            switch (capability)
            {
                case MixedRealityCapability.ArticulatedHand:
                    return (ControllerSimulationMode == ControllerSimulationMode.ArticulatedHand);

                case MixedRealityCapability.GGVHand:
                    // If any hand simulation is enabled, GGV interactions are supported.
                    return (ControllerSimulationMode != ControllerSimulationMode.Disabled);

                case MixedRealityCapability.EyeTracking:
                    return EyeGazeSimulationMode != EyeGazeSimulationMode.Disabled;

                case MixedRealityCapability.MotionController:
                    return ControllerSimulationMode == ControllerSimulationMode.MotionController;
            }

            return false;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            ControllerSimulationMode = InputSimulationProfile.DefaultControllerSimulationMode;
            EyeGazeSimulationMode = InputSimulationProfile.DefaultEyeGazeSimulationMode;
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();
        }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            var profile = InputSimulationProfile;

            if (indicators == null && profile.IndicatorsPrefab)
            {
                indicators = GameObject.Instantiate(profile.IndicatorsPrefab);
            }

            ResetMouseDelta();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            if (indicators)
            {
                UnityEngine.Object.Destroy(indicators);
            }

            DisableCameraControl();
            DisableControllerSimulation();
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            var profile = InputSimulationProfile;

            switch (ControllerSimulationMode)
            {
                case ControllerSimulationMode.Disabled:
                    DisableControllerSimulation();
                    break;

                case ControllerSimulationMode.ArticulatedHand:
                case ControllerSimulationMode.HandGestures:
                    EnableHandSimulation();
                    break;

                case ControllerSimulationMode.MotionController:
                    EnableMotionControllerSimulation();
                    break;
            }

            // If an XRDevice is present, the user will not be able to control the camera
            // as it is controlled by the device. We therefore disable camera controls in
            // this case.
            // This was causing issues while simulating in editor for VR, as the UpDown
            // camera movement is mapped to controller AXIS_3, which happens to be the 
            // select trigger for WMR controllers.
            if (profile.IsCameraControlEnabled && !DeviceUtility.IsPresent)
            {
                EnableCameraControl();
            }
            else
            {
                DisableCameraControl();
            }

            UpdateMouseDelta();

            if (UserInputEnabled)
            {
                if (dataProvider != null)
                {
                    if (dataProvider is SimulatedHandDataProvider handDataProvider)
                    {
                        handDataProvider.UpdateHandData(HandDataLeft, HandDataRight, HandDataGaze, mouseDelta);
                    }
                    else if (dataProvider is SimulatedMotionControllerDataProvider controllerDataProvider)
                    {
                        controllerDataProvider.UpdateControllerData(MotionControllerDataLeft, MotionControllerDataRight, mouseDelta);
                    }

                }

                if (cameraControl != null && CameraCache.Main != null)
                {
                    cameraControl.UpdateTransform(CameraCache.Main.transform, mouseDelta);
                }
            }

            switch (EyeGazeSimulationMode)
            {
                case EyeGazeSimulationMode.Disabled:
                    break;
                case EyeGazeSimulationMode.CameraForwardAxis:
                    // In the simulated eye gaze condition, let's set the eye tracking calibration status automatically to true
                    Service?.EyeGazeProvider?.UpdateEyeTrackingStatus(this, true);
                    Service?.EyeGazeProvider?.UpdateEyeGaze(this, new Ray(CameraCache.Main.transform.position, CameraCache.Main.transform.forward), DateTime.UtcNow);
                    break;
                case EyeGazeSimulationMode.Mouse:
                    // In the simulated eye gaze condition, let's set the eye tracking calibration status automatically to true
                    Service?.EyeGazeProvider?.UpdateEyeTrackingStatus(this, true);
                    Service?.EyeGazeProvider?.UpdateEyeGaze(this, CameraCache.Main.ScreenPointToRay(UnityEngine.Input.mousePosition), DateTime.UtcNow);
                    break;
            }
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            base.LateUpdate();

            var profile = InputSimulationProfile;

            // Apply hand data in LateUpdate to ensure external changes are applied.
            // HandDataLeft/Right can be modified after the services Update() call.
            if (ControllerSimulationMode == ControllerSimulationMode.Disabled)
            {
                RemoveAllControllerDevices();
            }
            else
            {
                DateTime currentTime = DateTime.UtcNow;
                double msSinceLastControllerUpdate = currentTime.Subtract(new DateTime(lastControllerUpdateTimestamp)).TotalMilliseconds;
                // TODO implement custom hand device update frequency here, use 1000/fps instead of 0
                if (msSinceLastControllerUpdate > 0)
                {
                    object controllerDataLeft = null;
                    object controllerDataRight = null;
                    switch (ControllerSimulationMode)
                    {
                        case ControllerSimulationMode.ArticulatedHand:
                        case ControllerSimulationMode.HandGestures:
                            controllerDataLeft = HandDataLeft;
                            controllerDataRight = HandDataRight;
                            break;

                        case ControllerSimulationMode.MotionController:
                            controllerDataLeft = MotionControllerDataLeft;
                            controllerDataRight = MotionControllerDataRight;
                            break;
                    }
                    UpdateControllerDevice(ControllerSimulationMode, Handedness.Left, controllerDataLeft);
                    UpdateControllerDevice(ControllerSimulationMode, Handedness.Right, controllerDataRight);

                    // HandDataGaze is only enabled if the user is simulating via mouse and keyboard
                    if (UserInputEnabled && profile.IsHandsFreeInputEnabled)
                        UpdateControllerDevice(ControllerSimulationMode.HandGestures, Handedness.None, HandDataGaze);
                    lastControllerUpdateTimestamp = currentTime.Ticks;
                }
            }
        }

        #endregion BaseInputDeviceManager Implementation

        private MixedRealityInputSimulationProfile inputSimulationProfile = null;

        /// <inheritdoc/>
        public MixedRealityInputSimulationProfile InputSimulationProfile
        {
            get
            {
                if (inputSimulationProfile == null)
                {
                    inputSimulationProfile = ConfigurationProfile as MixedRealityInputSimulationProfile;
                }
                return inputSimulationProfile;
            }
            set
            {
                inputSimulationProfile = value;
            }
        }

        /// <inheritdoc/>
        IMixedRealityEyeSaccadeProvider IMixedRealityEyeGazeDataProvider.SaccadeProvider => null;

        /// <inheritdoc/>
        bool IMixedRealityEyeGazeDataProvider.SmoothEyeTracking { get; set; }

        private void EnableCameraControl()
        {
            if (cameraControl == null)
            {
                cameraControl = new ManualCameraControl(InputSimulationProfile);

                if (CameraCache.Main != null)
                {
                    cameraControl.SetInitialTransform(CameraCache.Main.transform);
                }
            }
        }

        private void DisableCameraControl()
        {
            if (cameraControl != null)
            {
                cameraControl = null;
            }
        }

        private void EnableHandSimulation()
        {
            if (dataProvider == null)
            {
                DebugUtilities.LogVerbose("Creating a new hand simulation data provider");
                dataProvider = new SimulatedHandDataProvider(InputSimulationProfile);
            }
            else if (dataProvider is SimulatedMotionControllerDataProvider)
            {
                DebugUtilities.LogVerbose("Replacing motion controller simulation data provider with hand simulation data provider");
                RemoveAllControllerDevices();
                dataProvider = new SimulatedHandDataProvider(InputSimulationProfile);
            }
        }

        private void EnableMotionControllerSimulation()
        {
            if (dataProvider == null)
            {
                DebugUtilities.LogVerbose("Creating a new motion controller simulation data provider");
                dataProvider = new SimulatedMotionControllerDataProvider(InputSimulationProfile);
            }
            else if (dataProvider is SimulatedHandDataProvider)
            {
                DebugUtilities.LogVerbose("Replacing hand simulation data provider with motion controller simulation data provider");
                RemoveAllControllerDevices();
                dataProvider = new SimulatedMotionControllerDataProvider(InputSimulationProfile);
            }

        }

        private void DisableControllerSimulation()
        {
            RemoveAllControllerDevices();

            if (dataProvider != null)
            {
                DebugUtilities.LogVerbose("Destroying the controller simulation data provider");
                dataProvider = null;
            }
        }

        private void ResetMouseDelta()
        {
            lastMousePosition = UnityEngine.Input.mousePosition;

            mouseDelta.Reset();
        }

        private void UpdateMouseDelta()
        {
            var profile = InputSimulationProfile;

            bool isFocused = Application.isFocused;
            bool gainedFocus = !wasFocused && isFocused;
            wasFocused = isFocused;

            bool isCursorLocked = UnityEngine.Cursor.lockState != CursorLockMode.None;
            bool cursorLockChanged = wasCursorLocked != isCursorLocked;
            wasCursorLocked = isCursorLocked;

            // Reset in cases where mouse position is jumping
            if (gainedFocus || cursorLockChanged)
            {
                ResetMouseDelta();
            }
            else
            {
                Vector3 screenDelta;
                Vector3 worldDelta;
                if (UnityEngine.Cursor.lockState == CursorLockMode.Locked)
                {
                    screenDelta.x = UnityEngine.Input.GetAxis(profile.MouseX);
                    screenDelta.y = UnityEngine.Input.GetAxis(profile.MouseY);

                    worldDelta.z = UnityEngine.Input.GetAxis(profile.MouseScroll);
                }
                else
                {
                    // Use frame-to-frame mouse delta in pixels to determine mouse rotation.
                    // The traditional GetAxis("Mouse X") method doesn't work under Remote Desktop.
                    screenDelta.x = (UnityEngine.Input.mousePosition.x - lastMousePosition.x);
                    screenDelta.y = (UnityEngine.Input.mousePosition.y - lastMousePosition.y);

                    worldDelta.z = UnityEngine.Input.mouseScrollDelta.y;
                }

                // Interpret scroll values as world space delta
                worldDelta.z *= profile.ControllerDepthMultiplier;

                Vector2 worldDepthDelta = new Vector2(worldDelta.z, 0);

                // Convert world space scroll delta into screen space pixels
                screenDelta.z = WorldToScreen(worldDepthDelta).x;

                // Convert screen space x/y delta into world space
                Vector2 worldDelta2D = ScreenToWorld(screenDelta);
                worldDelta.x = worldDelta2D.x;
                worldDelta.y = worldDelta2D.y;

                // Viewport delta x and y can be computed from screen x/y.
                // Note that the conversion functions do not change Z, it is expected to always be in world space units.
                Vector3 viewportDelta = CameraCache.Main.ScreenToViewportPoint(screenDelta);
                // Compute viewport-scale z delta
                viewportDelta.z = WorldToViewport(worldDepthDelta).x;

                lastMousePosition = UnityEngine.Input.mousePosition;

                mouseDelta.screenDelta = screenDelta;
                mouseDelta.worldDelta = worldDelta;
                mouseDelta.viewportDelta = viewportDelta;
            }
        }

        // Default world-space distance for converting screen/viewport scroll offsets into world space depth offset.
        // The pixel-to-world-unit ratio changes with depth, so have to chose a fixed distance for conversion.
        // Center of the viewport is at (0.5, 0.5)
        private const float mouseWorldDepth = 0.5f;

        private Vector2 ScreenToWorld(Vector3 screenDelta)
        {
            Vector3 deltaViewport3D = new Vector3(
                screenDelta.x / (0.5f * CameraCache.Main.pixelWidth),
                screenDelta.y / (0.5f * CameraCache.Main.pixelHeight),
                1) * mouseWorldDepth;

            var invProjMat = Matrix4x4.Inverse(CameraCache.Main.projectionMatrix);
            Vector3 deltaWorld3D = invProjMat * deltaViewport3D;

            return new Vector2(deltaWorld3D.x, deltaWorld3D.y);
        }

        private Vector2 WorldToScreen(Vector2 deltaWorld)
        {
            Vector3 deltaWorld3D = new Vector3(deltaWorld.x, deltaWorld.y, mouseWorldDepth);

            Vector4 proj = CameraCache.Main.projectionMatrix * deltaWorld3D;
            Vector3 deltaViewport3D = -proj / proj.w;

            return new Vector2(
                deltaViewport3D.x * CameraCache.Main.pixelWidth,
                deltaViewport3D.y * CameraCache.Main.pixelHeight);
        }

        private Vector2 WorldToViewport(Vector2 deltaWorld)
        {
            Vector3 deltaWorld3D = new Vector3(deltaWorld.x, deltaWorld.y, mouseWorldDepth);

            Vector4 proj = CameraCache.Main.projectionMatrix * deltaWorld3D;
            Vector3 deltaViewport3D = -proj / proj.w;

            return new Vector2(deltaViewport3D.x, deltaViewport3D.y);
        }

        #region Obsolete Properties and Methods
        /// <inheritdoc />
        [Obsolete("Use ControllerSimulationMode instead.")]
        public HandSimulationMode HandSimulationMode
        {
            get => (HandSimulationMode)ControllerSimulationMode;
            set
            {
                ControllerSimulationMode = (ControllerSimulationMode)value;
            }
        }

        /// <inheritdoc />
        [Obsolete("Use IsSimulatingControllerLeft instead.")]
        public bool IsSimulatingHandLeft => IsSimulatingControllerLeft;
        /// <inheritdoc />
        [Obsolete("Use IsSimulatingControllerRight instead.")]
        public bool IsSimulatingHandRight => IsSimulatingControllerRight;

        /// <inheritdoc />
        [Obsolete("Use IsAlwaysVisibleControllerLeft instead.")]
        public bool IsAlwaysVisibleHandLeft
        {
            get => IsAlwaysVisibleControllerLeft;
            set { IsAlwaysVisibleControllerLeft = value; }
        }

        /// <inheritdoc />
        [Obsolete("Use IsAlwaysVisibleControllerRight instead.")]
        public bool IsAlwaysVisibleHandRight
        {
            get => IsAlwaysVisibleControllerRight;
            set { IsAlwaysVisibleControllerRight = value; }
        }

        /// <inheritdoc />
        [Obsolete("Use ControllerPositionLeft instead.")]
        public Vector3 HandPositionLeft
        {
            get => ControllerPositionLeft;
            set { ControllerPositionLeft = value; }
        }

        /// <inheritdoc />
        [Obsolete("Use ControllerPositionRight instead.")]
        public Vector3 HandPositionRight
        {
            get => ControllerPositionRight;
            set { ControllerPositionRight = value; }
        }

        /// <inheritdoc />
        [Obsolete("Use ControllerRotationLeft instead.")]
        public Vector3 HandRotationLeft
        {
            get => ControllerRotationLeft;
            set { ControllerRotationLeft = value; }
        }

        /// <inheritdoc />
        [Obsolete("Use ControllerRotationRight instead.")]
        public Vector3 HandRotationRight
        {
            get => ControllerRotationRight;
            set { ControllerRotationRight = value; }
        }

        /// <inheritdoc />
        [Obsolete("Use ResetControllerLeft instead.")]
        public void ResetHandLeft()
        {
            ResetControllerLeft();
        }

        /// <inheritdoc />
        [Obsolete("Use ResetControllerRight instead.")]
        public void ResetHandRight()
        {
            ResetControllerRight();
        }
        #endregion
    }
}
