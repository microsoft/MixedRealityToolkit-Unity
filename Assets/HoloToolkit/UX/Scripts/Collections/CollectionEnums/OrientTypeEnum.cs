// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity.Collections
{
    /// <summary>
    /// Orientation type enum for collections
    /// </summary>
    public enum OrientTypeEnum
    {
        None,                   // Don't rotate at all
        FaceOrigin,             // Rotate towards the origin
        FaceOriginReversed,     // Rotate towards the origin + 180 degrees
        FaceFoward,             // Zero rotation. Aka Parent Relative Forwards 
        FaceForwardReversed,    // Zero rotation + 180 degrees. Aka Parent Relative Backwards
        FaceParentUp,           // Parent Relative Up
        FaceParentDown,         // Parent Relative Down
        FaceCenterAxis,         // Lay flat on the surface, facing in
        FaceCenterAxisReversed  // Lay flat on the surface, facing out
    }
}
