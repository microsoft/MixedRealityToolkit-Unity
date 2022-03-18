// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface to handle raycasts into the scene. Used by FocusProvider to perform ray and sphere cast queries for pointers.
    /// </summary>
    /// <remarks>
    /// <para>Implementations of IMixedRealityRaycastProvider would likely use Unity's physics system to get hit results from Colliders. However,
    /// in a custom implementation, the raycast does not have to rely only on Unity-based Colliders to provide hit results, e.g. a
    /// GameObject may use a different mechanism for raycasting, and with a custom implementation, it could be included in the hit result.</para>
    /// </remarks>
    public interface IMixedRealityRaycastProvider : IMixedRealityService
    {
        /// <summary>
        /// Performs a raycast using the specified <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/>.
        /// </summary>
        /// <returns>Whether or not the raycast hit something.</returns>
        bool Raycast(RayStep step, LayerMask[] prioritizedLayerMasks, bool focusIndividualCompoundCollider, out MixedRealityRaycastHit hitInfo);

        /// <summary>
        /// Performs a sphere cast with the specified <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/> and radius.
        /// </summary>
        /// <returns>Whether or not the SphereCast hit something.</returns>
        bool SphereCast(RayStep step, float radius, LayerMask[] prioritizedLayerMasks, bool focusIndividualCompoundCollider, out MixedRealityRaycastHit hitInfo);

        /// <summary>
        /// Performs a graphics raycast against the specified layerMasks.
        /// </summary>
        /// <returns>The RaycastResult of the raycast.</returns>
        RaycastResult GraphicsRaycast(EventSystem eventSystem, PointerEventData pointerEventData, LayerMask[] layerMasks);
    }
}
