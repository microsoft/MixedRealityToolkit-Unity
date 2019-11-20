// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
    public class TwoHandRotateLogic
    {
        private Vector3 startHandlebar;
        private Quaternion startRotation;

        /// <summary>
        /// Setup the rotation logic.
        /// </summary>
        public void Setup(Dictionary<uint, Vector3> handsPressedMap, Transform t, RotationConstraintType rotationConstraint)
        {
            startHandlebar = ProjectHandlebarGivenConstraint(rotationConstraint, GetHandlebarDirection(handsPressedMap));
            startRotation = t.rotation;
        }

        /// <summary>
        /// Update the rotation based on input.
        /// </summary>
        /// <returns>Desired rotation</returns>
        public Quaternion Update(Dictionary<uint, Vector3> handsPressedMap, Quaternion currentRotation, RotationConstraintType rotationConstraint)
        {
            var handlebarDirection = ProjectHandlebarGivenConstraint(rotationConstraint, GetHandlebarDirection(handsPressedMap));
            return Quaternion.FromToRotation(startHandlebar, handlebarDirection) * startRotation;
        }

        private static Vector3 ProjectHandlebarGivenConstraint(RotationConstraintType constraint, Vector3 handlebarRotation)
        {
            Vector3 result = handlebarRotation;
            switch (constraint)
            {
                case RotationConstraintType.XAxisOnly:
                    result.x = 0;
                    break;
                case RotationConstraintType.YAxisOnly:
                    result.y = 0;
                    break;
                case RotationConstraintType.ZAxisOnly:
                    result.z = 0;
                    break;
                case RotationConstraintType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(constraint), constraint, null);
            }
            return result;
        }

        private static Vector3 GetHandlebarDirection(Dictionary<uint, Vector3> handsPressedMap)
        {
            Debug.Assert(handsPressedMap.Count > 1);
            var handsEnumerator = handsPressedMap.Values.GetEnumerator();
            handsEnumerator.MoveNext();
            var hand1 = handsEnumerator.Current;
            handsEnumerator.MoveNext();
            var hand2 = handsEnumerator.Current;
            handsEnumerator.Dispose();

            return hand2 - hand1;
        }
    }
}
