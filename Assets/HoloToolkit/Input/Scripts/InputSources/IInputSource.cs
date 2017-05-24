// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Flags used to indicate which input events are supported by an input source.
    /// </summary>
    [Flags]
    public enum SupportedInputEvents
    {
        SourceUpAndDown = (1 << 0),
        SourceClicked = (1 << 1),
        Hold = (1 << 2),
        Manipulation = (1 << 3),
        Navigation = (1 << 4)
    }

    /// <summary>
    /// Flags used to indicate which input information is supported by an input source.
    /// </summary>
    [Flags]
    public enum SupportedInputInfo
    {
        None = 0,
        Position = (1 << 0),
        Rotation = (1 << 1),
        PointingRay = (1 << 2),
        Thumbstick = (1 << 3),
        Touchpad = (1 << 4),
        Trigger = (1 << 5),
        Menu = (1 << 6),
        Grasp = (1 << 7),
    }

    /// <summary>
    /// Interface for an input source.
    /// An input source can be anything that a user can use to interact with a device.
    /// </summary>
    public interface IInputSource
    {
        /// <summary>
        /// Returns the input info that that the input source can provide.
        /// </summary>
        SupportedInputInfo GetSupportedInputInfo(uint sourceId);

        /// <summary>
        /// Returns whether the input source supports the specified input info type.
        /// </summary>
        /// <param name="sourceId">ID of the source.</param>
        /// <param name="inputInfo">Input info type that we want to get information about.</param>
        bool SupportsInputInfo(uint sourceId, SupportedInputInfo inputInfo);

        bool TryGetSourceKind(uint sourceId, out InteractionSourceKind sourceKind);

        /// <summary>
        /// Returns the position of the input source, if available.
        /// Not all input sources support positional information, and those that do may not always have it available.
        /// </summary>
        /// <param name="sourceId">ID of the source for which the position should be retrieved.</param>
        /// <param name="position">Out parameter filled with the position if available, otherwise <see cref="Vector3.zero"/>.</param>
        /// <returns>True if a position was retrieved, false if not.</returns>
        bool TryGetPosition(uint sourceId, out Vector3 position);

        /// <summary>
        /// Returns the rotation of the input source, if available.
        /// Not all input sources support rotation information, and those that do may not always have it available.
        /// </summary>
        /// <param name="sourceId">ID of the source for which the rotation should be retrieved.</param>
        /// <param name="rotation">Out parameter filled with the rotation if available, otherwise <see cref="Quaternion.identity"/>.</param>
        /// <returns>True if an rotation was retrieved, false if not.</returns>
        bool TryGetRotation(uint sourceId, out Quaternion rotation);

        /// <summary>
        /// Returns the pointing ray of the input source, if available.
        /// Not all input sources support pointing information, and those that do may not always have it available.
        /// </summary>
        /// <param name="sourceId">ID of the source for which the pointing ray should be retrieved.</param>
        /// <param name="pointingRay">Out parameter filled with the pointing ray if available.</param>
        /// <returns>True if a pointing ray was retrieved, false if not.</returns>
        bool TryGetPointingRay(uint sourceId, out Ray pointingRay);

        bool TryGetThumbstick(uint sourceId, out bool isPressed, out double x, out double y);
        bool TryGetTouchpad(uint sourceId, out bool isPressed, out bool isTouched, out double x, out double y);
        bool TryGetTrigger(uint sourceId, out bool isPressed, out double pressedValue);
        bool TryGetGrasp(uint sourceId, out bool isPressed);
        bool TryGetMenu(uint sourceId, out bool isPressed);
    }
}
