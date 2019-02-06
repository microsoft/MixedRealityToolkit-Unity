// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Physics
{
    /// <summary>
    /// Contains information about which game object has the focus currently.
    /// Also contains information about the normal of that point.
    /// </summary>
    public struct FocusDetails
    {
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
        public RaycastHit LastRaycastHit { get; set; }
    }
}
