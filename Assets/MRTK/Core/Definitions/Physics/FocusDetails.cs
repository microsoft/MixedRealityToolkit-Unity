// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    /// <summary>
    /// Contains information about which game object has the focus currently.
    /// Also contains information about the normal of that point.
    /// </summary>
    public struct FocusDetails
    {
        /// <summary>
        /// Distance along the ray until a hit, or until the end of the ray if no hit
        /// </summary>
        public float RayDistance { get; set; }

        /// <summary>
        /// The hit point of the raycast.
        /// </summary>
        public Vector3 Point { get; set; }

        /// <summary>
        /// The normal of the raycast.
        /// </summary>
        public Vector3 Normal { get; set; }

        /// <summary>
        /// The object hit by the last raycast.
        /// </summary>
        public GameObject Object { get; set; }

        /// <summary>
        /// The last raycast hit info.
        /// </summary>
        public MixedRealityRaycastHit LastRaycastHit { get; set; }

        /// <summary>
        /// The last raycast hit info for graphic raycast
        /// </summary>
        public RaycastResult LastGraphicsRaycastResult { get; set; }

        public Vector3 PointLocalSpace { get; set; }
        public Vector3 NormalLocalSpace { get; set; }
    }
}
