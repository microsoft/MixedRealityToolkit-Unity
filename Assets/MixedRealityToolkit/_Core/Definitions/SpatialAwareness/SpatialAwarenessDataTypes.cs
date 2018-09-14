// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// Enumeration defining the types of events that spatial awareness subsystems will send.
    /// </summary>[System.Flags]
    public enum SpatialAwarenessDataTypes
    {
        /// <summary>
        /// No spatial awareness data is desired.
        /// </summary>
        None = 0,

        /// <summary>
        /// Spatial awareness mesh data is desired.
        /// </summary>
        Mesh = 1 << 0,              // HEX: 0x00000001, Decimal: 1

        /// <summary>
        /// Spatial awareness planar surface data is desired.
        /// </summary>
        PlanarSurfaces = 1 << 1     // HEX: 0x00000002, Decimal: 2 
    }
}
