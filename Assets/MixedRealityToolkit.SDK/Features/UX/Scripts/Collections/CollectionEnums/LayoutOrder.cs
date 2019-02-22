// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.SDK.UX.Collections
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
