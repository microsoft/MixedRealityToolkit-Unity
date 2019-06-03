// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// The resulting hit information from an IMixedRealityRaycastProvider.
    /// </summary>
    public struct MixedRealityRaycastHitInfo
    {
        public float Distance;
        public Vector3 Point;
        public Vector3 Normal;
        public Transform Transform;

        public MixedRealityRaycastHitInfo(RaycastHit hitInfo) : this()
        {
            Distance = hitInfo.distance;
            Point = hitInfo.point;
            Normal = hitInfo.normal;
            Transform = hitInfo.transform;
        }
    }

    public interface IMixedRealityRaycastProvider : IMixedRealityDataProvider
    {
        bool RaycastSimplePhysicsStep(RayStep step, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHitInfo hitInfo);

        bool RaycastSimplePhysicsStep(RayStep step, float maxDistance, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHitInfo hitInfo);

        bool RaycastSpherePhysicsStep(RayStep step, float radius, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHitInfo hitInfo);

        bool RaycastSpherePhysicsStep(RayStep step, float radius, float maxDistance, LayerMask[] prioritizedLayerMasks, out MixedRealityRaycastHitInfo hitInfo);
    }
}
