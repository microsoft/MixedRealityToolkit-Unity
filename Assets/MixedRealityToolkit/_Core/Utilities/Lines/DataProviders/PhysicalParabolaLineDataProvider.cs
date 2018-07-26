// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders
{
    public class PhysicalParabolaLineDataProvider : ParabolaLineDataProvider
    {
        [Header("Physical Parabola Settings")]

        [SerializeField]
        private Vector3 direction = Vector3.forward;

        public Vector3 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        [SerializeField]
        private float velocity = 2f;

        public float Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        [SerializeField]
        private float timeMultiplier = 1f;

        public float TimeMultiplier
        {
            get { return timeMultiplier; }
            set { timeMultiplier = value; }
        }

        [SerializeField]
        private bool useCustomGravity = false;

        public bool UseCustomGravity
        {
            get { return useCustomGravity; }
            set { useCustomGravity = value; }
        }

        [SerializeField]
        private Vector3 gravity = Vector3.down * 9.8f;

        public Vector3 Gravity
        {
            get { return gravity; }
            set { gravity = value; }
        }

        /// <inheritdoc />
        public override int PointCount => 2;

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(int pointIndex)
        {
            switch (pointIndex)
            {
                case 0:
                    return Start;
                case 1:
                    return GetPointInternal(1f);
                default:
                    Debug.LogError("Invalid point index!");
                    return Vector3.zero;
            }
        }

        /// <summary>
        /// Sets the point at index.
        /// </summary>
        /// <remarks>
        /// This specific override can only set the start point of the parabola. Any other index besides 0 will throw an invalid point index error.
        /// </remarks>
        /// <param name="pointIndex"></param>
        /// <param name="point"></param>
        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            if (pointIndex == 0)
            {
                Start = point;
            }
            else
            {
                Debug.LogError("Invalid point index!");
            }
        }

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtility.GetPointAlongPhysicalParabola(Start, direction, velocity, useCustomGravity ? gravity : UnityEngine.Physics.gravity, normalizedDistance * timeMultiplier);
        }
    }
}