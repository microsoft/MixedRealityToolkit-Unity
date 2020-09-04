// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Use a Unity UI RectTransform as touchable surface.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Scripts/MRTK/Services/NearInteractionTouchableUnityUI")]
    public class NearInteractionTouchableUnityUI : NearInteractionTouchableSurface
    {
        private Lazy<RectTransform> rectTransform;

        public static IReadOnlyList<NearInteractionTouchableUnityUI> Instances => instances;

        public override Vector3 LocalCenter => Vector3.zero;
        public override Vector3 LocalPressDirection => Vector3.forward;
        public override Vector2 Bounds => rectTransform.Value.rect.size;

        private static readonly List<NearInteractionTouchableUnityUI> instances = new List<NearInteractionTouchableUnityUI>();

        public NearInteractionTouchableUnityUI()
        {
            rectTransform = new Lazy<RectTransform>(GetComponent<RectTransform>);
        }

        /// <inheritdoc />
        public override float DistanceToTouchable(Vector3 samplePoint, out Vector3 normal)
        {
            normal = transform.TransformDirection(-LocalPressDirection);

            Vector3 localPoint = transform.InverseTransformPoint(samplePoint);

            // touchables currently can only be touched within the bounds of the rectangle.
            // We return infinity to ensure that any point outside the bounds does not get touched.
            if (!rectTransform.Value.rect.Contains(localPoint))
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