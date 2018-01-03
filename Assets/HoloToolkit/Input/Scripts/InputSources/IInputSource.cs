// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Interface for an input source.
    /// An input source can be anything that a user can use to interact with a device.
    /// </summary>
    public interface IInputSource
    {
        /// <summary>
        /// Returns the input info that the input source can provide.
        /// </summary>
        SupportedInputInfo GetSupportedInputInfo(uint sourceId);

        /// <summary>
        /// Returns whether the input source supports the specified input info type.
        /// </summary>
        /// <param name="sourceId">ID of the source.</param>
        /// <param name="inputInfo">Input info type that we want to get information about.</param>
        bool SupportsInputInfo(uint sourceId, SupportedInputInfo inputInfo);

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
    }
}
