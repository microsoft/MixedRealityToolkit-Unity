// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Generates a parabolic line between two points.
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/Core/ParabolaConstrainedLineDataProvider")]
    public class ParabolaConstrainedLineDataProvider : ParabolaLineDataProvider
    {
        [SerializeField]
        [Tooltip("The point where this line will end.")]
        private MixedRealityPose endPoint = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// The point where this line will end.
        /// </summary>
        public MixedRealityPose EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }

        [SerializeField]
        [Vector3Range(-1f, 1f)]
        private Vector3 upDirection = Vector3.up;

        public Vector3 UpDirection
        {
            get { return upDirection; }
            set
            {
                upDirection.x = Mathf.Clamp(value.x, -1f, 1f);
                upDirection.y = Mathf.Clamp(value.y, -1f, 1f);
                upDirection.z = Mathf.Clamp(value.z, -1f, 1f);
            }
        }

        [SerializeField]
        [Range(0.01f, 10f)]
        private float height = 1f;

        public float Height
        {
            get { return height; }
            set { height = Mathf.Clamp(value, 0.01f, 10f); }
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
                    return endPoint.Position;
                default:
                    Debug.LogError("Invalid point index!");
                    return Vector3.zero;
            }
        }

        /// <inheritdoc />
        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            switch (pointIndex)
            {
                case 0:
                    break;
                case 1:
                    endPoint.Position = point;
                    break;
                default:
                    Debug.LogError("Invalid point index!");
                    break;
            }
        }

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtility.GetPointAlongConstrainedParabola(StartPoint.Position, endPoint.Position, upDirection, height, normalizedDistance);
        }

        #endregion Line Data Provider Implementation
    }
}