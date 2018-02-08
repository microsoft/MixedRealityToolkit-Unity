// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace MixedRealityToolkit.UX.Lines
{
    public class ParabolaPhysical : Parabola
    {
        [Header("Physical Parabola Settings")]
        public Vector3 Direction = Vector3.forward;
        public float Velocity = 2f;
        public float TimeMultiplier = 1f;
        public bool UseCustomGravity = false;
        public Vector3 Gravity = Vector3.down * 9.8f;

        public override int NumPoints
        {
            get
            {
                return 2;
            }
        }

        protected override Vector3 GetPointInternal(int pointIndex)
        {
            switch (pointIndex)
            {
                case 0:
                    return Start;

                case 1:
                    return GetPointInternal(1f);

                default:
                    return Vector3.zero;
            }
        }

        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            switch (pointIndex)
            {
                case 0:
                    Start = point;
                    break;

                default:
                    break;
            }
        }

        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtils.GetPointAlongPhysicalParabola(Start, Direction, Velocity, UseCustomGravity ? Gravity : Physics.gravity, normalizedDistance * TimeMultiplier);
        }
    }
}