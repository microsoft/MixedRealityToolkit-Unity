// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// Enumeration defining types of planar surfaces supported by Spatial Awareness.
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
        /// A vertical surface within the user’s space.
        /// </summary>
        Wall = 1 << 3,

        /// <summary>
        /// A surface upon which a character could sit, such as a chair or a couch.
        /// </summary>
        Seat = 1 << 4,

        /// <summary>
        /// A horizontal surface, above the floor.
        /// </summary>
        Table = 1 << 5,

        /// <summary>
        /// A doorway within a wall surface.
        /// </summary>
        Door = 1 << 6,

        /// <summary>
        /// Windows generally appear within a wall, ceiling or other surface.
        /// </summary>
        Window = 1 << 7,

        // Insert additional surface types here.

        /// <summary>
        /// A boundless world mesh.
        /// </summary>
        World = 1 << 30,

        /// <summary>
        /// Objects for which we have no observations
        /// </summary>
        Inferred = 1 << 31
    }
}
