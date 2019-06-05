// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface to handle raycasts into the scene.
    /// </summary>
    /// <remarks>
    /// Default implementations would likely use Unity's physics system to get hit results from Colliders. However, with a custom
    /// implementation of the interface, the raycast does not have to rely only on Unity-based Colliders to provide hit results, e.g. a
    /// GameObject may use a different mechanism for raycasting, and with a custom implementation, it could be included in the hit result.
    /// </remarks>
    public interface IMixedRealityRaycastProvider : IMixedRealityDataProvider
    {
        /// <summary>
        /// Performs a raycast using the specified <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/>.
        /// </summary>
        /// <returns>Whether or not the raycast hit something.</returns>
        bool Raycast(RayStep step, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHit hitInfo);

        /// <summary>
        /// Performs a sphere cast with the specified <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/> and radius.
        /// </summary>
        /// <returns>Whether or not the SphereCast hit something.</returns>
        bool SphereCast(RayStep step, float radius, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHit hitInfo);
    }
}
