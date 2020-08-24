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
        /// A surface upon which a character could sit, such as a chair or a couch.
        /// </summary>
        Seat = 1 << 4,

        /// <summary>
        /// 
        /// </summary>
        Table = 1 << 5,

        /// <summary>
        /// 
        /// </summary>
        Door = 1 << 6,

        /// <summary>
        /// 
        /// </summary>
        Window = 1 << 7,

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
