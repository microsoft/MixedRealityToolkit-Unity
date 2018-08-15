// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.SpatialAwareness
{
    /// <summary>
    /// Flags that indicate the semantic(s) for a surface in the environment.
    /// </summary>
    [Flags]
    public enum SpatialAwarenessSurfaceTypes
    {
        /// <summary>
        /// Catch-all for currently undefined surface types.
        /// </summary>
        Unknown = 1 << 0,

        /// <summary>
        /// The floor of the space.
        /// </summary>
        Floor = 1 << 1,

        /// <summary>
        /// The ceiling of the space.
        /// </summary>
        Ceiling = 1 << 2,

        /// <summary>
        /// A wall within the space.
        /// </summary>
        Wall = 1 << 3,

        /// <summary>
        /// A raised, horizontal surface such as a shelf or a table.
        /// </summary>
        /// <remarks>Platforms, like floors, that can be used for placing objects requiring a horizontal surface.</remarks>
        Platform = 1 << 10,

        /// <summary>
        /// An angled surface.
        /// </summary>
        /// <remarks>Angled surfaces can often be used as ramps.</remarks>
        Angled = 1 << 31,

        /// <summary>
        /// A horizontal surface.
        /// </summary>
        Horizontal = Floor | Ceiling | Platform,

        /// <summary>
        /// A horizontal surface upon which an object can be placed.
        /// </summary>
        HorizontalPlaceable = Floor | Platform,

        /// <summary>
        /// A surface from which an object can be hung.
        /// </summary>
        /// <remarks>Hangable surfaces can either be horizontal or vertical.</remarks>
        Hangable = Ceiling | Wall
    }
}
