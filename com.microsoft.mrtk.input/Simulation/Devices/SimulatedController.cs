// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

using GestureId = Microsoft.MixedReality.Toolkit.Input.GestureTypes.GestureId;

namespace Microsoft.MixedReality.Toolkit.Input.Simulation
{
    /// <summary>
    /// A derived XRController that adds pointerPosition and pointerRotation,
    /// to achieve parity with most OpenXR-based controllers, including articulated hands.
    /// </summary>
    [InputControlLayout(
        stateType = typeof(MRTKSimulatedControllerState),
        commonUsages = new[] { "LeftHand", "RightHand" },
        isGenericTypeOfDevice = false,
        displayName = "XR Simulated Controller (MRTK)"),
        Preserve]
    internal class MRTKSimulatedController : XRSimulatedController
    {
        /// <summary>
        /// The device's pointerPosition
        /// </summary>
        public Vector3Control PointerPosition { get; private set; }

        /// <summary>
        /// The device's pointerRotation, usually a head-based hand ray on HoloLens 2.
        /// </summary>
        public QuaternionControl PointerRotation { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            PointerPosition = GetChildControl<Vector3Control>(nameof(PointerPosition));
            PointerRotation = GetChildControl<QuaternionControl>(nameof(PointerRotation));
        }
    }

    /// <summary>
    /// Class for simulating motion controller position, rotation and controls. Used by the Input Simulator.
    /// </summary>
    internal class SimulatedController : IDisposable
    {
        private readonly MRTKSimulatedController simulatedController = null;
        private readonly IHandRay handRay = new HandRay();

        private MRTKSimulatedControllerState simulatedControllerState;

        private HandsAggregatorSubsystem handSubsystem = null;
        private HandsAggregatorSubsystem HandSubsystem => handSubsystem ??= HandsUtils.GetSubsystem();

        private SyntheticHandsSubsystem synthHandsSubsystem = null;
        private SyntheticHandsSubsystem SynthHands =>
            synthHandsSubsystem ??= XRSubsystemHelpers.GetFirstRunningSubsystem<SyntheticHandsSubsystem>();

        /// <summary>
        /// Returns the current position, in camera relative space, of the input device which controls this simulated controller.
        /// This is different from the devicePosition, which may be offset from this due to anchoring at a different point
        /// </summary>
        public Vector3 SimulatedInputPosition { get; private set; } = Vector3.zero;

        /// <summary>
        /// Returns the current rotation, in camera relative space, of the input device which controls this simulated controller
        /// This is separated from the deviceRotation, which may be offset from this due to anchoring at a different point
        /// </summary>
        public Quaternion SimulatedInputRotation { get; private set; } = Quaternion.identity;

        /// <summary>
        /// Returns the current position, in worldspace, of the simulated controller.
        /// </summary>
        public Vector3 WorldPosition => PlayspaceUtilities.ReferenceTransform.TransformPoint(simulatedControllerState.devicePosition);

        /// <summary>
        /// Returns the current rotation, in worldspace, of the simulated controller.
        /// </summary>
        public Quaternion WorldRotation => PlayspaceUtilities.ReferenceTransform.rotation * simulatedControllerState.deviceRotation;

        /// <summary>
        /// Returns the position of the index finger joint on the simulated device,
        /// or the pointer position if none is available from the hands aggregator.
        /// Useful for positioning the hand relative to the index joint, for pressing
        /// buttons or interacting with UI.
        /// </summary>
        /// <remarks>
        /// Returned in world space.
        /// </remarks>
        public Vector3 PokePosition
        {
            get
            {
                if (HandSubsystem.TryGetJoint(TrackedHandJoint.IndexTip,
                                              Handedness == Handedness.Left ? XRNode.LeftHand : XRNode.RightHand,
                                              out HandJointPose indexPose))
                {
                    return indexPose.Position;
                }
                else
                {
                    return simulatedControllerState.pointerPosition;
                }
            }
        }

        /// <summary>
        /// Gets the grab position
        /// </summary>
        /// <remarks>
        /// Returned in world space.
        /// </remarks>
        public Vector3 GrabPosition
        {
            get
            {
                if (HandSubsystem.TryGetPinchingPoint(Handedness == Handedness.Left ? XRNode.LeftHand : XRNode.RightHand,
                                                      out HandJointPose grabPose))
                {
                    return grabPose.Position;
                }
                else
                {
                    return simulatedControllerState.pointerPosition;
                }
            }
        }

        /// <summary>
        /// The handedness (ex: Left or Right) assigned to the simulated controller.
        /// </summary>
        public Handedness Handedness { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SimulatedController(
            Handedness handedness,
            Vector3 initialRelativePosition)
        {
            Handedness = handedness;
            simulatedController = InputSystem.AddDevice<MRTKSimulatedController>();
            if (simulatedController == null)
            {
                Debug.LogError("Failed to create the simulated controller.");
                return;
            }
            simulatedControllerState.Reset();

            SimulatedInputPosition = initialRelativePosition;

            SetUsage();

            SetRelativePoseWithOffset(
                initialRelativePosition,
                Quaternion.identity,
                Vector3.zero,
                ControllerRotationMode.UserControl,
                true);
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~SimulatedController()
        {
            Dispose();
        }

        /// <summary>
        /// Cleans up references to resources used by the camera simulation.
        /// </summary>
        public void Dispose()
        {
            if ((simulatedController != null) && simulatedController.added)
            {
                UnsetUsage();
                InputSystem.RemoveDevice(simulatedController);
            }
            GC.SuppressFinalize(this);
        }

        private static readonly ProfilerMarker ToggleNeutralPosePerfMarker =
            new ProfilerMarker("[MRTK] SimulatedController.ToggleNeutralPose");

        /// <summary>
        /// Toggles the simulated hand between flat or ready poses.
        /// </summary>
        public void ToggleNeutralPose()
        {
            using (ToggleNeutralPosePerfMarker.Auto())
            {
                XRNode? handNode = Handedness.ToXRNode();
                if (handNode.HasValue && SynthHands != null)
                {
                    GestureId neutralPose = SynthHands.GetNeutralPose(handNode.Value);
                    SynthHands.SetNeutralPose(
                                            handNode.Value,
                                            (neutralPose == GestureId.Flat) ? GestureId.Open : GestureId.Flat);
                }
                else
                {
                    Debug.LogError($"Failed to toggle the {Handedness} simulated hand neutral pose.");
                }
            }
        }

        private static readonly ProfilerMarker UpdatePerfMarker =
            new ProfilerMarker("[MRTK] SimulatedController.Update");

        // Smoothing time for the controller position.
        private const float MoveSmoothingTime = 0.02f;

        // Smoothed move delta.
        private Vector3 smoothedMoveDelta = Vector3.zero;

        /// <summary>
        /// Update the controller simulation with relative per-frame delta position and rotation.
        /// </summary>
        /// <param name="moveDelta">The change in the controller position.</param>
        /// <param name="rotationDelta">The change in the controller rotation.</param>
        /// <param name="controls">The desired state of the controller's controls.</param>
        /// <param name="rotationMode">The <see cref="ControllerRotationMode"/> in which the controller is operating.</param>
        /// <param name="anchorPosition">The position in world space which the controller should be around on</param>
        /// <param name="shouldUseRayVector">Should pointerRotation be set to the body/head ray vector, instead of the raw rotation?</param>
        /// <param name="isMovementSmoothed">Should controller movement along the Z axis be smoothed?</param>
        /// <param name="depthSensitivity">The sensitivity multiplier for depth movement.</param>
        /// <param name="jitterStrength">How strong should be the simulated controller jitter (inaccuracy)?</param>
        public void UpdateRelative(
            Vector3 moveDelta,
            Quaternion rotationDelta,
            ControllerControls controls,
            ControllerRotationMode rotationMode,
            bool shouldUseRayVector,
            Vector3 anchorPosition,
            bool isMovementSmoothed = true,
            float depthSensitivity = 1f,
            float jitterStrength = 0f)
        {
            using (UpdatePerfMarker.Auto())
            {
                if (simulatedController == null) { return; }

                if (controls.IsTracked)
                {
                    // Apply depth sensitivity
                    moveDelta.z *= depthSensitivity;

                    // Perform smoothing on the move delta.
                    // This is not framerate independent due to an apparent Unity editor issue
                    // where the *polling rate* of the mouse can cause lag in the editor. This
                    // causes delta times to vary dramatically while moving the mouse.
                    // Using fixedDeltaTime is a reasonable workaround until this issue is resolved.

                    // Only smooth on z for now. Smoothing on other axes causes problems with
                    // the screen-space hands positioning logic.
                    smoothedMoveDelta = new Vector3(moveDelta.x, moveDelta.y,
                        isMovementSmoothed ? Smoothing.SmoothTo(
                        smoothedMoveDelta.z,
                        moveDelta.z,
                        MoveSmoothingTime, Time.fixedDeltaTime) : moveDelta.z);

                    // This value helps control jitter severity.
                    const float jitterSeverity = 0.002f;
                    Vector3 jitter = jitterStrength * jitterSeverity * UnityEngine.Random.insideUnitSphere;

                    SimulatedInputPosition += smoothedMoveDelta + jitter;

                    // If we not have been told to face the camera, apply the rotation delta.
                    if (rotationMode == ControllerRotationMode.UserControl)
                    {
                        SimulatedInputRotation *= rotationDelta;
                    }

                    Vector3 offset = WorldPosition - anchorPosition;

                    SetRelativePoseWithOffset(SimulatedInputPosition, SimulatedInputRotation, offset, rotationMode, shouldUseRayVector);
                }

                ApplyState(controls);
            }
        }

        /// <summary>
        /// Updates this controller with the current values in <paramref name="controls"/>.
        /// </summary>
        /// <remarks>Often used to update tracking state without changing the controller's pose.</remarks>
        /// <param name="controls">Persistent controls data to apply.</param>
        public void UpdateControls(ControllerControls controls)
        {
            if (simulatedController == null) { return; }

            ApplyState(controls);
        }

        private static readonly ProfilerMarker UpdateAbsolutePerfMarker =
            new ProfilerMarker("[MRTK] SimulatedController.UpdateAbsolute");

        /// <summary>
        /// Update the controller simulation with a specified absolute pose in world-space.
        /// </summary>
        /// <param name="position">The worldspace position.</param>
        /// <param name="rotation">The worldspace rotation.</param>
        /// <param name="controls">The desired state of the controller's controls.</param>
        /// <param name="rotationMode">The <see cref="ControllerRotationMode"/> in which the controller is operating.</param>
        /// <param name="shouldUseRayVector">Should pointerRotation be set to the body/head ray vector, instead of the raw rotation?</param>
        public void UpdateAbsolute(
            Vector3 position,
            Quaternion rotation,
            ControllerControls controls,
            ControllerRotationMode rotationMode,
            bool shouldUseRayVector)
        {
            using (UpdateAbsolutePerfMarker.Auto())
            {
                if (simulatedController == null) { return; }

                SetWorldPose(position, rotation, rotationMode, shouldUseRayVector);
                ApplyState(controls);
            }
        }

        private static readonly ProfilerMarker UpdateRelativeToPokePerfMarker =
            new ProfilerMarker("[MRTK] SimulatedController.UpdateRelativeToPoke");

        /// <summary>
        /// Update the controller simulation with a specified absolute pose in world-space at which the
        /// poking position (usually index-tip) should be. 
        /// </summary>
        /// <param name="position">The worldspace position.</param>
        /// <param name="rotation">The worldspace rotation.</param>
        /// <param name="controls">The desired state of the controller's controls.</param>
        /// <param name="rotationMode">The <see cref="ControllerRotationMode"/> in which the controller is operating.</param>
        /// <param name="shouldUseRayVector">Should pointerRotation be set to the body/head ray vector, instead of the raw rotation?</param>
        public void UpdateAbsoluteWithPokeAnchor(
            Vector3 position,
            Quaternion rotation,
            ControllerControls controls,
            ControllerRotationMode rotationMode,
            bool shouldUseRayVector)
        {
            UpdateAbsoluteWithAnchor(position, rotation, controls, rotationMode, shouldUseRayVector, PokePosition);
        }

        /// <summary>
        /// Update the controller simulation with a specified absolute pose in world-space at which the
        /// poking position (usually index-tip) should be. 
        /// </summary>
        /// <param name="position">The worldspace position.</param>
        /// <param name="rotation">The worldspace rotation.</param>
        /// <param name="controls">The desired state of the controller's controls.</param>
        /// <param name="rotationMode">The <see cref="ControllerRotationMode"/> in which the controller is operating.</param>
        /// <param name="shouldUseRayVector">Should pointerRotation be set to the body/head ray vector, instead of the raw rotation?</param>
        public void UpdateAbsoluteWithGrabAnchor(
            Vector3 position,
            Quaternion rotation,
            ControllerControls controls,
            ControllerRotationMode rotationMode,
            bool shouldUseRayVector)
        {
            UpdateAbsoluteWithAnchor(position, rotation, controls, rotationMode, shouldUseRayVector, GrabPosition);
        }

        /// <summary>
        /// Update the controller simulation with a specified absolute pose in world-space anchored at the specified point.
        /// </summary>
        /// <param name="position">The worldspace position.</param>
        /// <param name="rotation">The worldspace rotation.</param>
        /// <param name="controls">The desired state of the controller's controls.</param>
        /// <param name="rotationMode">The <see cref="ControllerRotationMode"/> in which the controller is operating.</param>
        /// <param name="shouldUseRayVector">Should pointerRotation be set to the body/head ray vector, instead of the raw rotation?</param>
        public void UpdateAbsoluteWithAnchor(Vector3 position,
            Quaternion rotation,
            ControllerControls controls,
            ControllerRotationMode rotationMode,
            bool shouldUseRayVector,
            Vector3 anchorPoint)
        {
            Vector3 handToAnchorDelta = (anchorPoint - WorldPosition);
            Vector3 adjustedWorldPos = position - handToAnchorDelta;

            UpdateAbsolute(adjustedWorldPos, rotation, controls, rotationMode, shouldUseRayVector);
        }

        private static readonly ProfilerMarker ApplyStatePerfMarker =
            new ProfilerMarker("[MRTK] SimulatedController.ApplyState");

        private void ApplyState(ControllerControls controls)
        {
            using (ApplyStatePerfMarker.Auto())
            {
                simulatedControllerState.isTracked = controls.IsTracked;
                simulatedControllerState.trackingState = (int)(controls.TrackingState);

                // Axis controls
                simulatedControllerState.trigger = controls.TriggerAxis;
                simulatedControllerState.grip = controls.GripAxis;
                // todo: "soon"
                // simulatedControllerState.primary2DAxis = controls.Primary2DAxis;
                // simulatedControllerState.secondary2DAxis = controls.Secondary2DAxis;

                // Note: if trigger button is activated, the joint synthesizer will instantly pinch, without smoothing.
                // If smoothed pinch is desired, use controls.TriggerAxis.
                simulatedControllerState.WithButton(ControllerButton.TriggerButton, controls.TriggerButton);

                simulatedControllerState.WithButton(ControllerButton.GripButton, controls.GripButton);

                // todo: "soon"
                // simulatedControllerState.WithButton(ControllerButton.MenuButton, controls.MenuButton);
                // simulatedControllerState.WithButton(ControllerButton.PrimaryButton, controls.PrimaryButton);
                // simulatedControllerState.WithButton(ControllerButton.SecondaryButton, controls.SecondaryButton);
                // simulatedControllerState.WithButton(ControllerButton.PrimaryTouch, controls.PrimaryTouch);
                // simulatedControllerState.WithButton(ControllerButton.SecondaryTouch, controls.SecondaryTouch);
                // simulatedControllerState.WithButton(ControllerButton.Primary2DAxisClick, controls.Primary2DAxisClick);
                // simulatedControllerState.WithButton(ControllerButton.Secondary2DAxisClick, controls.Secondary2DAxisClick);
                // simulatedControllerState.WithButton(ControllerButton.Primary2DAxisTouch, controls.Primary2DAxisTouch);
                // simulatedControllerState.WithButton(ControllerButton.Secondary2DAxisTouch, controls.Secondary2DAxisTouch);

                InputState.Change(simulatedController, simulatedControllerState);
            }
        }

        /// <summary>
        /// Sets a controller to the specified camera-relative pose.
        /// </summary>
        /// <param name="position">The desired controller position, in camera relative space.</param>
        /// <param name="rotation">The desired controller rotation, in camera relative space.</param>
        /// <param name="offset">The amount to offset the controller, in world space</param>
        /// <param name="rotationMode">The <see cref="ControllerRotationMode"/> in which the controller is operating.</param>
        /// <param name="shouldUseRayVector">
        /// Should pointerRotation be set to the body/head ray vector, instead of the raw rotation?
        /// </param>
        /// <remarks><para>The incoming camera relative space pose is first being transformed into world space because the
        /// camera relative space is not necessarily the same as the rig relative space (due to the offset GameObject in between).
        /// Transform the parameters into world space and call SetWorldPose where they will be transformed into rig relative space.</para></remarks>
        private void SetRelativePoseWithOffset(
            Vector3 position,
            Quaternion rotation,
            Vector3 offset,
            ControllerRotationMode rotationMode,
            bool shouldUseRayVector)
        {
            Vector3 worldPosition = CameraCache.Main.transform.TransformPoint(position) + offset;
            Quaternion worldRotation = CameraCache.Main.transform.rotation * rotation;

            SetWorldPose(worldPosition, worldRotation, rotationMode, shouldUseRayVector);
        }

        private static readonly ProfilerMarker SetWorldPosePerfMarker =
            new ProfilerMarker("[MRTK] SimulatedController.SetWorldPose");

        /// <summary>
        /// Sets a controller to the specified worldspace pose.
        /// </summary>
        /// <param name="position">The desired controller position, in world space.</param>
        /// <param name="rotation">The desired controller rotation, in world space.</param>
        /// <param name="rotationMode">The <see cref="ControllerRotationMode"/> in which the controller is operating.</param>
        /// <param name="shouldUseRayVector">
        /// Should pointerRotation be set to the body/head ray vector, instead of the raw rotation?
        /// </param>
        private void SetWorldPose(
            Vector3 position,
            Quaternion rotation,
            ControllerRotationMode rotationMode,
            bool shouldUseRayVector)
        {
            using (SetWorldPosePerfMarker.Auto())
            {
                Vector3 rigLocalPosition = PlayspaceUtilities.ReferenceTransform.InverseTransformPoint(position);
                Quaternion rigLocalRotation = Quaternion.Inverse(PlayspaceUtilities.ReferenceTransform.rotation) * rotation;
                SetRigLocalPose(rigLocalPosition, rigLocalRotation, rotationMode, shouldUseRayVector);
            }
        }

        private static readonly ProfilerMarker SetRigLocalPosePerfMarker =
            new ProfilerMarker("[MRTK] SimulatedController.SetRigLocalPose");

        /// <summary>
        /// Sets a controller to the specified rig-local pose.
        /// </summary>
        /// <param name="position">The desired controller position, in rig-local space.</param>
        /// <param name="rotation">The desired controller rotation, in rig-local space.</param>
        /// <param name="rotationMode">The <see cref="ControllerRotationMode"/> in which the controller is operating.</param>
        /// <param name="shouldUseRayVector">
        /// Should pointerRotation be set to the body/head ray vector, instead of the raw rotation?
        /// </param>
        private void SetRigLocalPose(
            Vector3 position,
            Quaternion rotation,
            ControllerRotationMode rotationMode,
            bool shouldUseRayVector)
        {
            using (SetRigLocalPosePerfMarker.Auto())
            {
                simulatedControllerState.devicePosition = position;

                if (rotationMode == ControllerRotationMode.FaceCamera)
                {
                    Quaternion worldLookAtCamera = Quaternion.LookRotation(CameraCache.Main.transform.position - position);
                    Quaternion rigLocalLookAtCamera = Quaternion.Inverse(PlayspaceUtilities.ReferenceTransform.rotation) * worldLookAtCamera;
                    simulatedControllerState.deviceRotation = Smoothing.SmoothTo(
                        simulatedControllerState.deviceRotation,
                        rigLocalLookAtCamera,
                        0.1f,
                        MoveSmoothingTime);
                }
                else if (rotationMode == ControllerRotationMode.CameraAligned)
                {
                    Quaternion worldCameraForward = Quaternion.LookRotation(CameraCache.Main.transform.forward);
                    Quaternion rigLocalCameraForward = Quaternion.Inverse(PlayspaceUtilities.ReferenceTransform.rotation) * worldCameraForward;
                    simulatedControllerState.deviceRotation = Smoothing.SmoothTo(
                        simulatedControllerState.deviceRotation,
                        rigLocalCameraForward,
                        0.1f,
                        MoveSmoothingTime);
                }
                else
                {
                    simulatedControllerState.deviceRotation = rotation;
                }

                if (shouldUseRayVector && Handedness.ToXRNode().HasValue && HandSubsystem != null &&
                        HandSubsystem.TryGetHandCenter(Handedness.ToXRNode().Value, out HandJointPose palmPose))
                {
                    handRay.Update(PlayspaceUtilities.ReferenceTransform.TransformPoint(simulatedControllerState.devicePosition), -palmPose.Up, CameraCache.Main.transform, Handedness);
                    Ray ray = handRay.Ray;
                    simulatedControllerState.pointerPosition = PlayspaceUtilities.ReferenceTransform.InverseTransformPoint(ray.origin);
                    simulatedControllerState.pointerRotation = Quaternion.Inverse(PlayspaceUtilities.ReferenceTransform.rotation) * Quaternion.LookRotation(ray.direction);
                }
                else
                {
                    simulatedControllerState.pointerPosition = simulatedControllerState.devicePosition;
                    simulatedControllerState.pointerRotation = simulatedControllerState.deviceRotation;
                }

            }
        }

        /// <summary>
        /// Configures the input system device usage for the simulated controller.
        /// </summary>
        private void SetUsage()
        {
            Debug.Assert(
                (simulatedController != null) && simulatedController.added,
                "Cannot set device usage: simulated controller is either null or has not been added to the input system.");

            InputSystem.SetDeviceUsage(
                simulatedController,
                (Handedness == Handedness.Left) ?
                    UnityEngine.InputSystem.CommonUsages.LeftHand :
                    UnityEngine.InputSystem.CommonUsages.RightHand);
        }

        /// <summary>
        /// Removes the input system device usage association for the simulated controller.
        /// </summary>
        private void UnsetUsage()
        {
            Debug.Assert(
                (simulatedController != null) && simulatedController.added,
                "Cannot unset device usage: simulated controller is either null or has not been added to the input system.");

            InputSystem.RemoveDeviceUsage(
                simulatedController,
                Handedness == Handedness.Left ?
                    UnityEngine.InputSystem.CommonUsages.LeftHand :
                    UnityEngine.InputSystem.CommonUsages.RightHand);
        }
    }

    /// <summary>
    /// Full set of controller / hand controls supported by the input simulator.
    /// This is a class (instead of a struct), so that IEnumerator-based tests
    /// can obtain a reference to these controls. (Iterators cannot have ref-locals.)
    /// </summary>
    internal class ControllerControls
    {
        /// <summary>
        /// Indicates whether or not the controller is tracked.
        /// </summary>
        public bool IsTracked { get; internal set; }

        /// <summary>
        /// The tracked values (ex: position, rotation) for this controller.
        /// </summary>
        public InputTrackingState TrackingState { get; internal set; }

        /// <summary>
        /// Axis implemented trigger control.
        /// </summary>
        public float TriggerAxis { get; internal set; }

        /// <summary>
        /// Button implemented trigger control.
        /// </summary>
        public bool TriggerButton { get; internal set; }

        /// <summary>
        /// Axis implemented grip control.
        /// </summary>
        public float GripAxis { get; internal set; }

        /// <summary>
        /// Button implemented grip control.
        /// </summary>
        public bool GripButton { get; internal set; }

        // todo - more soon

        /// <summary>
        /// Resets the control state to initial conditions.
        /// </summary>
        public void Reset()
        {
            IsTracked = false;
            TrackingState = InputTrackingState.None;

            TriggerAxis = default;
            TriggerButton = default;
            GripAxis = default;
            GripButton = default;

            // todo - additional controls
        }
    }
}
