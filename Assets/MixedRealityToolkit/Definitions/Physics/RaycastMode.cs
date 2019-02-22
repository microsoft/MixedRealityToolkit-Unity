// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Physics
{
    /// <summary>
    /// Defines the different ways raycasting can be done.
    /// </summary>
    public enum RaycastMode
    {
        /// <summary>
        /// Use a simple raycast from a single point in a given direction.
        /// </summary>
        Simple,
        /// <summary>
        /// Complex raycast from multiple points using a box collider.
        /// </summary>
        Box,
        /// <summary>
        /// Use Sphere cast.
        /// </summary>
        Sphere
    }
}
