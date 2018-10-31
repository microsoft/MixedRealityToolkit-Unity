// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Physics
{
    /// <summary>
    /// Implements a scale logic that will scale an object based on the 
    /// ratio of the distance between hands.
    /// object_scale = start_object_scale * curr_hand_dist / start_hand_dist
    /// 
    /// Usage:
    /// When a manipulation starts, call Setup.
    /// Call Update any time to update the move logic and get a new rotation for the object.
    /// </summary>
    public class TwoHandScaleLogic
    {
        private Vector3 startObjectScale;
        private float startHandDistanceMeters;

        public virtual void Setup(Dictionary<uint, Vector3> handsPressedMap, Transform manipulationRoot)
        {
            startHandDistanceMeters = GetMinDistanceBetweenHands(handsPressedMap);
            startObjectScale = manipulationRoot.transform.localScale;
        }

        public virtual Vector3 Update(Dictionary<uint, Vector3> handsPressedMap)
        {
            return startObjectScale * (GetMinDistanceBetweenHands(handsPressedMap) / startHandDistanceMeters);
        }

        /// <summary>
        /// Finds the minimum distance between all pairs of hands
        /// </summary>
        /// <returns></returns>
        private static float GetMinDistanceBetweenHands(Dictionary<uint, Vector3> handsPressedMap)
        {
            var result = float.MaxValue;
            var handLocations = new Vector3[handsPressedMap.Values.Count];
            handsPressedMap.Values.CopyTo(handLocations, 0);
            for (int i = 0; i < handLocations.Length; i++)
            {
                for (int j = i + 1; j < handLocations.Length; j++)
                {
                    var distance = Vector3.Distance(handLocations[i], handLocations[j]);
                    if (distance < result)
                    {
                        result = distance;
                    }
                }
            }

            return result;
        }
    }
}
