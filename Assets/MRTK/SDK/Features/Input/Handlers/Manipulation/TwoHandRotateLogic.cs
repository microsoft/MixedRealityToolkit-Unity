// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    /// <summary>
    /// Implements common logic for rotating holograms using a handlebar metaphor. 
    /// each frame, object_rotation_delta = rotation_delta(current_hands_vector, previous_hands_vector)
    /// where hands_vector is the vector between two hand/controller positions.
    /// 
    /// Usage:
    /// When a manipulation starts, call Setup.
    /// Call Update any time to update the move logic and get a new rotation for the object.
    /// </summary>
    internal class TwoHandRotateLogic
    {
        private Vector3 startHandlebar;
        private Quaternion startRotation;

        /// <summary>
        /// Setup the rotation logic.
        /// </summary>
        /// <param name="handsPressedArray">Array with positions of down pointers</param>
        public void Setup(Vector3[] handsPressedArray, Transform t)
        {
            startHandlebar = GetHandlebarDirection(handsPressedArray);
            startRotation = t.rotation;
        }

        /// <summary>
        /// Update the rotation based on input.
        /// </summary>
        /// <param name="handsPressedArray">Array with positions of down pointers, order should be the same as handsPressedArray provided in Setup</param>
        /// <returns>Desired rotation</returns>
        public Quaternion Update(Vector3[] handsPressedArray, Quaternion currentRotation)
        {
            var handlebarDirection = GetHandlebarDirection(handsPressedArray);
            return Quaternion.FromToRotation(startHandlebar, handlebarDirection) * startRotation;
        }

        private static Vector3 GetHandlebarDirection(Vector3[] handsPressedArray)
        {
            Debug.Assert(handsPressedArray.Length > 1);
            return handsPressedArray[1] - handsPressedArray[0];
        }
    }
}
