// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Utilities.Interactions
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
        private Vector3 m_startObjectScale;
        private float m_startHandDistanceMeters;

        /// <summary>
        /// Initialize system with source info from controllers/hands
        /// </summary>
        /// <param name="handsPressedMap">Dictionary that maps inputSources to states</param>
        /// <param name="manipulationRoot">Transform of gameObject to be manipulated</param>
        public virtual void Setup(Dictionary<uint, Vector3> handsPressedMap, Transform manipulationRoot)
        {
            m_startHandDistanceMeters = GetMinDistanceBetweenHands(handsPressedMap);
            m_startObjectScale = manipulationRoot.transform.localScale;
        }

        /// <summary>
        /// update Gameobject with new Scale state
        /// </summary>
        /// <param name="handsPressedMap"></param>
        /// <returns>a Vector3 describing the new Scale of the object being manipulated</returns>
        public virtual Vector3 UpdateMap(Dictionary<uint, Vector3> handsPressedMap)
        {
            var ratioMultiplier = GetMinDistanceBetweenHands(handsPressedMap) / m_startHandDistanceMeters;
            return m_startObjectScale * ratioMultiplier;
        }

        private float GetMinDistanceBetweenHands(Dictionary<uint, Vector3> handsPressedMap)
        {
            var result = float.MaxValue;
            Vector3[] handLocations = new Vector3[handsPressedMap.Values.Count];
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
