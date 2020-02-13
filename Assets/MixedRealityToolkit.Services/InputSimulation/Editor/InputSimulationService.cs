// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;
using UnityEngine.XR;

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
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/InputSimulation/InputSimulationService.html")]
    public class InputSimulationService :
        BaseInputSimulationService,
        IInputSimulationService,
        IMixedRealityEyeGazeDataProvider,
        IMixedRealityCapabilityCheck
    {
        private ManualCameraControl cameraControl = null;
        private SimulatedHandDataProvider handDataProvider = null;

        private HandSimulationMode handSimulationMode;
        /// <inheritdoc />
        public HandSimulationMode HandSimulationMode
        {
            get => handSimulationMode;
            set
            {
                handSimulationMode = value;
            }
        }

        /// <inheritdoc />
        public SimulatedHandData HandDataLeft { get; } = new SimulatedHandData();
        /// <inheritdoc />
        public SimulatedHandData HandDataRight { get; } = new SimulatedHandData();

        /// <inheritdoc />
        public bool IsSimulatingHandLeft => (handDataProvider != null ? handDataProvider.IsSimulatingLeft : false);
        /// <inheritdoc />
        public bool IsSimulatingHandRight => (handDataProvider != null ? handDataProvider.IsSimulatingRight : false);

        /// <inheritdoc />
        public bool IsAlwaysVisibleHandLeft
        {
            get { return handDataProvider != null ? handDataProvider.IsAlwaysVisibleLeft : false; }
            set { if (handDataProvider != null) { handDataProvider.IsAlwaysVisibleLeft = value; } }
        }

        /// <inheritdoc />
        public bool IsAlwaysVisibleHandRight
        {
            get { return handDataProvider != null ? handDataProvider.IsAlwaysVisibleRight : false; }
            set { if (handDataProvider != null) { handDataProvider.IsAlwaysVisibleRight = value; } }
        }

        /// <inheritdoc />
        public Vector3 HandPositionLeft
        {
            get { return handDataProvider != null ? handDataProvider.HandStateLeft.ViewportPosition : Vector3.zero; }
            set { if (handDataProvider != null) { handDataProvider.HandStateLeft.ViewportPosition = value; } }
        }

        /// <inheritdoc />
        public Vector3 HandPositionRight
        {
            get { return handDataProvider != null ? handDataProvider.HandStateRight.ViewportPosition : Vector3.zero; }
            set { if (handDataProvider != null) { handDataProvider.HandStateRight.ViewportPosition = value; } }
        }

        /// <inheritdoc />
        public Vector3 HandRotationLeft
        {
            get { return handDataProvider != null ? handDataProvider.HandStateLeft.ViewportRotation : Vector3.zero; }
            set { if (handDataProvider != null) { handDataProvider.HandStateLeft.ViewportRotation = value; } }
        }

        /// <inheritdoc />
        public Vector3 HandRotationRight
        {
            get { return handDataProvider != null ? handDataProvider.HandStateRight.ViewportRotation : Vector3.zero; }
            set { if (handDataProvider != null) { handDataProvider.HandStateRight.ViewportRotation = value; } }
        }

        /// <inheritdoc />
        public void ResetHandLeft()
        {
            if (handDataProvider != null)
            {
                handDataProvider.ResetHand(Handedness.Left);
            }
        }

        /// <inheritdoc />
        public void ResetHandRight()
        {
            if (handDataProvider != null)
            {
                handDataProvider.ResetHand(Handedness.Right);
            }
        }

        /// <summary>
        /// If true then keyboard and mouse input are used to simulate hands.
        /// </summary>
        public bool UserInputEnabled { get; set; } = true;

        /// <summary>
        /// Timestamp of the last hand device update
        /// </summary>
        private long lastHandUpdateTimestamp = 0;

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
                    return (HandSimulationMode == HandSimulationMode.Articulated);

                case MixedRealityCapability.GGVHand:
                    // If any hand simulation is enabled, GGV interactions are supported.
                    return (HandSimulationMode != HandSimulationMode.Disabled);

                case MixedRealityCapability.EyeTracking:
                    return InputSimulationProfile.SimulateEyePosition;
            }

            return false;
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize();

            ArticulatedHandPose.LoadGesturePoses();

            HandSimulationMode = InputSimulationProfile.DefaultHandSimulationMode;
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            base.Destroy();

            ArticulatedHandPose.ResetGesturePoses();
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
                GameObject.Destroy(indicators);
            }

            DisableCameraControl();
            DisableHandSimulation();
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            var profile = InputSimulationProfile;

            switch (HandSimulationMode)
            {
                case HandSimulationMode.Disabled:
                    DisableHandSimulation();
                    break;

                case HandSimulationMode.Articulated:
                case HandSimulationMode.Gestures:
                    EnableHandSimulation();
                    break;
            }

            // If an XRDevice is present, the user will not be able to control the camera
            // as it is controlled by the device. We therefore disable camera controls in
            // this case.
            // This was causing issues while simulating in editor for VR, as the UpDown
            // camera movement is mapped to controller AXIS_3, which happens to be the 
            // select trigger for WMR controllers.
            if (profile.IsCameraControlEnabled && !XRDevice.isPresent)
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
                if (handDataProvider != null)
                {
                    handDataProvider.UpdateHandData(HandDataLeft, HandDataRight, mouseDelta);
                }

                if (cameraControl != null && CameraCache.Main)
                {
                    cameraControl.UpdateTransform(CameraCache.Main.transform, mouseDelta);
                }
            }

            if (profile.SimulateEyePosition)
            {
                // In the simulated eye gaze condition, let's set the eye tracking calibration status automatically to true
                Service?.EyeGazeProvider?.UpdateEyeTrackingStatus(this, true);

                // Update the simulated eye gaze with the current camera position and forward vector
                Service?.EyeGazeProvider?.UpdateEyeGaze(this, new Ray(CameraCache.Main.transform.position, CameraCache.Main.transform.forward), DateTime.UtcNow);
            }
        }

        /// <inheritdoc />
        public override void LateUpdate()
        {
            base.LateUpdate();

            var profile = InputSimulationProfile;

            // Apply hand data in LateUpdate to ensure external changes are applied.
            // HandDataLeft/Right can be modified after the services Update() call.
            if (HandSimulationMode == HandSimulationMode.Disabled)
            {
                RemoveAllHandDevices();
            }
            else
            {
                DateTime currentTime = DateTime.UtcNow;
                double msSinceLastHandUpdate = currentTime.Subtract(new DateTime(lastHandUpdateTimestamp)).TotalMilliseconds;
                // TODO implement custom hand device update frequency here, use 1000/fps instead of 0
                if (msSinceLastHandUpdate > 0)
                {
                    UpdateHandDevice(HandSimulationMode, Handedness.Left, HandDataLeft);
                    UpdateHandDevice(HandSimulationMode, Handedness.Right, HandDataRight);

                    lastHandUpdateTimestamp = currentTime.Ticks;
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
            if (handDataProvider == null)
            {
                handDataProvider = new SimulatedHandDataProvider(InputSimulationProfile);
            }
        }

        private void DisableHandSimulation()
        {
            RemoveAllHandDevices();

            if (handDataProvider != null)
            {
                handDataProvider = null;
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
                worldDelta.z *= profile.HandDepthMultiplier;

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
    }
}
