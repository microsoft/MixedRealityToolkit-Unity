// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// Enumeration defining the types of data that spatial awareness subsystems will send.
    /// </summary>
    public enum SpatialAwarenessDataType
    {
        /// <summary>
        /// No spatial awareness data is desired.
        /// </summary>
        None = 0,

        /// <summary>
        /// Spatial awareness mesh data is desired.
        /// </summary>
        Mesh,

        /// <summary>
        /// Spatial awareness planar surface data is desired.
        /// </summary>
        PlanarSurface
    }
}
