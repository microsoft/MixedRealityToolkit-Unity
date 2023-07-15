// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;
using TrackingState = UnityEngine.XR.InputTrackingState;

namespace Microsoft.MixedReality.Toolkit.Input.Simulation
{
    /// <summary>
    /// A simulated eye gaze device patterned after the Unity OpenXR Plugin <see href="https://docs.unity3d.com/Packages/com.unity.xr.openxr%401.4/api/UnityEngine.XR.OpenXR.Features.Interactions.EyeGazeInteraction.EyeGazeDevice.html">EyeGazeDevice</see>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This eye device is not completely identical in terms of usages or features compared
    /// to the OpenXR eye device, but it is a close approximation. Bind to this device with
    /// the usage wildcards; for example, <c>*/{gaze}/position</c> and <c>*{gaze}/rotation</c>. These usage bindings
    /// will also match any OpenXR compliant device in the real-world.
    /// <br/>
    /// It's desirable to simply derive from the official OpenXR eye gaze device.
    /// However, this class uses a custom <see href="https://docs.unity3d.com/Packages/com.unity.xr.openxr%401.4/api/UnityEngine.XR.OpenXR.Input.PoseControl.html">PoseControl</see> type,
    /// instead of the version found in the Unity input system package, <see cref="PoseControl"/>. Also, the OpenXR plugin package does not
    /// ship a corresponding <see cref="PoseState"/> struct along with its own 
    /// <see href="https://docs.unity3d.com/Packages/com.unity.xr.openxr%401.4/api/UnityEngine.XR.OpenXR.Input.PoseControl.html">PoseControl</see> type,
    /// as such cannot be injected to simulated input data into that control from the managed layer.
    /// <br/>
    /// When the duplicated <see href="https://docs.unity3d.com/Packages/com.unity.xr.openxr%401.4/api/UnityEngine.XR.OpenXR.Input.PoseControl.html">PoseControl</see>
    /// type is either removed, or receives a corresponding state struct, this can inherit directly from the Unity's <see href="https://docs.unity3d.com/Packages/com.unity.xr.openxr%401.4/api/UnityEngine.XR.OpenXR.Features.Interactions.EyeGazeInteraction.EyeGazeDevice.html">EyeGazeDevice</see>.
    /// </para>
    /// </remarks>
    [InputControlLayout(
        displayName = "Eye Gaze (MRTK)",
        isGenericTypeOfDevice = false),
        Preserve]
    public class SimulatedEyeGazeDevice : InputDevice
    {
        /// <summary>
        /// A <see cref="PoseControl"/> representing the <c>EyeGazeInteraction.pose</c> OpenXR binding.
        /// </summary>
        [Preserve, InputControl(offset = 0, usages = new[] { "Device", "gaze" })]
        public PoseControl pose { get; private set; }

        /// <inheritdoc/>
        protected override void FinishSetup()
        {
            base.FinishSetup();
            pose = GetChildControl<PoseControl>(nameof(pose));
        }
    }

    /// <summary>
    /// Class for simulating eye gaze. Used by the Input Simulator.
    /// </summary>
    internal class SimulatedEyeGaze : IDisposable
    {
        private SimulatedEyeGazeDevice simulatedEyeDevice = null;

        // We don't need our own custom state struct because we are not adding
        // any additional information to the default device layout.
        private PoseState poseState;

        /// <summary>
        /// Returns the current rotation, in camera relative space, of the simulated eye gaze device.
        /// </summary>
        /// <remarks>
        /// Kept in euler angles, to model FPS-like controls.
        /// </remarks>
        public Vector3 CameraRelativeRotation { get; private set; } = Vector3.zero;

        /// <summary>
        /// Retrieves the playspace relative position of the simulated eye gaze.
        /// </summary>
        internal Vector3 Position => poseState.position;

        /// <summary>
        /// Retrieves the playspace relative rotation of the simulated eye gaze.
        /// </summary>
        internal Quaternion Rotation => poseState.rotation;

        /// <summary>
        /// Initializes a new instance of a <see cref="SimulatedEyeGaze"/> class.
        /// </summary>
        public SimulatedEyeGaze()
        {
            // Unity fixed this bug in an opt-in fashion by defining USE_INPUT_SYSTEM_POSE_CONTROL to be the indicator
            // to use the Unity Input System's PoseControl and to deprecate the OpenXR package's PoseControl.
            // This is opt-in as of Unity's 1.6.0 OpenXR Plugin, so we'll adapt to the presence of USE_INPUT_SYSTEM_POSE_CONTROL.

            // The following write-up is left as-is until Unity makes this workaround unnecessary in all cases:

            // This RegisterLayout call is a workaround for a significant bug in the Unity Input System.
            // The Input System stores these Control types in a backend data structure.
            // However, these types are stored by their *short* name, not their fully
            // qualified/namespaced/assembly qualified name.
            //
            // The Unity Input System package contains the Pose struct, the PoseControl control,
            // and the associated PoseState that can be used to modify the PoseControl.
            // However, the Unity OpenXR Plugin package also contains a control called PoseControl,
            // which is, for all intents and purposes, completely identical to the PoseControl
            // found in the Input System package.
            // However, when both of these controls are registered at load time, there is 
            // a collision in the input system backend, as both controls' short names are identical.
            // This results in one of the two control types never being registered.
            //
            // During GetChildControl calls, the type of the queried control is checked against
            // the registered layout type. As one of the two duplicate PoseControl types was never
            // registered in the backend, the GetChildControl call fails, reporting that the two
            // types do not match.
            //
            // We work around this by registering a *third* control type, with a separate, disambiguating
            // name argument. This ensures that the InputSystem version of the PoseControl is actually
            // registered, and not clobbered by the OpenXR version.

            // The reason we must use the Unity Input System package's version of the PoseControl control
            // is that it offers us the PoseState struct which we need to be able to modify the PoseControl
            // from the managed/C# side. The OpenXR plugin's version of the PoseControl control does not
            // offer a corresponding PoseState, and as far as I can tell, cannot be used with a 3rd-party
            // state struct.

#if !USE_INPUT_SYSTEM_POSE_CONTROL
            InputSystem.RegisterLayout<PoseControl>("InputSystemPose");
#endif

            simulatedEyeDevice = InputSystem.AddDevice<SimulatedEyeGazeDevice>();
            if (simulatedEyeDevice == null)
            {
                Debug.LogError("Failed to create the simulated eye gaze device.");
                return;
            }
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~SimulatedEyeGaze()
        {
#if !USE_INPUT_SYSTEM_POSE_CONTROL
            // Remove/unregister the layout that we added as a workaround for the Unity bug.
            InputSystem.RemoveLayout("InputSystemPose");
#endif
            Dispose();
        }

        /// <summary>
        /// Cleans up references to resources used by the eye gaze simulation.
        /// </summary>
        public void Dispose()
        {
            if (simulatedEyeDevice != null)
            {
                InputSystem.RemoveDevice(simulatedEyeDevice);
            }
            GC.SuppressFinalize(this);
        }

        private static readonly ProfilerMarker UpdatePerfMarker =
            new ProfilerMarker("[MRTK] SimulatedEyeDevice.Update");

        private static readonly ProfilerMarker ChangePerfMarker =
            new ProfilerMarker("[MRTK] SimulatedEyeDevice.Change");

        /// <summary>
        /// Update the eye gaze simulation with relative per-frame delta rotation (a.k.a. look).
        /// </summary>
        /// <param name="isTracked">Indicates whether or not the eye gaze device is currently tracking.</param>
        /// <param name="lookDelta">The change in the eye gaze rotation (look).</param>
        /// <param name="eyeOffset">Offset to the origin of the eye gaze.</param>
        /// <remarks>
        /// It is not uncommon for Head Mounted Devices to have multiple origins; one for the user's head location,
        /// one for the user's eyes and others are possible.
        /// </remarks>
        public void Update(
            bool isTracked,
            Vector3 lookDelta,
            Vector3 eyeOffset)
        {
            if (simulatedEyeDevice == null) { return; }

            if (!simulatedEyeDevice.added)
            {
                simulatedEyeDevice = InputSystem.GetDeviceById(simulatedEyeDevice.deviceId) as SimulatedEyeGazeDevice;
                if (simulatedEyeDevice == null) { return; }
            }

            using (UpdatePerfMarker.Auto())
            {
                // Update the camera-relative Euler angle look rotation.
                CameraRelativeRotation += lookDelta;

                Vector3 eyePosition = Camera.main.transform.localPosition + (Camera.main.transform.localRotation * eyeOffset);
                Quaternion eyeRotation = poseState.rotation = Camera.main.transform.localRotation * Quaternion.Euler(CameraRelativeRotation);

                Change(isTracked, eyePosition, eyeRotation);
            }
        }

        /// <summary>
        /// Force the input change to the given pose and "is tracked" state.
        /// </summary>
        public void Change(
            bool isTracked, Vector3 position, Quaternion rotation)
        {
            using (ChangePerfMarker.Auto())
            {
                if (simulatedEyeDevice == null) { return; }

                poseState.isTracked = isTracked;
                poseState.trackingState = poseState.isTracked ?
                    TrackingState.Position | TrackingState.Rotation :
                    TrackingState.None;

                if (isTracked)
                {
                    poseState.position = position;
                    // todo - saccade support
                    poseState.rotation = rotation;

                }

                InputState.Change(simulatedEyeDevice.pose, poseState);
            }
        }

        /// <summary>
        /// Force the input change to the given "is tracked" state.
        /// </summary>
        public void Change(bool isTracked)
        {
            Change(isTracked, poseState.position, poseState.rotation);
        }
    }
}
