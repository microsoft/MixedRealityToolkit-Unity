// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Use a Unity UI RectTransform as touchable surface.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class NearInteractionTouchableUnityUI : BaseNearInteractionTouchable
    {
        private RectTransform rectTransform;

        public static IReadOnlyList<NearInteractionTouchableUnityUI> Instances => instances;
        private static readonly List<NearInteractionTouchableUnityUI> instances = new List<NearInteractionTouchableUnityUI>();

        /// <inheritdoc />
        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public override float DistanceToTouchable(Vector3 samplePoint, out Vector3 normal)
        {
            normal = -transform.forward;

            Vector3 localPoint = transform.InverseTransformPoint(samplePoint);

            // touchables currently can only be touched within the bounds of the rectangle.
            // We return infinity to ensure that any point outside the bounds does not get touched.
            if (!rectTransform.rect.Contains(localPoint))
            {
                return float.PositiveInfinity;
            }

            // Scale back to 3D space
            localPoint = transform.TransformSize(localPoint);

            return Math.Abs(localPoint.z);
        }

        protected void OnEnable()
        {
            instances.Add(this);
        }

        protected void OnDisable()
        {
            instances.Remove(this);
        }
    }
}