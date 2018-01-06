// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Implement this interface to register your pointer as a pointing source. This could be gaze based or motion controller based.
    /// </summary>
    public interface IPointingSource : IInputSource
    {
        bool InteractionEnabled { get; }

        float? ExtentOverride { get; }

        RayStep[] Rays { get; }

        LayerMask[] PrioritizedLayerMasksOverride { get; }

        PointerResult Result { get; set; }

        void OnPreRaycast();

        void OnPostRaycast();

        bool OwnsInput(BaseEventData eventData);

        bool FocusLocked { get; set; }

        /// <summary>
        /// Returns the position of the input source, if available.
        /// Not all input sources support positional information, and those that do may not always have it available.
        /// </summary>
        /// <param name="sourceId">ID of the source for which the position should be retrieved.</param>
        /// <param name="position">Out parameter filled with the position if available, otherwise <see cref="Vector3.zero"/>.</param>
        /// <returns>True if a position was retrieved, false if not.</returns>
        bool TryGetPointerPosition(uint sourceId, out Vector3 position);

        /// <summary>
        /// Returns the pointing ray of the input source, if available.
        /// Not all input sources support pointing information, and those that do may not always have it available.
        /// </summary>
        /// <param name="sourceId">ID of the source for which the pointing ray should be retrieved.</param>
        /// <param name="pointingRay">Out parameter filled with the pointing ray if available.</param>
        /// <returns>True if a pointing ray was retrieved, false if not.</returns>
        bool TryGetPointingRay(uint sourceId, out Ray pointingRay);

        /// <summary>
        /// Returns the rotation of the input source, if available.
        /// Not all input sources support rotation information, and those that do may not always have it available.
        /// </summary>
        /// <param name="sourceId">ID of the source for which the rotation should be retrieved.</param>
        /// <param name="rotation">Out parameter filled with the rotation if available, otherwise <see cref="Quaternion.identity"/>.</param>
        /// <returns>True if an rotation was retrieved, false if not.</returns>
        bool TryGetPointerRotation(uint sourceId, out Quaternion rotation);
    }
}
