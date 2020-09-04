// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Creates a parabolic line based on physics.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Core/ParabolaPhysicalLineDataProvider")]
    public class ParabolaPhysicalLineDataProvider : ParabolaLineDataProvider
    {
        [SerializeField]
        [Vector3Range(-1f, 1f)]
        private Vector3 direction = Vector3.forward;

        public Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction.x = Mathf.Clamp(value.x, -1f, 1f);
                direction.y = Mathf.Clamp(value.y, -1f, 1f);
                direction.z = Mathf.Clamp(value.z, -1f, 1f);
            }
        }

        [SerializeField]
        private float velocity = 2f;

        public float Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        [SerializeField]
        private float distanceMultiplier = 1f;

        public float DistanceMultiplier
        {
            get { return distanceMultiplier; }
            set { distanceMultiplier = value; }
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

        #region Line Data Provider Implementation

        /// <inheritdoc />
        public override int PointCount => 2;

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(int pointIndex)
        {
            switch (pointIndex)
            {
                case 0:
                    return StartPoint.Position;
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
        /// This specific override doesn't set any points.
        /// </remarks>
        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            // Intentionally does nothing. StartPoint is always the base.FirstPoint and EndPoint is always calculated by the physics.
        }

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtility.GetPointAlongPhysicalParabola(StartPoint.Position, direction, velocity, useCustomGravity ? gravity : UnityEngine.Physics.gravity, normalizedDistance * distanceMultiplier);
        }

        /// <inheritdoc />
        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            return Vector3.up;
        }

        #endregion Line Data Provider Implementation
    }
}