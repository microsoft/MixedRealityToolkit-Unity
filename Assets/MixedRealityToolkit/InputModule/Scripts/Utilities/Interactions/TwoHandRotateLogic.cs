// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace MixedRealityToolkit.InputModule.Utilities.Interations
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
        private const float MinHandDistanceForPitchM = 0.1f;
        private const float RotationMultiplier = 2f;
        public enum RotationConstraint
        {
            None,
            XAxisOnly,
            YAxisOnly,
            ZAxisOnly
        };

        private readonly RotationConstraint m_rotationConstraint;
        /// <summary>
        /// The current rotation constraint might be modified based on disambiguation logic, for example
        /// XOrYBasedOnInitialHandPosition might change the current rotation constraint based on the 
        /// initial hand positions at the start
        /// </summary>
        private RotationConstraint m_currentRotationConstraint;
        public RotationConstraint GetCurrentRotationConstraint()
        {
            return m_currentRotationConstraint;
        }
        private Vector3 m_previousHandlebarRotation;

        public TwoHandRotateLogic(RotationConstraint rotationConstraint)
        {
            m_rotationConstraint = rotationConstraint;
        }

        public void Setup(Dictionary<uint, Vector3> handsPressedMap, Transform manipulationRoot)
        {
            m_currentRotationConstraint = m_rotationConstraint;
            m_previousHandlebarRotation = GetHandlebarDirection(handsPressedMap, manipulationRoot);
        }

        private Vector3 ProjectHandlebarGivenConstraint(RotationConstraint constraint, Vector3 handlebarRotation, Transform manipulationRoot)
        {
            Vector3 result = handlebarRotation;
            switch (constraint)
            {
                case RotationConstraint.XAxisOnly:
                    result.x = 0;
                    break;
                case RotationConstraint.YAxisOnly:
                    result.y = 0;
                    break;
                case RotationConstraint.ZAxisOnly:
                    result.z = 0;
                    break;
            }
            return CameraCache.Main.transform.TransformDirection(result);
        }

        private Vector3 GetHandlebarDirection(Dictionary<uint, Vector3> handsPressedMap, Transform manipulationRoot)
        {
            Assert.IsTrue(handsPressedMap.Count > 1);
            var handsEnumerator = handsPressedMap.Values.GetEnumerator();
            handsEnumerator.MoveNext();
            var hand1 = handsEnumerator.Current;
            handsEnumerator.MoveNext();
            var hand2 = handsEnumerator.Current;

            // We project the handlebar direction into camera space because otherwise when we move our body the handlebard will move even 
            // though, relative to our heads, the handlebar is not moving.
            hand1 = CameraCache.Main.transform.InverseTransformPoint(hand1);
            hand2 = CameraCache.Main.transform.InverseTransformPoint(hand2);

            return hand2 - hand1;
        }

        public Quaternion Update(Dictionary<uint, Vector3> handsPressedMap, Transform manipulationRoot, Quaternion currentRotation)
        {
            var handlebarDirection = GetHandlebarDirection(handsPressedMap, manipulationRoot);
            var handlebarDirectionProjected = ProjectHandlebarGivenConstraint(m_currentRotationConstraint, handlebarDirection,
                manipulationRoot);
            var prevHandlebarDirectionProjected = ProjectHandlebarGivenConstraint(m_currentRotationConstraint,
                m_previousHandlebarRotation, manipulationRoot);
            m_previousHandlebarRotation = handlebarDirection;

            var rotationDelta = Quaternion.FromToRotation(prevHandlebarDirectionProjected, handlebarDirectionProjected);

            var angle = 0f;
            var axis = Vector3.zero;
            rotationDelta.ToAngleAxis(out angle, out axis);
            angle *= RotationMultiplier;

            if (m_currentRotationConstraint == RotationConstraint.YAxisOnly)
            {
                // If we are rotating about Y axis, then make sure we rotate about global Y axis.
                // Since the angle is obtained from a quaternion, we need to properly orient it (up or down) based
                // on the original axis-angle representation. 
                axis = Vector3.up * Vector3.Dot(axis, Vector3.up);
            }
            return Quaternion.AngleAxis(angle, axis) * currentRotation;
        }
    }
}
