// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Describes the type of display provided by the device.
    /// Maps to the value returned by the <see cref="XRDisplaySubsystem"/> when
    /// querying displayOpaque.
    /// </summary>
    public enum DisplayType
    {
        /// <summary>
        /// An unknown display type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// A see-through display that allows the physical world to be viewed.
        /// </summary>
        Transparent = 1,

        /// <summary>
        /// A display that blocks viewing of the physical world.
        /// </summary>
        Opaque = 2,
    }
}
