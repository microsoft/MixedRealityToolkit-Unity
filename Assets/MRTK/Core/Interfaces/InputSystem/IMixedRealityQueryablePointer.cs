// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface for handling pointers.
    /// </summary>
    public interface IMixedRealityQueryablePointer : IMixedRealityPointer
    {
        /// <summary>
        /// Called to have the pointer query the scene to determine which objects it is hitting. Updates hitinfo. 
        /// Used when the method for querying the scene utilizes a RaycastHit, such as when using UnityEngine.Physics.Raycast
        /// </summary>
        bool OnSceneQuery(LayerMask[] prioritizedLayerMasks, bool focusIndividualCompoundCollider, out MixedRealityRaycastHit hitInfo, out RayStep Ray, out int rayStepIndex );

        /// <summary>
        /// Called to have the pointer query the scene to determine which objects it is hitting. Updates hitObject, hitPoint, and hitDistance. 
        /// Used when the method for querying the scene does not utilize a RaycastHit. Examples of this include UnityEngine.Physics.SphereOverlap, which performs no raycast calls
        /// </summary>
        bool OnSceneQuery(LayerMask[] prioritizedLayerMasks, bool focusIndividualCompoundCollider, out GameObject hitObject, out Vector3 hitPoint, out float hitDistance);
    }
}
