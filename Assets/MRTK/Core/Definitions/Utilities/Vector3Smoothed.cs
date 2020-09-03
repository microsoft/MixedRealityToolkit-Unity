// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    [Serializable]
    public struct Vector3Smoothed
    {
        public Vector3 Current { get; set; }
        public Vector3 Goal { get; set; }
        public float SmoothTime { get; set; }

        public Vector3Smoothed(Vector3 value, float smoothingTime) : this()
        {
            Current = value;
            Goal = value;
            SmoothTime = smoothingTime;
        }

        public void Update(float deltaTime)
        {
            Current = Vector3.Lerp(Current, Goal, (Math.Abs(SmoothTime) < Mathf.Epsilon) ? 1.0f : deltaTime / SmoothTime);
        }

        public void SetGoal(Vector3 newGoal)
        {
            Goal = newGoal;
        }
    }
}