// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Physics
{
    /// <summary>
    /// FocusDetails struct contains information about which game object has the focus currently.
    /// Also contains information about the normal of that point.
    /// </summary>
    [Serializable]
    public struct FocusDetails
    {
        /// <summary>
        /// The hit point of the raycast.
        /// </summary>
        public Vector3 Point;

        /// <summary>
        /// The normal of the raycast.
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// The object hit by the last raycast.
        /// </summary>
        public GameObject Object;

        /// <summary>
        /// The last raycast hit info.
        /// </summary>
        public RaycastHit LastRaycastHit;
    }
}
