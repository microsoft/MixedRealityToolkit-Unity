// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Attributes;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders
{
    /// <summary>
    /// Generates a parabolic line between two points.
    /// </summary>
    public class ParabolaConstrainedLineDataProvider : ParabolaLineDataProvider
    {
        [SerializeField]
        [Tooltip("The point where this line will end.")]
        private Vector3 end = Vector3.zero;

        /// <summary>
        /// The point where this line will end.
        /// </summary>
        public Vector3 End
        {
            get { return end; }
            set { end = value; }
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

        #region Monobehaviour Implementation

        protected override void OnValidate()
        {
            if (end == StartPoint)
            {
                end = StartPoint + Vector3.forward;
            }
        }

        #endregion Monobehaviour Implementation

        #region Line Data Provider Implementation

        public override int PointCount => 2;

        protected override Vector3 GetPointInternal(int pointIndex)
        {
            switch (pointIndex)
            {
                case 0:
                    return StartPoint;
                case 1:
                    return end;
                default:
                    Debug.LogError("Invalid point index!");
                    return Vector3.zero;
            }
        }

        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            switch (pointIndex)
            {
                case 1:
                    end = point;
                    break;
                default:
                    Debug.LogError("Invalid point index!");
                    break;
            }
        }

        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtility.GetPointAlongConstrainedParabola(StartPoint, end, upDirection, height, normalizedDistance);
        }

        #endregion Line Data Provider Implementation
    }
}