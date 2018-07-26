// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders
{
    public class LineDataProvider : BaseMixedRealityLineDataProvider
    {
        [Header("LineDataProvider Settings")]

        [SerializeField]
        private Vector3 start = Vector3.zero;

        [SerializeField]
        private Vector3 end = Vector3.one;

        /// <inheritdoc />
        public override int PointCount => 2;

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(int pointIndex)
        {
            switch (pointIndex)
            {
                case 0:
                    return start;
                case 1:
                    return end;
                default:
                    Debug.LogError("Invalid point index");
                    return Vector3.zero;
            }
        }

        /// <inheritdoc />
        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            switch (pointIndex)
            {
                case 0:
                    start = point;
                    break;
                case 1:
                    end = point;
                    break;
                default:
                    Debug.LogError("Invalid point index");
                    break;
            }
        }

        /// <inheritdoc />
        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return Vector3.Lerp(start, end, normalizedDistance);
        }

        /// <inheritdoc />
        protected override float GetUnClampedWorldLengthInternal()
        {
            return Vector3.Distance(start, end);
        }

        /// <inheritdoc />
        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            return transform.up;
        }
    }
}