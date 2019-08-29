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
    public class NearInteractionTouchableUnityUI : BaseNearInteractionTouchable, INearInteractionTouchable
    {
        private Lazy<RectTransform> rectTransform;

        public static IReadOnlyList<NearInteractionTouchableUnityUI> Instances => instances;

        public Vector3 Forward => transform.TransformDirection(LocalForward);

        // UnityUI forward is the direction you are looking when looking at it.  Near Interaction forward is the direction the button or control faces, so the opposite of UnityUI forward.
        public Vector3 LocalForward => -Vector3.forward;

        public Vector3 LocalUp => Vector3.up;

        // See comment for LocalForward.  NearInteraction directions are rotated 180 degrees from UnityUI directions.
        public Vector3 LocalRight => -Vector3.right;

        public Vector3 LocalCenter => Vector3.zero;

        public Vector2 Bounds => rectTransform.Value.rect.size;

        private static readonly List<NearInteractionTouchableUnityUI> instances = new List<NearInteractionTouchableUnityUI>();

        public NearInteractionTouchableUnityUI()
        {
            rectTransform = new Lazy<RectTransform>(GetComponent<RectTransform>);
        }

        /// <inheritdoc />
        public override float DistanceToTouchable(Vector3 samplePoint, out Vector3 normal)
        {
            normal = Forward;

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