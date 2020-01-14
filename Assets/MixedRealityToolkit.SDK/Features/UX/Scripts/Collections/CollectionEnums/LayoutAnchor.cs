// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Describes where parent should be located
    /// relative to child layout elements. 
    /// </summary>
    public enum LayoutAnchor
    {
        /// <summary>
        /// Parent will be in upper left corner of collection
        /// </summary>
        UpperLeft,
        /// <summary>
        /// Parent will be in upper center of collection
        /// </summary>
        UpperCenter,
        /// <summary>
        /// Parent will be in upper right of collection
        /// </summary>
        UpperRight,
        /// <summary>
        /// Parent will be in middle left of collection
        /// </summary>
        MiddleLeft,
        /// <summary>
        /// Parent will be in middle center of collection
        /// </summary>
        MiddleCenter,
        /// <summary>
        /// Parent will be in middle right of collection
        /// </summary>
        MiddleRight,
        /// <summary>
        /// Parent will be in bottom left of collection
        /// </summary>
        BottomLeft,
        /// <summary>
        /// Parent will be in bottom center of collection
        /// </summary>
        BottomCenter,
        /// <summary>
        /// Parent will be in bottom right of collection
        /// </summary>
        BottomRight
    };
}