// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Implements the Raycast Provider for handling raycasts into the scene.
    /// </summary>
    public interface IMixedRealityRaycastProvider : IMixedRealityDataProvider
    {
        /// <summary>
        /// Simple raycasts each physics <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/>.
        /// </summary>
        /// <returns>Whether or not the raycast hit something.</returns>
        bool RaycastSimplePhysicsStep(RayStep step, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHit hitInfo);

        /// <summary>
        /// Simple raycasts each physics <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/> within a specified maximum distance.
        /// </summary>
        /// <returns>Whether or not the raycast hit something.</returns>
        bool RaycastSimplePhysicsStep(RayStep step, float maxDistance, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHit hitInfo);

        /// <summary>
        /// Sphere raycasts each physics <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/>.
        /// </summary>
        /// <returns>Whether or not the raycast hit something.</returns>
        bool RaycastSpherePhysicsStep(RayStep step, float radius, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHit hitInfo);

        /// <summary>
        /// Sphere raycasts each physics <see cref="Microsoft.MixedReality.Toolkit.Physics.RayStep"/>.
        /// </summary>
        /// <returns>Whether or not the raycast hit something.</returns>
        bool RaycastSpherePhysicsStep(RayStep step, float radius, float maxDistance, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHit hitInfo);
    }
}
