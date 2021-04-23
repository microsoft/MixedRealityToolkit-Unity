// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Collection layout type enum
    /// </summary>
    public enum LayoutOrder
    {
        /// <summary>
        /// Sort by column, then by row
        /// </summary>
        ColumnThenRow = 0,
        /// <summary>
        /// Sort by row, then by column
        /// </summary>
        RowThenColumn,
        /// <summary>
        /// Sort horizontally
        /// </summary>
        Horizontal,
        /// <summary>
        /// Sort vertically
        /// </summary>
        Vertical
    }
}
