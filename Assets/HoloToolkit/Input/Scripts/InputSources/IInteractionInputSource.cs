// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity.InputModule;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

public interface IInteractionInputSource : IInputSource
{
#if UNITY_WSA
    bool TryGetSourceKind(uint sourceId, out InteractionSourceKind sourceKind);
#endif

    /// <summary>
    /// Returns the position of the input source, if available.
    /// Not all input sources support positional information, and those that do may not always have it available.
    /// </summary>
    /// <param name="sourceId">ID of the source for which the position should be retrieved.</param>
    /// <param name="position">Out parameter filled with the position if available, otherwise <see cref="Vector3.zero"/>.</param>
    /// <returns>True if a position was retrieved, false if not.</returns>
    bool TryGetGripPosition(uint sourceId, out Vector3 position);

    /// <summary>
    /// Returns the rotation of the input source, if available.
    /// Not all input sources support rotation information, and those that do may not always have it available.
    /// </summary>
    /// <param name="sourceId">ID of the source for which the rotation should be retrieved.</param>
    /// <param name="rotation">Out parameter filled with the rotation if available, otherwise <see cref="Quaternion.identity"/>.</param>
    /// <returns>True if an rotation was retrieved, false if not.</returns>
    bool TryGetPointerRotation(uint sourceId, out Quaternion rotation);

    /// <summary>
    /// Returns the rotation of the input source, if available.
    /// Not all input sources support rotation information, and those that do may not always have it available.
    /// </summary>
    /// <param name="sourceId">ID of the source for which the rotation should be retrieved.</param>
    /// <param name="rotation">Out parameter filled with the rotation if available, otherwise <see cref="Quaternion.identity"/>.</param>
    /// <returns>True if an rotation was retrieved, false if not.</returns>
    bool TryGetGripRotation(uint sourceId, out Quaternion rotation);

    bool TryGetThumbstick(uint sourceId, out bool isPressed, out Vector2 position);
    bool TryGetTouchpad(uint sourceId, out bool isPressed, out bool isTouched, out Vector2 position);
    bool TryGetSelect(uint sourceId, out bool isPressed, out double pressedValue);
    bool TryGetGrasp(uint sourceId, out bool isPressed);
    bool TryGetMenu(uint sourceId, out bool isPressed);
}
