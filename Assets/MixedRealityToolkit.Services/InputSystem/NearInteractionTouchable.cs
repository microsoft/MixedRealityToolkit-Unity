// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Add a NearInteractionTouchable to your scene and configure a touchable surface
    /// in order to get PointerDown and PointerUp events whenever a PokePointer touches this surface.
    /// </summary>
    public class NearInteractionTouchable : ColliderNearInteractionTouchable
    {
        /// <summary>
        /// Local space forward direction
        /// </summary>
        [SerializeField]
        protected Vector3 localForward = Vector3.forward;
        public Vector3 LocalForward { get => localForward; }

        /// <summary>
        /// Local space forward direction
        /// </summary>
        [SerializeField]
        protected Vector3 localUp = Vector3.up;
        public Vector3 LocalUp { get => localUp; }

        /// <summary>
        /// Returns true if the LocalForward and LocalUp vectors are orthogonal.
        /// </summary>
        /// <remarks>
        /// LocalRight is computed using the cross product and is always orthogonal to LocalForward and LocalUp.
        /// </remarks>
        public bool AreLocalVectorsOrthogonal => Vector3.Dot(localForward, localUp) == 0;

        /// <summary>
        /// Local space object center
        /// </summary>
        [SerializeField]
        protected Vector3 localCenter = Vector3.zero;
        public Vector3 LocalCenter { get => localCenter; set { localCenter = value; } }

        public Vector3 LocalRight
        {
            get
            {
                Vector3 cross = Vector3.Cross(localUp, localForward);
                if (cross == Vector3.zero)
                {
                    // vectors are collinear return default right
                    return Vector3.right;
                }
                else
                {
                    return cross;
                }
            }
        }

        public Vector3 Forward => transform.TransformDirection(localForward);

        public void SetLocalForward(Vector3 newLocalForward)
        {
            localForward = newLocalForward;
            localUp = Vector3.Cross(localForward, LocalRight).normalized;
        }

        public void SetLocalUp(Vector3 newLocalUp)
        {
            localUp = newLocalUp;
            localForward = Vector3.Cross(LocalRight, localUp).normalized;
        }

        /// <summary>
        /// Local space forward direction
        /// </summary>
        [SerializeField]
        protected Vector2 bounds = Vector2.zero;
        public Vector2 Bounds { get => bounds; set { bounds = value; } }

        protected new void OnValidate()
        {
            if (Application.isPlaying)
            {   // Don't validate during play mode
                return;
            }

            base.OnValidate();

            Debug.Assert(localForward.magnitude > 0);
            Debug.Assert(localUp.magnitude > 0);
            string hierarchy = gameObject.transform.EnumerateAncestors(true).Aggregate("", (result, next) => next.gameObject.name + "=>" + result);
            if (localUp.sqrMagnitude == 1 && localForward.sqrMagnitude == 1)
            {
                Debug.Assert(Vector3.Dot(localForward, localUp) == 0, $"localForward and localUp not perpendicular for object {hierarchy}. Did you set Local Forward correctly?");
            }

            localForward = localForward.normalized;
            localUp = localUp.normalized;

            bounds.x = Mathf.Max(bounds.x, 0);
            bounds.y = Mathf.Max(bounds.y, 0);
        }

        public override float DistanceToTouchable(Vector3 samplePoint, out Vector3 normal)
        {
            normal = Forward;

            Vector3 localPoint = transform.InverseTransformPoint(samplePoint) - localCenter;

            // Get surface coordinates
            Vector3 planeSpacePoint = new Vector3(
                Vector3.Dot(localPoint, LocalRight),
                Vector3.Dot(localPoint, localUp),
                Vector3.Dot(localPoint, localForward));

            // touchables currently can only be touched within the bounds of the rectangle.
            // We return infinity to ensure that any point outside the bounds does not get touched.
            if (planeSpacePoint.x < -bounds.x / 2 ||
                planeSpacePoint.x > bounds.x / 2 ||
                planeSpacePoint.y < -bounds.y / 2 ||
                planeSpacePoint.y > bounds.y / 2)
            {
                return float.PositiveInfinity;
            }

            // Scale back to 3D space
            planeSpacePoint = transform.TransformSize(planeSpacePoint);

            return Math.Abs(planeSpacePoint.z);
        }
    }
}