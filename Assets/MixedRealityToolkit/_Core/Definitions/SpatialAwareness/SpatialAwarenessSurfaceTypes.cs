// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.SpatialAwareness
{
    /// <summary>
    /// Values that indicate the type of a surface within the environment.
    /// </summary>
    public enum SpatialAwarenessSurfaceTypes
    {
        /// <summary>
        /// Catch-all for currently undefined surface types.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The floor of the space.
        /// </summary>
        Floor,

        /// <summary>
        /// The ceiling of the space.
        /// </summary>
        Ceiling,

        /// <summary>
        /// A wall within the space.
        /// </summary>
        Wall,

        /// <summary>
        /// A raised, horizontal surface such as a shelf or a table.
        /// </summary>
        /// <remarks>Platforms, like floors, that can be used for placing objects requiring a horizontal surface.</remarks>
        Platform,

        /// <summary>
        /// An angled surface.
        /// </summary>
        /// <remarks>Angled surfaces can often be used as ramps.</remarks>
        Angled,
    }
}
