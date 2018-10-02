// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities
{
    [Serializable]
    public struct QuaternionSmoothed
    {
        public Quaternion Current { get; set; }
        public Quaternion Goal { get; set; }
        public float SmoothTime { get; set; }

        public QuaternionSmoothed(Quaternion value, float smoothingTime) : this()
        {
            Current = value;
            Goal = value;
            SmoothTime = smoothingTime;
        }

        public void Update(float deltaTime)
        {
            Current = Quaternion.Slerp(Current, Goal, (Math.Abs(SmoothTime) < Mathf.Epsilon) ? 1.0f : deltaTime / SmoothTime);
        }

        public void SetGoal(Quaternion newGoal)
        {
            Goal = newGoal;
        }
    }
}