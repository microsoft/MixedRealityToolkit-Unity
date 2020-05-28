// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{

    /// <summary>
    /// Implements a two-handle elastic "stretch" logic, which allows for
    /// either one or two pointers to stretch along a particular axis.
    /// 
    /// Built around the differential equations for a damped harmonic oscillator.
    /// Intended for use with critically and over-damped oscillators.
    /// 
    /// Usage:
    /// When a manipulation starts, call Setup.
    /// Call Update any time to update the move logic and get a new rotation for the object.
    /// </summary>
    internal class ElasticHandleLogic
    {
        private Vector3 leftInitialPosition;
        private Vector3 rightInitialPosition;

        private LinearElasticSystem elasticSystem;

        private bool isSetup;

        /// <summary>
        /// Initialize system with source info from controllers/hands.
        /// Left and right handles must both be specified, but this does not
        /// preclude single-hand interaction when Update() is called.
        /// </summary>
        /// <param name="leftHandleStart">World position of "left" handle point.</param>
        /// <param name="rightHandleStart">World position of "right" handle point.</param>
        /// <param name="extentInfo">Properties of the linear 1-D elastic extent to be manipulated</param>
        /// <param name="elasticProperties">Properties of the elastic material/spring.</param>
        /// <param name="leftHandleVelocity">Optional, initial velocity in 1-dimensional stretch space</param>
        /// <param name="rightHandleVelocity">Optional, initial velocity in 1-dimensional stretch space</param>
        public virtual void Setup(Vector3 leftHandleStart, Vector3 rightHandleStart,
                                    ElasticExtentProperties<float> extentInfo, ElasticProperties elasticProperties,
                                    float leftHandleVelocity = 0.0f, float rightHandleVelocity = 0.0f)
        {
            isSetup = true;
            leftInitialPosition = leftHandleStart;
            rightInitialPosition = rightHandleStart;
            elasticSystem = new LinearElasticSystem((leftHandleStart - rightHandleStart).magnitude, 0.0f, extentInfo, elasticProperties);
        }

        /// <summary>
        /// Update the internal state of the damped harmonic oscillator, given the left and right pointer positions.
        /// Note, both left and right input positions are nullable; set the pointer position to null
        /// if only one of the handles is being interacted with. Returns the calculated distance between the
        /// handles.
        /// 
        /// </summary>
        /// <param name="leftPointer">World position of the pointer manipulating the left handle.</param>
        /// <param name="rightPointer">World position of the pointer manipulating the right handle.</param>
        /// <param name="deltaTime">Amount of time to simulate this step.</param>
        /// <param name="normalizedAxis">World axis of the two-handle system.</param>
        public virtual float Update(Vector3? leftPointer, Vector3? rightPointer, float deltaTime, Vector3? normalizedAxis = null)
        {
            // If we have not been Setup() yet
            if (!isSetup) { return 0.1f; }

            var handleAxis = normalizedAxis ?? (rightInitialPosition - leftInitialPosition).normalized;

            var handDistance = elasticSystem.GetCurrentValue();
            if (leftPointer.HasValue && rightPointer.HasValue)
            {
                // If we have both pointers, we simply project the vector between the
                // two hands onto the handle axis vector.
                handDistance = Vector3.Dot(rightPointer.Value - leftPointer.Value, handleAxis);
            } else if (!leftPointer.HasValue && rightPointer.HasValue)
            {
                // If we only have a right pointer, calculate the hand distance
                // as twice the distance of the right pointer from the center.
                handDistance = 2.0f * Vector3.Dot(rightPointer.Value - (leftInitialPosition + rightInitialPosition) / 2.0f, handleAxis);
            } else if (leftPointer.HasValue && !rightPointer.HasValue)
            {
                // If we only have a left pointer, calculate the hand distance
                // as twice the distance of the left pointer from the center.
                handDistance = 2.0f * -Vector3.Dot(leftPointer.Value - (leftInitialPosition + rightInitialPosition) / 2.0f, handleAxis);
            }

            return elasticSystem.ComputeIteration(handDistance, deltaTime);
        }

        public virtual Vector3 GetCenterPosition(Vector3? leftPointer, Vector3? rightPointer)
        {
            if (leftPointer.HasValue && rightPointer.HasValue)
            {
                return (leftPointer.Value + rightPointer.Value) / 2.0f;
            }
            else if (!leftPointer.HasValue && rightPointer.HasValue)
            {
                return (rightPointer.Value + leftInitialPosition) / 2.0f;
            }
            else if (leftPointer.HasValue && !rightPointer.HasValue)
            {
                return (leftPointer.Value + rightInitialPosition) / 2.0f;
            } else
            {
                return Vector3.zero; // Failure case.
            }
        }
    }
}
