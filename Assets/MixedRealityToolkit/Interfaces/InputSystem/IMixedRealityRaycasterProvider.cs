// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public interface IMixedRealityRaycasterProvider : IMixedRealityDataProvider
    {
        bool RaycastSimplePhysicsStep(RayStep step, LayerMask[] prioritizedLayerMasks, out RaycastHit physicsHit);

        bool RaycastSimplePhysicsStep(RayStep step, float maxDistance, LayerMask[] prioritizedLayerMasks, out RaycastHit physicsHit);

        bool RaycastSpherePhysicsStep(RayStep step, float radius, LayerMask[] prioritizedLayerMasks, out RaycastHit physicsHit);

        bool RaycastSpherePhysicsStep(RayStep step, float radius, float maxDistance, LayerMask[] prioritizedLayerMasks, out RaycastHit physicsHit);
    }
}
