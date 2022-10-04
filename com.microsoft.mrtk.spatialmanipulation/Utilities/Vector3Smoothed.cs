// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// Encapsulated vector that smoothly moves toward a goal.
    /// </summary>
    [Serializable]
    public struct Vector3Smoothed
    {
        /// <summary>
        /// The current value of the vector.
        /// </summary>
        public Vector3 Current { get; private set; }

        /// <summary>
        /// The desired destination value for the vector.
        /// </summary>
        public Vector3 Goal { get; set; }

        /// <summary>
        /// The length of time the smoothing should use to reach the goal.
        /// </summary>
        public float SmoothTime { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value">The initial value of the vector.</param>
        /// <param name="smoothingTime">The amount of time to perform the smoothing.</param>
        public Vector3Smoothed(Vector3 value, float smoothingTime) : this()
        {
            Current = value;
            Goal = value;
            SmoothTime = smoothingTime;
        }

        /// <summary>
        /// Updates the value of the Goal property.
        /// </summary>
        /// <param name="goal">The new goal for the smoothed vector.</param>
        public void SetGoal(Vector3 goal)
        {
            Goal = goal;
        }

        /// <summary>
        /// Update the vector towards the goal.
        /// </summary>
        /// <param name="deltaTime">The time interval since the most recent update.</param>
        public void Update(float deltaTime)
        {
            Current = Vector3.Lerp(Current, Goal, (Math.Abs(SmoothTime) < Mathf.Epsilon) ? 1.0f : deltaTime / SmoothTime);
        }
    }
}
