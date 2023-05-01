// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Microsoft.MixedReality.Toolkit.Input.Simulation
{
    /// <summary>
    /// Input device and HMD navigation simulator.
    /// </summary>
    [AddComponentMenu("MRTK/Input/Input Simulator")]
    public class InputSimulator : MonoBehaviour
    {
        #region MonoBehaviour

        private void Awake()
        {
            ApplyControlSet(ControlSet);
        }

        private static readonly ProfilerMarker UpdatePerfMarker =
            new ProfilerMarker("[MRTK] InputSimulator.Update");

        private bool isSimulating = true;

        private void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                bool shouldSimulate = !XRDisplaySubsystemHelpers.AreAnyActive();
                if (isSimulating != shouldSimulate)
                {
                    isSimulating = shouldSimulate;

                    if (!isSimulating)
                    {
                        DisableSimulatedHMD();
                        DisableSimulatedEyeGaze();
                        DisableSimulatedController(Handedness.Left);
                        DisableSimulatedController(Handedness.Right);
                    }
                }

                if (!isSimulating)
                {
                    return;
                }

                // Camera
                if (cameraSettings.SimulationEnabled)
                {
                    EnableSimulatedHMD();
                    UpdateSimulatedHMD();
                }
                else
                {
                    DisableSimulatedHMD();
                }

                // Eyes
                if (eyeGazeSettings.SimulationEnabled)
                {
                    EnableSimulatedEyeGaze();
                    UpdateSimulatedEyeGaze();
                }
                else
                {
                    DisableSimulatedEyeGaze();
                }

                // Controllers/Hands
                if (leftControllerSettings.SimulationMode != ControllerSimulationMode.Disabled)
                {
                    UpdateSimulatedController(Handedness.Left);
                }
                else
                {
                    DisableSimulatedController(Handedness.Left);
                }

                if (rightControllerSettings.SimulationMode != ControllerSimulationMode.Disabled)
                {
                    UpdateSimulatedController(Handedness.Right);
                }
                else
                {
                    DisableSimulatedController(Handedness.Right);
                }
            }
        }

        private void OnDisable()
        {
            DisableSimulatedHMD();
            DisableSimulatedEyeGaze();
            DisableSimulatedController(Handedness.Left);
            DisableSimulatedController(Handedness.Right);
        }

        #endregion MonoBehaviour

        #region Camera

        private SimulatedHMD simulatedHMD = null;

        [SerializeField]
        [Tooltip("Input simulation control compatibility set")]
        private SimulatorControlScheme controlSet = null;

        /// <summary>
        /// Input simulation control compatibility set.
        /// </summary>
        public SimulatorControlScheme ControlSet
        {
            get => controlSet;
            set => controlSet = value;
        }

        [SerializeField]
        private CameraSimulationSettings cameraSettings;

        /// <summary>
        /// The settings used to configure and control the simulated camera.
        /// </summary>
        public CameraSimulationSettings CameraSettings
        {
            get => cameraSettings;
            set => cameraSettings = value;
        }

        /// <summary>
        /// Enables simulated camera control.
        /// </summary>
        /// <remarks>
        /// This method creates the camera simulation object(s) as needed. If called while
        /// already enabled, this method does nothing.
        /// </remarks>
        private void EnableSimulatedHMD()
        {
            simulatedHMD ??= new SimulatedHMD();
        }

        /// <summary>
        /// Disables simulated camera control.
        /// </summary>
        /// <remarks>
        /// This method cleans up the camera simulation object(s) as needed. If called while
        /// already enabled, this method does nothing.
        /// </remarks>
        private void DisableSimulatedHMD()
        {
            if (simulatedHMD != null)
            {
                simulatedHMD.Dispose();
                simulatedHMD = null;
            }
        }

        private static readonly ProfilerMarker UpdateSimulatedHMDPerfMarker =
            new ProfilerMarker("[MRTK] InputSimulator.UpdateSimulatedHMD");

        /// <summary>
        /// Updates the camera simulation.
        /// </summary>
        private void UpdateSimulatedHMD()
        {
            if (simulatedHMD == null) { return; }

            using (UpdateSimulatedHMDPerfMarker.Auto())
            {
                // Get the position change
                Vector3 positionDelta = new Vector3(
                    CameraSettings.MoveHorizontal.action.ReadValue<float>(),
                    CameraSettings.MoveVertical.action.ReadValue<float>(),
                    CameraSettings.MoveDepth.action.ReadValue<float>());

                // Get the rotation change
                Vector3 rotationDelta = new Vector3(
                    // Unity inverts the camera pitch by default, mirroring how the neck works (move forward to look down)
                    CameraSettings.Pitch.action.ReadValue<float>() * (!CameraSettings.InvertPitch ? -1 : 1),
                    CameraSettings.Yaw.action.ReadValue<float>(),
                    CameraSettings.Roll.action.ReadValue<float>());

                // Update the simulated camera
                simulatedHMD.Update(
                    positionDelta * 0.1f,
                    rotationDelta,
                    CameraSettings.IsMovementSmoothed,
                    CameraSettings.MoveSpeed,
                    CameraSettings.RotationSensitivity);
            }
        }

        #endregion Camera

        #region Eyes

        private SimulatedEyeGaze simulatedEyeGaze = null;

        [SerializeField]
        private EyeGazeSimulationSettings eyeGazeSettings;

        /// <summary>
        /// The settings used to configure and control the simulated eye gaze.
        /// </summary>
        public EyeGazeSimulationSettings EyeGazeSettings
        {
            get => eyeGazeSettings;
            set => eyeGazeSettings = value;
        }

        /// <summary>
        /// Enables simulated eye gaze.
        /// </summary>
        /// <remarks>
        /// This method creates the eye gaze simulation object(s) as needed. If called while
        /// already enabled, this method does nothing.
        /// </remarks>
        private void EnableSimulatedEyeGaze()
        {
            simulatedEyeGaze ??= new SimulatedEyeGaze();
        }

        /// <summary>
        /// Disables simulated eye gaze.
        /// </summary>
        /// <remarks>
        /// This method cleans up the eye gaze simulation object(s) as needed. If called while
        /// already disabled, this method does nothing.
        /// </remarks>
        private void DisableSimulatedEyeGaze()
        {
            if (simulatedEyeGaze != null)
            {
                simulatedEyeGaze.Dispose();
                simulatedEyeGaze = null;
            }
        }

        private static readonly ProfilerMarker UpdateSimulatedEyesPerfMarker =
            new ProfilerMarker("[MRTK] InputSimulator.UpdateSimulatedEyes");

        /// <summary>
        /// Updates the eye gaze simulation.
        /// </summary>
        private void UpdateSimulatedEyeGaze()
        {
            if (simulatedEyeGaze == null) { return; }

            using (UpdateSimulatedEyesPerfMarker.Auto())
            {
                Vector3 lookDelta = new Vector3(
                    -eyeGazeSettings.LookVertical.action.ReadValue<float>(),
                    eyeGazeSettings.LookHorizontal.action.ReadValue<float>(),
                    0f);

                simulatedEyeGaze.Update(
                    true,
                    lookDelta,
                    eyeGazeSettings.EyeOriginOffset);
            }
        }

        #endregion Eyes

        #region Controllers

        private SimulatedController simulatedLeftController = null;
        private float leftTriggerSmoothVelocity;
        private SimulatedController simulatedRightController = null;
        private float rightTriggerSmoothVelocity;

        private ControllerControls leftControllerControls = new ControllerControls();
        private ControllerControls rightControllerControls = new ControllerControls();

        [SerializeField]
        private ControllerSimulationSettings leftControllerSettings;

        /// <summary>
        /// The settings used to configure and control the simulated left hand/controller.
        /// </summary>
        public ControllerSimulationSettings LeftControllerSettings
        {
            get => leftControllerSettings;
            set => leftControllerSettings = value;
        }

        [SerializeField]
        private ControllerSimulationSettings rightControllerSettings;

        /// <summary>
        /// The settings used to configure and control the simulated right hand/controller.
        /// </summary>
        public ControllerSimulationSettings RightControllerSettings
        {
            get => rightControllerSettings;
            set => rightControllerSettings = value;
        }

        // Should we pass the trigger button straight through to the device?
        // This will not smooth the trigger press; typically, we should
        // modulate the trigger axis control ourselves for smooth pinch/unpinch.
        private bool shouldUseTriggerButton = false;

        // TODO: Drive from inspector/simulator options.
        private float triggerSmoothTime = 0.1f;

        // TODO: Drive from inspector/simulator options.
        private float triggerSmoothDeadzone = 0.005f;

        /// <summary>
        /// Enables the simulated controller.
        /// </summary>
        /// <param name="handedness">
        /// The <see cref="Handedness"/> of the controller to be enabled.
        /// </param>
        /// <remarks>
        /// This method creates the controller simulation object(s) as needed. If called while
        /// already enabled, this method does nothing.
        /// </remarks>
        /// <returns>
        /// Current, or created, <see cref="SimulatedController"/> instance or null.
        /// </returns>
        private SimulatedController EnableSimulatedController(
            Handedness handedness,
            ControllerSimulationSettings ctrlSettings,
            Vector3 startPosition)
        {
            if (!IsSupportedHandedness(handedness))
            {
                Debug.LogError($"Unable to enable simulated controller. Unsupported handedness ({handedness}).");
                return null;
            }

            ref SimulatedController simCtrl = ref GetControllerReference(handedness);
            if (simCtrl != null) { return simCtrl; }
            simCtrl = new SimulatedController(handedness, ctrlSettings, startPosition);

            ControllerControls controls = GetControllerControls(handedness);
            controls.Reset();

            return simCtrl;
        }

        /// <summary>
        /// Disables the simulated controller.
        /// </summary>
        /// <param name="handedness">
        /// The <see cref="Handedness"/> of the controller to be disabled.
        /// </param>
        /// <remarks>
        /// This method cleans up controller simulation object(s) as needed. If called while
        /// already enabled, this method does nothing.
        /// </remarks>
        private void DisableSimulatedController(Handedness handedness)
        {
            if (!IsSupportedHandedness(handedness))
            {
                Debug.Log($"Unable to disable simulated controller. Unsupported handedness ({handedness}).");
                return;
            }

            ref SimulatedController simCtrl = ref GetControllerReference(handedness);
            if (simCtrl == null) { return; }

            simCtrl.Dispose();
            simCtrl = null;
        }

        private static readonly ProfilerMarker UpdateSimulatedControllerPerfMarker =
            new ProfilerMarker("[MRTK] InputSimulator.UpdateSimulatedController");

        private static readonly Quaternion NoRotation = Quaternion.Euler(0f, 0f, 0f);

        /// <summary>
        /// Updates the controller simulation.
        /// </summary>
        /// <param name="handedness">
        /// The <see cref="Handedness"/> of the controller to be updated.
        /// </param>
        private void UpdateSimulatedController(Handedness handedness)
        {
            using (UpdateSimulatedControllerPerfMarker.Auto())
            {
                if (!IsSupportedHandedness(handedness))
                {
                    Debug.Log($"Unable to update the simulated controller. Unsupported handedness ({handedness}).");
                    return;
                }

                ControllerSimulationSettings ctrlSettings = (handedness == Handedness.Left) ?
                    leftControllerSettings : rightControllerSettings;
                if (ctrlSettings == null) { return; }

                // Has the user toggled latched tracking?
                if (ctrlSettings.Toggle.action.WasPerformedThisFrame())
                {
                    ctrlSettings.ToggleState = !ctrlSettings.ToggleState;
                }

                // Is momentary tracking enabled?
                bool isTracked = ctrlSettings.Track.action.IsPressed();

                ref SimulatedController simCtrl = ref GetControllerReference(handedness);

                if (ctrlSettings.ToggleState || isTracked)
                {
                    if (simCtrl == null)
                    {
                        // Get the start position for the controller.
                        Vector3 startPosition = ctrlSettings.DefaultPosition;
                        if (isTracked)
                        {
                            Vector3 screenPos = new Vector3(
                                Mouse.current.position.ReadValue().x,
                                Mouse.current.position.ReadValue().y,
                                ctrlSettings.DefaultPosition.z);
                            startPosition = ScreenToCameraRelative(screenPos);
                        }

                        // Create the simulated controller.
                        simCtrl = EnableSimulatedController(
                            handedness,
                            ctrlSettings,
                            startPosition);
                    }
                }
                else
                {
                    DisableSimulatedController(handedness);
                    simCtrl = null;
                }

                if (simCtrl == null) { return; }

                // Has the user asked to change the neutral pose?
                if (ctrlSettings.ToggleSecondaryHandshapes.action.WasPerformedThisFrame())
                {
                    simCtrl.ToggleNeutralPose();
                }

                // Is our simulated controller controlled by a mouse? If so, we should
                // calculate the desired delta from the mouse position on screen.
                bool isControlledByMouse = ctrlSettings.MoveHorizontal.action.RaisedByMouse() ||
                                           ctrlSettings.MoveVertical.action.RaisedByMouse();

                bool isControllingRotation = ctrlSettings.Pitch.action.IsPressed() ||
                                  ctrlSettings.Yaw.action.IsPressed() ||
                                  ctrlSettings.Roll.action.IsPressed();

                Vector3 positionDelta = Vector3.zero;
                Quaternion rotationDelta = NoRotation;

                // Update the rotation mode if the user wants to face the camera
                if (ctrlSettings.FaceTheCamera.action.WasPerformedThisFrame())
                {
                    ctrlSettings.RotationMode = (ctrlSettings.RotationMode == ControllerRotationMode.FaceCamera) ?
                        ControllerRotationMode.CameraAligned : ControllerRotationMode.FaceCamera;
                }
                else if (isControllingRotation)
                {
                    // Ignore position delta if the user is trying to manually rotate the hand
                    rotationDelta = Quaternion.Euler(
                        // Unity appears to invert the controller pitch by default (move forward to look down)
                        ctrlSettings.Pitch.action.ReadValue<float>() * (!ctrlSettings.InvertPitch ? -1 : 1),
                        ctrlSettings.Yaw.action.ReadValue<float>(),
                        ctrlSettings.Roll.action.ReadValue<float>());

                    if (rotationDelta != NoRotation) { ctrlSettings.RotationMode = ControllerRotationMode.UserControl; }
                }
                else
                {
                    if (isControlledByMouse && !ctrlSettings.ToggleState) // If tracking is latched, we do not want to 1:1 track the mouse location.
                    {
                        /* TODO: this needs work, also depth moves the hands towards the vanishing point
                        Vector3 screenDepth = CameraRelativeToScreen(new Vector3(
                            simCtrl.RelativePosition.z,
                            0f, ctrlSettings.DefaultPosition.z));
                        */

                        Vector3 mouseScreenPos = new Vector3(
                            Mouse.current.position.ReadValue().x,
                            Mouse.current.position.ReadValue().y,
                            ctrlSettings.DefaultPosition.z);
                        // TODO: related to the above - screenDepth.x + ctrlSettings.MoveDepth.action.ReadValue<float>());

                        Vector3 inputPosition = ScreenToCameraRelative(mouseScreenPos);

                        positionDelta = inputPosition - simCtrl.CameraRelativePose.position;
                        positionDelta.z = ctrlSettings.MoveDepth.action.ReadValue<float>();
                    }
                    else
                    {
                        positionDelta = new Vector3(
                            ctrlSettings.MoveHorizontal.action.ReadValue<float>(),
                            ctrlSettings.MoveVertical.action.ReadValue<float>(),
                            ctrlSettings.MoveDepth.action.ReadValue<float>());
                    }
                }

                ControllerControls controls = GetControllerControls(handedness);

                ref float triggerSmoothVelocity = ref (handedness == Handedness.Left ? ref leftTriggerSmoothVelocity : ref rightTriggerSmoothVelocity);

                // TODO: Currently triggerAxis is driven only from ctrlSettings.TriggerButton.action.
                // We will eventually drive this from the ctrlSettings.TriggerAxis.action as well.
                // Needs work to be able to intuitively combine trigger axis from sim input with click.
                float targetValue = ctrlSettings.TriggerButton.action.IsPressed() ? 1 : 0;

                controls.TriggerAxis = Mathf.SmoothDamp(controls.TriggerAxis,
                                                        targetValue,
                                                        ref triggerSmoothVelocity,
                                                        triggerSmoothTime);

                if(Mathf.Abs(controls.TriggerAxis - targetValue) < triggerSmoothDeadzone)
                {
                    controls.TriggerAxis = targetValue;
                }
#if LATER
// TODO: mappings need to be sorted out for these
                // Axes available to hands and controllers
                float axisDeltaReading = ctrlSettings.TriggerAxis.action.ReadValue<float>();
                if ((axisDeltaReading != 0) && (controls.TriggerAxis != axisDeltaReading))
                {
                    controls.TriggerAxis += axisDeltaReading;
                }
                axisDeltaReading = ctrlSettings.GripAxis.action.ReadValue<float>();
                if ((axisDeltaReading != 0) && (controls.GripAxis != axisDeltaReading))
                {
                    controls.GripAxis += axisDeltaReading;
                }
#endif // LATER

                // Buttons available to hands and controllers
                // Should we pass the trigger button straight through to the device?
                // This will not smooth the trigger press; typically, we should
                // modulate the trigger axis control ourselves for smooth pinch/unpinch,
                // and this is false.
                if (shouldUseTriggerButton)
                {
                    controls.TriggerButton = ctrlSettings.TriggerButton.action.IsPressed();
                }
                controls.GripButton = ctrlSettings.GripButton.action.IsPressed();

                if (ctrlSettings.SimulationMode == ControllerSimulationMode.MotionController)
                {
                    // Axes available to controllers
                    // TODO: "soon"
                    // controls.Primary2DAxis = ctrlSettings.Primary2DAxis.action.ReadValue(float)();
                    // controls.Secondary2DAxis = ctrlSettings.Secondary2DAxis.action.ReadValue(float)();

                    // Buttons available to controllers
                    // TODO: "soon"
                    // controls.MenuButton = ctrlSettings.MenuButton.action.ReadValue(float)() > 0f;
                    // controls.PrimaryButton = ctrlSettings.PrimaryButton.action.ReadValue(float)() > 0f;
                    // controls.SecondaryButton = ctrlSettings.SecondaryButton.action.ReadValue(float)() > 0f;
                    // controls.PrimaryTouch = ctrlSettings.PrimaryTouch.action.ReadValue(float)() > 0f;
                    // controls.SecondaryTouch = ctrlSettings.SecondaryTouch.action.ReadValue(float)() > 0f;
                    // controls.Primary2DAxisClick = ctrlSettings.Primary2DAxisClick.action.ReadValue(float)() > 0f;
                    // controls.Secondary2DAxisClick = ctrlSettings.Secondary2DAxisClick.action.ReadValue(float)() > 0f;
                    // controls.Primary2DAxisTouch = ctrlSettings.Primary2DAxisTouch.action.ReadValue(float)() > 0f;
                    // controls.Secondary2DAxisTouch = ctrlSettings.Secondary2DAxisTouch.action.ReadValue(float)() > 0f;
                }

                controls.IsTracked = ctrlSettings.ToggleState || isTracked;
                controls.TrackingState = controls.IsTracked ?
                    (InputTrackingState.Position | InputTrackingState.Rotation) : InputTrackingState.None;

                bool shouldUseRayVector = ctrlSettings.SimulationMode == ControllerSimulationMode.ArticulatedHand;

                // determine which offset we are using
                Vector3 anchorPosition = simCtrl.WorldPosition;
                switch (ctrlSettings.AnchorPoint)
                {
                    case ControllerAnchorPoint.Device:
                        anchorPosition = simCtrl.WorldPosition;
                        break;
                    case ControllerAnchorPoint.IndexFinger:
                        anchorPosition = simCtrl.PokePosition;
                        break;
                    case ControllerAnchorPoint.Grab:
                        anchorPosition = simCtrl.GrabPosition;
                        break;
                }

                simCtrl.UpdateRelative(
                    positionDelta,
                    rotationDelta,
                    controls,
                    ctrlSettings.RotationMode,
                    shouldUseRayVector,
                    anchorPosition,
                    ctrlSettings.IsMovementSmoothed,
                    ctrlSettings.DepthSensitivity,
                    ctrlSettings.JitterStrength);
            }
        }

        /// <summary>
        /// Converts a camera relative location  into screen (ex: within the Unity Editor window) space.
        /// </summary>
        /// <param name="cameraRelativePos">The location relative to the camera to convert.</param>
        /// <returns>
        /// Location in screen space.
        /// </returns>
        private Vector3 CameraRelativeToScreen(Vector3 cameraRelativePos)
        {
            // First, convert from the camera space to world space.
            Vector3 worldPos = Camera.main.transform.TransformPoint(cameraRelativePos);

            // Then convert from world to screen space.
            return Camera.main.WorldToScreenPoint(worldPos);
        }

        /// <summary>
        /// Converts a screen location (ex: within the Unity Editor window) into camera relative space.
        /// </summary>
        /// <param name="screenPos">The location on screen to convert.</param>
        /// <returns>
        /// Location relative to the game window's camera space.
        /// </returns>
        private Vector3 ScreenToCameraRelative(Vector3 screenPos)
        {
            // First, convert from the screen space to world space.
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            // Then convert from world to camera relative space.
            return Camera.main.transform.InverseTransformPoint(worldPos);
        }

        /// <summary>
        /// Determines if the specified <see cref="Handedness"/> is supported by the input simulator.
        /// </summary>
        /// <param name="handedness"><see cref="Handedness"/> value (ex: Left).</param>
        /// <returns>
        /// True if the specified <see cref="Handedness"/> is supported, or false.
        /// </returns>
        private bool IsSupportedHandedness(Handedness handedness)
        {
            return !((handedness != Handedness.Left) && (handedness != Handedness.Right));
        }

        private static readonly string UnsupportedHandednessLog = $"Unsupported Handedness. Must be {Handedness.Left} or {Handedness.Right}";

        /// <summary>
        /// Gets a reference to the <see cref="SimulatedController"/> indicated by the
        /// specified <see cref="Handedness"/>.
        /// </summary>
        /// <param name="handedness"><see cref="Handedness"/> value (ex: Left).</param>
        /// <returns>
        /// The <see cref="SimulatedController"/> associated with the specified <see cref="Handedness"/>.
        /// </returns>
        private ref SimulatedController GetControllerReference(Handedness handedness)
        {
            Debug.Assert(
                IsSupportedHandedness(handedness),
                UnsupportedHandednessLog);

            return ref (handedness == Handedness.Left ? ref simulatedLeftController : ref simulatedRightController);
        }

        /// <summary>
        /// Gets a reference to the <see cref="ControllerControls"/> indicated by the
        /// specified <see cref="Handedness"/>.
        /// </summary>
        /// <param name="handedness"><see cref="Handedness"/> value (ex: Left).</param>
        /// <returns>
        /// The <see cref="ControllerControls"/> associated with the specified <see cref="Handedness"/>.
        /// </returns>
        private ControllerControls GetControllerControls(Handedness handedness)
        {
            Debug.Assert(
                IsSupportedHandedness(handedness),
                UnsupportedHandednessLog);

            return handedness == Handedness.Left ? leftControllerControls : rightControllerControls;
        }

        #endregion Controllers

        #region Helpers

        private InputActionManager actionManager = null;

        /// <summary>
        /// Applies the users selected control compatibility set.
        /// </summary>
        /// <param name="set">the <see cref="SimulatorControlScheme"/> to be applied.</param>
        private void ApplyControlSet(SimulatorControlScheme set)
        {
            if (actionManager == null)
            {
                if (!TryGetComponent(out actionManager))
                {
                    Debug.LogError("[InputSimulator] No InputActionManager found - unable to apply control set selection.");
                    return;
                }
            }

            int assetCount = actionManager.actionAssets.Count;
            if (assetCount == 0)
            {
                Debug.LogError("[InputSimulator] InputActionManager has no registered InputActionAssets - unable to apply control set selection.");
                return;
            }

            for (int i = 0; i < assetCount; i++)
            {
                actionManager.actionAssets[i].bindingMask = new InputBinding() { groups = GetControlSetName(set) };
            }
        }

        /// <summary>
        /// Gets the display name for the simulator control set.
        /// </summary>
        /// <param name="set">the <see cref="SimulatorControlScheme"/> for which the name is being requested.</param>
        /// <returns>The <see cref="SimulatorControlScheme"/>'s display name.</returns>
        private string GetControlSetName(SimulatorControlScheme set)
        {
            return set.name;
        }

        // This is a utility method called by XRSimulatedHMD and XRSimulatedController
        // since both devices share the same command handling. This intercepts sync commands
        // and returns GenericSuccess. Otherwise, simulated devices will fail to sync and
        // cause issues like wonky state after tabbing out/tabbing in.
        internal static unsafe bool TryExecuteCommand(InputDeviceCommand* commandPtr, out long result)
        {
            // This replicates the logic in XRToISXDevice::IOCTL (XRInputToISX.cpp).
            var type = commandPtr->type;
            if (type == RequestSyncCommand.Type)
            {
                // The state is maintained by XRDeviceSimulator, so no need for any change
                // when focus is regained. Returning success instructs Input System to not
                // reset the device.
                result = InputDeviceCommand.GenericSuccess;
                return true;
            }

            if (type == QueryCanRunInBackground.Type)
            {
                // This ensures all simulated devices always report canRunInBackground = true,
                // regardless of whether they were explicitly marked as such.
                ((QueryCanRunInBackground*)commandPtr)->canRunInBackground = true;
                result = InputDeviceCommand.GenericSuccess;
                return true;
            }

            result = default;
            return false;
        }

        #endregion Helpers
    }
}
