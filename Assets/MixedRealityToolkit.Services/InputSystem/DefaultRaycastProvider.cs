// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The default implementation of IMixedRealityRaycastProvider.
    /// </summary>
    public class DefaultRaycastProvider : BaseDataProvider, IMixedRealityRaycastProvider
    {
        public DefaultRaycastProvider(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            MixedRealityInputSystemProfile profile) : base(registrar, inputSystem, null, DefaultPriority, profile)
        { }

        /// <inheritdoc />
        public bool Raycast(RayStep step, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHit hitInfo)
        {
            var result = MixedRealityRaycaster.RaycastSimplePhysicsStep(step, step.Length, prioritizedLayerMasks, out RaycastHit physicsHit);
            hitInfo = new MixedRealityRaycastHit(physicsHit);
            return result;
        }

        /// <inheritdoc />
        public bool SphereCast(RayStep step, float radius, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHit hitInfo)
        {
            var result = MixedRealityRaycaster.RaycastSpherePhysicsStep(step, radius, step.Length, prioritizedLayerMasks, out RaycastHit physicsHit);
            hitInfo = new MixedRealityRaycastHit(physicsHit);
            return result;
        }
    }
}
