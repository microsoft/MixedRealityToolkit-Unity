// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// Enumeration defining the types of planar surfaces that are supported by the spatial awareness surface finding subsystem.
    /// </summary>
    [System.Flags]
    public enum SpatialAwarenessSurfaceTypes
    {
        /// <summary>
        /// An unknown / unsupported type of surface.
        /// </summary>
        Unknown = 1 << 0,

        /// <summary>
        /// The environment’s floor.
        /// </summary>
        Floor = 1 << 1,

        /// <summary>
        /// The environment’s ceiling.
        /// </summary>
        Ceiling = 1 << 2,

        /// <summary>
        /// A wall within the user’s space.
        /// </summary>
        Wall = 1 << 3,

        /// <summary>
        /// A raised, horizontal surface such as a shelf.
        /// </summary>
        /// <remarks>
        /// Platforms, like floors, that can be used for placing objects
        /// requiring a horizontal surface.
        /// </remarks>
        Platform = 1 << 4
    }
}
