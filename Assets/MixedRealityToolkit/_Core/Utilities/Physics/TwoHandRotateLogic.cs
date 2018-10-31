// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Physics
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
        private const float RotationMultiplier = 2f;
        private readonly RotationConstraintType initialRotationConstraint;

        private RotationConstraintType currentRotationConstraint;
        private Vector3 previousHandlebarRotation;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rotationConstraint"></param>
        public TwoHandRotateLogic(RotationConstraintType rotationConstraint)
        {
            initialRotationConstraint = rotationConstraint;
        }

        /// <summary>
        /// The current rotation constraint might be modified based on disambiguation logic, for example
        /// XOrYBasedOnInitialHandPosition might change the current rotation constraint based on the 
        /// initial hand positions at the start
        /// </summary>
        public RotationConstraintType GetCurrentRotationConstraint()
        {
            return currentRotationConstraint;
        }

        /// <summary>
        /// Setup the rotation logic.
        /// </summary>
        /// <param name="handsPressedMap"></param>
        public void Setup(Dictionary<uint, Vector3> handsPressedMap)
        {
            currentRotationConstraint = initialRotationConstraint;
            previousHandlebarRotation = GetHandlebarDirection(handsPressedMap);
        }

        /// <summary>
        /// Update the rotation based on input.
        /// </summary>
        /// <param name="handsPressedMap"></param>
        /// <param name="currentRotation"></param>
        /// <returns></returns>
        public Quaternion Update(Dictionary<uint, Vector3> handsPressedMap, Quaternion currentRotation)
        {
            var handlebarDirection = GetHandlebarDirection(handsPressedMap);
            var handlebarDirectionProjected = ProjectHandlebarGivenConstraint(currentRotationConstraint, handlebarDirection);
            var prevHandlebarDirectionProjected = ProjectHandlebarGivenConstraint(currentRotationConstraint, previousHandlebarRotation);

            previousHandlebarRotation = handlebarDirection;

            var rotationDelta = Quaternion.FromToRotation(prevHandlebarDirectionProjected, handlebarDirectionProjected);

            float angle;
            Vector3 axis;
            rotationDelta.ToAngleAxis(out angle, out axis);
            angle *= RotationMultiplier;

            if (currentRotationConstraint == RotationConstraintType.YAxisOnly)
            {
                // If we are rotating about Y axis, then make sure we rotate about global Y axis.
                // Since the angle is obtained from a quaternion, we need to properly orient it (up or down) based
                // on the original axis-angle representation. 
                axis = Vector3.up * Vector3.Dot(axis, Vector3.up);
            }

            return Quaternion.AngleAxis(angle, axis) * currentRotation;
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

            return CameraCache.Main.transform.TransformDirection(result);
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

            // We project the handlebar direction into camera space because otherwise when we move our body the handlebar will move even 
            // though, relative to our heads, the handlebar is not moving.
            hand1 = CameraCache.Main.transform.InverseTransformPoint(hand1);
            hand2 = CameraCache.Main.transform.InverseTransformPoint(hand2);

            return hand2 - hand1;
        }
    }
}
