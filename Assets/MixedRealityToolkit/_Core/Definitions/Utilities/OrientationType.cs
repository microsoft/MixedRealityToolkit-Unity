// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities
{
    /// <summary>
    /// Orientation type enum for collections
    /// </summary>
    public enum OrientationType
    {
        /// <summary>
        /// Don't rotate at all
        /// </summary>
        None,
        /// <summary>
        /// Rotate towards the origin
        /// </summary>
        FaceOrigin,
        /// <summary>
        /// Rotate towards the origin + 180 degrees
        /// </summary>
        FaceOriginReversed,
        /// <summary>
        /// Zero rotation
        /// </summary>
        FaceFoward,
        /// <summary>
        /// Zero rotation + 180 degrees
        /// </summary>
        FaceForwardReversed,
    }
}
