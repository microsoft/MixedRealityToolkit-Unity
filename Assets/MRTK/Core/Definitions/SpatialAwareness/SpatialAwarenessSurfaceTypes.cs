// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
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
