// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.Devices
{
    public interface IMixedRealityControllerVisualizer
    {
        /// <summary>
        /// The <see cref="GameObject"/> reference for this controller.
        /// </summary>
        /// <remarks>
        /// This reference may not always be available when called.
        /// </remarks>
        GameObject GameObjectProxy { get; }

        /// <summary>
        /// The current controller reference.
        /// </summary>
        IMixedRealityController Controller { get; set; }

        // TODO add defined elements or transforms?
    }
}