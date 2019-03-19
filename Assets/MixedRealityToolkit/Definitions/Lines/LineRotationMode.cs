// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Defines how to calculate the line's rotation at any given point.
    /// </summary>
    public enum LineRotationMode
    {
        /// <summary>
        /// Don't rotate
        /// </summary>
        None = 0,
        /// <summary>
        /// Use velocity to calculate the line's rotation
        /// </summary>
        Velocity,
        /// <summary>
        /// Rotate relative to direction from origin point
        /// </summary>
        RelativeToOrigin,
    }
}