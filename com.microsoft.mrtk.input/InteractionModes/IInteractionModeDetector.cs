// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Defines some base properties that a MRTK Interactor has
    /// </summary>
    public interface IInteractionModeDetector
    {
        /// <summary>
        /// The interaction mode to be toggled if when the detector detects the appropriate game state
        /// </summary>
        InteractionMode ModeOnDetection { get; }

        /// <summary>
        /// Determines whether the detector has detected the appropriate conditions.
        /// For example, raising a teleport gesture may change the controller's interaction mode to "teleport"
        /// </summary>
        /// <returns>Returns whether the appropriate conditions detected</returns>
        bool IsModeDetected();

        /// <summary>
        /// Gets a list of the GameObjects which represent the "controllers" that this interaction mode detector has jurisdiction over
        /// </summary>
        /// <returns> Returns the list of the GameObjects which represent the "controllers" that this interaction mode detector has jurisdiction over</returns>
        List<GameObject> GetControllers();
    }
}
