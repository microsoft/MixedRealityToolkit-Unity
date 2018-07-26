// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders
{
    public class ConstrainedParabolaLineDataProvider : ParabolaLineDataProvider
    {
        [Header("Constrained Parabola Settings")]

        [SerializeField]
        private Vector3 end = Vector3.forward;

        public Vector3 End
        {
            get { return end; }
            set { end = value; }
        }

        [SerializeField]
        private Vector3 upDirection = Vector3.up;

        public Vector3 UpDirection
        {
            get { return upDirection; }
            set { upDirection = value; }
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
                case 0:
                    Start = point;
                    break;

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