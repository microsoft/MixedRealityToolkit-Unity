// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// Flatten behavior when object is surrounded by BoundingBoxRig.
    /// </summary>
    public enum FlattenModeEnum
    {
        DoNotFlatten,   // Always use XYZ axis
        FlattenX,       // Flatten the X axis
        FlattenY,       // Flatten the Y axis
        FlattenZ,       // Flatten the Z axis
        FlattenAuto,    // Flatten the smallest relative axis if it falls below threshold
    }
}
