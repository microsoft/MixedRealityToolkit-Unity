// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Add a NearInteractionTouchableVolume to your scene and configure a touchable volume
    /// in order to get PointerDown and PointerUp events whenever a PokePointer collides with this volume.
    /// </summary>
    public class NearInteractionTouchableVolume : BaseNearInteractionTouchable
    {
        protected void OnValidate()
        {
            touchableCollider = GetComponent<Collider>();
            usesCollider = touchableCollider != null;
        }

        public override float DistanceToSurface(Vector3 samplePoint, out Vector3 normal)
        {
            if (usesCollider)
            {
                touchableCollider = GetComponent<Collider>();

                Vector3 closest = touchableCollider.ClosestPoint(samplePoint);

                normal = (samplePoint - closest);
                if (normal == Vector3.zero)
                {
                    // inside object, use vector to centre as normal
                    normal = samplePoint - transform.TransformVector(touchableCollider.bounds.center);
                    normal.Normalize();
                    return 0;
                }
                else
                {
                    float dist = normal.magnitude;
                    normal.Normalize();
                    return dist;
                }
            }

            normal = Vector3.forward;
            return float.PositiveInfinity;
        }
    }
}