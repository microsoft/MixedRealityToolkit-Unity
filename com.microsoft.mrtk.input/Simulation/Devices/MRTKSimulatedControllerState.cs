// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

namespace Microsoft.MixedReality.Toolkit.Input.Simulation
{
    /// <summary>
    /// State for input device representing a simulated XR handed controller.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 91)]
    public struct MRTKSimulatedControllerState : IInputStateTypeInfo
    {
        /// <summary>
        /// Memory format identifier for <see cref="XRSimulatedControllerState"/>.
        /// </summary>
        /// <seealso cref="InputStateBlock.format"/>
        public static FourCC FormatId => new FourCC('M', 'S', 'S', 'C');

        /// <inheritdoc />
        FourCC IInputStateTypeInfo.format => FormatId;

        /// <summary>
        /// The primary touchpad or joystick on a device.
        /// </summary>
        [InputControl(usage = "Primary2DAxis", aliases = new[] { "thumbstick", "joystick" })]
        [FieldOffset(0)]
        public Vector2 primary2DAxis;

        /// <summary>
        /// A trigger-like control, pressed with the index finger.
        /// </summary>
        [InputControl(usage = "Trigger", layout = "Axis")]
        [FieldOffset(8)]
        public float trigger;

        /// <summary>
        /// Represents the user's grip on the controller.
        /// </summary>
        [InputControl(usage = "Grip", layout = "Axis")]
        [FieldOffset(12)]
        public float grip;

        /// <summary>
        /// A secondary touchpad or joystick on a device.
        /// </summary>
        [InputControl(usage = "Secondary2DAxis")]
        [FieldOffset(16)]
        public Vector2 secondary2DAxis;

        /// <summary>
        /// All the buttons on this device.
        /// </summary>
        [InputControl(name = nameof(XRSimulatedController.primaryButton), usage = "PrimaryButton", layout = "Button", bit = (uint)ControllerButton.PrimaryButton)]
        [InputControl(name = nameof(XRSimulatedController.primaryTouch), usage = "PrimaryTouch", layout = "Button", bit = (uint)ControllerButton.PrimaryTouch)]
        [InputControl(name = nameof(XRSimulatedController.secondaryButton), usage = "SecondaryButton", layout = "Button", bit = (uint)ControllerButton.SecondaryButton)]
        [InputControl(name = nameof(XRSimulatedController.secondaryTouch), usage = "SecondaryTouch", layout = "Button", bit = (uint)ControllerButton.SecondaryTouch)]
        [InputControl(name = nameof(XRSimulatedController.gripButton), usage = "GripButton", layout = "Button", bit = (uint)ControllerButton.GripButton, alias = "gripPressed")]
        [InputControl(name = nameof(XRSimulatedController.triggerButton), usage = "TriggerButton", layout = "Button", bit = (uint)ControllerButton.TriggerButton, alias = "triggerPressed")]
        [InputControl(name = nameof(XRSimulatedController.menuButton), usage = "MenuButton", layout = "Button", bit = (uint)ControllerButton.MenuButton)]
        [InputControl(name = nameof(XRSimulatedController.primary2DAxisClick), usage = "Primary2DAxisClick", layout = "Button", bit = (uint)ControllerButton.Primary2DAxisClick)]
        [InputControl(name = nameof(XRSimulatedController.primary2DAxisTouch), usage = "Primary2DAxisTouch", layout = "Button", bit = (uint)ControllerButton.Primary2DAxisTouch)]
        [InputControl(name = nameof(XRSimulatedController.secondary2DAxisClick), usage = "Secondary2DAxisClick", layout = "Button", bit = (uint)ControllerButton.Secondary2DAxisClick)]
        [InputControl(name = nameof(XRSimulatedController.secondary2DAxisTouch), usage = "Secondary2DAxisTouch", layout = "Button", bit = (uint)ControllerButton.Secondary2DAxisTouch)]
        [InputControl(name = nameof(XRSimulatedController.userPresence), usage = "UserPresence", layout = "Button", bit = (uint)ControllerButton.UserPresence)]
        [FieldOffset(24)]
        public ushort buttons;

        /// <summary>
        /// Value representing the current battery life of this device.
        /// </summary>
        [InputControl(usage = "BatteryLevel", layout = "Axis")]
        [FieldOffset(26)]
        public float batteryLevel;

        /// <summary>
        /// Represents the values being tracked for this device.
        /// </summary>
        [InputControl(usage = "TrackingState", layout = "Integer")]
        [FieldOffset(30)]
        public int trackingState;

        /// <summary>
        /// Informs to the developer whether the device is currently being tracked.
        /// </summary>
        [InputControl(usage = "IsTracked", layout = "Button")]
        [FieldOffset(34)]
        public bool isTracked;

        /// <summary>
        /// The position of the device.
        /// </summary>
        [InputControl(usage = "DevicePosition")]
        [FieldOffset(35)]
        public Vector3 devicePosition;

        /// <summary>
        /// The rotation of this device.
        /// </summary>
        [InputControl(usage = "DeviceRotation")]
        [FieldOffset(47)]
        public Quaternion deviceRotation;

        /// <summary>
        /// The device's pointer position.
        /// </summary>
        [InputControl(usage = "PointerPosition")]
        [FieldOffset(63)]
        public Vector3 pointerPosition;

        /// <summary>
        /// The device's pointer rotation.
        /// </summary>
        [InputControl(usage = "PointerRotation")]
        [FieldOffset(75)]
        public Quaternion pointerRotation;

        /// <summary>
        /// Set the button mask for the given <paramref name="button"/>.
        /// </summary>
        /// <param name="button">Button whose state to set.</param>
        /// <param name="state">Whether to set the bit on or off.</param>
        /// <returns>The same <see cref="XRSimulatedControllerState"/> with the change applied.</returns>
        /// <seealso cref="buttons"/>
        public MRTKSimulatedControllerState WithButton(ControllerButton button, bool state = true)
        {
            var bit = 1 << (int)button;
            if (state)
                buttons |= (ushort)bit;
            else
                buttons &= (ushort)~bit;
            return this;
        }

        /// <summary>
        /// Resets the current state.
        /// </summary>
        public void Reset()
        {
            primary2DAxis = default;
            trigger = default;
            grip = default;
            secondary2DAxis = default;
            buttons = default;
            batteryLevel = default;
            trackingState = default;
            isTracked = default;
            devicePosition = default;
            deviceRotation = Quaternion.identity;
            pointerPosition = default;
            pointerRotation = Quaternion.identity;
        }
    }
}
