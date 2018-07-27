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
        [Header("Constrained Parabola Settings")]

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
                if (upDirection.x > 1f)
                {
                    upDirection.x = 1f;
                }
                else if (upDirection.x < -1f)
                {
                    upDirection.x = -1f;
                }
                else
                {
                    upDirection.x = value.x;
                }

                if (upDirection.y > 1f)
                {
                    upDirection.y = 1f;
                }
                else if (upDirection.y < -1f)
                {
                    upDirection.y = -1f;
                }
                else
                {
                    upDirection.y = value.y;
                }

                if (upDirection.z > 1f)
                {
                    upDirection.z = 1f;
                }
                else if (upDirection.z < -1f)
                {
                    upDirection.z = -1f;
                }
                else
                {
                    upDirection.z = value.z;
                }
            }
        }

        [SerializeField]
        [Range(0.01f, 10f)]
        private float height = 1f;

        public float Height
        {
            get { return height; }
            set
            {
                if (value < 0.01f)
                {
                    height = 0.01f;
                }
                else if (value > 10f)
                {
                    height = 10f;
                }
                else
                {
                    height = value;
                }
            }
        }

        private void OnValidate()
        {
            if (end == Start)
            {
                end = Start + Vector3.forward;
            }
        }

        public override int PointCount => 2;

        protected override Vector3 GetPointInternal(int pointIndex)
        {
            switch (pointIndex)
            {
                case 0:
                    return Start;
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
            return LineUtility.GetPointAlongConstrainedParabola(Start, end, upDirection, height, normalizedDistance);
        }
    }
}