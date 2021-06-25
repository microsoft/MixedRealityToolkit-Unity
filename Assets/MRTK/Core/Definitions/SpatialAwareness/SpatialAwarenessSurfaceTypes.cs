// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// Enumeration defining the types of planar surfaces supported by Spatial Awareness.
    /// </summary>
    [System.Flags]
    public enum SpatialAwarenessSurfaceTypes
    {
        /// <summary>
        /// A surface that cannot yet be categorized.
        /// </summary>
        /// <remarks>
        /// Unknown should not be confused with Background. Unknown surfaces may
        /// have no classification or there may not yet be enough data to assign
        /// a surface type. Additional environmental scanning may provide the necessary
        /// data to classify the surface.
        /// </remarks>
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
        /// A large, raised surface upon which objects can be placed.
        /// </summary>
        /// <remarks>
        /// Platforms can represent tables, countertops, shelves or other horizontal surfaces.
        /// </remarks>
        Platform = 1 << 4,

        /// <summary>
        /// A surface that does not fit one of the defined surface types.
        /// </summary>
        /// <remarks>
        /// <para>Platforms, like floors, that can be used for placing objects requiring a horizontal surface.</para>
        /// <para>Background should not be confused with Unknown. There is sufficient data to 
        /// classify the surface and it has been found to not correspond to a defined type.</para>
        /// </remarks>
        Background = 1 << 5,

        /// <summary>
        /// A boundless world mesh.
        /// </summary>
        World = 1 << 6,

        /// <summary>
        /// Objects for which we have no observations
        /// </summary>
        Inferred = 1 << 7
    }
}
