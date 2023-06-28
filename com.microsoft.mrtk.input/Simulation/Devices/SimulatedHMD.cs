// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Scripting;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

namespace Microsoft.MixedReality.Toolkit.Input.Simulation
{
    /// <summary>
    /// A derived XRSimulatedHMD, mostly to add the missing sync command
    /// functionality from the base class(es). If/when XRSimulatedHMD gets
    /// the sync command functionality by default, we'll remove this class.
    /// </summary>
    [InputControlLayout(
        stateType = typeof(XRSimulatedHMDState),
        isGenericTypeOfDevice = false,
        displayName = "XR Simulated HMD (MRTK)"),
        Preserve]
    internal class MRTKSimulatedHMD : XRSimulatedHMD
    {
        /// <inheritdoc />
        protected override unsafe long ExecuteCommand(InputDeviceCommand* commandPtr)
        {
            return InputSimulator.TryExecuteCommand(commandPtr, out var result)
                ? result
                : base.ExecuteCommand(commandPtr);
        }
    }

    /// <summary>
    /// Class for simulating user / camera movement and rotation. Used by the MRTK Input Simulator.
    /// </summary>
    internal class SimulatedHMD : IDisposable
    {
        private readonly MRTKSimulatedHMD simulatedHmd = null;
        private XRSimulatedHMDState simulatedHmdState;

        // Smoothing time for the camera position.
        private const float moveSmoothingTime = 0.02f;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SimulatedHMD()
        {
            simulatedHmd = InputSystem.AddDevice<MRTKSimulatedHMD>();
            if (simulatedHmd == null)
            {
                Debug.LogError("Failed to create the simulated HMD.");
            }
            simulatedHmdState.Reset();
            InputState.Change(simulatedHmd, simulatedHmdState);
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~SimulatedHMD()
        {
            Dispose();
        }

        /// <summary>
        /// Cleans up references to resources used by the camera simulation.
        /// </summary>
        public void Dispose()
        {
            if ((simulatedHmd != null) && simulatedHmd.added)
            {
                InputSystem.RemoveDevice(simulatedHmd);
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Places the simulated camera at the initial position and rotation.
        /// </summary>
        public void ResetToOrigin()
        {
            simulatedHmdState.Reset();
            InputState.Change(simulatedHmd, simulatedHmdState);
        }

        private Vector3 cameraRotation = Vector3.zero;

        private Vector3 smoothedMoveDelta;

        private static readonly ProfilerMarker UpdatePerfMarker =
            new ProfilerMarker("[MRTK] SimulatedHMD.Update");

        private static readonly ProfilerMarker ChangePerfMarker =
            new ProfilerMarker("[MRTK] SimulatedHMD.Change");

        /// <summary>
        /// Retrieves the playspace relative position of the simulated camera
        /// </summary>
        internal Vector3 Position => simulatedHmdState.devicePosition;

        /// <summary>
        /// Retrieves the playspace relative rotation of the simulated camera
        /// </summary>
        internal Quaternion Rotation => simulatedHmdState.deviceRotation;

        /// <summary>
        /// Updates the simulated camera.
        /// </summary>
        /// <param name="moveDelta">Change in camera position.</param>
        /// <param name="rotationDelta">Change in camera rotation.</param>
        /// <param name="isSmoothed">Should smoothing be applied to camera movement?</param>
        /// <param name="moveSpeed">Multiplier used to adjust how fast the camera moves.</param>
        /// <param name="rotationSensitivity">Multiplier used to adjust the sensitivity of the camera rotation.</param>
        public void Update(
            Vector3 moveDelta,
            Vector3 rotationDelta,
            bool isSmoothed = true,
            float moveSpeed = 1f,
            float rotationSensitivity = 1f)
        {
            using (UpdatePerfMarker.Auto())
            {
                if (simulatedHmd == null) { return; }

                // Perform smoothing on move delta.
                // Yes; this isn't framerate independent. However; there's currently a Unity editor bug
                // wherein the *polling rate* of your mouse can cause severe lagspikes in the
                // editor; this causes deltaTimes to go all over the place while moving your mouse.
                // Using fixedDeltaTime is a good workaround until this bug is resolved.
                smoothedMoveDelta = isSmoothed ?
                    Smoothing.SmoothTo(smoothedMoveDelta, moveDelta, moveSmoothingTime, Time.fixedDeltaTime) :
                    moveDelta;

                simulatedHmdState.trackingState = (int)(InputTrackingState.Position | InputTrackingState.Rotation);

                // HMD poses are relative to the camera floor offset.
                Transform origin = PlayspaceUtilities.XROrigin.CameraFloorOffsetObject.transform;
                Vector3 cameraPosition = simulatedHmdState.centerEyePosition + Quaternion.Inverse(origin.rotation) * Camera.main.transform.rotation * (smoothedMoveDelta * moveSpeed);

                // Update camera rotation
                cameraRotation += rotationDelta * rotationSensitivity;

                Change(cameraPosition, Quaternion.Euler(cameraRotation));
            }
        }

        /// <summary>
        /// Force the input change to change to the given pose.
        /// </summary>
        public void Change(Vector3 position, Quaternion rotation)
        {
            using (ChangePerfMarker.Auto())
            {
                if (simulatedHmd == null) { return; }

                simulatedHmdState.isTracked = true;
                simulatedHmdState.trackingState = (int)(InputTrackingState.Position | InputTrackingState.Rotation);

                simulatedHmdState.centerEyePosition = position;
                simulatedHmdState.devicePosition = simulatedHmdState.centerEyePosition;

                simulatedHmdState.centerEyeRotation = rotation;
                simulatedHmdState.deviceRotation = simulatedHmdState.centerEyeRotation;

                cameraRotation = rotation.eulerAngles;

                InputState.Change(simulatedHmd, simulatedHmdState);
            }
        }
    }
}
