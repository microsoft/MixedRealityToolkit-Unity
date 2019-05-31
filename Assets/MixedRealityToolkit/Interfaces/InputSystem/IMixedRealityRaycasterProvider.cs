// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The resulting hit information from an IMixedRealityRaycasterProvider.
    /// </summary>
    public struct RaycasterHit
    {
        public float Distance;
        public Vector3 Point;
        public Vector3 Normal;
        public Transform Transform;

        public RaycasterHit(RaycastHit physicsHit) : this()
        {
            Distance = physicsHit.distance;
            Point = physicsHit.point;
            Normal = physicsHit.normal;
            Transform = physicsHit.transform;
        }
    }

    public interface IMixedRealityRaycasterProvider : IMixedRealityDataProvider
    {
        bool RaycastSimplePhysicsStep(RayStep step, LayerMask[] prioritizedLayerMasks, out RaycasterHit physicsHit);

        bool RaycastSimplePhysicsStep(RayStep step, float maxDistance, LayerMask[] prioritizedLayerMasks, out RaycasterHit physicsHit);

        bool RaycastSpherePhysicsStep(RayStep step, float radius, LayerMask[] prioritizedLayerMasks, out RaycasterHit physicsHit);

        bool RaycastSpherePhysicsStep(RayStep step, float radius, float maxDistance, LayerMask[] prioritizedLayerMasks, out RaycasterHit physicsHit);
    }
}
