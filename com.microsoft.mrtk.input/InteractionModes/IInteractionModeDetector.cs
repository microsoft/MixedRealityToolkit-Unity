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
        /// Determine whether the detector has detected the appropriate conditions.
        /// </summary>
        /// <remarks>
        /// One possible usage can be when a teleport gesture occurs, am <see cref="IInteractionModeDetector"/> can change the controller's interaction mode to a <see cref="InteractionMode"/> representing teleportation.
        /// </remarks>
        /// <returns>Returns whether the appropriate conditions detected.</returns>
        bool IsModeDetected();

        /// <summary>
        /// Get a list of the <see cref="GameObject"/> instances which represent the controllers that this interaction mode detector has jurisdiction over.
        /// </summary>
        /// <returns>The list of the <see cref="GameObject"/> instances which represent the controllers that this interaction mode detector has jurisdiction over.</returns>
        List<GameObject> GetControllers();
    }
}
