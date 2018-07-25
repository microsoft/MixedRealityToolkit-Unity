// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines
{
    public class Ellipse : LineBase
    {
        const int MaxPoints = 2048;

        [Header ("Ellipse Settings")]
        public int Resolution = 36;
        public Vector2 Radius = new Vector2(1f, 1f);

        public override int PointCount
        {
            get
            {
                Resolution = Mathf.Clamp(Resolution, 0, MaxPoints);
                return Resolution;
            }
        }

        protected override Vector3 GetPointInternal(float normalizedDistance)
        {
            return LineUtility.GetEllipsePoint(Radius.x, Radius.y, normalizedDistance * 2f * Mathf.PI);
        }

        protected override Vector3 GetPointInternal(int pointIndex)
        {
            float angle = ((float)pointIndex / Resolution) * 2f * Mathf.PI;
            return LineUtility.GetEllipsePoint(Radius.x, Radius.y, angle);
        }

        protected override void SetPointInternal(int pointIndex, Vector3 point)
        {
            // Does nothing for an ellipse
            return;
        }

        protected override float GetUnClampedWorldLengthInternal()
        {
            // Crude approximation
            // TODO optimize
            float distance = 0f;
            Vector3 last = GetUnClampedPoint(0f);
            for (int i = 1; i < 10; i++)
            {
                Vector3 current = GetUnClampedPoint((float)i / 10);
                distance += Vector3.Distance(last, current);
            }
            return distance;
        }
    }
}