// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.UX.Collections.Enums
{
    /// <summary>
    /// Orientation type enum for collections
    /// </summary>
    public enum OrientTypeEnum
    {
        None,                   // Don't rotate at all
        FaceOrigin,             // Rotate towards the origin
        FaceOriginReversed,     // Rotate towards the origin + 180 degrees
        FaceFoward,             // Zero rotation
        FaceForwardReversed,    // Zero rotation + 180 degrees
    }
}
