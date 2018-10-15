// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// Flatten behavior when object is surrounded by BoundingBoxRig.
    /// </summary>
    public enum FlattenModeEnum
    {
        /// <summary>
        /// // Always use XYZ axis
        /// </summary>
        DoNotFlatten = 0,
        /// <summary>
        /// Flatten the X axis
        /// </summary>
        FlattenX,
        /// <summary>
        /// Flatten the Y axis
        /// </summary>
        FlattenY,
        /// <summary>
        /// Flatten the Z axis
        /// </summary>
        FlattenZ,
        /// <summary>
        /// Flatten the smallest relative axis if it falls below threshold
        /// </summary>
        FlattenAuto, 
    }
}
