// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.VisualizationSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices
{
    public interface IMixedRealityVisualizer : IMixedRealityControllerPoseSynchronizer
    {
        /// <summary>
        /// The Visualization manager responsible for this controller
        /// </summary>
        IMixedRealityVisualizationSystem VisualizationManager { get; set; }

        /// <summary>
        /// The <see cref="UnityEngine.GameObject"/> reference for this controller.
        /// </summary>
        /// <remarks>
        /// This reference may not always be available when called.
        /// </remarks>
        GameObject GameObjectReference { get; }

        // TODO add defined elements or transforms?
    }
}