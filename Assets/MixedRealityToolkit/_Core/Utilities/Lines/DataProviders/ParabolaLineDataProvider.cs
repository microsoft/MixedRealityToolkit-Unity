// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities.Lines.DataProviders
{
    public abstract class ParabolaLineDataProvider : BaseMixedRealityLineDataProvider
    {
        [Header("Parabola Settings")]

        [SerializeField]
        private Vector3 start = Vector3.zero;

        public Vector3 Start
        {
            get { return start; }
            set { start = value; }
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

        protected override Vector3 GetUpVectorInternal(float normalizedLength)
        {
            return transform.up;
        }
    }
}