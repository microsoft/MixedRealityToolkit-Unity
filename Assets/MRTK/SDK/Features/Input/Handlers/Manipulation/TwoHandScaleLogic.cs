// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
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
    internal class TwoHandScaleLogic
    {
        private Vector3 startObjectScale;
        private float startHandDistanceMeters;

        /// <summary>
        /// Initialize system with source info from controllers/hands
        /// </summary>
        /// <param name="handsPressedArray">Array with positions of down pointers</param>
        /// <param name="manipulationRoot">Transform of gameObject to be manipulated</param>
        public virtual void Setup(Vector3[] handsPressedArray, Transform manipulationRoot)
        {
            startHandDistanceMeters = GetMinDistanceBetweenHands(handsPressedArray);
            startObjectScale = manipulationRoot.transform.localScale;
        }

        /// <summary>
        /// update GameObject with new Scale state
        /// </summary>
        /// <param name="handsPressedArray">Array with positions of down pointers, order should be the same as handsPressedArray provided in Setup</param>
        /// <returns>a Vector3 describing the new Scale of the object being manipulated</returns>
        public virtual Vector3 UpdateMap(Vector3[] handsPressedArray)
        {
            var ratioMultiplier = GetMinDistanceBetweenHands(handsPressedArray) / startHandDistanceMeters;
            return startObjectScale * ratioMultiplier;
        }

        private float GetMinDistanceBetweenHands(Vector3[] handsPressedArray)
        {
            var result = float.MaxValue;
            for (int i = 0; i < handsPressedArray.Length; i++)
            {
                for (int j = i + 1; j < handsPressedArray.Length; j++)
                {
                    var distance = Vector3.Distance(handsPressedArray[i], handsPressedArray[j]);
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
