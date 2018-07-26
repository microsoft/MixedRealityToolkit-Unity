// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Lines
{
    /// <summary>
    /// Default options for getting a rotation along a line
    /// </summary>
    public enum LineRotationType
    {
        None,                           // Don't rotate
        Velocity,                       // Use velocity
        RelativeToOrigin,               // Rotate relative to direction from origin point
    }
}