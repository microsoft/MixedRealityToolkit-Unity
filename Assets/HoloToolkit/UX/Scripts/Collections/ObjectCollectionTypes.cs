//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

namespace HoloToolkit.Unity.Collections
{
    /// <summary>
    /// The type of surface to map the collect to.
    /// </summary>
    public enum SurfaceTypeEnum
    {
        Cylinder,
        Plane,
        Sphere,
        Scatter,
    }

    /// <summary>
    /// Sorting type for collections
    /// </summary>
    public enum SortTypeEnum
    {
        None,                   // Don't sort, just display in order received
        Transform,              // Sort by transform order
        Alphabetical,           // Sort by transform name
        TransformReversed,      // Sort by transform order reversed
        AlphabeticalReversed,   // Sort by transform name reversed
    }

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

    /// <summary>
    /// Collection layout type enum
    /// </summary>
    public enum LayoutTypeEnum
    {
        ColumnThenRow,          // Sort by column, then by row
        RowThenColumn,          // Sort by row, then by column
    }
}
